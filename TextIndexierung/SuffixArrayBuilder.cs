using System.Xml;

namespace TextIndexierung;

public class SuffixArrayBuilder
{
    public int[] BuildSuffixArray(byte[] inputText)
    {
        var buckets = GetBuckets(inputText);
        var suffixMarks = MarkSuffixes(inputText);

        return Induce(inputText, buckets, suffixMarks);
    }

    private int[] Induce(byte[] inputText, Bucket[] buckets, SuffixClass[] suffixMarks)
    {
        var suffixArray = Enumerable.Repeat(-1, inputText.Length).ToArray();

        InitializeSuffixArray(inputText, buckets, suffixMarks, suffixArray);
        var (reducedText, offsets) = CreateLmsNames(inputText, suffixArray, suffixMarks);

        int[] recursionSuffixArray;

        if (new HashSet<byte>(reducedText).Count == reducedText.Length)
        {
            // Unique, dann counting sort
            recursionSuffixArray = reducedText.OrderBy(a => a).Select(x => (int)x).ToArray();
        }
        else
        {
            recursionSuffixArray = BuildSuffixArray(reducedText);
            // Hier irgendwie noch rueckmapping
        }

        PostitionLmsCharacters(inputText, suffixArray, recursionSuffixArray, suffixMarks, buckets, offsets);

        return suffixArray;
    }

    private void PostitionLmsCharacters(byte[] inputText, int[] suffixArray, int[] summarySuffixArray, SuffixClass[] suffixMarks, Bucket[] buckets, int[] offsets)
    {
        ResetBucketsPointers(buckets);
        suffixArray[0] = inputText.Length - 1;

        for (int i = summarySuffixArray.Length - 1; i > 1; i--)
        {
            var charIndex = offsets[summarySuffixArray[i]];
            var bucketIndex = inputText[charIndex];
            var bucket = buckets[bucketIndex];

            suffixArray[bucket.TailPointer] = charIndex;
            bucket.TailPointer--;
        }

        ResetBucketsPointers(buckets);
        this.InduceLeftRight(inputText, suffixArray, buckets, suffixMarks);
    }

    private (byte[] reducedText, int[] offsets) CreateLmsNames(byte[] inputText, int[] suffixArray,
        SuffixClass[] suffixMarks)
    {
        var lmsNames = Enumerable.Repeat(-1, suffixArray.Length).ToArray();
        var counter = 0;
        lmsNames[suffixArray[0]] = counter;
        var previous = suffixArray[0];
        var reducedTextSize = 1;

        for (var i = 1; i < suffixArray.Length; i++)
            if (suffixMarks[i] == SuffixClass.LeftMostSmaller)
            {
                if (!AreLmsBlocksEqual(inputText, previous, suffixArray[i], suffixMarks)) counter++;

                previous = suffixArray[i];
                lmsNames[suffixArray[i]] = counter;
                reducedTextSize++;
            }


        var reducedText = new byte[reducedTextSize];
        var offsets = new int[reducedTextSize];
        var reducedTextIndex = 0;

        for (var i = 0; i < lmsNames.Length; i++)
            if (lmsNames[i] != -1)
            {
                reducedText[reducedTextIndex] = (byte)lmsNames[i];
                offsets[reducedTextIndex] = i;
                reducedTextIndex++;
            }

        return (reducedText, offsets);
    }

    private bool AreLmsBlocksEqual(byte[] text, int previousOffset, int currentOffset, SuffixClass[] suffixMarks)
    {
        if (previousOffset == text.Length || currentOffset == text.Length) return false;
        if (text[previousOffset] != text[currentOffset]) return false;

        for (var i = 1; i < text.Length; i++)
        {
            if (text[previousOffset + i] != text[currentOffset + i]) return false;

            var prevIsLms = suffixMarks[previousOffset + i] == SuffixClass.LeftMostSmaller;
            var currentIsLms = suffixMarks[currentOffset + i] == SuffixClass.LeftMostSmaller;

            if (prevIsLms != currentIsLms) return false;
            if (prevIsLms && currentIsLms) return true;
        }

        return false;
    }

    private void InitializeSuffixArray(byte[] inputText, Bucket[] buckets, SuffixClass[] suffixMarks, int[] suffixArray)
    {
        // Scan from left to right and insert S* suffixes at the end of its bucket
        for (var i = 0; i < inputText.Length; i++)
            if (suffixMarks[i] == SuffixClass.LeftMostSmaller)
            {
                var character = inputText[i];
                var bucket = buckets[character];
                suffixArray[bucket.TailPointer] = i;
                bucket.TailPointer--;
            }

        InduceLeftRight(inputText, suffixArray, buckets, suffixMarks);
    }

    private void InduceLeftRight(byte[] inputText, int[] suffixArray, Bucket[] buckets, SuffixClass[] suffixMarks)
    {
        int countInsertions = 0;

        // Scan from left to right for L suffixes
        for (var i = 0; i < inputText.Length; i++)
            // if T(SA[i] - 1) is L-Type)
            if (suffixArray[i] > 0 && suffixMarks[suffixArray[i] - 1] == SuffixClass.Larger)
            {
                var character = inputText[suffixArray[i] - 1];
                var bucket = buckets[character];
                suffixArray[bucket.HeadPointer] = suffixArray[i] - 1;
                bucket.HeadPointer++;

                countInsertions++;
            }

        // Scan from right to left for S suffixes
        ResetBucketsPointers(buckets);

        for (var i = inputText.Length - 1; i > 1; i--)
        {
            // if T(SA[i] - 1) is S-Type
            var type = suffixMarks[suffixArray[i] - 1];

            if (type == SuffixClass.Smaller || type == SuffixClass.LeftMostSmaller)
            {
                var character = inputText[suffixArray[i] - 1];
                var bucket = buckets[character];
                suffixArray[bucket.TailPointer] = suffixArray[i] - 1;
                bucket.TailPointer--;

                countInsertions++;

                if (countInsertions == inputText.Length - 1) break;
            }
        }
    }

    internal SuffixClass[] MarkSuffixes(byte[] inputText)
    {
        var suffixClasses = new SuffixClass[inputText.Length];
        var n = inputText.Length; // Rename length to have naming from definition

        // Per definition: rightmost suffix is always type S
        suffixClasses[--n] = SuffixClass.Smaller;

        for (var i = n - 1; i >= 0; i--)
        {
            var previousChar = inputText[i + 1];
            var currentChar = inputText[i];

            if (currentChar < previousChar)
            {
                suffixClasses[i] = SuffixClass.Smaller;
            }
            else if (currentChar > previousChar)
            {
                suffixClasses[i] = SuffixClass.Larger;

                if (suffixClasses[i + 1] == SuffixClass.Smaller)
                    // If previous char was already S-Type and this one is L, than previous is a leftmost S-suffix
                    suffixClasses[i + 1] = SuffixClass.LeftMostSmaller;
            }
            else
            {
                suffixClasses[i] = suffixClasses[i + 1];
            }
        }

        return suffixClasses;
    }

    /// <summary>
    ///     Suffix array induced sorting step 1:
    ///     Get bin sizes for each character.
    /// </summary>
    /// <param name="inputText"></param>
    /// <returns>Dictionary where each character is mapped to its frequency.</returns>
    private int[] GetBinSizes(byte[] inputText)
    {
        var binSizesForChar = new int[byte.MaxValue];

        for (var index = 0; index < inputText.Length; index++)
        {
            var c = inputText[index];

            binSizesForChar[c]++;
        }

        return binSizesForChar;
    }

    // This should have O(1) because we iterate max byte.MaxValue times.
    internal Bucket[] GetBuckets(byte[] inputText)
    {
        var binSizesForChar = GetBinSizes(inputText);

        var buckets = new Bucket[byte.MaxValue];
        // TailPointer starts on the end, HeadPointer starts on the starting index
        var previousBucket = new Bucket
        { StartIndex = 0, EndIndex = binSizesForChar[0], TailPointer = binSizesForChar[0] - 1, HeadPointer = 0 };
        buckets[0] = previousBucket;

        for (var i = 1; i < binSizesForChar.Length; i++)
        {
            var startIndex = previousBucket.EndIndex;
            var endIndex = binSizesForChar[i] + previousBucket.EndIndex;

            var bucket = new Bucket
            {
                StartIndex = startIndex,
                EndIndex = endIndex,
                TailPointer = endIndex - 1,
                HeadPointer = startIndex
            };

            previousBucket = bucket;
            buckets[i] = bucket;
        }

        return buckets;
    }

    private void ResetBucketsPointers(Bucket[] buckets)
    {
        foreach (var bucket in buckets)
        {
            bucket.TailPointer = bucket.EndIndex - 1;
            bucket.HeadPointer = bucket.StartIndex;
        }
    }
}