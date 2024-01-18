using System.Linq;
using UnityEngine;

public class HistoryHandler : MonoBehaviour
{
    private GameController gameController;
    private PremoveHandler premoveHandler;
    private BoardController boardController;
    public bool ShowsHistory { get => historyIndex != -1; }
    private int historyIndex;

    public void Awake()
    {
        gameController = StaticReferences.gameController.Value;
        premoveHandler = StaticReferences.premoveHandler.Value;
        boardController = StaticReferences.boardController.Value;
    }


    public void Start()
    {
        historyIndex = -1;
    }

    public void Backward()
    {
        if (!ShowsHistory)
        {
            premoveHandler.Clear();
            if (gameController.gameState.History.Count == 0) return;
            historyIndex = gameController.gameState.History.Count;
        }

        // Can't go more backward, do nothing
        if (historyIndex == 0) return;

        historyIndex -= 1;

        gameController.gameVisual.HistoryBack();
        var reversibleMove = gameController.gameState.History[historyIndex];
        boardController.AnimateUndoMove(reversibleMove);
    }

    public void Forward()
    {
        if (!ShowsHistory) return;
        var reversibleMove = gameController.gameState.History[historyIndex];
        var move = new Move(reversibleMove);
        boardController.AnimateMove(move, false, true);
        ++historyIndex;

        if (historyIndex == gameController.gameState.History.Count)
        {
            historyIndex = -1;
        }
    }

    public void ResetHistory()
    {
        if (!ShowsHistory) return;
        boardController.ResetPieces(gameController.gameState.BoardState);
        historyIndex = -1;
    }

    public GameStateInterface GetGameAtHistory()
    {
        var historyGame = gameController.gameStateFactory.FromGameState(gameController.gameState);
        if (!ShowsHistory) return historyGame;

        for (var moveIndex = historyGame.History.Count; moveIndex > historyIndex; --moveIndex)
        {
            historyGame.UndoMove();
        }

        return historyGame;
    }
}
