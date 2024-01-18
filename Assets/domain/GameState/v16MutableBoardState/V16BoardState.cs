using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V16BoardState : BoardStateInterface
{
  public bool whiteTurn;
  public bool WhiteTurn { get => whiteTurn; }
  public List<PiecePosition> piecePositions
  {
    get
    {
      var pieceIndices = allPiecesBitBoard.extractIndices();
      var piecePositions = new List<PiecePosition>(pieceIndices.Length);

      for (var pieceIndex = 0; pieceIndex < pieceIndices.Length; ++pieceIndex)
      {
        piecePositions.Add(new PiecePosition("", pieceBoard[pieceIndices[pieceIndex]], pieceIndices[pieceIndex].toBoardPosition()));
      }

      return piecePositions;
    }
  }

  public readonly ulong[] bitBoards;
  public CastleFlags CastleFlags { get; private set; }
  public int EnPassantColumn { get; private set; }
  public readonly PieceType[] pieceBoard = new PieceType[64];
  private Hashable hashable;
  public ulong allPiecesBitBoard;

  public V16BoardState()
  {
    whiteTurn = true;
    CastleFlags = CastleFlags.All;
    EnPassantColumn = -1;
    bitBoards = new ulong[12];

    bitBoards[WhiteKing] = BoardPosition.fromColRow(4, 0).toBitBoard();
    bitBoards[WhiteQueen] = BoardPosition.fromColRow(3, 0).toBitBoard();
    bitBoards[WhiteRook] = BoardPosition.fromColRow(0, 0).toBitBoard() | BoardPosition.fromColRow(7, 0).toBitBoard();
    bitBoards[WhiteBishop] = BoardPosition.fromColRow(2, 0).toBitBoard() | BoardPosition.fromColRow(5, 0).toBitBoard();
    bitBoards[WhiteKnight] = BoardPosition.fromColRow(1, 0).toBitBoard() | BoardPosition.fromColRow(6, 0).toBitBoard();
    bitBoards[WhitePawn] = ((1ul << 8) - 1) << 8;
    bitBoards[BlackKing] = BoardPosition.fromColRow(4, 7).toBitBoard();
    bitBoards[BlackQueen] = BoardPosition.fromColRow(3, 7).toBitBoard();
    bitBoards[BlackRook] = BoardPosition.fromColRow(0, 7).toBitBoard() | BoardPosition.fromColRow(7, 7).toBitBoard();
    bitBoards[BlackBishop] = BoardPosition.fromColRow(2, 7).toBitBoard() | BoardPosition.fromColRow(5, 7).toBitBoard();
    bitBoards[BlackKnight] = BoardPosition.fromColRow(1, 7).toBitBoard() | BoardPosition.fromColRow(6, 7).toBitBoard();
    bitBoards[BlackPawn] = ((1ul << 8) - 1) << 48;

    foreach (var piecePosition in piecePositions)
    {
      pieceBoard[piecePosition.position.index] = piecePosition.pieceType;
    }

    allPiecesBitBoard = 0L;
    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      allPiecesBitBoard |= bitBoards[bitBoardIndex];
    }
  }

  public V16BoardState(BoardStateInterface other)
  {
    whiteTurn = other.WhiteTurn;
    CastleFlags = other.CastleFlags;
    EnPassantColumn = other.EnPassantColumn;
    bitBoards = piecePositionsToBitBoards(other.piecePositions);

    foreach (var piecePosition in other.piecePositions)
    {
      pieceBoard[piecePosition.position.index] = piecePosition.pieceType;
    }

    allPiecesBitBoard = 0L;
    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      allPiecesBitBoard |= bitBoards[bitBoardIndex];
    }
  }

  public V16BoardState(bool whiteTurn, List<PiecePosition> piecePositions, CastleFlags castleFlags)
  {
    this.whiteTurn = whiteTurn;
    this.CastleFlags = castleFlags;
    EnPassantColumn = -1;
    bitBoards = piecePositionsToBitBoards(piecePositions);

    foreach (var piecePosition in piecePositions)
    {
      pieceBoard[piecePosition.position.index] = piecePosition.pieceType;
    }

    allPiecesBitBoard = 0L;
    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      allPiecesBitBoard |= bitBoards[bitBoardIndex];
    }
  }

  public class BoardStatePlay
  {
    public readonly PieceType sourcePieceType;
    public readonly PiecePosition killedPiece;

    public BoardStatePlay(PieceType sourcePieceType, PiecePosition killedPiece)
    {
      this.sourcePieceType = sourcePieceType;
      this.killedPiece = killedPiece;
    }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var killedPieceType = pieceBoard[move.target.index];
    PiecePosition killedPiece = null;

    var sourceBitBoard = move.source.index.toBitBoard();
    var targetBitBoard = move.target.index.toBitBoard();

    if (killedPieceType != PieceType.Nothing)
    {
      killedPiece = new PiecePosition(string.Empty, killedPieceType, move.target);
      pieceBoard[move.target.index] = PieceType.Nothing;
      bitBoards[PieceTypeToBitBoardIndex(killedPieceType)] ^= targetBitBoard;
    }

    PieceType sourcePieceType = pieceBoard[move.source.index];
    if (sourcePieceType == PieceType.Nothing)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    pieceBoard[move.source.index] = PieceType.Nothing;

    this.allPiecesBitBoard = (this.allPiecesBitBoard & ~sourceBitBoard) | targetBitBoard;

    if (move.promotion == PieceType.Nothing)
    {
      pieceBoard[move.target.index] = sourcePieceType;
      bitBoards[PieceTypeToBitBoardIndex(sourcePieceType)] ^= sourceBitBoard | targetBitBoard;
    }
    else
    {
      pieceBoard[move.target.index] = move.promotion;
      bitBoards[PieceTypeToBitBoardIndex(sourcePieceType)] ^= sourceBitBoard;
      bitBoards[PieceTypeToBitBoardIndex(move.promotion)] ^= targetBitBoard;
    }

    var enemyPawnBitBoardIndex = sourcePieceType.IsWhite() ? BlackPawn : WhitePawn;

    // En passant
    if (sourcePieceType.IsPawn() && move.source.col != move.target.col && killedPiece == null)
    {
      var killedPiecePosition = new BoardPosition(move.target.col, move.source.row);
      killedPiece = new PiecePosition("0", sourcePieceType.IsWhite() ? PieceType.BlackPawn : PieceType.WhitePawn, killedPiecePosition);
      var killedPieceBitBoard = killedPiecePosition.index.toBitBoard();
      bitBoards[enemyPawnBitBoardIndex] ^= killedPieceBitBoard;
      pieceBoard[killedPiecePosition.index] = PieceType.Nothing;
      this.allPiecesBitBoard ^= killedPieceBitBoard;
    }

    var sourceOrTarget = sourceBitBoard | targetBitBoard;
    var lostCastleRights = (((sourceOrTarget & whiteKingRookPosition) != 0) ? CastleFlags.WhiteKing : CastleFlags.Nothing)
      | (((sourceOrTarget & whiteQueenRookPosition) != 0) ? CastleFlags.WhiteQueen : CastleFlags.Nothing)
      | (((sourceOrTarget & blackKingRookPosition) != 0) ? CastleFlags.BlackKing : CastleFlags.Nothing)
      | (((sourceOrTarget & blackQueenRookPosition) != 0) ? CastleFlags.BlackQueen : CastleFlags.Nothing);

    this.CastleFlags = this.CastleFlags & ~lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      var rookSwap = GetRookSwapBitBoard(move.target.index);
      bitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= rookSwap.bitBoard;
      this.allPiecesBitBoard ^= rookSwap.bitBoard;
      pieceBoard[rookSwap.source] = PieceType.Nothing;
      pieceBoard[rookSwap.target] = sourcePieceType.IsWhite() ? PieceType.WhiteRook : PieceType.BlackRook;
    }

    this.EnPassantColumn = -1;

    // Only note enPassant if neighbour square has an enemy pawn to leverage legal transpositions
    if (sourcePieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2)
    {
      var targetNeighbourBitBoard = move.target.index.toBitBoard().shiftColumn(-1) | move.target.index.toBitBoard().shiftColumn(1);
      if ((bitBoards[enemyPawnBitBoardIndex] & targetNeighbourBitBoard) != 0)
      {
        this.EnPassantColumn = move.source.col;
      }
    }

    this.hashable = null;
    this.whiteTurn = !this.whiteTurn;

    return new BoardStatePlay(
      sourcePieceType,
      killedPiece
    );
  }

  public void UndoMove(ReversibleMove reversibleMove)
  {
    var targetBitBoard = reversibleMove.target.index.toBitBoard();
    var sourceBitBoard = reversibleMove.source.index.toBitBoard();

    PieceType sourcePieceType = pieceBoard[reversibleMove.target.index];

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    pieceBoard[reversibleMove.target.index] = PieceType.Nothing;
    this.allPiecesBitBoard = (this.allPiecesBitBoard | sourceBitBoard) & ~targetBitBoard;
    if (reversibleMove.promotion == PieceType.Nothing)
    {
      pieceBoard[reversibleMove.source.index] = sourcePieceType;
      bitBoards[PieceTypeToBitBoardIndex(sourcePieceType)] ^= sourceBitBoard | targetBitBoard;
    }
    else
    {
      var prePromotionPieceType = sourcePieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn;
      pieceBoard[reversibleMove.source.index] = prePromotionPieceType;
      bitBoards[PieceTypeToBitBoardIndex(prePromotionPieceType)] ^= sourceBitBoard;
      bitBoards[PieceTypeToBitBoardIndex(sourcePieceType)] ^= targetBitBoard;
    }

    if (reversibleMove.killed != null)
    {
      var killedBitBoard = reversibleMove.killed.position.index.toBitBoard();
      bitBoards[PieceTypeToBitBoardIndex(reversibleMove.killed.pieceType)] |= killedBitBoard;
      allPiecesBitBoard |= killedBitBoard;
      pieceBoard[reversibleMove.killed.position.index] = reversibleMove.killed.pieceType;
    }

    this.CastleFlags = this.CastleFlags | reversibleMove.lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      var rookSwap = GetRookSwapBitBoard(reversibleMove.target.index);
      bitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= rookSwap.bitBoard;
      this.allPiecesBitBoard ^= rookSwap.bitBoard;
      pieceBoard[rookSwap.target] = PieceType.Nothing;
      pieceBoard[rookSwap.source] = sourcePieceType.IsWhite() ? PieceType.WhiteRook : PieceType.BlackRook;
    }

    this.hashable = null;

    this.EnPassantColumn = reversibleMove.oldEnPassantColumn;
    this.whiteTurn = !this.whiteTurn;
  }

  public PieceType FastPlayKing(bool isWhite, int source, int target, PieceType newSourcePiece)
  {
    var sourceBitBoard = source.toBitBoard();
    var targetBitBoard = target.toBitBoard();

    var targetPieceType = pieceBoard[target];
    if (targetPieceType != PieceType.Nothing)
    {
      bitBoards[PieceTypeToBitBoardIndex(targetPieceType)] ^= targetBitBoard;
    }

    var sourceAndTargetBitBoard = sourceBitBoard | targetBitBoard;
    bitBoards[isWhite ? WhiteKing : BlackKing] ^= sourceAndTargetBitBoard;

    allPiecesBitBoard |= targetBitBoard;
    if (newSourcePiece == PieceType.Nothing)
    {
      allPiecesBitBoard &= ~sourceBitBoard;
    }
    else
    {
      bitBoards[PieceTypeToBitBoardIndex(newSourcePiece)] ^= sourceBitBoard;
    }

    pieceBoard[source] = newSourcePiece;
    pieceBoard[target] = isWhite ? PieceType.WhiteKing : PieceType.BlackKing;
    return targetPieceType;
  }

  public override bool Equals(object obj)
  {
    throw new Exception("Method not supported");
  }

  public override int GetHashCode()
  {
    throw new Exception("Method not supported");
  }

  private static ulong[] piecePositionsToBitBoards(List<PiecePosition> piecePositions)
  {
    var bitBoards = new ulong[12];

    for (var pieceIndex = 0; pieceIndex < piecePositions.Count; ++pieceIndex)
    {
      var piecePosition = piecePositions[pieceIndex];
      bitBoards[PieceTypeToBitBoardIndex(piecePosition.pieceType)] |= piecePosition.position.index.toBitBoard();
    }

    return bitBoards;
  }

  private readonly static ulong whiteKingRookPosition = BoardPosition.fromColRow(7, 0).toBitBoard() | BoardPosition.fromColRow(4, 0).toBitBoard();
  private readonly static ulong whiteQueenRookPosition = BoardPosition.fromColRow(0, 0).toBitBoard() | BoardPosition.fromColRow(4, 0).toBitBoard();
  private readonly static ulong blackKingRookPosition = BoardPosition.fromColRow(7, 7).toBitBoard() | BoardPosition.fromColRow(4, 7).toBitBoard();
  private readonly static ulong blackQueenRookPosition = BoardPosition.fromColRow(0, 7).toBitBoard() | BoardPosition.fromColRow(4, 7).toBitBoard();

  private class RookSwapInfo
  {
    public readonly ulong bitBoard;
    public readonly int source;
    public readonly int target;
    public RookSwapInfo(int source, int target)
    {
      this.bitBoard = source.toBitBoard() | target.toBitBoard();
      this.source = source;
      this.target = target;
    }
  }

  private readonly static RookSwapInfo[] rookSwapInfo = new RookSwapInfo[] {
    new RookSwapInfo(BoardPosition.fromColRow(0, 0), BoardPosition.fromColRow(3, 0)),
    new RookSwapInfo(BoardPosition.fromColRow(7, 0), BoardPosition.fromColRow(5, 0)),
    new RookSwapInfo(BoardPosition.fromColRow(0, 7), BoardPosition.fromColRow(3, 7)),
    new RookSwapInfo(BoardPosition.fromColRow(7, 7), BoardPosition.fromColRow(5, 7)),
  };

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static RookSwapInfo GetRookSwapBitBoard(int kingTargetIndex)
  {
    return rookSwapInfo[(kingTargetIndex - 2) % 8 % 3 + 2 * (kingTargetIndex / 54)];
  }

  public static int WhitePawn = 0;
  public static int BlackPawn = 1;
  public static int WhiteRook = 2;
  public static int BlackRook = 3;
  public static int WhiteKnight = 4;
  public static int BlackKnight = 5;
  public static int WhiteBishop = 6;
  public static int BlackBishop = 7;
  public static int WhiteQueen = 8;
  public static int BlackQueen = 9;
  public static int WhiteKing = 10;
  public static int BlackKing = 11;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int PieceTypeToBitBoardIndex(PieceType pieceType)
  {
    return 2 * ((ulong)pieceType).lsbUnchecked() + (((int)pieceType >> 6) - 1);
  }

  public Hashable GetHashable()
  {
    if (hashable != null) return hashable;
    hashable = new Hashable(this);
    return hashable;
  }

  public class Hashable : BoardStateInterface
  {
    public readonly bool whiteTurn;
    public bool WhiteTurn { get => whiteTurn; }
    public List<PiecePosition> piecePositions
    {
      get
      {
        throw new Exception("Method not supported");
      }
    }

    private readonly ulong whitePawnBitBoard;
    private readonly ulong whiteRookBitBoard;
    private readonly ulong whiteKnightBitBoard;
    private readonly ulong whiteBishopBitBoard;
    private readonly ulong whiteQueenBitBoard;
    private readonly ulong whiteKingBitBoard;
    private readonly ulong blackPawnBitBoard;
    private readonly ulong blackRookBitBoard;
    private readonly ulong blackKnightBitBoard;
    private readonly ulong blackBishopBitBoard;
    private readonly ulong blackQueenBitBoard;
    private readonly ulong blackKingBitBoard;
    public CastleFlags CastleFlags { get; }
    public int EnPassantColumn { get; }

    public Hashable(V16BoardState boardState)
    {
      this.whiteTurn = boardState.whiteTurn;
      this.CastleFlags = boardState.CastleFlags;
      this.EnPassantColumn = boardState.EnPassantColumn;

      this.whitePawnBitBoard = boardState.bitBoards[WhitePawn];
      this.whiteRookBitBoard = boardState.bitBoards[WhiteRook];
      this.whiteKnightBitBoard = boardState.bitBoards[WhiteKnight];
      this.whiteBishopBitBoard = boardState.bitBoards[WhiteBishop];
      this.whiteQueenBitBoard = boardState.bitBoards[WhiteQueen];
      this.whiteKingBitBoard = boardState.bitBoards[WhiteKing];
      this.blackPawnBitBoard = boardState.bitBoards[BlackPawn];
      this.blackRookBitBoard = boardState.bitBoards[BlackRook];
      this.blackKnightBitBoard = boardState.bitBoards[BlackKnight];
      this.blackBishopBitBoard = boardState.bitBoards[BlackBishop];
      this.blackQueenBitBoard = boardState.bitBoards[BlackQueen];
      this.blackKingBitBoard = boardState.bitBoards[BlackKing];
    }

    public BoardStatePlay PlayMove(Move move)
    {
      throw new Exception("Method not supported");
    }

    public V16BoardState UndoMove(ReversibleMove reversibleMove)
    {
      throw new Exception("Method not supported");
    }

    public override bool Equals(object obj)
    {
      var other = (Hashable)obj;

      return CastleFlags == other.CastleFlags
        && EnPassantColumn == other.EnPassantColumn
        && whiteTurn == other.whiteTurn
        && whitePawnBitBoard == other.whitePawnBitBoard
        && whiteRookBitBoard == other.whiteRookBitBoard
        && whiteKnightBitBoard == other.whiteKnightBitBoard
        && whiteBishopBitBoard == other.whiteBishopBitBoard
        && whiteQueenBitBoard == other.whiteQueenBitBoard
        && whiteKingBitBoard == other.whiteKingBitBoard
        && blackPawnBitBoard == other.blackPawnBitBoard
        && blackRookBitBoard == other.blackRookBitBoard
        && blackKnightBitBoard == other.blackKnightBitBoard
        && blackBishopBitBoard == other.blackBishopBitBoard
        && blackQueenBitBoard == other.blackQueenBitBoard
        && blackKingBitBoard == other.blackKingBitBoard;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = EnPassantColumn + 2;
        hashCode = hashCode * 17 + (int)CastleFlags + 1;
        hashCode = hashCode * 2 + (whiteTurn ? 1 : 0);

        hashCode ^= (int)whitePawnBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)whiteRookBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)whiteKnightBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)whiteBishopBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)whiteQueenBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)whiteKingBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackPawnBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackRookBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackKnightBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackBishopBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackQueenBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)blackKingBitBoard + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);

        hashCode ^= (int)(whitePawnBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(whiteRookBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(whiteKnightBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(whiteBishopBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(whiteQueenBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(whiteKingBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackPawnBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackRookBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackKnightBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackBishopBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackQueenBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        hashCode ^= (int)(blackKingBitBoard >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);

        return hashCode;
      }
    }
  }
}
