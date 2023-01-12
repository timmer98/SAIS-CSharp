using System.Collections;
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
        /// </summary>
        /// <param name="inputBytes">Byte array of an ASCII encoded input text.</param>
        /// <returns>Suffix array of <see cref="inputBytes"/></returns>
        public Span<int> BuildSuffixArray(byte[] inputBytes)
        {
            memoryManager.CheckPeak();

            var sentinelArray = new IntArray(inputBytes.Length + 1);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                sentinelArray[i] = inputBytes[i] + 1;
            }

            sentinelArray[inputBytes.Length] = 0;

            var SA =  BuildSuffixArray(sentinelArray);
            var suffixArrayWithoutAppendedSentinel = SA.AsSpan(1);

            return suffixArrayWithoutAppendedSentinel;
        }

        /// <summary>
        /// Recursive call to run the SAIS algorithm with the reduced text.
        /// </summary>
        /// <param name="inputText">The input text, which could be the initial loaded text or a reduced text.</param>
        /// <param name="alphabetSize">The number of unique characters in <see cref="inputText"/></param>
        /// <returns></returns>
        private int[] BuildSuffixArray(IBaseArray inputText, int alphabetSize = 256)
        {
            var suffixArray = new int[inputText.Length];
            Array.Fill(suffixArray, -1);
            var buckets = this.GetBuckets(inputText, alphabetSize);
            var suffixMarks = this.MarkSuffixes(inputText);

            this.InsertLmsCharacters(inputText, suffixArray, buckets, suffixMarks);
            this.ResetBuckets(buckets);
            this.InduceLeft(inputText, suffixArray, buckets, suffixMarks);
            this.InduceRight(inputText, suffixArray, buckets, suffixMarks);

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

            suffixArray = Enumerable.Repeat(-1, suffixArray.Length).ToArray();
            this.MapReducedSuffixArray(inputText, reducedSuffixArray, suffixArray, buckets, suffixMarks, offsets);
            this.ResetBuckets(buckets);
            InduceLeft(inputText, suffixArray, buckets, suffixMarks);
            InduceRight(inputText, suffixArray, buckets, suffixMarks);

            memoryManager.CheckPeak();

            return suffixArray;
        }

        private void MapReducedSuffixArray(IBaseArray inputText, int[] reducedSuffixArray, int[] suffixArray,
            Bucket[] buckets, BitArray suffixMarks, int[] offsets)
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

        private (IBaseArray reducedText, int[] offsets, int alphabetSize) BuildSummary(IBaseArray inputText, int[] suffixArray,
            BitArray suffixMarks)
        {
            var lmsNames = Enumerable.Repeat(-1, suffixArray.Length).ToArray();
            lmsNames[suffixArray[0]] = 0;
            var counter = 0;
            var reducedTextSize = 1;
            var previous = suffixArray[0];

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

            for (int i = 0, j = 0; i < lmsNames.Length; i++)
                if (lmsNames[i] != -1)
                {
                    reducedText[j] = lmsNames[i];
                    offsets[j] = i;
                    j++;
                }

            return (reducedText, offsets, counter + 1);
        }

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

        private bool IsLmsSuffix(BitArray suffixMarks, int index)
        {
            if (index == 0) return false;

            if (suffixMarks[index] == false && suffixMarks[index - 1] == true) return true;

            return false;
        }

        private void InduceRight(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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

        private void InduceLeft(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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

        private void InsertLmsCharacters(IBaseArray inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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
        /// <param name="buckets"></param>
        private void ResetBuckets(Bucket[] buckets)
        {
            foreach (var bucket in buckets) bucket.ResetPointers();
        }
    }
}