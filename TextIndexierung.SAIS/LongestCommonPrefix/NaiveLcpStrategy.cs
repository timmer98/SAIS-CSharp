namespace TextIndexierung.SAIS.LongestCommonPrefix;

public class NaiveLcpStrategy : ILcpStrategy
{
    public int[] ComputeLcpArrayParallel(byte[] inputText, int[] suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        Parallel.For(1, inputText.Length, (int i) => LoopBody(inputText, suffixArray, lcpArray, i));

        return lcpArray;
    }
    
    public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        for (int i = 1; i < inputText.Length; i++)
        {
            LoopBody(inputText, suffixArray, lcpArray, i);
        }

        return lcpArray;
    }

    private void LoopBody(ReadOnlySpan<byte> inputText, int[] suffixArray, int[] lcpArray, int i)
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