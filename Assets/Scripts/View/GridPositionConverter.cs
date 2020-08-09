using UnityEngine;

namespace View
{
    public struct GridPositionConverter
    {
        private readonly Vector2 _gridPivot;
        private readonly Vector2 _tileSize;

        public GridPositionConverter(Vector2 gridPivot, Vector2 tileSize)
        {
            _gridPivot = gridPivot;
            _tileSize = tileSize;
        }

        private (Vector2, Vector2) GetVectors()
        {
            var horizontalVector = new Vector2(_tileSize.x / 2, -_tileSize.y / 2);
            var verticalVector = new Vector2(_tileSize.x / 2, _tileSize.y / 2);
            return (horizontalVector, verticalVector);
        }

        public Vector3 GridPositionToWorldPosition(Vector2 gridPosition)
        {
            var (horizontalVector, verticalVector) = GetVectors();

            return new Vector3
            (
                gridPosition.x * horizontalVector.x + gridPosition.y * verticalVector.x + _gridPivot.x,
                0f,
                gridPosition.x * horizontalVector.y + gridPosition.y * verticalVector.y + _gridPivot.y
            );
        }

        public Vector2 WorldToGridPosition(Vector2 worldPosition)
        {
            var (horizontalVector, verticalVector) = GetVectors();

            worldPosition -= _gridPivot;
            var reciprocalDet = 1 / (horizontalVector.x * verticalVector.y - verticalVector.x * horizontalVector.y);

            return new Vector2
            {
                x = reciprocalDet * (worldPosition.x * verticalVector.y - worldPosition.y * verticalVector.x),
                y = reciprocalDet * (worldPosition.x * -horizontalVector.y + worldPosition.y * horizontalVector.x),
            };
        }
    }
}