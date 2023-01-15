namespace TextIndexierung.SAIS.Helper
{
    internal class ArrayHelper
    {
        /// <summary>
        /// Allocates and initializes an array with a given size and value to initialize each position.
        /// </summary>
        /// <param name="size">Size of the returned array.</param>
        /// <param name="initializeWith">Value to initialize each position with.</param>
        /// <returns>Integer array of size <see cref="size"/> with <see cref="initializeWith"/> on every position.</returns>
        internal static int[] GetInitializedIntArray(int size, int initializeWith)
        {
            var array = new int[size];
            Array.Fill(array, initializeWith);
            return array;
        }
    }
}
