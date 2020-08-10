using System.Collections.Generic;
using Model.Board;

namespace Model.Characters
{
    public interface IAttackAction
    {
        void ApplyOnTarget(Character user, Field target);
        IEnumerable<Field> ValidTargets(Character character, IBoard board);
    }
}