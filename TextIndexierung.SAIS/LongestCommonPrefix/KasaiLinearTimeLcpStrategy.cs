namespace TextIndexierung.SAIS.LongestCommonPrefix;

/// <summary>
/// Implementation of the Kasai linear time longest common prefix array construction algorithm.
/// </summary>
public class KasaiLinearTimeLcpStrategy : ILcpStrategy
{
    public int[] ComputeLcpArray(Span<byte> inputText, ArraySegment<int> suffixArray)
    {
        var lcpArray = new int[suffixArray.Count];
        var inverseSuffixArray = ComputeInverseLcp(suffixArray);

        var l = 0;
        lcpArray[0] = 0;

        for (var i = 0; i < suffixArray.Count; i++)
        {
            if (inverseSuffixArray[i] != 0)
            {
                var j = suffixArray[inverseSuffixArray[i] - 1];
                while (inputText[i + l] == inputText[j + l])
                {
                    l++;
                }

                lcpArray[inverseSuffixArray[i]] = l;
                l = l > 0 ? l - 1 : 0;
            }
        }

        return lcpArray;
    }

    private int[] ComputeInverseLcp(ArraySegment<int> suffixArray)
    {
        var inverseSuffixArray = new int[suffixArray.Count];

        for (var i = 0; i < inverseSuffixArray.Length; i++) inverseSuffixArray[suffixArray[i]] = i;

        return inverseSuffixArray;
    }
}