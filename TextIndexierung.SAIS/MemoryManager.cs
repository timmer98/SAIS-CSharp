namespace TextIndexierung.SAIS
{
    public class MemoryManager
    {
        public long PeakMemoryInBytes { get; private set; }

        public void CheckPeak()
        {
            var currentMemory = GC.GetTotalMemory(true); // TODO: Check force full collection.
           
            if (currentMemory > this.PeakMemoryInBytes)
            {
                this.PeakMemoryInBytes = currentMemory;
            }
        }
    }
}
