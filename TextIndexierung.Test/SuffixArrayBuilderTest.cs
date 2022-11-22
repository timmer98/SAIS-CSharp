using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextIndexierung.Test;

[TestClass]
public class SuffixArrayBuilderTest
{
    public static byte[] LECTURE_TEST_STRING = Encoding.ASCII.GetBytes("ababcabcabba$");

    public static byte[] EXAMPLE_STRING = Encoding.ASCII.GetBytes("immissiissippi$");

    [TestMethod]
    public void BuildSuffixArray_WithLectureText_ShouldReturnCorrectArray()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();

        var suffixArray = suffixArrayBuilder.BuildSuffixArray(LECTURE_TEST_STRING);

        suffixArray[0].Should().Be(12);
        suffixArray[1].Should().Be(11);
        suffixArray[2].Should().Be(0);
        suffixArray[3].Should().Be(8);
        suffixArray[4].Should().Be(5);
        suffixArray[5].Should().Be(2);
        suffixArray[6].Should().Be(10);
        suffixArray[7].Should().Be(1);
        suffixArray[8].Should().Be(9);
        suffixArray[9].Should().Be(6);
        suffixArray[10].Should().Be(3);
        suffixArray[11].Should().Be(7);
        suffixArray[12].Should().Be(4);
    }

    [TestMethod]
    public void BuildSuffixArray_WithExampleText_ShouldReturnCorrectArray()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();

        var suffixArray = suffixArrayBuilder.BuildSuffixArray(EXAMPLE_STRING);

        suffixArray[0].Should().Be(14);
        suffixArray[1].Should().Be(13);
        suffixArray[2].Should().Be(6);
        suffixArray[3].Should().Be(0);
        suffixArray[4].Should().Be(10);
        suffixArray[5].Should().Be(3);
        suffixArray[6].Should().Be(7);
        suffixArray[7].Should().Be(2);
        suffixArray[8].Should().Be(1);
        suffixArray[9].Should().Be(12);
        suffixArray[10].Should().Be(11);
        suffixArray[11].Should().Be(5);
        suffixArray[12].Should().Be(9);
        suffixArray[13].Should().Be(4);
        suffixArray[14].Should().Be(8);
    }

    [TestMethod]
    public void MarkSuffixes_WithLectureText_ShouldMarkCorrectly()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var marks = suffixArrayBuilder.MarkSuffixes(LECTURE_TEST_STRING);

        marks[0].Should().Be(SuffixClass.Smaller);
        marks[1].Should().Be(SuffixClass.Larger);
        marks[2].Should().Be(SuffixClass.LeftMostSmaller);
        marks[3].Should().Be(SuffixClass.Smaller);
        marks[4].Should().Be(SuffixClass.Larger);
        marks[5].Should().Be(SuffixClass.LeftMostSmaller);
        marks[6].Should().Be(SuffixClass.Smaller);
        marks[7].Should().Be(SuffixClass.Larger);
        marks[8].Should().Be(SuffixClass.LeftMostSmaller);
        marks[9].Should().Be(SuffixClass.Larger);
        marks[10].Should().Be(SuffixClass.Larger);
        marks[11].Should().Be(SuffixClass.Larger);
        marks[12].Should().Be(SuffixClass.LeftMostSmaller);
    }

    [TestMethod]
    public void GetBuckets_WithLectureText_ShouldReturnCorrectBuckets()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var buckets = suffixArrayBuilder.GetBuckets(LECTURE_TEST_STRING);

        var dollarBucket = buckets[36]; // Buckets indexed by ASCII value
        var aBucket = buckets[97];
        var bBucket = buckets[98];
        var cBucket = buckets[99];

        dollarBucket.StartIndex.Should().Be(0);
        dollarBucket.EndIndex.Should().Be(1);
        dollarBucket.TailPointer.Should().Be(0);
        dollarBucket.HeadPointer.Should().Be(0);

        aBucket.StartIndex.Should().Be(1);
        aBucket.EndIndex.Should().Be(6);
        aBucket.TailPointer.Should().Be(5);
        aBucket.HeadPointer.Should().Be(1);

        bBucket.StartIndex.Should().Be(6);
        bBucket.EndIndex.Should().Be(11);
        bBucket.TailPointer.Should().Be(10);
        bBucket.HeadPointer.Should().Be(6);

        cBucket.StartIndex.Should().Be(11);
        cBucket.EndIndex.Should().Be(13);
        cBucket.TailPointer.Should().Be(12);
        cBucket.HeadPointer.Should().Be(11);
    }
}