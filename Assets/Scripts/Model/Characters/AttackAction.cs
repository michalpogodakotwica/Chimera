using System.Collections.Generic;
using System.Linq;
using Model.Board;
using UnityEngine;

namespace Model.Characters
{
    public class AttackAction : IAttackAction
    {
        private readonly int _damage;
        private readonly int _range;
        
        public AttackAction(int damage, int range)
        {
            _damage = damage;
            _range = range;
        }

        public void ApplyOnTarget(Character user, Field target)
        {
            if(target.OccupiedBy == null)
                return;
            Debug.Log($"{user.ID} attacks {target.OccupiedBy.ID}");
            target.OccupiedBy.DealDamage(_damage);
        }

        public IEnumerable<Field> ValidTargets(Character character, IBoard board)
        {
            return board.GetFieldsInRange(character.OccupiedField, _range, CollisionDetectionType.None)
                .Where(f => f.OccupiedBy != null && f.OccupiedBy.Team != character.Team);
        }
    }
}