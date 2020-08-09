namespace Model.Board.SquareGridBoard
{
    public class Tile : Field
    {
        public readonly int X;
        public readonly int Y;
        
        public Tile(string id, int x, int y) : base(id)
        {
            X = x;
            Y = y;
        }
    }
}