using Model.Characters;

namespace Model.Board
{
    public interface IFieldEffect
    {
        void OnEnterTile(Character character);
        void OnExitTile(Character character);
    }
}