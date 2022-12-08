using System.Xml;
using TextIndexierung.Extensions;
using TextIndexierung.Models;

namespace TextIndexierung.Old;

public class SuffixArrayBuilder
{
    public int[] BuildSuffixArray(byte[] inputBytes)
    {
        var inputText = inputBytes.Select(x => (int)x).ToArray();
        return BuildSuffixArray(inputText);
    }

    private int[] BuildSuffixArray(int[] inputText, int alphabetSize = 256)
    {
        var buckets = GetBuckets(inputText, alphabetSize);
        var suffixMarks = MarkSuffixes(inputText);

        return Induce(inputText, buckets, suffixMarks);
    }

    private int[] Induce(int[] inputText, Bucket[] buckets, SuffixClass[] suffixMarks)
    {
        var suffixArray = Enumerable.Repeat(-1, inputText.Length).ToArray();
        PlaceLmsSuffixes(inputText, buckets, suffixMarks, suffixArray);
        InduceLType(inputText, suffixArray, buckets, suffixMarks);
        InduceSType(inputText, suffixArray, buckets, suffixMarks);
        var (reducedText, offsets, alphabetSize) = CreateLmsNames(inputText, suffixArray, suffixMarks);

        int[] sa1 = BuildSa1(reducedText, alphabetSize);

        suffixArray = InduceSuffixArrayFromSa1(inputText, suffixArray, sa1, suffixMarks, buckets, offsets);

        return suffixArray;
    }

    private int[] BuildSa1(int[] reducedText, int alphabetSize)
    {
        int[] sa1;

        if (alphabetSize == reducedText.Length)
        {
            // Unique, dann counting sort
            sa1 = new int[reducedText.Length];

            for (int i = 0; i < reducedText.Length; i++)
            {
                sa1[reducedText[i]] = i;
            }
        }
        else
        {
            // Fire a recursive call
            sa1 = BuildSuffixArray(reducedText, alphabetSize);
        }

        return sa1;
    }

    private int[] InduceSuffixArrayFromSa1(int[] inputText, int[] suffixArray, int[] summarySuffixArray,
        SuffixClass[] suffixMarks, Bucket[] buckets, int[] offsets)
    {
        buckets.ResetBucketsPointers();
        suffixArray = Enumerable.Repeat(-1, suffixArray.Length).ToArray();

        for (var i = summarySuffixArray.Length - 1; i >= 0; i--)
        {
            var charIndex = offsets[summarySuffixArray[i]];
            var bucketIndex = inputText[charIndex];
            var bucket = buckets[bucketIndex];
            
            suffixArray[bucket.TailPointer] = charIndex;
            bucket.TailPointer--;
        }

        buckets.ResetBucketsPointers();
        InduceLType(inputText, suffixArray, buckets, suffixMarks);
        InduceSType(inputText, suffixArray, buckets, suffixMarks);

        return suffixArray;
    }

    private (int[] reducedText, int[] offsets, int alphabetSize) CreateLmsNames(int[] inputText, int[] suffixArray,
        SuffixClass[] suffixMarks)
    {
        var lmsNames = Enumerable.Repeat(-1, suffixArray.Length).ToArray();
        var counter = 0;
        lmsNames[suffixArray[0]] = counter;
        var previous = suffixArray[0];
        var reducedTextSize = 1;

        for (var i = 0; i < suffixArray.Length; i++)
            if (suffixArray[i] >= 0 && suffixMarks[suffixArray[i]] == SuffixClass.LeftMostSmaller)
            {
                if (!AreLmsSubstringsEqual(inputText, previous, suffixArray[i], suffixMarks)) counter++;

                previous = suffixArray[i];
                lmsNames[suffixArray[i]] = counter;
                reducedTextSize++;
            }

        var reducedText = new int[reducedTextSize];
        var offsets = new int[reducedTextSize];
        var reducedTextIndex = 0;

        for (var i = 0; i < lmsNames.Length; i++)
            if (lmsNames[i] != -1)
            {
                reducedText[reducedTextIndex] = (byte)lmsNames[i];
                offsets[reducedTextIndex] = i;
                reducedTextIndex++;
            }

        return (reducedText, offsets, counter + 1);
    }

    private bool AreLmsSubstringsEqual(int[] text, int previousOffset, int currentOffset, SuffixClass[] suffixMarks)
    {
        if (previousOffset == text.Length - 1 || currentOffset == text.Length - 1) return false;
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

    private void PlaceLmsSuffixes(int[] inputText, Bucket[] buckets, SuffixClass[] suffixMarks, int[] suffixArray)
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

    }

    private void InduceLType(int[] inputText, int[] suffixArray, Bucket[] buckets, SuffixClass[] suffixMarks)
    {
        // Scan from left to right for L suffixes
        for (var i = 0; i < inputText.Length; i++)
        {
            // if T(SA[i] - 1) is L-Type)
            if (suffixArray[i] > 0 && suffixMarks[suffixArray[i] - 1] == SuffixClass.Larger)
            {
                var character = inputText[suffixArray[i] - 1];
                var bucket = buckets[character];
                suffixArray[bucket.HeadPointer] = suffixArray[i] - 1;
                bucket.HeadPointer++;
            }
        }
    }

    private void InduceSType(int[] inputText, int[] suffixArray, Bucket[] buckets, SuffixClass[] suffixMarks) 
    {
        // Scan from right to left for S suffixes
        buckets.ResetBucketsPointers();

        for (var i = inputText.Length - 1; i >= 0; i--)
        {
            if (suffixArray[i] - 1 < 0) continue;

            // if T(SA[i] - 1) is S-Type
            var type = suffixMarks[suffixArray[i] - 1];

            if (type == SuffixClass.Smaller || type == SuffixClass.LeftMostSmaller)
            {
                var character = inputText[suffixArray[i] - 1];
                var bucket = buckets[character];
                suffixArray[bucket.TailPointer] = suffixArray[i] - 1;
                bucket.TailPointer--;
            }
        }
    }

    internal SuffixClass[] MarkSuffixes(int[] inputText)
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
    /// <param name="alphabetSize"></param>
    /// <returns>Dictionary where each character is mapped to its frequency.</returns>
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



    // This should have O(1) because we iterate max byte.MaxValue times.
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
}