using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class HUD : MonoBehaviour, IHUDView
    {
        public event Action SkipTurnButtonClicked;

        [SerializeField] 
        private Button _skipTurnButton = default;

        private IDisposable _skipButtonDisposable;
        
        private void OnEnable()
        {
            _skipButtonDisposable = _skipTurnButton
                .OnClickAsObservable()
                .Subscribe(_ => SkipTurnButtonClicked?.Invoke());
        }

        private void OnDisable()
        {
            _skipButtonDisposable.Dispose();
        }

        public void SetSkipTurnButtonState(bool isAvailable)
        {
            _skipTurnButton.gameObject.SetActive(isAvailable);
        }
    }
}