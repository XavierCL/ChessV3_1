using UnityEngine;

public class PieceAnimation
{
    public float startTimeSeconds { get; set; }
    public float endTimeSeconds { get; set; }
    public Vector2 startPosition { get; set; }
    public BoardPosition endPosition { get; set; }
    public GameObject gameObject { get; set; }
    public PieceType newType { get; set; }
}