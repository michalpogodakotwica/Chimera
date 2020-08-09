using System;

namespace View
{
    public interface IHUDView
    {
        event Action SkipTurnButtonClicked;
        void SetSkipTurnButtonState(bool isAvailable);
    }
}