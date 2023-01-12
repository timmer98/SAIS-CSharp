using System.Collections;
using TextIndexierung.SAIS.Helper;
using TextIndexierung.SAIS.Model;

namespace TextIndexierung.SAIS
{
    /// <summary>
    /// The SuffixArrayBuilder class contains all methods to build the suffix array.
    /// The algorithm used is the Suffix Array Induced Sorting (SAIS) linear time construction algorithm.
    /// </summary>
    public class SuffixArrayBuilder
    {
        private MemoryManager memoryManager;

        public SuffixArrayBuilder()
        {
            this.memoryManager = new MemoryManager();
        }

        public SuffixArrayBuilder(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;
        }

        /// <summary>
        /// SAIS algorithm to build the suffix array.
        /// A artificial sentinel is appended to the input. To not allocate a new array and copy the contents
        /// an ArraySegment is returned.
        /// </summary>
        /// <param name="inputBytes">Byte array of an ASCII encoded input text.</param>
        /// <returns>Suffix array of <see cref="inputBytes"/> as ArraySegment.</returns>
        public ArraySegment<int> BuildSuffixArray(byte[] inputBytes)
        {
            memoryManager.CheckPeak();

            var sentinelArray = SentinelHelper.AppendSentinel(inputBytes);
            var suffixArray =  BuildSuffixArray(sentinelArray);

            return SentinelHelper.RemoveAppendedSentinelFromSuffixArray(suffixArray);
        }

        /// <summary>
        /// Recursive call to run the SAIS algorithm with the reduced text.
        /// </summary>
        /// <param name="inputText">The input text, which could be the initial loaded text or a reduced text.</param>
        /// <param name="alphabetSize">The number of unique characters in <see cref="inputText"/></param>
        /// <returns>The suffix array of <see cref="inputText"/>.</returns>
        private int[] BuildSuffixArray(IBaseArray inputText, int alphabetSize = 256)
        {
            var suffixArray = ArrayHelper.GetInitializedIntArray(inputText.Length, -1);

            var buckets = this.GetBuckets(inputText, alphabetSize);
            var suffixMarks = this.MarkSuffixes(inputText);

            this.InsertLmsSuffixes(inputText, suffixArray, buckets, suffixMarks);
            this.ResetBuckets(buckets);
            this.InduceLeftToRight(inputText, suffixArray, buckets, suffixMarks);
            this.InduceRightToLeft(inputText, suffixArray, buckets, suffixMarks);

            var (reducedText, offsets, newAlphabetSize) = this.BuildSummary(inputText, suffixArray, suffixMarks);
            int[] reducedSuffixArray;

            memoryManager.CheckPeak();

            if (reducedText.Length == newAlphabetSize)
            {
                // All signs unique? -> counting sort;
                reducedSuffixArray = new int[reducedText.Length];
                for (var i = 1; i < reducedText.Length; i++) reducedSuffixArray[reducedText[i]] = i;
            }
            else
            {
                // Some signs have the same value? -> Reduce the text further
                reducedSuffixArray = this.BuildSuffixArray(reducedText, newAlphabetSize);
            }

            Array.Fill(suffixArray, -1);

            this.MapReducedSuffixArray(inputText, reducedSuffixArray, suffixArray, buckets, offsets);
            this.ResetBuckets(buckets);

            InduceLeftToRight(inputText, suffixArray, buckets, suffixMarks);
            InduceRightToLeft(inputText, suffixArray, buckets, suffixMarks);

            memoryManager.CheckPeak();

            return suffixArray;
        }

        /// <summary>
        /// Maps reduced suffix array back into the normal suffix array.
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="reducedSuffixArray"></param>
        /// <param name="suffixArray"></param>
        /// <param name="buckets"></param>
        /// <param name="offsets">Offsets of the super signs in the suffix array.</param>
        private void MapReducedSuffixArray(IBaseArray inputText, int[] reducedSuffixArray, int[] suffixArray,
            Bucket[] buckets, int[] offsets)
        {
            this.ResetBuckets(buckets);

            for (var i = reducedSuffixArray.Length - 1; i >= 0; i--)
            {
                var charIndex = offsets[reducedSuffixArray[i]];
                var bucket = buckets[inputText[charIndex]];
                suffixArray[bucket.TailPointer] = charIndex;
                bucket.TailPointer--;
            }
        }

        /// <summary>
        /// Builds a summary text with super signs (ranks of LMS substrings).
        /// </summary>
        /// <param name="inputText">Input text to check the LMS substrings.</param>
        /// <param name="suffixArray">Calculated suffix array in this recursion step.</param>
        /// <param name="suffixMarks">Suffix marks for the input text.</param>
        /// <returns>A 3-Tuple as text summary with the reduced text, its offsets (for reversing) and its alphabet size.</returns>
        private (IBaseArray reducedText, int[] offsets, int alphabetSize) BuildSummary(IBaseArray inputText, int[] suffixArray,
            BitArray suffixMarks)
        {
            var lmsNames = ArrayHelper.GetInitializedIntArray(suffixArray.Length, -1);
            lmsNames[suffixArray[0]] = 0;
            var counter = 0;
            var reducedTextSize = 1;
            var previous = suffixArray[0];

            // Check LMS substrings
            for (var i = 1; i < suffixArray.Length; i++)
                if (IsLmsSuffix(suffixMarks, suffixArray[i]))
                {
                    reducedTextSize++;
                    var current = suffixArray[i];

                    if (!this.AreLmsSubstringsEqual(inputText, previous, current, suffixMarks)) counter++;

                    lmsNames[current] = counter;
                    previous = current;
                }

            var reducedText = new IntArray(reducedTextSize);
            var offsets = new int[reducedTextSize];

            // Build reduced text from ranks of lms substrings
            for (int i = 0, j = 0; i < lmsNames.Length; i++)
                if (lmsNames[i] != -1)
                {
                    reducedText[j] = lmsNames[i];
                    offsets[j] = i;
                    j++;
                }

            return (reducedText, offsets, counter + 1);
        }

        /// <summary>
        /// Checks if two LMS substrings are equal.
        /// </summary>
        /// <param name="text">Input text of this recursion.</param>
        /// <param name="previousOffset">Starting index for the previous LMS substring.</param>
        /// <param name="currentOffset">Starting index for the current LMS substring.</param>
        /// <param name="suffixMarks">Suffix marks of <see cref="text"/>.</param>
        /// <returns>True if equal, otherwise false.</returns>
        private bool AreLmsSubstringsEqual(IBaseArray text, int previousOffset, int currentOffset, BitArray suffixMarks)
        {
            if (previousOffset == text.Length - 1 || currentOffset == text.Length - 1) return false;
            if (text[previousOffset] != text[currentOffset]) return false;

            for (var i = 1; i < text.Length; i++)
            {
                if (text[previousOffset + i] != text[currentOffset + i]) return false;

                var prevIsLms = this.IsLmsSuffix(suffixMarks, previousOffset + i);
                var currentIsLms = this.IsLmsSuffix(suffixMarks, currentOffset + i);

                if (prevIsLms != currentIsLms) return false;
                if (prevIsLms && currentIsLms) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the suffix at position <see cref="index"/> is an LMS suffix.
        /// </summary>
        /// <param name="suffixMarks">Suffix marks of the input text.</param>
        /// <param name="index">Index to check.</param>
        /// <returns>True if LMS suffix, otherwise false.</returns>
        private bool IsLmsSuffix(BitArray suffixMarks, int index)
        {
            if (index == 0) return false;

            if (suffixMarks[index] == false && suffixMarks[index - 1] == true) return true;

            return false;
        }

        /// <summary>
        /// Inducing suffixes form right to left.
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="suffixArray"></param>
        /// <param name="buckets"></param>
        /// <param name="suffixMarks"></param>
        private void InduceRightToLeft(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
        {
            for (var i = suffixArray.Length - 1; i >= 0; i--)
                if (suffixArray[i] > 0 && suffixMarks[suffixArray[i] - 1] == false)
                {
                    var character = inputText[suffixArray[i] - 1];
                    var bucket = buckets[character];
                    suffixArray[bucket.TailPointer] = suffixArray[i] - 1;
                    bucket.TailPointer--;
                }
        }

        /// <summary>
        /// Inducing positions from left to right.
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="suffixArray"></param>
        /// <param name="buckets"></param>
        /// <param name="suffixMarks"></param>
        private void InduceLeftToRight(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
        {
            for (var i = 0; i < suffixArray.Length; i++)
                // SA[i] must be greater than 0 because -1 is invalid and 0 is indexOutOfRange after decrement
                if (suffixArray[i] > 0 && suffixMarks[suffixArray[i] - 1] == true)
                {
                    var character = inputText[suffixArray[i] - 1];
                    var bucket = buckets[character];
                    suffixArray[bucket.HeadPointer] = suffixArray[i] - 1;
                    bucket.HeadPointer++;
                }
        }

        /// <summary>
        /// Inserts LMS suffixes at the end of the according buckets.
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="suffixArray"></param>
        /// <param name="buckets"></param>
        /// <param name="suffixMarks"></param>
        private void InsertLmsSuffixes(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
        {
            // Start at 1 because first element can't be LMS by definition
            for (var i = 1; i < inputText.Length; i++)
                if (!suffixMarks[i] && suffixMarks[i - 1]) // Means: if the character is leftmost s suffix
                {
                    var character = inputText[i];
                    var bucket = buckets[character]; // Get bucket for current character
                    suffixArray[bucket.TailPointer] = i; // Insert suffix pointer at bucket end
                    bucket.TailPointer--; // Decrement tail pointer
                }
        }

        /// <summary>
        /// Marks all suffixes as L- or S-Type suffixes. 
        /// </summary>
        /// <param name="inputText">Input text as <see cref="IBaseArray"/>. Could be the initial text or a reduced text.</param>
        /// <returns>Returns a BitArray, where true means L-Type suffix and false means S-Type.</returns>
        internal BitArray MarkSuffixes(IBaseArray inputText)
        {
            var suffixTypes = new BitArray(inputText.Length);
            suffixTypes[inputText.Length - 1] = false; // Sentinel is always S-Type

            for (var i = inputText.Length - 2; i >= 0; i--)
                if (inputText[i] > inputText[i + 1])
                    suffixTypes[i] = true; // L-Type
                else if (inputText[i] < inputText[i + 1])
                    suffixTypes[i] = false; // S-Type
                else
                    // Same lexicographic value -> Same type as previous}
                    suffixTypes[i] = suffixTypes[i + 1];

            return suffixTypes;
        }

        /// <summary>
        /// Gets the Buckets for every sign.
        /// </summary>
        /// <param name="inputText">Input as IBaseArray. </param>
        /// <param name="alphabetSize">Number of unique characters in <see cref="inputText"/></param>
        /// <returns>An array of <see cref="Bucket"/> instances. The bucket for every character can be accessed through the array index.</returns>
        internal Bucket[] GetBuckets(IBaseArray inputText, int alphabetSize = 256)
        {
            var binSizesForChar = GetBinSizes(inputText, alphabetSize);

            var buckets = new Bucket[alphabetSize];

            // TailPointer starts on the end, HeadPointer starts on the starting index
            var previousBucket = new Bucket(startIndex: 0, endIndex: binSizesForChar[0]);
            buckets[0] = previousBucket;

            for (var i = 1; i < binSizesForChar.Length; i++)
            {
                var startIndex = previousBucket.EndIndex;
                var endIndex = binSizesForChar[i] + previousBucket.EndIndex;

                var bucket = new Bucket(startIndex, endIndex);

                previousBucket = bucket;
                buckets[i] = bucket;
            }

            return buckets;
        }

        /// <summary>
        /// Gets the bin sizes of each characters bin.
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="alphabetSize"></param>
        /// <returns>An int array containing each bin size at the corresponding characters index.</returns>
        private int[] GetBinSizes(IBaseArray inputText, int alphabetSize = 256)
        {
            var binSizesForChar = new int[alphabetSize];

            for (var index = 0; index < inputText.Length; index++)
            {
                var c = inputText[index];

                binSizesForChar[c]++;
            }

            return binSizesForChar;
        }

        /// <summary>
        /// Resets head and tail pointers for every bucket.
        /// </summary>
        /// <param name="buckets">Buckets to reset.</param>
        private void ResetBuckets(Bucket[] buckets)
        {
            foreach (var bucket in buckets) bucket.ResetPointers();
        }
    }
}