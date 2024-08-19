using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChessPlayer 
{
    public TeamColor team { get; set; }
    public Board board { get; set; }
    public List<Piece> activePieces { get; private set; }

    public ChessPlayer(TeamColor team, Board board)
    {
        this.team = team;
        this.board = board;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
           activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach(var piece in activePieces)
        {
            if (board.HasPiece(piece))
                piece.SelectAvaliableSquares();
        }
    }

    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p.isAttackingPieceOfType<T>()).ToArray();
    }

    public Piece[] GetPiecesOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p is T).ToArray();
    }

    public void RemoveMovesEnablingAttackOnPiece<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var coords in selectedPiece.avaliableMoves)
        {
            Piece pieceOnSquare = board.GetPieceOnSquare(coords);
            board.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
            opponent.GenerateAllPossibleMoves();
            if(opponent.ChechIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnSquare);
        }
        foreach (var coords in coordsToRemove)
        {
            selectedPiece.avaliableMoves.Remove(coords);
        }
    }

    private bool ChechIfIsAttackingPiece<T>() where T : Piece
    {
        foreach(var piece in activePieces)
        {
            if(board.HasPiece(piece) && piece.isAttackingPieceOfType<T>())
            return true;
        }
        return false;
    }

    public bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
    {
       foreach (var piece in activePieces)
        {
            foreach(var coords in piece.avaliableMoves)
            {
                Piece pieceOnCoords = board.GetPieceOnSquare(coords);
                board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
                opponent.GenerateAllPossibleMoves();
                if(!opponent.ChechIfIsAttackingPiece<T>())
                {
                    board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
                    return true;
                }
                board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
            }
        }
        return false;
    }

    internal void OnRestarted()
    {
        activePieces.Clear();
    }
}
