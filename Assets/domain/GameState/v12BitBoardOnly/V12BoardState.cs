using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V12BoardState : BoardStateInterface
{
  public bool whiteTurn { get; }
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
  public CastleFlags castleFlags { get; }
  public int enPassantColumn { get; }

  public V12BoardState()
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

  public V12BoardState(BoardStateInterface other)
  {
    whiteTurn = other.whiteTurn;
    castleFlags = other.castleFlags;
    enPassantColumn = other.enPassantColumn;
    bitBoards = piecePositionsToBitBoards(other.piecePositions);
  }

  public V12BoardState(
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

  public V12BoardState(bool whiteTurn, List<PiecePosition> piecePositions, CastleFlags castleFlags)
  {
    this.whiteTurn = whiteTurn;
    this.castleFlags = castleFlags;
    enPassantColumn = -1;
    bitBoards = piecePositionsToBitBoards(piecePositions);
  }

  public class BoardStatePlay
  {
    public readonly V12BoardState boardState;
    public readonly PiecePosition sourcePiece;
    public readonly PiecePosition killedPiece;

    public BoardStatePlay(V12BoardState boardState, PiecePosition sourcePiece, PiecePosition killedPiece)
    {
      this.boardState = boardState;
      this.sourcePiece = sourcePiece;
      this.killedPiece = killedPiece;
    }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var newBitBoards = new ulong[12];
    Array.Copy(bitBoards, newBitBoards, 12);

    PiecePosition killedPiece = null;
    var targetBitBoard = move.target.index.toBitBoard();

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((targetBitBoard & newBitBoards[bitBoardIndex]) != 0)
      {
        killedPiece = new PiecePosition("0", BitBoardIndexToPieceType[bitBoardIndex], move.target.index.toBoardPosition());
        newBitBoards[bitBoardIndex] ^= targetBitBoard;
        break;
      }
    }

    PieceType sourcePieceType = PieceType.Nothing;
    var sourceBitBoard = move.source.index.toBitBoard();

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((sourceBitBoard & newBitBoards[bitBoardIndex]) != 0)
      {
        sourcePieceType = BitBoardIndexToPieceType[bitBoardIndex];
        newBitBoards[bitBoardIndex] ^= sourceBitBoard;

        if (move.promotion == PieceType.Nothing)
        {
          newBitBoards[bitBoardIndex] ^= targetBitBoard;
        }
        else
        {
          newBitBoards[PieceTypeToBitBoardIndex(move.promotion)] |= targetBitBoard;
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
      newBitBoards[enemyPawnBitBoardIndex] ^= killedPiecePosition.index.toBitBoard();
    }

    var whiteKingRookPosition = BoardPosition.fromColRow(7, 0).toBitBoard();
    var whiteQueenRookPosition = BoardPosition.fromColRow(0, 0).toBitBoard();
    var blackKingRookPosition = BoardPosition.fromColRow(7, 7).toBitBoard();
    var blackQueenRookPosition = BoardPosition.fromColRow(0, 7).toBitBoard();
    var sourceOrTarget = sourceBitBoard | targetBitBoard;

    var lostCastleRights = CastleFlags.Nothing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || (sourceOrTarget & whiteKingRookPosition) != 0) lostCastleRights |= CastleFlags.WhiteKing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || (sourceOrTarget & whiteQueenRookPosition) != 0) lostCastleRights |= CastleFlags.WhiteQueen;
    if (sourcePiece.pieceType == PieceType.BlackKing || (sourceOrTarget & blackKingRookPosition) != 0) lostCastleRights |= CastleFlags.BlackKing;
    if (sourcePiece.pieceType == PieceType.BlackKing || (sourceOrTarget & blackQueenRookPosition) != 0) lostCastleRights |= CastleFlags.BlackQueen;

    var castleFlags = this.castleFlags & ~lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      var rookSwapBitBoard = 0ul;
      if (move.target.index == BoardPosition.fromColRow(6, 0))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(7, 0).toBitBoard() | BoardPosition.fromColRow(5, 0).toBitBoard();
      }
      else if (move.target.index == BoardPosition.fromColRow(2, 0))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(0, 0).toBitBoard() | BoardPosition.fromColRow(3, 0).toBitBoard();
      }
      else if (move.target.index == BoardPosition.fromColRow(6, 7))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(7, 7).toBitBoard() | BoardPosition.fromColRow(5, 7).toBitBoard();
      }
      else if (move.target.index == BoardPosition.fromColRow(2, 7))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(0, 7).toBitBoard() | BoardPosition.fromColRow(3, 7).toBitBoard();
      }

      newBitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= rookSwapBitBoard;
    }

    var enPassantColumn = -1;

    // Only note enPassant if neighbour square has an enemy pawn to leverage legal transpositions
    if (sourcePiece.pieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2)
    {
      var targetNeighbourBitBoard = move.target.index.toBitBoard().shiftColumn(-1) | move.target.index.toBitBoard().shiftColumn(1);
      if ((newBitBoards[enemyPawnBitBoardIndex] & targetNeighbourBitBoard) != 0)
      {
        enPassantColumn = move.source.col;
      }
    }

    return new BoardStatePlay(
      new V12BoardState(
        !whiteTurn,
        newBitBoards,
        castleFlags,
        enPassantColumn
      ),
      sourcePiece,
      killedPiece
    );
  }

  public V12BoardState UndoMove(ReversibleMove reversibleMove)
  {
    var newBitBoards = new ulong[12];
    Array.Copy(bitBoards, newBitBoards, 12);

    var targetBitBoard = reversibleMove.target.index.toBitBoard();
    var sourceBitBoard = reversibleMove.source.index.toBitBoard();
    var sourcePieceType = PieceType.Nothing;

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      if ((targetBitBoard & newBitBoards[bitBoardIndex]) != 0)
      {
        sourcePieceType = BitBoardIndexToPieceType[bitBoardIndex];
        newBitBoards[bitBoardIndex] ^= targetBitBoard;

        if (reversibleMove.promotion == PieceType.Nothing)
        {
          newBitBoards[bitBoardIndex] ^= sourceBitBoard;
        }
        else
        {
          newBitBoards[sourcePieceType.IsWhite() ? WhitePawn : BlackPawn] ^= sourceBitBoard;
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
      newBitBoards[PieceTypeToBitBoardIndex(reversibleMove.killed.pieceType)] ^= reversibleMove.killed.position.index.toBitBoard();
    }

    var castleFlags = this.castleFlags | reversibleMove.lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      var rookSwapBitBoard = 0ul;

      if (reversibleMove.target.index == BoardPosition.fromColRow(6, 0))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(7, 0).toBitBoard() | BoardPosition.fromColRow(5, 0).toBitBoard();
      }
      else if (reversibleMove.target.index == BoardPosition.fromColRow(2, 0))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(0, 0).toBitBoard() | BoardPosition.fromColRow(3, 0).toBitBoard();
      }
      else if (reversibleMove.target.index == BoardPosition.fromColRow(6, 7))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(7, 7).toBitBoard() | BoardPosition.fromColRow(5, 7).toBitBoard();
      }
      else if (reversibleMove.target.index == BoardPosition.fromColRow(2, 7))
      {
        rookSwapBitBoard = BoardPosition.fromColRow(0, 7).toBitBoard() | BoardPosition.fromColRow(3, 7).toBitBoard();
      }

      newBitBoards[sourcePieceType.IsWhite() ? WhiteRook : BlackRook] ^= rookSwapBitBoard;
    }

    return new V12BoardState(
      !whiteTurn,
      newBitBoards,
      castleFlags,
      reversibleMove.oldEnPassantColumn
    );
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
    var other = (V12BoardState)obj;
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
        var bitHash = (bitBoards[index] + 1) * 0xADDEA54B659DBADBul;
        hashCode = hashCode * 0x513B6CD7 + (int)bitHash;
        hashCode = hashCode * 0x1DD41EA5 + (int)(bitHash >> 32);
      }

      return hashCode;
    }
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
}
