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

    public PieceType GetSpritePieceType(Sprite sprite)
    {
        switch (sprite.name)
        {
            case "Chess_Pieces_Sprite_9c_0":
                return PieceType.WhiteKing;
            case "Chess_Pieces_Sprite_9c_1":
                return PieceType.WhiteQueen;
            case "Chess_Pieces_Sprite_9c_2":
                return PieceType.WhiteBishop;
            case "Chess_Pieces_Sprite_9c_3":
                return PieceType.WhiteKnight;
            case "Chess_Pieces_Sprite_9c_4":
                return PieceType.WhiteRook;
            case "Chess_Pieces_Sprite_9c_5":
                return PieceType.WhitePawn;
            case "Chess_Pieces_Sprite_9c_6":
                return PieceType.BlackKing;
            case "Chess_Pieces_Sprite_9c_7":
                return PieceType.BlackQueen;
            case "Chess_Pieces_Sprite_9c_8":
                return PieceType.BlackBishop;
            case "Chess_Pieces_Sprite_9c_9":
                return PieceType.BlackKnight;
            case "Chess_Pieces_Sprite_9c_10":
                return PieceType.BlackRook;
            case "Chess_Pieces_Sprite_9c_11":
                return PieceType.BlackPawn;
        }

        throw new System.Exception("Error loading non existing piece type");
    }
}
