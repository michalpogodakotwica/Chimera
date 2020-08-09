namespace Model.Characters
{
    public class Damage : IAction
    {
        private readonly int _damage;

        public Damage(int damage)
        {
            _damage = damage;
        }

        public void Apply(Character target)
        {
            target.DealDamage(_damage);
        }
    }
}