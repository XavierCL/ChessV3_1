using System;
using UnityEngine;

public static class StaticReferences
{
    public static Lazy<PromotionHandler> promotionHandler { get; } = new Lazy<PromotionHandler>(() => GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>());
    public static Lazy<BoardController> boardController { get; } = new Lazy<BoardController>(() => GameObject.Find(nameof(BoardController)).GetComponent<BoardController>());
    public static Lazy<GameController> gameController { get; } = new Lazy<GameController>(() => GameObject.Find(nameof(GameController)).GetComponent<GameController>());
    public static Lazy<Clocks> clocks { get; } = new Lazy<Clocks>(() => GameObject.Find(nameof(Clocks)).GetComponent<Clocks>());
    public static Lazy<Shapes> shapes { get; } = new Lazy<Shapes>(() => GameObject.Find(nameof(Shapes)).GetComponent<Shapes>());
    public static Lazy<PieceSprites> pieceSprites { get; } = new Lazy<PieceSprites>(() => GameObject.Find(nameof(PieceSprites)).GetComponent<PieceSprites>());
}
