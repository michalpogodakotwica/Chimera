namespace Model.Characters
{
    public interface IAction
    {
        void Apply(Character target);
    }
}