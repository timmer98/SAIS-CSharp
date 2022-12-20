namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    public interface ILcpStrategy
    {
        public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray);
    }
}
