using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.Models
{
    internal class ByteArray : IBaseArray
    {
        private readonly byte[] _array;

        public ByteArray(byte[] array)
        {
            _array = array;
        }

        int IBaseArray.this[int key]
        {
            get => _array[key];
            set => _array[key] = (byte)value;
        }

        int IBaseArray.Length => _array.Length;

        public bool AreEntriesDistinct()
        {
            if (new HashSet<byte>(_array).Count == _array.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
