namespace TextIndexierung.SAIS.Model
{
    public interface IBaseArray
    {
        public int Length { get; }

        public int this[int key] { get; set; }
    }
}