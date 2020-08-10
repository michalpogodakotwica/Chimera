using System;
using Model.Board;
using Model.Characters;
using UniRx;
using View;

namespace Mediator
{
    public class ActiveCharactersViewModelMediator : IDisposable
    {
        private readonly CompositeDisposable _modelListenersDisposable = new CompositeDisposable();
        private readonly IBoardView _view;
        private readonly Arena _model;
        
        public ActiveCharactersViewModelMediator(Arena model, IBoardView boardView)
        {
            _view = boardView;
            _model = model;
            AddListeners();
        }

        private void AddListeners()
        {
            _model.ActiveCharacters.ObserveAdd()
                .Subscribe(OnCharacterAdded)
                .AddTo(_modelListenersDisposable);
        }
        
        private void OnCharacterAdded(CollectionAddEvent<Character> addEvent)
        {
            var agentDisposables = new CompositeDisposable();
            
            _view.AddCharacter(addEvent.Value);
            Observable.FromEvent<Field>(
                    h => addEvent.Value.OnMoved += h,
                    h => addEvent.Value.OnMoved -= h
                )
                .Subscribe(c => _view.Move(addEvent.Value, c))
                .AddTo(agentDisposables);

            addEvent.Value.Health.Subscribe(h => _view.SetCharacterHealth(addEvent.Value, h)).AddTo(agentDisposables);

            _model.ActiveCharacters.ObserveRemove()
                .Subscribe(removeEvent =>
                {
                    if (removeEvent.Value == addEvent.Value)
                    {
                        agentDisposables.Dispose();
                        _view.RemoveCharacter(removeEvent.Value);
                    }
                })
                .AddTo(agentDisposables);
            
            agentDisposables.AddTo(_modelListenersDisposable);
        }

        public void Dispose()
        {
            _modelListenersDisposable?.Dispose();
        }
    }
}