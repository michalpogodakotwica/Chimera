using System;
using System.Collections.Generic;
using Model.Board;
using Model.Characters.Controllers;
using UniRx;
using UnityEngine;

namespace Model.Characters
{
    public class Character : IDisposable
    {
        public readonly string ID;
        public readonly int Team;
        
        public event Action<Character> OnDispose;

        private readonly ReactiveProperty<int> _health;
        private readonly ReactiveProperty<int> _speed;
        private readonly ReactiveCollection<ICharacterEffect> _turnModifiers;

        public IController Controller { get; }
        public Field OccupiedField { get; private set; }
        public IAttackAction AttackAction { get; }

        public event Action<Field> OnMoved;

        public Character(string id, int health, int speed, IController controller, int team, IAttackAction attackAction)
        {
            ID = id;
            _health = new ReactiveProperty<int>(health);
            _speed = new ReactiveProperty<int>(speed);
            _turnModifiers = new ReactiveCollection<ICharacterEffect>();
            AttackAction = attackAction;
            Controller = controller;
            Team = team;
        }
        
        public void MoveTo(Field field)
        {
            OccupiedField?.Exit(this);
            
            if(OccupiedField == null)
                Debug.Log($"{ID} starts on {field.ID}");
            else if(field != null)
                Debug.Log($"{ID} moves to {field.ID}");
            
            OccupiedField = field;
            OnMoved?.Invoke(field);
            OccupiedField?.Enter(this);
        }

        public void FollowPath(IEnumerable<Field> path)
        {
            foreach (var field in path)
                MoveTo(field);
        }

        public void StartTurn()
        {
            foreach (var turnModifier in _turnModifiers)
                turnModifier.OnTurnStarted(this);
        }
        
        public void EndTurn()
        {
            foreach (var turnModifier in _turnModifiers)
                turnModifier.OnTurnEnded(this);
        }
        
        public void DealDamage(int damage)
        {
            _health.Value -= damage;
            if (_health.Value <= 0)
                Die();
        }

        private void Die()
        {
            Debug.Log($"{ID} dies");
            Dispose();
        }

        public IReadOnlyReactiveProperty<int> Health => _health;
        public IReadOnlyReactiveProperty<int> Speed => _speed;

        public void Dispose()
        {
            OnDispose?.Invoke(this);
            MoveTo(null);
            _health?.Dispose();
            _speed?.Dispose();
            _turnModifiers?.Dispose();
        }
    }
}