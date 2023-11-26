
using System.Collections.Generic;

public interface GameStateInterface
{
    public int turn { get; }
    public bool whiteTurn { get => turn % 2 == 0; }
    public int staleTurns { get; }
    public List<ReversibleMove> history { get; }
    public BoardStateInterface BoardState { get; }
    public Dictionary<BoardStateInterface, ushort> Snapshots { get; }

    public List<Move> getLegalMoves();
    public void PlayMove(Move move);
    public GameEndState GetGameEndState();
    public void UndoMove();
}
