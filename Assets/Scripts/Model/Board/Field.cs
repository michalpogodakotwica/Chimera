using System.Collections.Generic;
using Model.Characters;
using UniRx;

namespace Model.Board
{
    public class Field
    {
        public readonly string ID;
        public Character OccupiedBy { get; private set; }
        
        public readonly ReactiveProperty<bool> IsWalkable = new ReactiveProperty<bool>(true);
        
        private readonly List<IFieldEffect> _fieldModifiers = new List<IFieldEffect>();

        public Field(string id)
        {
            ID = id;
        }

        public void Enter(Character character)
        {
            OccupiedBy = character;
            foreach (var fieldModifier in _fieldModifiers)
                fieldModifier.OnEnterTile(character);
        }

        public void Exit(Character character)
        {
            OccupiedBy = null;
            foreach (var fieldModifier in _fieldModifiers)
                fieldModifier.OnExitTile(character);
        }
    }
}