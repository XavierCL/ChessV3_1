using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HumanAiGameVisual : HumanGameVisual
{
    private bool humanWhite;
    private PremoveHandler premoveHandler;

    public HumanAiGameVisual(bool humanWhite)
    {
        this.humanWhite = humanWhite;
        premoveHandler = StaticReferences.premoveHandler.Value;
    }

    public override void StartGame(GameStateInterface gameState)
    {
        if (!humanWhite) boardController.RotateBlackBottom();
        clocks.Restart(!humanWhite);
        base.StartGame(gameState);
    }

    public override void PlayAnimatedMove(Move move, bool animated)
    {
        base.PlayAnimatedMove(move, humanWhite == gameController.gameState.BoardState.whiteTurn);
        PopPremoveQueueIfNeeded();
    }

    public override void GameOver(GameStateInterface gameState)
    {
        premoveHandler.Clear();
        base.GameOver(gameState);
    }

    public override void Cleanup()
    {
        if (!humanWhite) boardController.RotateWhiteBottom();
        base.Cleanup();
    }

    public override void BoardMousePress()
    {
        if (gameController.gameEndState != GameEndState.Ongoing) return;
        if (historyHandler.ShowsHistory) return;

        var collision = GetPointerCollision();

        if (collision == null)
        {
            premoveHandler.Clear();
            return;
        }

        var isHumanTurn = humanWhite == gameController.gameState.BoardState.whiteTurn;

        if (promotionHandler.PromotionInProgress)
        {
            var move = promotionHandler.FinalizePromotion(collision, humanWhite);

            if (move == null && isHumanTurn) return;

            if (move == null)
            {
                // Premoving, cancel premove
                premoveHandler.Clear();
                return;
            }

            if (isHumanTurn)
            {
                gameController.PlayMove(move);
                return;
            }

            // Premoving, add to queue
            premoveHandler.Push(move);
            return;
        }

        // Check own pieces
        var pieceAtPosition = boardController.GetPieceAtGameObject(collision);

        if (pieceAtPosition == null
        || humanWhite && !pieceAtPosition.pieceType.IsWhite()
        || !humanWhite && !pieceAtPosition.pieceType.IsBlack())
        {
            premoveHandler.Clear();
            return;
        }

        StartDrawPieceToPointer(collision, pieceAtPosition.position);
    }

    public override void BoardMouseRelease()
    {
        if (!selectedPiece) return;

        var endBoardPosition = GetPointerBoardPosition();

        if (Equals(startPosition, endBoardPosition))
        {
            CancelDrawPieceToPointer();
            return;
        }

        if (!IsPremoving())
        {
            // Reset if move is illegal while in live mode.
            var validMoves = gameController.gameState.getLegalMoves().Where(move => move.source.Equals(startPosition) && move.target.Equals(endBoardPosition)).ToList();
            if (validMoves.Count == 0)
            {
                CancelDrawPieceToPointer();
                return;
            }
        }

        var sourcePieceType = boardController.GetPieceAtGameObject(selectedPiece);

        if (sourcePieceType.pieceType.IsPawn() && (endBoardPosition.row == 0 || endBoardPosition.row == 7))
        {
            // Pawn promotion
            promotionHandler.PromptPromotion(startPosition, endBoardPosition, humanWhite);
        }
        else
        {
            // Play move
            PlayMoveOrPremove(new Move(startPosition, endBoardPosition, PieceType.Nothing));
        }

        ForceStopDrawPieceToPointer();
    }

    protected override void UpdateSelectedHover()
    {
        base.UpdateSelectedHover();

        if (!selectedPiece) return;

        var targetPosition = GetPointerBoardPosition();

        if (IsPremoving())
        {
            if (Equals(targetPosition, startPosition)) return;

            boardController.DrawCurrentTarget(targetPosition);
        }

        // Draw legal move dots
        var legalTargetPositions = gameController
            .gameState
            .getLegalMoves()
            .Where(move => Equals(move.source, startPosition))
            .Select(move => move.target)
            .ToList();

        boardController.DrawPossibleTargets(legalTargetPositions);

        // Draw currently highlighted square if it's legal
        if (!legalTargetPositions.Any(position => Equals(position, targetPosition))) return;

        boardController.DrawCurrentTarget(targetPosition);

        return;
    }

    private void PlayMoveOrPremove(Move move)
    {
        if (IsPremoving())
        {
            premoveHandler.Push(move);
            return;
        }

        gameController.PlayMove(move);
    }

    private async void PopPremoveQueueIfNeeded()
    {
        if (humanWhite != gameController.gameState.BoardState.whiteTurn) return;

        // Execute on next lifecycle so GameController.PlayMove has time to finish
        await Task.Delay(1);

        premoveHandler.ExecuteNext();
    }

    private bool IsPremoving()
    {
        return humanWhite != gameController.gameState.BoardState.whiteTurn || premoveHandler.HasMoves();
    }
}
