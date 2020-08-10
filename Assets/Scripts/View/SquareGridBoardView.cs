using System;
using System.Collections.Generic;
using Input;
using Model.Board;
using Model.Board.SquareGridBoard;
using Model.Characters;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace View
{
    [Serializable]
    public class SquareGridBoardView : MonoBehaviour, IBoardView, IDisposable
    {
        public event Action<Field> OnFieldTapped;

        [SerializeField] 
        private CharacterView _characterPrefab = default;
        
        [SerializeField] 
        private FieldView _tilePrefab = default;
        
        private GridPositionConverter _positionConverter;
        
        private FieldView[,] _tilesViews;
        private Dictionary<Character, CharacterView> _charactersViews = new Dictionary<Character, CharacterView>();
        
        private CompositeDisposable _tilesDisposables;
        
        public void GenerateView(SquareGridBoard board)
        {
            _tilesDisposables = new CompositeDisposable();
            _charactersViews = new Dictionary<Character, CharacterView>();
            _tilesViews = new FieldView[board.Tiles.GetLength(0), board.Tiles.GetLength(1)];
            
            var tileSize = new Vector2(2, 2);
            var offset = new Vector2(-(board.Tiles.GetLength(0) * tileSize.x) / 2 + 1, 0);
            _positionConverter = new GridPositionConverter(offset, new Vector2(2, 2));
            
            var cachedTransform = transform;
            
            for (var x = 0; x < board.Tiles.GetLength(0); x++)
            for (var y = 0; y < board.Tiles.GetLength(1); y++)
            {
                var tile = Instantiate(
                    _tilePrefab,
                    _positionConverter.GridPositionToWorldPosition(new Vector2(x, y)),
                    Quaternion.Euler(0, 45, 0),
                    cachedTransform
                );
                
                tile.name = $"Tile({x},{y})";
                _tilesViews[x, y] = tile;
                
                var dispatcher = tile.GetComponent<RayCastDispatcher>();
                var x1 = x;
                var y1 = y;

                Observable.FromEvent(
                        h => dispatcher.DispatchRayCastAction += h,
                        h => dispatcher.DispatchRayCastAction -= h
                    )
                    .Subscribe(_ => OnFieldTapped?.Invoke(board.Tiles[x1, y1]))
                    .AddTo(_tilesDisposables);
            }
        }

        public void SetHighlight(Field field, FieldHighlightType fieldHighlightType)
        {
            var tile = (Tile) field;
            
            var fieldVisuals = _tilesViews[tile.X, tile.Y];
            fieldVisuals.SetHighlight(fieldHighlightType);
        }

        public void SetFieldWalkable(Field field, bool walkable)
        {
            var tile = (Tile) field;
            var fieldVisuals = _tilesViews[tile.X, tile.Y];
            fieldVisuals.SetFieldWalkable(walkable);
        }

        public void AddCharacter(Character character)
        {
            var startingTile = (Tile) character.OccupiedField;
            var startingCoordinates = new Vector2(startingTile.X, startingTile.Y);

            var characterView = Instantiate(
                _characterPrefab,
                _positionConverter.GridPositionToWorldPosition(startingCoordinates),
                Quaternion.identity
            );
            characterView.SetTeam(character.Team);
            
            _charactersViews.Add(character, characterView);
        }

        public void Move(Character character, Field to)
        {
            var characterView = _charactersViews[character];
            var targetTile = (Tile) to;
            characterView.transform.position =
                _positionConverter.GridPositionToWorldPosition(new Vector2(targetTile.X, targetTile.Y));
        }

        public void RemoveCharacter(Character character)
        {
            _charactersViews[character].Dispose();
            _charactersViews.Remove(character);
        }

        public void SetCharacterHealth(Character character, int health)
        {
            _charactersViews[character].SetCharacterHealth(health);
        }

        public void Dispose()
        {
            foreach (var tile in _tilesViews)
                tile.Dispose();
            _tilesDisposables?.Dispose();
            _tilesViews = null;
            
            foreach (var character in _charactersViews)
                character.Value.Dispose();
            _charactersViews = null;
        }
    }
}