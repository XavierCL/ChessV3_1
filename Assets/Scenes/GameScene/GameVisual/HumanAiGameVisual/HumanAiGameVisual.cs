using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanAiGameVisual : HumanGameVisual
{
    private bool humanWhite;
    private PremoveHandler premoveHandler;

    public HumanAiGameVisual(bool humanWhite)
    {
        this.humanWhite = humanWhite;
        premoveHandler = GameObject.Find(nameof(PremoveHandler)).GetComponent<PremoveHandler>();
    }

    public override void StartGame(GameState gameState)
    {
        if (!humanWhite) boardController.RotateBlackBottom();
        clocks.Restart(!humanWhite);
        base.StartGame(gameState);
    }

    public override void PlayAnimatedMove(Move move, bool animated)
    {
        base.PlayAnimatedMove(move, animated);
        PopPremoveQueueIfNeeded();
    }

    public override void GameOver(GameState gameState)
    {
        premoveHandler.Clear(gameState);
        base.GameOver(gameState);
    }

    public override void Cleanup()
    {
        if (!humanWhite) boardController.RotateWhiteBottom();
        base.Cleanup();
    }

    public override void BoardMousePress()
    {
        var collision = GetPointerCollision();

        if (collision == null)
        {
            premoveHandler.Clear(gameController.gameState);
            return;
        }

        var isHumanTurn = humanWhite == gameController.gameState.whiteTurn;

        if (promotionHandler.PromotionInProgress)
        {
            var move = promotionHandler.FinalizePromotion(collision);

            if (move == null && isHumanTurn) return;

            if (move == null)
            {
                // Premoving, cancel premove
                premoveHandler.Clear(gameController.gameState);
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

        if (humanWhite && !pieceAtPosition.pieceType.IsWhite()
        || !humanWhite && !pieceAtPosition.pieceType.IsBlack())
        {
            premoveHandler.Clear(gameController.gameState);
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

        if (humanWhite == gameController.gameState.whiteTurn && !premoveHandler.HasMoves())
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
            promotionHandler.PromptPromotion(startPosition, endBoardPosition);
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

        var isPremoving = humanWhite == gameController.gameState.whiteTurn || premoveHandler.HasMoves();
        var targetPosition = GetPointerBoardPosition();

        if (isPremoving)
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

        boardController.DrawPossibleTargets(legalTargetPositions, startPosition);

        // Draw currently highlighted square if it's legal
        if (!legalTargetPositions.Any(position => Equals(position, targetPosition))) return;

        boardController.DrawCurrentTarget(targetPosition);

        return;
    }

    private void PlayMoveOrPremove(Move move)
    {
        var isPremoving = humanWhite == gameController.gameState.whiteTurn && premoveHandler.HasMoves();

        if (isPremoving)
        {
            premoveHandler.Push(move);
            return;
        }

        gameController.PlayMove(move);
    }

    private void PopPremoveQueueIfNeeded()
    {
        if (humanWhite != gameController.gameState.whiteTurn) return;
        premoveHandler.ExecuteNext();
    }
}
