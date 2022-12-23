namespace TextIndexierung.SAIS.Model
{
    public class ByteArray : IBaseArray
    {
        private byte[] _array;

        public ByteArray(int size)
        {
            _array = new byte[size];
        }

        public ByteArray(byte[] array)
        {
            this._array = array;
        }

        public int Length => _array.Length;

        public int this[int key]
        {
            get => _array[key];
            set => _array[key] = (byte)value;
        }
    }
}
