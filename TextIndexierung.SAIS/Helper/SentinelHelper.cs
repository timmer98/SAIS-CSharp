using TextIndexierung.SAIS.Model;

namespace TextIndexierung.SAIS.Helper
{
    internal class SentinelHelper
    {
        /// <summary>
        /// Appends a sentinel to the end of the input text and increments each character value by one.
        /// This has to be reversed after calculation with <see cref="RemoveAppendedSentinelFromSuffixArray"/>.
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <returns></returns>
        internal static IntArray AppendSentinel(byte[] inputBytes)
        {
            var sentinelArray = new IntArray(inputBytes.Length + 1);

            // Create a new array with length + 1 and increment every character by one
            // to handle texts containing the value 0.
            for (int i = 0; i < inputBytes.Length; i++)
            {
                sentinelArray[i] = inputBytes[i] + 1;
            }

            // Sentinel is appended at the end.
            sentinelArray[inputBytes.Length] = 0;

            return sentinelArray;
        }

        /// <summary>
        /// Removes the appended sentinel from the calculated suffix array.
        /// </summary>
        /// <param name="suffixArray">Suffix array with appended sentinel at position 0.</param>
        /// <returns>Suffix array without the extra sentinel as ArraySegment.</returns>
        internal static ArraySegment<int> RemoveAppendedSentinelFromSuffixArray(int[] suffixArray)
        {
            // From here we have to continue with an ArraySegment, which can be used just like a normal array
            // but cuts the first element out (the previously appended sentinel) without allocating a new array.
            var segment = new ArraySegment<int>(suffixArray, 1, suffixArray.Length - 1);
            return segment;
        }
    }
}