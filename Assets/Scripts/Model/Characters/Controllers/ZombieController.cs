using System.Linq;
using Model.Board;
using UnityEngine;
using Utils;

namespace Model.Characters.Controllers
{
    public class ZombieController : IController
    {
        public void StartTurn(Character character, Arena arena)
        {
            var enemies = arena.TurnQueue.Where(c => c.Team != character.Team).ToArray();
            var validAttackPositions = enemies
                .SelectMany(c => arena.Board.GetFieldsInRange(c.OccupiedField, 1, CollisionDetectionType.None))
                .Where(f => f.IsWalkable.Value && (f.OccupiedBy == null || f.OccupiedBy == character))
                .ToArray();
            
            var validPaths = validAttackPositions.Select(f =>
                {
                    var validPath = arena.Board.TryToGetPath(character.OccupiedField, f, out var path, out var length);
                    return (f, validPath, path, length);
                })
                .Where(c => c.validPath)
                .ToArray();

            if (!validPaths.Any())
            {
                arena.StartNextTurn();
                return;
            }

            var closestEnemyInfo = validPaths.MinBy(c => c.length);
            var pathInRange = closestEnemyInfo.path.Take(Mathf.Min(character.Speed.Value, (int) closestEnemyInfo.length)).ToArray();
            
            character.FollowPath(pathInRange);
            
            if(character.OccupiedField == null)
                arena.StartNextTurn();

            var validAttackTargets = character.AttackAction.ValidTargets(character, arena.Board).ToArray();
            if(validAttackTargets.Any())
                character.AttackAction.ApplyOnTarget(character, validAttackTargets.First());

            arena.StartNextTurn();
        }
    }
}