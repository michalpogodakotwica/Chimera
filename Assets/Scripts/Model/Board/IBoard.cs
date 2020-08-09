using System.Collections.Generic;

namespace Model.Board
{
    public interface IBoard
    {
        IEnumerable<Field> AllFields { get; }
        IEnumerable<Field> GetFieldsInRange(Field start, float distance, CollisionDetectionType collisionDetectionType);
        bool TryToGetPath(Field start, Field end, out List<Field> path, out float length);
    }
}