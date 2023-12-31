using UnityEngine;

public class PieceGameObject
{
    public string id { get; set; }
    public GameObject gameObject { get; set; }
    public BoardPosition? position { get; set; }
    public PieceType pieceType { get; set; } = PieceType.Nothing;
}
