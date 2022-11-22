using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.LongestCommonPrefix
{
    internal interface ILcpStrategy
    {
        public int[] ComputeLcpArray(Span<byte> inputText, int[] suffixArray);
    }
}
