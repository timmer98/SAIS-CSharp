using System.Collections;

namespace TextIndexierung.SAIS
{
    public class SuffixArrayBuilder
    {
        private MemoryManager memoryManager;

        public SuffixArrayBuilder(MemoryManager memoryManager)
        {
            this.memoryManager = memoryManager;
        }

        public int[] BuildSuffixArray(byte[] inputBytes)
        {
            var inputText = inputBytes.Select(x => (int)x).ToArray();
            memoryManager.CheckPeak();
            return BuildSuffixArray(inputText);
        }

        public int[] BuildSuffixArray(int[] inputText, int alphabetSize = 256)
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
                // No equal signs -> counting sort;
                reducedSuffixArray = new int[reducedText.Length];
                for (var i = 1; i < reducedText.Length; i++) reducedSuffixArray[reducedText[i]] = i;
            }
            else
            {
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

        private void MapReducedSuffixArray(int[] inputText, int[] reducedSuffixArray, int[] suffixArray,
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

        private (int[] reducedText, int[] offsets, int alphabetSize) BuildSummary(int[] inputText, int[] suffixArray,
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

            var reducedText = new int[reducedTextSize];
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

        private bool AreLmsSubstringsEqual(int[] text, int previousOffset, int currentOffset, BitArray suffixMarks)
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

        private void InduceRight(int[] inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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

        private void InduceLeft(int[] inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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

        private void InsertLmsCharacters(int[] inputText, int[] suffixArray, Bucket[] buckets, BitArray suffixMarks)
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

        internal BitArray MarkSuffixes(int[] inputText)
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

        internal Bucket[] GetBuckets(int[] inputText, int alphabetSize = 256)
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

        private int[] GetBinSizes(int[] inputText, int alphabetSize = 256)
        {
            var binSizesForChar = new int[alphabetSize];

            for (var index = 0; index < inputText.Length; index++)
            {
                var c = inputText[index];

                binSizesForChar[c]++;
            }

            return binSizesForChar;
        }

        private void ResetBuckets(Bucket[] buckets)
        {
            foreach (var bucket in buckets) bucket.ResetPointers();
        }
    }
}