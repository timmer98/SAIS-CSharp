namespace TextIndexierung.SAIS.LongestCommonPrefix;

/// <summary>
/// Naive lcp construction in O(n^2) time.
/// </summary>
public class NaiveLcpStrategy : ILcpStrategy
{
    /// <summary>
    /// Naive lcp construction naively parallelized.
    /// </summary>
    public int[] ComputeLcpArrayParallel(byte[] inputText, int[] suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        Parallel.For(1, inputText.Length, (int i) => LoopBody(inputText, suffixArray, lcpArray, i));

        return lcpArray;
    }
    
    /// <summary>
    /// Standard sequential naive lcp construction.
    /// </summary>
    public int[] ComputeLcpArray(Span<byte> inputText, Span<int> suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        for (int i = 1; i < inputText.Length; i++)
        {
            LoopBody(inputText, suffixArray, lcpArray, i);
        }

        return lcpArray;
    }

    private void LoopBody(ReadOnlySpan<byte> inputText, Span<int> suffixArray, Span<int> lcpArray, int i)
    {
        var lcpLength = GetLcpLength(inputText.Slice(suffixArray[i - 1]), inputText.Slice(suffixArray[i]));
        lcpArray[i] = lcpLength;
    }

    private int GetLcpLength(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        var shortestLenght = a.Length < b.Length ? b.Length : a.Length;

        for (var i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return i;

        return shortestLenght;
    }
}