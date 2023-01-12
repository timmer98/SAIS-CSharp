namespace TextIndexierung.SAIS.Model
{
    /// <summary>
    /// The IBaseArray interface is used to get a consistent interface to an array with numbers that fit into the integer space, e.g. int, short, byte.
    /// </summary>
    public interface IBaseArray
    {
        /// <summary>
        /// Length of the array.
        /// </summary>
        public int Length { get; }

        public int this[int key] { get; set; }
    }
}