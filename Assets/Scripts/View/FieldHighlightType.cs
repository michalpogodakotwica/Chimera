using System;

namespace View
{
    [Flags]
    // values control displaying order
    public enum FieldHighlightType
    {
        Default = 0,
        Walkable = 1,
        ActiveCharacter =  1 << 1,
        Path = 1 << 2,
        PossibleTarget = 1 << 3,
        Target = 1 << 4
    }
}