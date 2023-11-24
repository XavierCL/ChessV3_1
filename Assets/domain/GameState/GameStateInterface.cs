
using System.Collections.Generic;
using System.Linq;

public interface GameStateInterface
{
    public int turn { get; }
    public bool whiteTurn { get => turn % 2 == 0; }
    public List<PiecePosition> piecePositions { get; }
    public List<ReversibleMove> history { get; }
    public List<Move> getLegalMoves();

    public void PlayMove(Move move);

    public GameEndState GetGameEndState();
    public void UndoMove();
}
