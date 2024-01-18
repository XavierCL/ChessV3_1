using System.Collections.Generic;

// This game state adds support for piece position id with any board state. Use this in the UI.
public class PieceIdGameStateAdapter : GameStateInterface
{
  private readonly GameStateInterface underlying;
  public override BoardStateInterface BoardState { get => boardState; }
  public PieceIdBoardStateAdapter boardState;
  public override List<ReversibleMove> History { get; }
  public override int StaleTurns { get => underlying.StaleTurns; }
  public override Dictionary<BoardStateInterface, ushort> Snapshots => underlying.Snapshots;

  public PieceIdGameStateAdapter(GameStateFactoryInterface gameStateFactory)
  {
    underlying = gameStateFactory.StartingPosition();
    History = new List<ReversibleMove>();
    boardState = PieceIdBoardStateAdapter.FromStartingPosition(underlying);
  }

  public PieceIdGameStateAdapter(GameStateFactoryInterface gameStateFactory, GameStateInterface gameState)
  {
    underlying = gameStateFactory.FromGameState(gameState);
    History = new List<ReversibleMove>(gameState.History);
    boardState = PieceIdBoardStateAdapter.FromMiddlegame(underlying);
  }

  public PieceIdGameStateAdapter(GameStateFactoryInterface gameStateFactory, string fen)
  {
    underlying = gameStateFactory.FromFen(fen);
    History = new List<ReversibleMove>(underlying.History);
    boardState = PieceIdBoardStateAdapter.FromMiddlegame(underlying);
  }

  public override IReadOnlyList<Move> getLegalMoves()
  {
    return underlying.getLegalMoves();
  }

  public override ReversibleMove PlayMove(Move move)
  {
    var reversibleMove = underlying.PlayMove(move);
    var boardPlay = boardState.PlayMove(move);
    boardState = boardPlay.boardState;

    var reversibleWithId = new ReversibleMove(
      reversibleMove.source,
      reversibleMove.target,
      reversibleMove.oldStaleTurns,
      reversibleMove.promotion,
      reversibleMove.lostCastleRights,
      reversibleMove.oldEnPassantColumn,
      boardPlay.killedPiece
    );

    History.Add(reversibleWithId);
    return reversibleWithId;
  }

  public override void UndoMove()
  {
    underlying.UndoMove();
    var reversibleMove = History[^1];
    History.RemoveAt(History.Count - 1);
    boardState = boardState.UndoMove(reversibleMove);
  }

  public override GameEndState GetGameEndState()
  {
    return underlying.GetGameEndState();
  }
}
