using System;
using Model.Board;
using Model.Characters;

namespace View
{
    public interface IBoardView
    {
        event Action<Field> OnFieldTapped;
        void SetHighlight(Field field, FieldHighlightType highlightType);

        void SetFieldWalkable(Field field, bool walkable);
        
        void AddCharacter(Character character);
        void Move(Character character, Field to);
        void RemoveCharacter(Character character);

        void SetCharacterHealth(Character character, int health);
    }
}