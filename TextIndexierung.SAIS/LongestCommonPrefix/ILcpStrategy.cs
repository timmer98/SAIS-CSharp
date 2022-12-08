namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    internal interface ILcpStrategy
    {
        public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray);
    }
}
