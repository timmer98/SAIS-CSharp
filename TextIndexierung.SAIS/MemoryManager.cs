namespace TextIndexierung.SAIS
{
    /// <summary>
    /// Memory Manager class is used to check the amount of memory currently managed by the garbage collector.
    /// This is used for the output.
    /// </summary>
    public class MemoryManager
    {
        public long PeakMemoryInBytes { get; private set; }

        /// <summary>
        /// Needs to be called after memory allocation to check and save the current peak memory consumption.
        /// </summary>
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
