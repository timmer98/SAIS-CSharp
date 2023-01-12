namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    /// <summary>
    /// Phi linear time lcp construction algorithm.
    /// </summary>
    public class PhiLinearTimeLcpStrategy : ILcpStrategy
    {
        public int[] ComputeLcpArray(Span<byte> inputText, ArraySegment<int> suffixArray)
        {
            var lcpArray = new int[suffixArray.Count];
            var phiArray = ComputePhiArray(suffixArray);

            int l = 0;

            for (int i = 0; i < suffixArray.Count; i++)
            {
                int j = phiArray[i];

                while (i + l < inputText.Length && j + l < inputText.Length && inputText[i + l] == inputText[j + l])
                {
                    l++;
                }

                phiArray[i] = l;
                l = Math.Max(0, l - 1);
            }

            for (int i = 0; i < phiArray.Length; i++)
            {
                lcpArray[i] = phiArray[suffixArray[i]];
            }

            return lcpArray;
        }

        private int[] ComputePhiArray(ArraySegment<int> suffixArray)
        {
            int[] phiArray = new int[suffixArray.Count];

            phiArray[^1] = suffixArray[^1];

            for (int i = 1; i < suffixArray.Count; i++)
            {
                phiArray[suffixArray[i]] = suffixArray[i - 1];
            }

            return phiArray;
        }
    }
}
