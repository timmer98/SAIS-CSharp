namespace TextIndexierung.SAIS.LongestCommonPrefix
{
    /// <summary>
    /// Interface to unify LCP construction arrays.
    /// </summary>
    public interface ILcpStrategy
    {
        /// <summary>
        /// Computes the LCP array from the input text and suffix array.
        /// </summary>
        /// <param name="inputText">Input text as Span<byte>.</byte></param>
        /// <param name="suffixArray">Previously calculated suffix array.</param>
        /// <returns>LCP array.</returns>
        public int[] ComputeLcpArray(Span<byte> inputText, ArraySegment<int> suffixArray);
    }
}
