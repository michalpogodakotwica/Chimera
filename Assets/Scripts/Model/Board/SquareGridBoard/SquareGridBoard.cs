using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Model.Board.SquareGridBoard
{
    public class SquareGridBoard : IBoard
    {
        public readonly Tile[,] Tiles;

        public SquareGridBoard(int width, int height)
        {
            Tiles = new Tile[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                Tiles[x, y] = new Tile($"({x}, {y})", x, y);
            }
        }

        public static Vector2Int GetIndexOf(Field field)
        {
            var tile = (Tile) field;
            return new Vector2Int(tile.X, tile.Y);
        }
        
        private IEnumerable<Vector2Int> Neighbours(Vector2Int index)
        {
            if (index.x + 1 < Tiles.GetLength(0))
                yield return new Vector2Int(index.x + 1, index.y);

            if (index.x - 1 >= 0)
                yield return new Vector2Int(index.x - 1, index.y);

            if (index.y + 1 < Tiles.GetLength(1))
                yield return new Vector2Int(index.x, index.y + 1);

            if (index.y - 1 >= 0)
                yield return new Vector2Int(index.x, index.y - 1);
        }

        public IEnumerable<Field> GetFieldsInRange(Field start, float distance, CollisionDetectionType collisionDetectionType)
        {
            var startingIndex = GetIndexOf(start);

            if (startingIndex == new Vector2Int(-1, -1))
                yield break;

            switch (collisionDetectionType)
            {
                case CollisionDetectionType.None:
                    for (var x = 0; x < Tiles.GetLength(0); x++)
                    for (var y = 0; y < Tiles.GetLength(1); y++)
                    {
                        var positionDifference = Mathf.Abs(startingIndex.x - x) + Mathf.Abs(startingIndex.y - y);
                        if (positionDifference <= distance)
                            yield return Tiles[x, y];
                    }

                    break;

                case CollisionDetectionType.ValidPath:
                    var validIndexes = new List<Vector2Int>((int) distance * (int) distance);
                    var visited = new List<Vector2Int>((int) distance * (int) distance);
                    var currentStep = new List<Vector2Int>();

                    visited.Add(startingIndex);
                    validIndexes.Add(startingIndex);
                    currentStep.Add(startingIndex);

                    for (var i = 0; i < distance; i++)
                    {
                        var nextStep = new List<Vector2Int>();
                        foreach (var index in currentStep)
                        {
                            foreach (var neighbour in Neighbours(index))
                            {
                                if (visited.Contains(neighbour))
                                    continue;

                                visited.Add(neighbour);
                                if (!Tiles[neighbour.x, neighbour.y].IsWalkable.Value || Tiles[neighbour.x, neighbour.y].OccupiedBy != null)
                                    continue;

                                validIndexes.Add(neighbour);
                                nextStep.Add(neighbour);
                            }
                            currentStep = nextStep;
                        }
                    }

                    foreach (var validIndex in validIndexes)
                    {
                        yield return Tiles[validIndex.x, validIndex.y];
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(collisionDetectionType), collisionDetectionType, null);
            }
        }

        public bool TryToGetPath(Field start, Field end, out List<Field> path, out float length)
        {
            if (start == end)
            {
                length = 0;
                path = new List<Field>();
                return true;
            }

            var startingIndex = GetIndexOf(start);

            if (startingIndex == new Vector2Int(-1, -1))
            {
                path = null;
                length = -1;
                return false;
            }

            var distances = new int[Tiles.GetLength(0), Tiles.GetLength(1)];
            var previous = new Vector2Int[Tiles.GetLength(0), Tiles.GetLength(1)];

            for (var x = 0; x < Tiles.GetLength(0); x++)
            for (var y = 0; y < Tiles.GetLength(1); y++)
            {
                distances[x, y] = int.MaxValue;
                previous[x, y] = new Vector2Int(-1, -1);
            }

            distances[startingIndex.x, startingIndex.y] = 0;
            var visited = new BitArray(Tiles.Length);
            var heap = new Heap<(Vector2Int node, int distance)>((a, b) => a.distance.CompareTo(b.distance));
            heap.Push((startingIndex, 0));

            while (heap.Count > 0)
            {
                var current = heap.Pop();
                if (visited[current.node.y * Tiles.GetLength(0) + current.node.x])
                    continue;

                var neighbours = Neighbours(current.node);
                foreach (var neighbour in neighbours)
                {
                    if (!Tiles[neighbour.x, neighbour.y].IsWalkable.Value || Tiles[neighbour.x, neighbour.y].OccupiedBy != null)
                        continue;
                    
                    if (visited[neighbour.y * Tiles.GetLength(0) + neighbour.x])
                        continue;

                    var distance = distances[current.node.x, current.node.y] + 1;
                    if (distance < distances[neighbour.x, neighbour.y])
                    {
                        previous[neighbour.x, neighbour.y] = current.node;
                        distances[neighbour.x, neighbour.y] = distance;
                        heap.Push((neighbour, distance));
                    }
                }
                visited[current.node.y * Tiles.GetLength(0) + current.node.x] = true;
            }

            var currentIndex = GetIndexOf(end);
            if (previous[currentIndex.x, currentIndex.y] == new Vector2Int(-1, -1))
            {
                path = null;
                length = -1;
                return false;
            }
            
            path = new List<Field>();
            length = distances[currentIndex.x, currentIndex.y];
            
            while (currentIndex != startingIndex)
            {
                path.Add(Tiles[currentIndex.x, currentIndex.y]);
                currentIndex = previous[currentIndex.x, currentIndex.y];
            }

            path.Reverse();
            return true;
        }
        
        public IEnumerable<Field> AllFields => Tiles.Cast<Field>();
    }
}