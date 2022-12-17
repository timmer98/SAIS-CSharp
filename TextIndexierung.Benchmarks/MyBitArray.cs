using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.Benchmarks
{
    public class MyBitArray
    {
        private readonly byte[] array;

        private const byte BITWISE_AND_MODULO = 8 - 1;

        public int Length { get; }

        public MyBitArray(int size)
        {
            this.Length = size;
            var realSize = (size >> 3) + 1; // Bit shift by 3 to the right corresponds to integer division by 8
            this.array = new byte[realSize];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureIndexBounds(int index)
        {
            if (index > this.Length) throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue(int index, bool value)
        {
            this.EnsureIndexBounds(index);

            var (internalIndex, remainder) = this.GetInternalIndex(index);
            var bit = value ? 1 : 0;
            this.array[internalIndex] = (byte)(this.array[internalIndex] ^ bit << remainder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int, int) GetInternalIndex(int index)
        {
            return (index >> 3, index & BITWISE_AND_MODULO);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetValue(int index)
        {
            this.EnsureIndexBounds(index);

            var (internalIndex, remainder) = GetInternalIndex(index);

            var workingByte = this.array[internalIndex];
            return (workingByte >> remainder & 1) == 1;
        }

        public bool this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }
    }
}
