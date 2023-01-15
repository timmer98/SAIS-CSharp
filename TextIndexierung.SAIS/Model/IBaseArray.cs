namespace TextIndexierung.SAIS.Model
{
    /// <summary>
    /// The IBaseArray interface is used to get a consistent interface to an array with numbers that fit into the integer space, e.g. int, short, byte.
    /// Could be used to implement e.g. a long array in the future to support larger text files.
    /// </summary>
    public interface IBaseArray
    {
        /// <summary>
        /// Length of the array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Index operator to access a single element by index.
        /// </summary>
        /// <param name="key">Index of an element.</param>
        /// <returns>Item at the given index.</returns>
        public int this[int key] { get; set; }
    }
}