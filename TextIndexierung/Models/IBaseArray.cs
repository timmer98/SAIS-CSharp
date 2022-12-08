using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextIndexierung.Models
{
    internal interface IBaseArray
    {
        internal int this[int key] { get; set; }

        internal int Length { get; }

        bool AreEntriesDistinct();
    }
}
