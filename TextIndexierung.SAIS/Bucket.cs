using System;

namespace TextIndexierung.SAIS
{
    /// <summary>
    /// Holds the start and end index as well as the current tail and head pointer of a bucket for a character.
    /// </summary>
    internal class Bucket
    {
        internal readonly int StartIndex;
        internal readonly int EndIndex;
        internal int TailPointer;
        internal int HeadPointer;

        public Bucket(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            TailPointer = endIndex - 1;
            HeadPointer = startIndex;
        }

        /// <summary>
        /// Resets pointers of the bucket back to start and end.
        /// </summary>
        public void ResetPointers()
        {
            this.TailPointer = this.EndIndex - 1;
            this.HeadPointer = this.StartIndex;
        }

        public override string ToString()
        {
            return $"Start: {StartIndex} End: {EndIndex}, Tail: {TailPointer}, Head: {HeadPointer}";
        }
    }
}