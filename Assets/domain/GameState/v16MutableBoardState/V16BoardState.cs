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
      return bitBoards[WhiteKing].extractIndices().Select(index => new PiecePosition("", PieceType.WhiteKing, index.toBoardPosition()))
        .Concat(bitBoards[WhiteQueen].extractIndices().Select(index => new PiecePosition("", PieceType.WhiteQueen, index.toBoardPosition())))
        .Concat(bitBoards[WhiteRook].extractIndices().Select(index => new PiecePosition("", PieceType.WhiteRook, index.toBoardPosition())))
        .Concat(bitBoards[WhiteBishop].extractIndices().Select(index => new PiecePosition("", PieceType.WhiteBishop, index.toBoardPosition())))
        .Concat(bitBoards[WhiteKnight].extractIndices().Select(index => new PiecePosition("", PieceType.WhiteKnight, index.toBoardPosition())))
        .Concat(bitBoards[WhitePawn].extractIndices().Select(index => new PiecePosition("", PieceType.WhitePawn, index.toBoardPosition())))
        .Concat(bitBoards[BlackKing].extractIndices().Select(index => new PiecePosition("", PieceType.BlackKing, index.toBoardPosition())))
        .Concat(bitBoards[BlackQueen].extractIndices().Select(index => new PiecePosition("", PieceType.BlackQueen, index.toBoardPosition())))
        .Concat(bitBoards[BlackRook].extractIndices().Select(index => new PiecePosition("", PieceType.BlackRook, index.toBoardPosition())))
        .Concat(bitBoards[BlackBishop].extractIndices().Select(index => new PiecePosition("", PieceType.BlackBishop, index.toBoardPosition())))
        .Concat(bitBoards[BlackKnight].extractIndices().Select(index => new PiecePosition("", PieceType.BlackKnight, index.toBoardPosition())))
        .Concat(bitBoards[BlackPawn].extractIndices().Select(index => new PiecePosition("", PieceType.BlackPawn, index.toBoardPosition())))
        .ToList();
    }
  }

  public readonly ulong[] bitBoards;
  public CastleFlags castleFlags { get; private set; }
  public int enPassantColumn { get; private set; }
  private Hashable hashable;

  public V16BoardState()
  {
    whiteTurn = true;
    castleFlags = CastleFlags.All;
    enPassantColumn = -1;
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
  }

  public V16BoardState(BoardStateInterface other)
  {
    whiteTurn = other.WhiteTurn;
    castleFlags = other.castleFlags;
    enPassantColumn = other.enPassantColumn;
    bitBoards = piecePositionsToBitBoards(other.piecePositions);
  }

  public V16BoardState(
    bool whiteTurn,
    ulong[] bitBoards,
    CastleFlags castleFlags,
    int enPassantColumn
  )
  {
    this.whiteTurn = whiteTurn;
    this.castleFlags = castleFlags;
    this.enPassantColumn = enPassantColumn;
    this.bitBoards = bitBoards;
  }

  public V16BoardState(bool whiteTurn, List<PiecePosition> piecePositions, CastleFlags castleFlags)
  {
    this.whiteTurn = whiteTurn;
    this.castleFlags = castleFlags;
    enPassantColumn = -1;
    bitBoards = piecePositionsToBitBoards(piecePositions);
  }

  public class BoardStatePlay
  {
    public readonly PiecePosition sourcePiece;
    public readonly PiecePosition killedPiece;

    public BoardStatePlay(PiecePosition sourcePiece, PiecePosition killedPiece)
    {
      this.sourcePiece = sourcePiece;
      this.killedPiece = killedPiece;
    }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    PiecePosition killedPiece = null;
    var targetBitBoard = move.target.index.toBitBoard();

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((targetBitBoard & bitBoards[bitBoardIndex]) != 0)
      {
        killedPiece = new PiecePosition("0", BitBoardIndexToPieceType[bitBoardIndex], move.target.index.toBoardPosition());
        bitBoards[bitBoardIndex] ^= targetBitBoard;
        break;
      }
    }

    PieceType sourcePieceType = PieceType.Nothing;
    var sourceBitBoard = move.source.index.toBitBoard();

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((sourceBitBoard & bitBoards[bitBoardIndex]) != 0)
      {
        sourcePieceType = BitBoardIndexToPieceType[bitBoardIndex];
        bitBoards[bitBoardIndex] ^= sourceBitBoard;

        if (move.promotion == PieceType.Nothing)
        {
          bitBoards[bitBoardIndex] ^= targetBitBoard;
        }
        else
        {
          bitBoards[PieceTypeToBitBoardIndex(move.promotion)] |= targetBitBoard;
        }

        break;
      }
    }

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    var sourcePiece = new PiecePosition("0", sourcePieceType, move.source);
    var enemyPawnBitBoardIndex = sourcePieceType.IsWhite() ? BlackPawn : WhitePawn;

    // En passant
    if (sourcePieceType.IsPawn() && move.source.col != move.target.col && killedPiece == null)
    {
      var killedPiecePosition = new BoardPosition(move.target.col, move.source.row);
      killedPiece = new PiecePosition("0", sourcePieceType.IsWhite() ? PieceType.BlackPawn : PieceType.WhitePawn, killedPiecePosition);
      bitBoards[enemyPawnBitBoardIndex] ^= killedPiecePosition.index.toBitBoard();
    }

    var sourceOrTarget = sourceBitBoard | targetBitBoard;

    var lostCastleRights = CastleFlags.Nothing;
    if (sourcePieceType == PieceType.WhiteKing || (sourceOrTarget & whiteKingRookPosition) != 0) lostCastleRights |= CastleFlags.WhiteKing;
    if (sourcePieceType == PieceType.WhiteKing || (sourceOrTarget & whiteQueenRookPosition) != 0) lostCastleRights |= CastleFlags.WhiteQueen;
    if (sourcePieceType == PieceType.BlackKing || (sourceOrTarget & blackKingRookPosition) != 0) lostCastleRights |= CastleFlags.BlackKing;
    if (sourcePieceType == PieceType.BlackKing || (sourceOrTarget & blackQueenRookPosition) != 0) lostCastleRights |= CastleFlags.BlackQueen;

    this.castleFlags = this.castleFlags & ~lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      bitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= GetRookSwapBitBoard(move.target.index);
    }

    this.enPassantColumn = -1;

    // Only note enPassant if neighbour square has an enemy pawn to leverage legal transpositions
    if (sourcePiece.pieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2)
    {
      var targetNeighbourBitBoard = move.target.index.toBitBoard().shiftColumn(-1) | move.target.index.toBitBoard().shiftColumn(1);
      if ((bitBoards[enemyPawnBitBoardIndex] & targetNeighbourBitBoard) != 0)
      {
        this.enPassantColumn = move.source.col;
      }
    }

    this.hashable = null;
    this.whiteTurn = !this.whiteTurn;

    return new BoardStatePlay(
      sourcePiece,
      killedPiece
    );
  }

  public void UndoMove(ReversibleMove reversibleMove)
  {
    var targetBitBoard = reversibleMove.target.index.toBitBoard();
    var sourceBitBoard = reversibleMove.source.index.toBitBoard();
    var sourcePieceType = PieceType.Nothing;

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((targetBitBoard & bitBoards[bitBoardIndex]) != 0)
      {
        sourcePieceType = BitBoardIndexToPieceType[bitBoardIndex];
        bitBoards[bitBoardIndex] ^= targetBitBoard;

        if (reversibleMove.promotion == PieceType.Nothing)
        {
          bitBoards[bitBoardIndex] ^= sourceBitBoard;
        }
        else
        {
          bitBoards[sourcePieceType.IsWhite() ? WhitePawn : BlackPawn] ^= sourceBitBoard;
        }

        break;
      }
    }

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    if (reversibleMove.killed != null)
    {
      bitBoards[PieceTypeToBitBoardIndex(reversibleMove.killed.pieceType)] ^= reversibleMove.killed.position.index.toBitBoard();
    }

    this.castleFlags = this.castleFlags | reversibleMove.lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      bitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= GetRookSwapBitBoard(reversibleMove.target.index);
    }

    this.hashable = null;

    this.enPassantColumn = reversibleMove.oldEnPassantColumn;
    this.whiteTurn = !this.whiteTurn;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PieceType GetPieceTypeAtPosition(int boardPosition)
  {
    var bitBoard = boardPosition.toBitBoard();
    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((bitBoard & bitBoards[bitBoardIndex]) != 0)
      {
        return BitBoardIndexToPieceType[bitBoardIndex];
      }
    }

    return PieceType.Nothing;
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

  private readonly static ulong whiteKingRookPosition = BoardPosition.fromColRow(7, 0).toBitBoard();
  private readonly static ulong whiteQueenRookPosition = BoardPosition.fromColRow(0, 0).toBitBoard();
  private readonly static ulong blackKingRookPosition = BoardPosition.fromColRow(7, 7).toBitBoard();
  private readonly static ulong blackQueenRookPosition = BoardPosition.fromColRow(0, 7).toBitBoard();
  private readonly static int whiteKingCastleDestination = BoardPosition.fromColRow(6, 0);
  private readonly static int whiteQueenCastleDestination = BoardPosition.fromColRow(2, 0);
  private readonly static int blackKingCastleDestination = BoardPosition.fromColRow(6, 7);
  private readonly static ulong whiteKingCastleRookSwap = BoardPosition.fromColRow(7, 0).toBitBoard() | BoardPosition.fromColRow(5, 0).toBitBoard();
  private readonly static ulong whiteQueenCastleRookSwap = BoardPosition.fromColRow(0, 0).toBitBoard() | BoardPosition.fromColRow(3, 0).toBitBoard();
  private readonly static ulong blackKingCastleRookSwap = BoardPosition.fromColRow(7, 7).toBitBoard() | BoardPosition.fromColRow(5, 7).toBitBoard();
  private readonly static ulong blackQueenCastleRookSwap = BoardPosition.fromColRow(0, 7).toBitBoard() | BoardPosition.fromColRow(3, 7).toBitBoard();

  private static ulong GetRookSwapBitBoard(int kingTargetIndex)
  {
    if (kingTargetIndex == whiteKingCastleDestination)
    {
      return whiteKingCastleRookSwap;
    }
    else if (kingTargetIndex == whiteQueenCastleDestination)
    {
      return whiteQueenCastleRookSwap;
    }
    else if (kingTargetIndex == blackKingCastleDestination)
    {
      return blackKingCastleRookSwap;
    }
    else
    {
      return blackQueenCastleRookSwap;
    }
  }

  public static int WhitePawn = 0;
  public static int BlackPawn = 1;
  public static int WhiteRook = 2;
  public static int BlackRook = 3;
  public static int WhiteBishop = 4;
  public static int BlackBishop = 5;
  public static int WhiteKnight = 6;
  public static int BlackKnight = 7;
  public static int WhiteQueen = 8;
  public static int BlackQueen = 9;
  public static int WhiteKing = 10;
  public static int BlackKing = 11;

  public static PieceType[] BitBoardIndexToPieceType = new PieceType[] {
    PieceType.WhitePawn,
    PieceType.BlackPawn,
    PieceType.WhiteRook,
    PieceType.BlackRook,
    PieceType.WhiteBishop,
    PieceType.BlackBishop,
    PieceType.WhiteKnight,
    PieceType.BlackKnight,
    PieceType.WhiteQueen,
    PieceType.BlackQueen,
    PieceType.WhiteKing,
    PieceType.BlackKing,
  };

  public static int PieceTypeToBitBoardIndex(PieceType pieceType)
  {
    switch (pieceType)
    {
      case PieceType.WhitePawn:
        return WhitePawn;
      case PieceType.BlackPawn:
        return BlackPawn;
      case PieceType.WhiteRook:
        return WhiteRook;
      case PieceType.BlackRook:
        return BlackRook;
      case PieceType.WhiteBishop:
        return WhiteBishop;
      case PieceType.BlackBishop:
        return BlackBishop;
      case PieceType.WhiteKnight:
        return WhiteKnight;
      case PieceType.BlackKnight:
        return BlackKnight;
      case PieceType.WhiteQueen:
        return WhiteQueen;
      case PieceType.BlackQueen:
        return BlackQueen;
      case PieceType.WhiteKing:
        return WhiteKing;
      case PieceType.BlackKing:
        return BlackKing;
      default:
        throw new Exception("Invalid piece type");
    }
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

    public readonly ulong[] bitBoards;
    public CastleFlags castleFlags { get; }
    public int enPassantColumn { get; }

    public Hashable(
      V16BoardState boardState
    )
    {
      this.whiteTurn = boardState.whiteTurn;
      this.castleFlags = boardState.castleFlags;
      this.enPassantColumn = boardState.enPassantColumn;
      this.bitBoards = new ulong[12];
      Array.Copy(boardState.bitBoards, this.bitBoards, 12);
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
      if (castleFlags != other.castleFlags
      || enPassantColumn != other.enPassantColumn
      || whiteTurn != other.whiteTurn) return false;

      for (var index = 0; index < bitBoards.Length; ++index)
      {
        if (bitBoards[index] != other.bitBoards[index])
        {
          return false;
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = enPassantColumn + 2;
        hashCode = hashCode * 17 + (int)castleFlags + 1;
        hashCode = hashCode * 2 + (whiteTurn ? 1 : 0);

        for (var index = 0; index < bitBoards.Length; ++index)
        {
          hashCode ^= (int)bitBoards[index] + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
          hashCode ^= (int)(bitBoards[index] >> 32) + (int)0x9e3779b9 + (hashCode << 6) + (hashCode >> 2);
        }

        return hashCode;
      }
    }
  }
}
