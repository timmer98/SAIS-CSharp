namespace TextIndexierung.SAIS.LongestCommonPrefix;

internal class NaiveLcpStrategy : ILcpStrategy
{
    public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray)
    {
        var lcpArray = new int[inputText.Length];
        lcpArray[0] = 0;

        for (var i = 1; i < inputText.Length; i++)
        {
            var lcpLength = GetLcpLength(inputText.Slice(suffixArray[i - 1]), inputText.Slice(suffixArray[i]));
            lcpArray[i] = lcpLength;
        }

        return lcpArray;
    }

    private int GetLcpLength(Span<byte> a, Span<byte> b)
    {
        var shortestLenght = a.Length < b.Length ? b.Length : a.Length;

        for (var i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return i;

        return shortestLenght;
    }
}