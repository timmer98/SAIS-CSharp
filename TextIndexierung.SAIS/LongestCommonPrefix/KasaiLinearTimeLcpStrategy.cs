namespace TextIndexierung.SAIS.LongestCommonPrefix;

/// <summary>
/// Implementation of the Kasai linear time longest common prefix array construction algorithm.
/// </summary>
public class KasaiLinearTimeLcpStrategy : ILcpStrategy
{
    public int[] ComputeLcpArrayParallel(byte[] inputText, int[] suffixArray)
    {
        var lcpArray = new int[suffixArray.Length];
        var inverseSuffixArray = ComputeInverseLcpParallel(suffixArray);

        var l = 0;
        lcpArray[0] = 0;

        for (var i = 0; i < suffixArray.Length; i++)
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

    private int[] ComputeInverseLcpParallel(int[] suffixArray)
    {
        var inverseSuffixArray = new int[suffixArray.Length];

        var quarter = (int) suffixArray.Length / 4;

        var task1 = Task.Run(() =>
        {
            for (var i = 0; i < quarter; i++) inverseSuffixArray[suffixArray[i]] = i;
        });

        var task2 = Task.Run(() =>
        {
            for (var i = quarter + 1; i < 2*quarter; i++) inverseSuffixArray[suffixArray[i]] = i;
        });

        var task3 = Task.Run(() =>
        {
            for (var i = 2 * quarter + 1; i < 3 * quarter; i++) inverseSuffixArray[suffixArray[i]] = i;
        });

        var task4 = Task.Run(() =>
        {
            for (var i = 3 * quarter + 1; i < suffixArray.Length; i++) inverseSuffixArray[suffixArray[i]] = i;
        });

        Task.WaitAll(new Task[]{ task1, task2, task3, task4});

        return inverseSuffixArray;
    }

    public int[] ComputeLcpArray(Span<byte> inputText, Span<int> suffixArray)
    {
        var lcpArray = new int[suffixArray.Length];
        var inverseSuffixArray = ComputeInverseLcp(suffixArray);

        var l = 0;
        lcpArray[0] = 0;

        for (var i = 0; i < suffixArray.Length; i++)
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

    private int[] ComputeInverseLcp(Span<int> suffixArray)
    {
        var inverseSuffixArray = new int[suffixArray.Length];

        for (var i = 0; i < inverseSuffixArray.Length; i++) inverseSuffixArray[suffixArray[i]] = i;

        return inverseSuffixArray;
    }
}