using System;
using Model.Board.SquareGridBoard;
using Model.Characters;
using Model.Characters.Controllers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "SquareBoardGenerator")]
    public class SquareBoardGenerator : ScriptableObject
    {
        [SerializeField] 
        [HideInInspector]
        private BoardData _boardData;

        [ShowInInspector, TableMatrix(ResizableColumns = false, RowHeight = 8, SquareCells = true)]
        private FieldType[,] Tiles
        {
            get => _boardData?.GetTiles();
            set => _boardData = new BoardData(value);
        }

        public SquareGridBoard GenerateBoard(IController playerController, IController zombieController)
        {
            var tiles = Tiles;
            var squareGridBoard = new SquareGridBoard(tiles.GetLength(0), tiles.GetLength(1));
            var playerIndex = 0;
            var enemyIndex = 0;

            for (var x = 0; x < tiles.GetLength(0); x++)
            for (var y = 0; y < tiles.GetLength(1); y++)
            {
                switch (tiles[x, y])
                {
                    case FieldType.Default:
                        break;
                    
                    case FieldType.Blocked:
                        squareGridBoard.Tiles[x, y].IsWalkable.Value = false;
                        break;
                    
                    case FieldType.Player:
                        var playerCharacter = new Character($"Player {playerIndex}", 2, 3, playerController, 0, new Damage(1));
                        playerCharacter.MoveTo(squareGridBoard.Tiles[x, y]);
                        playerIndex++;
                        break;
                    
                    case FieldType.Zombie:
                        var zombie = new Character($"Zombie {enemyIndex}", 2, 1, zombieController, 1, new Damage(1));
                        zombie.MoveTo(squareGridBoard.Tiles[x, y]);
                        enemyIndex++;
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return squareGridBoard;
        }

        [Serializable]
        public class BoardData
        {
            [SerializeField]
            private int _width;
            [SerializeField]
            private int _height;
            [SerializeField]
            private FieldType[] _tiles;

            public BoardData(FieldType[,] tiles)
            {
                _width = tiles.GetLength(0);
                _height = tiles.GetLength(1);
                
                _tiles = new FieldType[tiles.Length];
                for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    _tiles[y * _width + x] = tiles[x, y];
            }

            public FieldType[,] GetTiles()
            {
                var result = new FieldType[_width, _height];
                for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    result[x, y] = _tiles[y * _width + x];
                return result;
            }
        }
    }

    // Doing this as enum is just a kind of quick hack.
    // Proper object-oriented implementation would require custom editor and would take too much time.
    public enum FieldType
    {
        Default,
        Blocked,
        Player,
        Zombie
    }
}