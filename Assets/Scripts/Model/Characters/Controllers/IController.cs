namespace Model.Characters.Controllers
{
    public interface IController
    {
        void StartTurn(Character character, Arena arena);
    }
}