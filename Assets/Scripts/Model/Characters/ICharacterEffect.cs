namespace Model.Characters
{
    public interface ICharacterEffect
    {
        void OnTurnStarted(Character character);
        void OnTurnEnded(Character character);
    }
}