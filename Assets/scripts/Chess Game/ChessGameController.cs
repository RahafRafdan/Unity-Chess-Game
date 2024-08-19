using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(PiecesCreator))]
public class ChessGameController : MonoBehaviour
{

    private enum GameState { Init, Play, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIManager uiManager;

    private PiecesCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;
    private GameState state;


    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<PiecesCreator>();
    }

    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }


    private void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        uiManager.HideUI();
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMove(activePlayer);
        SetGameState(GameState.Play);
    }

    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnRestarted();
        blackPlayer.OnRestarted();
        StartNewGame();

    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void SetGameState(GameState state)
    {
        this.state = state;
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }
    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }
    private void GenerateAllPossiblePlayerMove(ChessPlayer Player)
    {
        Player.GenerateAllPossibleMoves();
    }


    public bool IsTeamTurnActivate(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMove(activePlayer);
        GenerateAllPossiblePlayerMove(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
            EndGame();
        else
            changeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] KingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if(KingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.avaliableMoves.Count;
            if(availableKingMoves == 0)
            {
                bool cancoverking = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!cancoverking)
                    return true;
            }
        }
        return false;
    }

    private void EndGame()
    {
        uiManager.OnGameFinished(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private void changeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer; 
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer Player)
    {
        return Player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }
}

