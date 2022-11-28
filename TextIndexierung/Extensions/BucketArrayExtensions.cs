namespace TextIndexierung.Extensions
{
    internal static class BucketArrayExtensions
    {
        internal static void ResetBucketsPointers(this Bucket[] buckets)
        {
            foreach (var bucket in buckets)
            {
                bucket.ResetPointers();
            }
        }
    }
}