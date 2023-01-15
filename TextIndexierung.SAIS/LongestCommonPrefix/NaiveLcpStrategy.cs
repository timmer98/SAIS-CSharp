namespace TextIndexierung.SAIS.LongestCommonPrefix;

/// <summary>
/// Naive lcp construction in O(n^2) time.
/// </summary>
public class NaiveLcpStrategy : ILcpStrategy
{
    /// <summary>
    /// Naive lcp construction naively parallelized.
    /// </summary>
    public int[] ComputeLcpArrayParallel(byte[] inputText, ArraySegment<int> suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        Parallel.For(1, inputText.Length, (int i) =>
        {
            var lcpLength = GetLcpLength(inputText.AsSpan().Slice(suffixArray[i - 1]), inputText.AsSpan().Slice(suffixArray[i]));
            lcpArray[i] = lcpLength;
        });

        return lcpArray;
    }
    
    /// <summary>
    /// Standard sequential naive lcp construction.
    /// </summary>
    public int[] ComputeLcpArray(Span<byte> inputText, ArraySegment<int> suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        for (int i = 1; i < inputText.Length; i++)
        {
            var lcpLength = GetLcpLength(inputText.Slice(suffixArray[i - 1]), inputText.Slice(suffixArray[i]));
            lcpArray[i] = lcpLength;
        }

        return lcpArray;
    }

    /// <summary>
    /// Gets length of common prefixes.
    /// </summary>
    /// <param name="a">Text span a.</param>
    /// <param name="b">Text span b to compare with.</param>
    /// <returns>Length of equal characters in both texts.</returns>
    private int GetLcpLength(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        var shortestLenght = a.Length < b.Length ? a.Length : b.Length;

        for (var i = 0; i < shortestLenght; i++)
            if (a[i] != b[i])
                return i;

        return shortestLenght;
    }
}