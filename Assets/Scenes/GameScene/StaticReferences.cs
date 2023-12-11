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
    public static Lazy<GameObject> topClock { get; } = new Lazy<GameObject>(() => GameObject.Find("TopTime"));
    public static Lazy<GameObject> bottomClock { get; } = new Lazy<GameObject>(() => GameObject.Find("BottomTime"));
    public static Lazy<EndStateText> endStateText { get; } = new Lazy<EndStateText>(() => GameObject.Find(nameof(EndStateText)).GetComponent<EndStateText>());
    public static Lazy<HistoryHandler> historyHandler { get; } = new Lazy<HistoryHandler>(() => GameObject.Find(nameof(HistoryHandler)).GetComponent<HistoryHandler>());
    public static Lazy<PremoveHandler> premoveHandler { get; } = new Lazy<PremoveHandler>(() => GameObject.Find(nameof(PremoveHandler)).GetComponent<PremoveHandler>());
}
