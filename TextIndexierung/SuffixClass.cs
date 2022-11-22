using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung
{
    [Flags]
    internal enum SuffixClass
    {
        Smaller = 0b0001,            
        LeftMostSmaller = 0b0010,    
        Larger = 0b0100              
    }
}
