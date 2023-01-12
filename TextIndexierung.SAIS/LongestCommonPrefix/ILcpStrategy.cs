namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    public interface ILcpStrategy
    {
        public int[] ComputeLcpArray(Span<byte> inputText, ArraySegment<int> suffixArray);
    }
}
