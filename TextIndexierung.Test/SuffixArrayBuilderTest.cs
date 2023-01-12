using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextIndexierung.SAIS;
using TextIndexierung.SAIS.Model;

namespace TextIndexierung.Test;

[TestClass]
public class SuffixArrayBuilderTest
{
    public static byte[] LECTURE_TEST_STRING = Encoding.ASCII.GetBytes("ababcabcabba$");

    public static byte[] EXAMPLE_STRING = Encoding.ASCII.GetBytes("immissiissippi$");

    [TestMethod]
    public void BuildSuffixArray_WithLargeLoremIpsumFile_ShouldNotThrow()
    {
        
        // Arrange
        var text = File.ReadAllText("..\\..\\..\\..\\..\\loremipsum.txt");
        var textBytes = Encoding.ASCII.GetBytes(text);
        textBytes = textBytes.Append((byte)0).ToArray();

        var suffixArrayBuilder = new SuffixArrayBuilder();

        // Act
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        // Assert
        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);
        checkResult.Should().BeGreaterThanOrEqualTo(0);
    }

    [TestMethod]
    public void BuildSuffixArray_WithLoremIpsum_ShouldNotThrow()
    {
        // Arrange
        var text = Constants.LOREM_IPSUM;
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var textBytes = Encoding.ASCII.GetBytes(text);

        // Act
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(textBytes);

        // Assert
        var checkResult = SuffixArrayChecker.Check(textBytes, suffixArray, textBytes.Length, true);
        checkResult.Should().BeGreaterThanOrEqualTo(0);
    }

    [TestMethod]
    public void BuildSuffixArray_WithLectureText_ShouldReturnCorrectArray()
    {
        // Arrange
        var suffixArrayBuilder = new SuffixArrayBuilder();

        // Act
        var suffixArray = suffixArrayBuilder.BuildSuffixArray(LECTURE_TEST_STRING);

        // Assert
        suffixArray.ToArray().Should().Equal(12, 11, 0, 8, 5, 2, 10, 1, 9, 6, 3, 7, 4);
        var checkResult = SuffixArrayChecker.Check(LECTURE_TEST_STRING, suffixArray, LECTURE_TEST_STRING.Length, true);
        checkResult.Should().BeGreaterThanOrEqualTo(0);
    }

    [TestMethod]
    public void BuildSuffixArray_WithExampleText_ShouldReturnCorrectArray()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();

        var suffixArray = suffixArrayBuilder.BuildSuffixArray(EXAMPLE_STRING);

        suffixArray.ToArray().Should().Equal(14, 13, 6, 0, 10, 3, 7, 2, 1, 12, 11, 5, 9, 4, 8);
    }

    [TestMethod]
    public void BuildSuffixArray_WithAnotherText_ShouldReturnCorrectSuffixArray()
    {
        // Arrange
        var builder = new SuffixArrayBuilder();
        var text = Encoding.ASCII.GetBytes("ABANANABANDANA$");

        // Act
        var sa = builder.BuildSuffixArray(text);

        // Assert
        sa.ToArray().Should().Equal(14, 13, 0, 6, 11, 4, 2, 8, 1, 7, 10, 12, 5, 3, 9);
    }

    [TestMethod]
    public void MarkSuffixes_WithLectureText_ShouldMarkCorrectly()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var input = new ByteArray(LECTURE_TEST_STRING);
        var marks = suffixArrayBuilder.MarkSuffixes(input);

        marks[0].Should().Be(false);
        marks[1].Should().Be(true);
        marks[2].Should().Be(false);
        marks[3].Should().Be(false);
        marks[4].Should().Be(true);
        marks[5].Should().Be(false);
        marks[6].Should().Be(false);
        marks[7].Should().Be(true);
        marks[8].Should().Be(false);
        marks[9].Should().Be(true);
        marks[10].Should().Be(true);
        marks[11].Should().Be(true);
        marks[12].Should().Be(false);
    }

    [TestMethod]
    public void GetBuckets_WithLectureText_ShouldReturnCorrectBuckets()
    {
        var suffixArrayBuilder = new SuffixArrayBuilder();
        var input = new ByteArray(LECTURE_TEST_STRING);
        var buckets = suffixArrayBuilder.GetBuckets(input);

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