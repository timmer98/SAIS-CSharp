using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.Models
{
    internal class IntArray : IBaseArray {
        private readonly int[] _array;

        public IntArray(int[] array)
        {
            this._array = array;
        }

        int IBaseArray.this[int key]
        {
            get => _array[key];
            set => _array[key] = value;
        }

        int IBaseArray.Length => _array.Length;

        public bool AreEntriesDistinct()
        {
            if (new HashSet<int>(_array).Count == _array.Length)
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
