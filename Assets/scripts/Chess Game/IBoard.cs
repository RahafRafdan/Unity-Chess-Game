using UnityEngine;

public interface IBoard
{
    Vector3 CalculatePositionFromCoords(Vector2Int coords);
    bool HasPiece(Piece piece);
    void OnSquareSelected(Vector3 inputPosition);
}