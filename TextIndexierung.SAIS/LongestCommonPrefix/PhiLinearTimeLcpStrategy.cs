namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    public class PhiLinearTimeLcpStrategy : ILcpStrategy
    {
        public int[] ComputeParallelLcpArray(byte[] inputText, int[] suffixArray)
        {
            var lcpArray = new int[suffixArray.Length];
            var phiArray = ComputePhiArray(suffixArray);

            int l = 0;

            for (int i = 0; i < suffixArray.Length; i++)
            {
                int j = phiArray[i];

                while (inputText[i + l] == inputText[j + l])
                {
                    l = l + 1;
                }

                phiArray[i] = l;
                l = Math.Max(0, l - 1);
            }

            for (int i = 0; i < suffixArray.Length; i++)
            {
                lcpArray[i] = phiArray[suffixArray[i]];
            }

            return lcpArray;
        }


        public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray)
        {
            var lcpArray = new int[suffixArray.Length];
            var phiArray = ComputePhiArray(suffixArray);

            int l = 0;

            for (int i = 0; i < suffixArray.Length; i++)
            {
                int j = phiArray[i];

                while (inputText[i + l] == inputText[j + l])
                {
                    l = l + 1;
                }

                phiArray[i] = l;
                l = Math.Max(0, l - 1);
            }

            Parallel.For(0, suffixArray.Length, (i) =>
            {
                lcpArray[i] = phiArray[suffixArray[i]];
            });

            return lcpArray;
        }

        private int[] ComputePhiArray(int[] suffixArray)
        {
            int[] phiArray = new int[suffixArray.Length];

            for (int i = 1; i < suffixArray.Length; i++)
            {
                phiArray[suffixArray[i]] = suffixArray[i - 1];
            }

            return phiArray;
        }

        private int[] ComputeParallelPhiArray(int[] suffixArray)
        {
            int[] phiArray = new int[suffixArray.Length];

            Parallel.For(1, suffixArray.Length, (i) =>
            {
                phiArray[suffixArray[i]] = suffixArray[i - 1];

            });

            return phiArray;
        }

    }
}
