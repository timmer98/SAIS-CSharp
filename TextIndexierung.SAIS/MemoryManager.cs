namespace TextIndexierung.SAIS
{
    /// <summary>
    /// Memory Manager class is used to check the amount of memory currently managed by the garbage collector.
    /// This is used for the output.
    /// </summary>
    public class MemoryManager
    {
        public long PeakMemoryInBytes { get; private set; }

        public void CheckPeak()
        {
            var currentMemory = GC.GetTotalMemory(true);
           
            if (currentMemory > this.PeakMemoryInBytes)
            {
                this.PeakMemoryInBytes = currentMemory;
            }
        }
    }
}
