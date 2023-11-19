using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanOnlyGameVisual : HumanGameVisual
{
    public override void StartGame(GameState gameState)
    {
        base.StartGame(gameState);
        clocks.Restart(false);
    }

    public override void PlayAnimatedMove(Move move, bool animated)
    {
        boardController.RotateBoard();
        clocks.Swap();
        base.PlayAnimatedMove(move, true);
    }

    public override void BoardMousePress()
    {
        var collision = GetPointerCollision();

        if (collision == null)
        {
            return;
        }

        if (promotionHandler.PromotionInProgress)
        {
            var move = promotionHandler.FinalizePromotion(collision);
            if (move == null) return;

            gameController.PlayMove(move);
        }

        // Check own pieces
        var pieceAtPosition = boardController.GetPieceAtGameObject(collision);

        if (pieceAtPosition.pieceType == PieceType.Nothing) return;
        if (gameController.gameState.whiteTurn != pieceAtPosition.pieceType.IsWhite()) return;

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

        // Reset if move is illegal while in live mode.
        var validMoves = gameController.gameState.getLegalMoves().Where(move => move.source.Equals(startPosition) && move.target.Equals(endBoardPosition)).ToList();
        if (validMoves.Count == 0)
        {
            CancelDrawPieceToPointer();
            return;
        }

        if (validMoves.Count > 1)
        {
            // Pawn promotion
            promotionHandler.PromptPromotion(startPosition, endBoardPosition);
        }
        else
        {
            // Play move
            gameController.PlayMove(new Move(startPosition, endBoardPosition, PieceType.Nothing));
        }

        ForceStopDrawPieceToPointer();
    }

    protected override void UpdateSelectedHover()
    {
        base.UpdateSelectedHover();

        if (!selectedPiece) return;

        // Draw legal move dots
        var legalTargetPositions = gameController
            .gameState
            .getLegalMoves()
            .Where(move => Equals(move.source, startPosition))
            .Select(move => move.target)
            .ToList();

        boardController.DrawPossibleTargets(legalTargetPositions, startPosition);

        // Draw currently highlighted square if it's legal
        var targetPosition = GetPointerBoardPosition();
        if (!legalTargetPositions.Any(position => Equals(position, targetPosition))) return;

        boardController.DrawCurrentTarget(targetPosition);
    }
}
