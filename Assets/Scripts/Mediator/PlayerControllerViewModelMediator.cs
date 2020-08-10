using System;
using System.Collections.Generic;
using System.Linq;
using Model.Board;
using Model.Characters.Controllers;
using UniRx;
using View;

namespace Mediator
{
    public class PlayerControllerViewModelMediator : IDisposable
    {
        private readonly Dictionary<Field, FieldHighlightType> _fieldsVisuals = new Dictionary<Field, FieldHighlightType>();
        private readonly CompositeDisposable _modelListeners = new CompositeDisposable();

        private readonly IBoardView _boardView;
        private readonly IHUDView _hudView;
        private readonly PlayerController _model;
        
        public PlayerControllerViewModelMediator(PlayerController model, IBoardView boardView, IHUDView hudView)
        {
            _boardView = boardView;
            _hudView = hudView;
            _model = model;
            AddListeners();
        }

        public void Dispose()
        {
            RemoveListeners();
        }
        
        private void AddListeners()
        {
            _model.ValidMovementTargets.ObserveAdd()
                .Subscribe(e => OnHighlightTypeAdded(e.Value, FieldHighlightType.Walkable))
                .AddTo(_modelListeners);
            _model.ValidMovementTargets.ObserveRemove()
                .Subscribe(e => OnHighlightTypeRemoved(e.Value, FieldHighlightType.Walkable))
                .AddTo(_modelListeners);
            _model.ValidMovementTargets.ObserveReset()
                .Subscribe(_ => OnHighlightTypeCleared(FieldHighlightType.Walkable))
                .AddTo(_modelListeners);
		
            _model.Path.ObserveAdd()
                .Subscribe(e => OnHighlightTypeAdded(e.Value, FieldHighlightType.Path))
                .AddTo(_modelListeners);
            _model.Path.ObserveRemove()
                .Subscribe(e => OnHighlightTypeRemoved(e.Value, FieldHighlightType.Path))
                .AddTo(_modelListeners);
            _model.Path.ObserveReset()
                .Subscribe(_ => OnHighlightTypeCleared(FieldHighlightType.Path))
                .AddTo(_modelListeners);

            _model.ValidAttackTargets.ObserveAdd()
                .Subscribe(e => OnHighlightTypeAdded(e.Value, FieldHighlightType.PossibleTarget))
                .AddTo(_modelListeners);
            _model.ValidAttackTargets.ObserveRemove()
                .Subscribe(e => OnHighlightTypeRemoved(e.Value, FieldHighlightType.PossibleTarget))
                .AddTo(_modelListeners);
            _model.ValidAttackTargets.ObserveReset()
                .Subscribe(_ => OnHighlightTypeCleared(FieldHighlightType.PossibleTarget))
                .AddTo(_modelListeners);
            
            _model.AttackTarget.Subscribe(field =>
            {
                OnHighlightTypeCleared(FieldHighlightType.Target);
                if (field != null)
                    OnHighlightTypeAdded(field, FieldHighlightType.Target);
            }).AddTo(_modelListeners);

            _model.ActiveCharacterField.Subscribe(field =>
            {
                OnHighlightTypeCleared(FieldHighlightType.ActiveCharacter);
                if (field != null)
                    OnHighlightTypeAdded(field, FieldHighlightType.ActiveCharacter);
            }).AddTo(_modelListeners);

            Observable.FromEvent<Field>(
                    h => _boardView.OnFieldTapped += h,
                    h => _boardView.OnFieldTapped -= h
                )
                .Subscribe(_model.OnFieldTapped)
                .AddTo(_modelListeners);
            
            _model.CanSkipTurn.Subscribe(canSkip =>
            {
                _hudView.SetSkipTurnButtonState(canSkip);                
            }).AddTo(_modelListeners);
            
            Observable.FromEvent(
                    h => _hudView.SkipTurnButtonClicked += h,
                    h => _hudView.SkipTurnButtonClicked -= h
                )
                .Subscribe(_ => _model.EndTurn())
                .AddTo(_modelListeners);
        }

        private void RemoveListeners()
        {
            _modelListeners.Clear();
        }

        private void OnHighlightTypeCleared(FieldHighlightType fieldHighlightType)
        {
            foreach (var fieldVisuals in _fieldsVisuals.Where(fieldVisuals => (fieldVisuals.Value & fieldHighlightType) == fieldHighlightType).ToArray())
            {
                _fieldsVisuals[fieldVisuals.Key] = fieldVisuals.Value & ~fieldHighlightType;
                UpdateFieldVisuals(fieldVisuals.Key);
            }
        }
        
        private void OnHighlightTypeRemoved(Field removedField, FieldHighlightType fieldHighlightType)
        {
            if (!_fieldsVisuals.ContainsKey(removedField))
                return;
            
            _fieldsVisuals[removedField] = _fieldsVisuals[removedField] & ~fieldHighlightType;
            UpdateFieldVisuals(removedField);
        }
        
        private void OnHighlightTypeAdded(Field addedField, FieldHighlightType fieldHighlightType)
        {
            if (!_fieldsVisuals.ContainsKey(addedField))
                _fieldsVisuals.Add(addedField, fieldHighlightType);
            else
                _fieldsVisuals[addedField] = _fieldsVisuals[addedField] | fieldHighlightType;
            UpdateFieldVisuals(addedField);
        }

        private void UpdateFieldVisuals(Field field)
        {
            if (!_fieldsVisuals.ContainsKey(field))
            {
                _boardView.SetHighlight(field, FieldHighlightType.Default);
                return;
            }

            var visualsValues = Enum.GetValues(typeof(FieldHighlightType)).Cast<FieldHighlightType>().Reverse();
            foreach (var value in visualsValues)
            {
                if ((_fieldsVisuals[field] & value) == value)
                {
                    _boardView.SetHighlight(field, value);
                    return;
                }
            }
        }
    }
}