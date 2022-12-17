using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.SAIS
{
    public class MemoryManager
    {
        public int PeakMemory { get; private set; }

        public void CheckPeak()
        {
            GC.GetTotalMemory(true); // TODO: Check force full collection.
        }
    }
}
