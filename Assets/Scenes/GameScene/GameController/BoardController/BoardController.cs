using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public float AnimationSeconds = 0.1f;
    public float AnimationMinimumSpeed = 10f;
    public bool BoardRotated = false;
    public PieceSprites pieceSprites;
    public Shapes shapes;
    public List<PieceGameObject> pieceGameObjects;
    public List<PieceAnimation> pieceAnimations = new List<PieceAnimation>();
    public Color PossibleTargetColor = Color.green;
    public Color CurrentTargetColor = Color.green;
    public float PossibleTargetRadius = 0.2f;
    public float SingleFrameZ = 0.01f;

    public void Awake()
    {
        GetPieceGameObjects();
        pieceSprites = GameObject.Find(nameof(PieceSprites)).GetComponent<PieceSprites>();
        shapes = GameObject.Find(nameof(Shapes)).GetComponent<Shapes>();
    }

    void Update()
    {
        var frameTime = Time.time;

        // Handle finished animations
        foreach (var pieceAnimation in pieceAnimations.Where(pieceAnimation => pieceAnimation.endTimeSeconds <= frameTime))
        {
            var worldPosition = BoardPositionToWorldPosition(pieceAnimation.endPosition);
            pieceAnimation.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            var spriteRenderer = pieceAnimation.gameObject.GetComponent<SpriteRenderer>();

            if (pieceAnimation.newType.HasValue)
            {
                spriteRenderer.sprite = pieceSprites.GetSpriteFor(pieceAnimation.newType.Value);
            }
            spriteRenderer.sortingLayerName = "Pieces";

            // Todo handle rock
        }

        pieceAnimations = pieceAnimations.Where(pieceAnimation => pieceAnimation.endTimeSeconds > frameTime).ToList();

        // Handle ongoing animations
        foreach (var pieceAnimation in pieceAnimations)
        {
            var endWorldPosition = BoardPositionToWorldPosition(pieceAnimation.endPosition);
            var animationRatio = Mathf.InverseLerp(pieceAnimation.startTimeSeconds, pieceAnimation.endTimeSeconds, frameTime);
            var deltaPosition = endWorldPosition - pieceAnimation.startPosition;
            var currentPosition = pieceAnimation.startPosition + deltaPosition * animationRatio;
            pieceAnimation.gameObject.transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
            pieceAnimation.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Animation";
        }
    }

    public void DrawPossibleTargets(GameState gameState, BoardPosition source)
    {
        var possibleTargets = gameState.getLegalMoves().Where(move => move.source.Equals(source)).Select(move => move.target);
        foreach (var possibleTarget in possibleTargets)
        {
            var possibleWorldPosition = BoardPositionToWorldPosition(possibleTarget);
            shapes.Circle(new Vector3(possibleWorldPosition.x, possibleWorldPosition.y, SingleFrameZ), PossibleTargetRadius, PossibleTargetColor);
        }
    }

    public void DrawCurrentTarget(GameState gameState, BoardPosition source, Vector2 targetWorldPosition)
    {
        var targetPosition = WorldPositionToBoardPosition(targetWorldPosition);
        if (!gameState.getLegalMoves().Any(move => move.source.Equals(source) && move.target.Equals(targetPosition))) return;

        var normalizedTargetWorldPosition = BoardPositionToWorldPosition(targetPosition);
        shapes.Rectangle(new Vector3(normalizedTargetWorldPosition.x, normalizedTargetWorldPosition.y, SingleFrameZ), 1, 1, CurrentTargetColor);
    }

    public void ResetPieces(GameState gameState)
    {
        var piecePositions = gameState.getPiecePositions().Select((piecePosition, index) => (piecePosition, index)).ToList();
        foreach (var (piecePosition, index) in piecePositions)
        {
            var pieceGameObject = GetPieceGameObjects()[index];
            var worldPosition = BoardPositionToWorldPosition(piecePosition.position);
            pieceGameObject.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            pieceGameObject.gameObject.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(piecePosition.pieceType);
            pieceGameObject.gameObject.SetActive(true);
            pieceGameObject.position = piecePosition.position;
        }

        foreach (var index in Enumerable.Range(piecePositions.Count, GetPieceGameObjects().Count - piecePositions.Count))
        {
            GetPieceGameObjects()[index].gameObject.SetActive(false);
            GetPieceGameObjects()[index].position = null;
        }
    }

    public BoardPosition WorldPositionToBoardPosition(Vector2 worldPosition)
    {
        if (BoardRotated) worldPosition = -worldPosition;
        return new BoardPosition((int)(worldPosition.x + 4), (int)(worldPosition.y + 4));
    }

    public Vector2 BoardPositionToWorldPosition(BoardPosition boardPosition)
    {
        var worldPosition = new Vector2(boardPosition.col - 3.5f, boardPosition.row - 3.5f);
        return BoardRotated ? -worldPosition : worldPosition;
    }

    [ContextMenu("Rotate board")]
    public void RotateBoard()
    {
        var rotation = BoardRotated ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 0, 180));
        GameObject.Find("TilesAndPieces").transform.rotation = rotation;
        GetPieceGameObjects().ForEach(piece => piece.gameObject.transform.localRotation = rotation);
        BoardRotated = !BoardRotated;
    }

    public void RotateWhiteBottom()
    {
        if (BoardRotated) RotateBoard();
    }

    public void RotateBlackBottom()
    {
        if (!BoardRotated) RotateBoard();
    }

    public void AnimatePiece(GameObject gameObject, BoardPosition destination, PieceType? newType, bool animated)
    {
        pieceAnimations = pieceAnimations.Where(pieceAnimation => !pieceAnimation.gameObject.Equals(gameObject)).ToList();
        var startTime = Time.time;
        var startPosition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        var endWorldPosition = BoardPositionToWorldPosition(destination);
        var deltaNorm = (endWorldPosition - startPosition).magnitude;
        var speed = Mathf.Clamp(deltaNorm / AnimationSeconds, AnimationMinimumSpeed, deltaNorm / AnimationSeconds);
        var finalDuration = animated ? deltaNorm / speed : 0;
        pieceAnimations.Add(new PieceAnimation
        {
            gameObject = gameObject,
            startPosition = startPosition,
            startTimeSeconds = startTime,
            endPosition = destination,
            endTimeSeconds = startTime + finalDuration,
            newType = newType,
        });
    }

    public void AnimateMove(Move move, bool animated)
    {
        var pieceGameObject = GetPieceGameObjects().Find(pieceGameObject => Equals(pieceGameObject.position, move.source));
        if (pieceGameObject == null) return;

        // Kill targets
        var killedTarget = pieceGameObjects.Find(possibleTarget => Equals(possibleTarget.position, move.target));
        if (killedTarget != null)
        {
            killedTarget.gameObject.SetActive(false);
            killedTarget.position = null;
        }

        // Todo handle en-passant

        pieceGameObject.position = move.target;
        AnimatePiece(pieceGameObject.gameObject, move.target, move.promotion, animated);
    }

    private List<PieceGameObject> GetPieceGameObjects()
    {
        if (pieceGameObjects != null) return pieceGameObjects;

        pieceGameObjects = new List<PieceGameObject> {
            new PieceGameObject { gameObject = GameObject.Find("a1") },
            new PieceGameObject { gameObject = GameObject.Find("a2") },
            new PieceGameObject { gameObject = GameObject.Find("a7") },
            new PieceGameObject { gameObject = GameObject.Find("a8") },
            new PieceGameObject { gameObject = GameObject.Find("b2") },
            new PieceGameObject { gameObject = GameObject.Find("b7") },
            new PieceGameObject { gameObject = GameObject.Find("b8") },
            new PieceGameObject { gameObject = GameObject.Find("b1") },
            new PieceGameObject { gameObject = GameObject.Find("c1") },
            new PieceGameObject { gameObject = GameObject.Find("c2") },
            new PieceGameObject { gameObject = GameObject.Find("c7") },
            new PieceGameObject { gameObject = GameObject.Find("c8") },
            new PieceGameObject { gameObject = GameObject.Find("d1") },
            new PieceGameObject { gameObject = GameObject.Find("d2") },
            new PieceGameObject { gameObject = GameObject.Find("d7") },
            new PieceGameObject { gameObject = GameObject.Find("d8") },
            new PieceGameObject { gameObject = GameObject.Find("e1") },
            new PieceGameObject { gameObject = GameObject.Find("e2") },
            new PieceGameObject { gameObject = GameObject.Find("e7") },
            new PieceGameObject { gameObject = GameObject.Find("e8") },
            new PieceGameObject { gameObject = GameObject.Find("f1") },
            new PieceGameObject { gameObject = GameObject.Find("f2") },
            new PieceGameObject { gameObject = GameObject.Find("f7") },
            new PieceGameObject { gameObject = GameObject.Find("f8") },
            new PieceGameObject { gameObject = GameObject.Find("g1") },
            new PieceGameObject { gameObject = GameObject.Find("g2") },
            new PieceGameObject { gameObject = GameObject.Find("g7") },
            new PieceGameObject { gameObject = GameObject.Find("g8") },
            new PieceGameObject { gameObject = GameObject.Find("h1") },
            new PieceGameObject { gameObject = GameObject.Find("h2") },
            new PieceGameObject { gameObject = GameObject.Find("h7") },
            new PieceGameObject { gameObject = GameObject.Find("h8") },
        };

        return pieceGameObjects;
    }
}
