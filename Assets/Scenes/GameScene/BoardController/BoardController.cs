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
        shapes = GameObject.Find(nameof(Shapes)).GetComponent<Shapes>();
    }

    void Update()
    {
        UpdateAnimation();
    }

    public void DrawPossibleTargets(List<BoardPosition> possibleTargets, BoardPosition source)
    {
        foreach (var possibleTarget in possibleTargets)
        {
            var possibleWorldPosition = BoardPositionToWorldPosition(possibleTarget);
            shapes.Circle(new Vector3(possibleWorldPosition.x, possibleWorldPosition.y, SingleFrameZ), PossibleTargetRadius, PossibleTargetColor);
        }
    }

    public void DrawCurrentTarget(BoardPosition targetPosition)
    {
        var normalizedTargetWorldPosition = BoardPositionToWorldPosition(targetPosition);
        shapes.Rectangle(new Vector3(normalizedTargetWorldPosition.x, normalizedTargetWorldPosition.y, SingleFrameZ), 1, 1, CurrentTargetColor);
    }

    public void ClearAnimations()
    {
        pieceAnimations.Clear();
    }

    public void ResetPieces(GameState gameState)
    {
        var pieceGameObjects = GetPieceGameObjects().ToDictionary(piece => piece.id);
        var unseenPieceIds = new HashSet<string>(pieceGameObjects.Keys);
        foreach (var piecePosition in gameState.getPiecePositions())
        {
            unseenPieceIds.Remove(piecePosition.id);
            var pieceGameObject = pieceGameObjects[piecePosition.id];
            var worldPosition = BoardPositionToWorldPosition(piecePosition.position);
            pieceGameObject.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            var spriteRenderer = pieceGameObject.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetPieceSprites().GetSpriteFor(piecePosition.pieceType);
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "Pieces";
            pieceGameObject.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            pieceGameObject.position = piecePosition.position;
        }

        foreach (var unseenPieceId in unseenPieceIds)
        {
            var pieceGameObject = pieceGameObjects[unseenPieceId];
            var spriteRenderer = pieceGameObject.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            pieceGameObject.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            pieceGameObject.position = null;
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

    public void AnimatePiece(GameObject gameObject, BoardPosition destination, PieceType newType, bool animated)
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

    public void AnimateMove(Move move, bool simpleKill, bool animated)
    {
        var pieceGameObject = GetPieceGameObjects().Find(pieceGameObject => Equals(pieceGameObject.position, move.source));
        if (pieceGameObject == null) return;

        // Kill targets
        var killedTarget = pieceGameObjects.Find(possibleTarget => Equals(possibleTarget.position, move.target));
        if (killedTarget != null)
        {
            killedTarget.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            killedTarget.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            killedTarget.position = null;
        }

        if (!simpleKill)
        {
            // Todo handle en-passant & rock
        }

        pieceGameObject.position = move.target;
        AnimatePiece(pieceGameObject.gameObject, move.target, move.promotion, animated);
    }

    public PiecePosition GetPieceAtGameObject(GameObject gameObject)
    {
        var foundPieceGameObject = GetPieceGameObjects().FirstOrDefault(pieceGameObject => Equals(pieceGameObject.gameObject, gameObject));

        if (foundPieceGameObject == null || !foundPieceGameObject.position.HasValue) return null;

        var pieceType = GetPieceSprites().GetSpritePieceType(foundPieceGameObject.gameObject.GetComponent<SpriteRenderer>().sprite);

        return new PiecePosition(foundPieceGameObject.id, pieceType, foundPieceGameObject.position.Value);
    }

    private void UpdateAnimation()
    {
        var frameTime = Time.time;

        // Handle finished animations
        foreach (var pieceAnimation in pieceAnimations.Where(pieceAnimation => pieceAnimation.endTimeSeconds <= frameTime))
        {
            var worldPosition = BoardPositionToWorldPosition(pieceAnimation.endPosition);
            pieceAnimation.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
            var spriteRenderer = pieceAnimation.gameObject.GetComponent<SpriteRenderer>();

            if (pieceAnimation.newType != PieceType.Nothing)
            {
                spriteRenderer.sprite = pieceSprites.GetSpriteFor(pieceAnimation.newType);
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

    private List<PieceGameObject> GetPieceGameObjects()
    {
        if (pieceGameObjects != null) return pieceGameObjects;

        pieceGameObjects = new List<PieceGameObject> {
            new PieceGameObject { id="a1", gameObject = GameObject.Find("a1") },
            new PieceGameObject { id="a2", gameObject = GameObject.Find("a2") },
            new PieceGameObject { id="a6", gameObject = GameObject.Find("a7") },
            new PieceGameObject { id="a7", gameObject = GameObject.Find("a8") },
            new PieceGameObject { id="b1", gameObject = GameObject.Find("b2") },
            new PieceGameObject { id="b2", gameObject = GameObject.Find("b7") },
            new PieceGameObject { id="b6", gameObject = GameObject.Find("b8") },
            new PieceGameObject { id="b7", gameObject = GameObject.Find("b1") },
            new PieceGameObject { id="c1", gameObject = GameObject.Find("c1") },
            new PieceGameObject { id="c2", gameObject = GameObject.Find("c2") },
            new PieceGameObject { id="c6", gameObject = GameObject.Find("c7") },
            new PieceGameObject { id="c7", gameObject = GameObject.Find("c8") },
            new PieceGameObject { id="d1", gameObject = GameObject.Find("d1") },
            new PieceGameObject { id="d2", gameObject = GameObject.Find("d2") },
            new PieceGameObject { id="d6", gameObject = GameObject.Find("d7") },
            new PieceGameObject { id="d7", gameObject = GameObject.Find("d8") },
            new PieceGameObject { id="e1", gameObject = GameObject.Find("e1") },
            new PieceGameObject { id="e2", gameObject = GameObject.Find("e2") },
            new PieceGameObject { id="e6", gameObject = GameObject.Find("e7") },
            new PieceGameObject { id="e7", gameObject = GameObject.Find("e8") },
            new PieceGameObject { id="f1", gameObject = GameObject.Find("f1") },
            new PieceGameObject { id="f2", gameObject = GameObject.Find("f2") },
            new PieceGameObject { id="f6", gameObject = GameObject.Find("f7") },
            new PieceGameObject { id="f7", gameObject = GameObject.Find("f8") },
            new PieceGameObject { id="g1", gameObject = GameObject.Find("g1") },
            new PieceGameObject { id="g2", gameObject = GameObject.Find("g2") },
            new PieceGameObject { id="g6", gameObject = GameObject.Find("g7") },
            new PieceGameObject { id="g7", gameObject = GameObject.Find("g8") },
            new PieceGameObject { id="h1", gameObject = GameObject.Find("h1") },
            new PieceGameObject { id="h2", gameObject = GameObject.Find("h2") },
            new PieceGameObject { id="h6", gameObject = GameObject.Find("h7") },
            new PieceGameObject { id="h7", gameObject = GameObject.Find("h8") },
        };

        return pieceGameObjects;
    }

    private PieceSprites GetPieceSprites()
    {
        if (pieceSprites != null) return pieceSprites;

        pieceSprites = GameObject.Find(nameof(PieceSprites)).GetComponent<PieceSprites>();

        return pieceSprites;
    }
}
