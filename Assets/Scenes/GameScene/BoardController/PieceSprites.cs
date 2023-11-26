using UnityEngine;

public class PieceSprites : MonoBehaviour
{
    public Sprite[] Sprites;

    public Sprite GetSpriteFor(PieceType pieceType)
    {
        switch (pieceType)
        {
            case PieceType.WhitePawn:
                return Sprites[5];
            case PieceType.WhiteRook:
                return Sprites[4];
            case PieceType.WhiteKnight:
                return Sprites[3];
            case PieceType.WhiteBishop:
                return Sprites[2];
            case PieceType.WhiteQueen:
                return Sprites[1];
            case PieceType.WhiteKing:
                return Sprites[0];
            case PieceType.BlackPawn:
                return Sprites[11];
            case PieceType.BlackRook:
                return Sprites[10];
            case PieceType.BlackKnight:
                return Sprites[9];
            case PieceType.BlackBishop:
                return Sprites[8];
            case PieceType.BlackQueen:
                return Sprites[7];
            case PieceType.BlackKing:
                return Sprites[6];
        }

        throw new System.Exception("Error loading non existing sprite");
    }
}
