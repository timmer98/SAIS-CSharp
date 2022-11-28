namespace TextIndexierung;

internal class Bucket
{
    internal int StartIndex;
    internal int EndIndex;
    internal int TailPointer;
    internal int HeadPointer;

    public Bucket(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        TailPointer = endIndex - 1;
        HeadPointer = startIndex;
    }

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