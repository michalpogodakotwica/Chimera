using System.Linq;
using Model.Board;
using UniRx;

namespace Model.Characters.Controllers
{
    public class PlayerController : IController
    {
        private const float MovementEndPrecision = 0.01f;
        
        private readonly ReactiveProperty<Field> _attackTarget = new ReactiveProperty<Field>();
        private readonly ReactiveProperty<Field> _activeCharacterField = new ReactiveProperty<Field>();
        private readonly ReactiveCollection<Field> _path = new ReactiveCollection<Field>();
        private readonly ReactiveCollection<Field> _validMovementTargets = new ReactiveCollection<Field>();
        private readonly ReactiveCollection<Field> _validAttackTargets = new ReactiveCollection<Field>();
        private readonly ReactiveProperty<bool> _canSkipTurn = new ReactiveProperty<bool>();
        
        private Arena _arena;
        private Character _activeCharacter;
        
        private float _currentPathLength;
        private float _movementLeft;
        
        public void StartTurn(Character character, Arena arena)
        {
            _activeCharacter = character;
            _activeCharacterField.Value = _activeCharacter.OccupiedField;
            
            _arena = arena;
            _movementLeft = character.Speed.Value;
            EnterMoveState(_movementLeft);

            _activeCharacter.OnDispose += OnCharacterDisposed;
            _canSkipTurn.Value = true;
        }

        private void EnterMoveState(float range)
        {
            _validMovementTargets.Clear();
            _validAttackTargets.Clear();
            _attackTarget.Value = null;
            _path.Clear();
            
            foreach (var field in _arena.Board.GetFieldsInRange(_activeCharacter.OccupiedField, range, CollisionDetectionType.ValidPath))
                if(field != _activeCharacter.OccupiedField)
                    _validMovementTargets.Add(field);

            foreach (var field in _arena.Board.GetFieldsInRange(_activeCharacter.OccupiedField, 1, CollisionDetectionType.None))
                if (field.OccupiedBy != null && field.OccupiedBy.Team != _activeCharacter.Team)
                    _validAttackTargets.Add(field);

            if(_validAttackTargets.Count == 0 && _movementLeft < MovementEndPrecision)
                EndTurn();
        }

        public void EndTurn()
        {
            if(_activeCharacter != null)
                _activeCharacter.OnDispose -= OnCharacterDisposed;
            
            _activeCharacter = null;
            _canSkipTurn.Value = false;
            
            _activeCharacterField.Value = null;
            _attackTarget.Value = null;
            _validMovementTargets.Clear();
            _validAttackTargets.Clear();
            _path.Clear();
            
            _arena.StartNextTurn();
        }

        private void OnCharacterDisposed(Character character)
        {
            if(_activeCharacter == character)
                EndTurn();
        }
        
        public void OnFieldTapped(Field field)
        {
            if (_activeCharacter == null)
                return;

            if (_attackTarget.Value != null)
            {
                if (_attackTarget.Value == field)
                {
                    _movementLeft -= _currentPathLength;
                    _activeCharacter.Attack(field.OccupiedBy);
                    EndTurn();
                    return;
                }
                _attackTarget.Value = null;
            }
            
            if (_path.Count != 0)
            {
                if (_path.Last() == field)
                {
                    _activeCharacter.FollowPath(_path);
                    _activeCharacterField.Value = _activeCharacter.OccupiedField;
                    
                    _movementLeft -= _currentPathLength;
                    EnterMoveState(_movementLeft);
                    return;
                }
                _path.Clear();
            }

            if (_validAttackTargets.Contains(field))
            {
                _attackTarget.Value = field;
                return;
            }

            if (!_validMovementTargets.Contains(field)) 
                return;
            
            var hasPath = _arena.Board.TryToGetPath(_activeCharacter.OccupiedField, field, out var path, out var length);
            if(!hasPath)
                return;

            _currentPathLength = length;
            foreach (var pathField in path)
                _path.Add(pathField);
        }

        public IReadOnlyReactiveCollection<Field> ValidMovementTargets => _validMovementTargets;
        public IReadOnlyReactiveCollection<Field> ValidAttackTargets => _validAttackTargets;
        public IReadOnlyReactiveCollection<Field> Path => _path;
        public IReadOnlyReactiveProperty<Field> AttackTarget => _attackTarget;
        public IReadOnlyReactiveProperty<Field> ActiveCharacterField => _activeCharacterField;
        public IReadOnlyReactiveProperty<bool> CanSkipTurn => _canSkipTurn;
    }
}