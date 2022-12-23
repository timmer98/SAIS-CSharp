namespace TextIndexierung.SAIS.Model
{
    public class IntArray : IBaseArray
    {
        private int[] _array;

        public IntArray(int size)
        {
            this._array = new int[size];
        }

        public int Length => _array.Length;

        public int this[int key]
        {
            get => _array[key];
            set => _array[key] = value;
        }
    }
}