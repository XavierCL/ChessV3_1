
using System.Collections.Generic;

public class GameState
{
    public int turn { get; set; }
    public bool whiteTurn { get => turn % 2 == 0; }

    public List<Move> getLegalMoves()
    {
        return new List<Move> { new Move { source = new BoardPosition(0, 1), target = new BoardPosition(0, 2) } };
    }
}
