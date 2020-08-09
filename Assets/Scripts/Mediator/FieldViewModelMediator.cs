using System;
using Model.Board;
using UniRx;
using View;

namespace Mediator
{
    public class FieldViewModelMediator : IDisposable
    {
        private readonly IBoardView _view;
        private readonly Field _model;

        private readonly CompositeDisposable _modelListeners = new CompositeDisposable();
        
        public FieldViewModelMediator(Field model, IBoardView view)
        {
            _view = view;
            _model = model;
            AddListeners();
        }

        private void AddListeners()
        {
            _model.IsWalkable.Subscribe(w => _view.SetFieldWalkable(_model, w)).AddTo(_modelListeners);
        }

        public void Dispose()
        {
            _modelListeners?.Dispose();
        }
    }
}