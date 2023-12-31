using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V11BoardState : BoardStateInterface
{
  public bool WhiteTurn { get; }
  private List<PiecePosition> _piecePositions;
  public List<PiecePosition> piecePositions
  {
    get
    {
      if (_piecePositions != null) return _piecePositions;

      _piecePositions = pieceIndices.Select(index => new PiecePosition("", boardPieces[index], index.toBoardPosition())).ToList();

      return _piecePositions;
    }
  }

  private int[] _pieceIndices;
  public int[] pieceIndices
  {
    get
    {
      if (_pieceIndices != null) return _pieceIndices;

      var bitCount = allBitBoard.bitCount();
      _pieceIndices = new int[bitCount];
      var currentBitBoard = allBitBoard;
      for (var index = 0; index < bitCount; ++index)
      {
        var nextPiece = currentBitBoard.lsb();
        currentBitBoard ^= 1ul << nextPiece;
        _pieceIndices[index] = nextPiece;
      }

      return _pieceIndices;
    }
  }

  public readonly ulong allBitBoard;
  public readonly PieceType[] boardPieces;
  public CastleFlags castleFlags { get; }
  public int enPassantColumn { get; }
  public readonly int whiteKingPosition;
  public readonly int blackKingPosition;

  public V11BoardState()
  {
    WhiteTurn = true;
    castleFlags = CastleFlags.All;
    enPassantColumn = -1;
    allBitBoard = 0;

    var piecePositions = new List<PiecePosition> {
      new PiecePosition("a1", PieceType.WhiteRook, new BoardPosition(0, 0)),
      new PiecePosition("b1", PieceType.WhiteKnight, new BoardPosition(1, 0)),
      new PiecePosition("c1", PieceType.WhiteBishop, new BoardPosition(2, 0)),
      new PiecePosition("d1", PieceType.WhiteQueen, new BoardPosition(3, 0)),
      new PiecePosition("e1", PieceType.WhiteKing, new BoardPosition(4, 0)),
      new PiecePosition("f1", PieceType.WhiteBishop, new BoardPosition(5, 0)),
      new PiecePosition("g1", PieceType.WhiteKnight, new BoardPosition(6, 0)),
      new PiecePosition("h1", PieceType.WhiteRook, new BoardPosition(7, 0)),
      new PiecePosition("a2", PieceType.WhitePawn, new BoardPosition(0, 1)),
      new PiecePosition("b2", PieceType.WhitePawn, new BoardPosition(1, 1)),
      new PiecePosition("c2", PieceType.WhitePawn, new BoardPosition(2, 1)),
      new PiecePosition("d2", PieceType.WhitePawn, new BoardPosition(3, 1)),
      new PiecePosition("e2", PieceType.WhitePawn, new BoardPosition(4, 1)),
      new PiecePosition("f2", PieceType.WhitePawn, new BoardPosition(5, 1)),
      new PiecePosition("g2", PieceType.WhitePawn, new BoardPosition(6, 1)),
      new PiecePosition("h2", PieceType.WhitePawn, new BoardPosition(7, 1)),
      new PiecePosition("a7", PieceType.BlackRook, new BoardPosition(0, 7)),
      new PiecePosition("b7", PieceType.BlackKnight, new BoardPosition(1, 7)),
      new PiecePosition("c7", PieceType.BlackBishop, new BoardPosition(2, 7)),
      new PiecePosition("d7", PieceType.BlackQueen, new BoardPosition(3, 7)),
      new PiecePosition("e7", PieceType.BlackKing, new BoardPosition(4, 7)),
      new PiecePosition("f7", PieceType.BlackBishop, new BoardPosition(5, 7)),
      new PiecePosition("g7", PieceType.BlackKnight, new BoardPosition(6, 7)),
      new PiecePosition("h7", PieceType.BlackRook, new BoardPosition(7, 7)),
      new PiecePosition("a6", PieceType.BlackPawn, new BoardPosition(0, 6)),
      new PiecePosition("b6", PieceType.BlackPawn, new BoardPosition(1, 6)),
      new PiecePosition("c6", PieceType.BlackPawn, new BoardPosition(2, 6)),
      new PiecePosition("d6", PieceType.BlackPawn, new BoardPosition(3, 6)),
      new PiecePosition("e6", PieceType.BlackPawn, new BoardPosition(4, 6)),
      new PiecePosition("f6", PieceType.BlackPawn, new BoardPosition(5, 6)),
      new PiecePosition("g6", PieceType.BlackPawn, new BoardPosition(6, 6)),
      new PiecePosition("h6", PieceType.BlackPawn, new BoardPosition(7, 6)),
    };

    boardPieces = new PieceType[64];
    foreach (var piecePosition in piecePositions)
    {
      boardPieces[piecePosition.position.index] = piecePosition.pieceType;
      allBitBoard ^= 1ul << piecePosition.position.index;
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position.index;
        }
        else
        {
          blackKingPosition = piecePosition.position.index;
        }
      }
    }
  }


  public V11BoardState(BoardStateInterface other)
  {
    WhiteTurn = other.WhiteTurn;
    castleFlags = other.castleFlags;
    enPassantColumn = other.enPassantColumn;
    allBitBoard = 0;

    boardPieces = new PieceType[64];
    foreach (var piecePosition in other.piecePositions)
    {
      boardPieces[piecePosition.position.index] = piecePosition.pieceType;
      allBitBoard ^= 1ul << piecePosition.position.index;
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position.index;
        }
        else
        {
          blackKingPosition = piecePosition.position.index;
        }
      }
    }
  }

  public V11BoardState(bool whiteTurn, PieceType[] boardPieces, ulong allBitBoard, CastleFlags castleFlags, int enPassantColumn, int whiteKingPosition, int blackKingPosition)
  {
    this.WhiteTurn = whiteTurn;
    this.boardPieces = boardPieces;
    this.allBitBoard = allBitBoard;
    this.castleFlags = castleFlags;
    this.enPassantColumn = enPassantColumn;
    this.whiteKingPosition = whiteKingPosition;
    this.blackKingPosition = blackKingPosition;
  }

  public V11BoardState(bool whiteTurn, List<PiecePosition> piecePositions, CastleFlags castleFlags)
  {
    this.WhiteTurn = whiteTurn;
    this.castleFlags = castleFlags;
    enPassantColumn = -1;
    allBitBoard = 0;

    boardPieces = new PieceType[64];
    foreach (var piecePosition in piecePositions)
    {
      boardPieces[piecePosition.position.index] = piecePosition.pieceType;
      allBitBoard ^= 1ul << piecePosition.position.index;
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position.index;
        }
        else
        {
          blackKingPosition = piecePosition.position.index;
        }
      }
    }
  }

  public class BoardStatePlay
  {
    public readonly V11BoardState boardState;
    public readonly PiecePosition sourcePiece;
    public readonly PiecePosition killedPiece;

    public BoardStatePlay(V11BoardState boardState, PiecePosition sourcePiece, PiecePosition killedPiece)
    {
      this.boardState = boardState;
      this.sourcePiece = sourcePiece;
      this.killedPiece = killedPiece;
    }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var newBoardPieces = new PieceType[64];
    var newAllBitBoard = allBitBoard;
    Array.Copy(boardPieces, newBoardPieces, 64);
    PiecePosition killedPiece = null;

    var killedPieceType = boardPieces[move.target.index];
    if (killedPieceType != PieceType.Nothing)
    {
      killedPiece = new PiecePosition("0", killedPieceType, move.target);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
      newAllBitBoard ^= 1ul << killedPiece.position.index;
    }

    var sourcePieceType = newBoardPieces[move.source.index];

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    var sourcePiece = new PiecePosition("0", sourcePieceType, move.source);

    newBoardPieces[move.target.index] = move.promotion == PieceType.Nothing ? newBoardPieces[sourcePiece.position.index] : move.promotion;
    newBoardPieces[move.source.index] = PieceType.Nothing;
    newAllBitBoard ^= 1ul << move.target.index;
    newAllBitBoard ^= 1ul << move.source.index;

    // En passant
    if (sourcePieceType.IsPawn() && move.source.col != move.target.col && killedPiece == null)
    {
      var killedPiecePosition = new BoardPosition(move.target.col, move.source.row);
      killedPiece = new PiecePosition("0", boardPieces[killedPiecePosition.index], killedPiecePosition);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
      newAllBitBoard ^= 1ul << killedPiece.position.index;
    }

    var whiteKingRookPosition = BoardPosition.fromColRow(7, 0);
    var whiteQueenRookPosition = BoardPosition.fromColRow(0, 0);
    var blackKingRookPosition = BoardPosition.fromColRow(7, 7);
    var blackQueenRookPosition = BoardPosition.fromColRow(0, 7);

    var lostCastleRights = CastleFlags.Nothing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || move.source.index == whiteKingRookPosition || move.target.index == whiteKingRookPosition) lostCastleRights |= CastleFlags.WhiteKing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || move.source.index == whiteQueenRookPosition || move.target.index == whiteQueenRookPosition) lostCastleRights |= CastleFlags.WhiteQueen;
    if (sourcePiece.pieceType == PieceType.BlackKing || move.source.index == blackKingRookPosition || move.target.index == blackKingRookPosition) lostCastleRights |= CastleFlags.BlackKing;
    if (sourcePiece.pieceType == PieceType.BlackKing || move.source.index == blackQueenRookPosition || move.target.index == blackQueenRookPosition) lostCastleRights |= CastleFlags.BlackQueen;

    var castleFlags = this.castleFlags & ~lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      var oldRookPosition = 0;
      var newRookPosition = 0;
      if (move.source.index == BoardPosition.fromColRow(4, 0) && move.target.index == BoardPosition.fromColRow(6, 0))
      {
        oldRookPosition = BoardPosition.fromColRow(7, 0);
        newRookPosition = BoardPosition.fromColRow(5, 0);
      }
      else if (move.source.index == BoardPosition.fromColRow(4, 0) && move.target.index == BoardPosition.fromColRow(2, 0))
      {
        oldRookPosition = BoardPosition.fromColRow(0, 0);
        newRookPosition = BoardPosition.fromColRow(3, 0);
      }
      else if (move.source.index == BoardPosition.fromColRow(4, 7) && move.target.index == BoardPosition.fromColRow(6, 7))
      {
        oldRookPosition = BoardPosition.fromColRow(7, 7);
        newRookPosition = BoardPosition.fromColRow(5, 7);
      }
      else if (move.source.index == BoardPosition.fromColRow(4, 7) && move.target.index == BoardPosition.fromColRow(2, 7))
      {
        oldRookPosition = BoardPosition.fromColRow(0, 7);
        newRookPosition = BoardPosition.fromColRow(3, 7);
      }

      newBoardPieces[newRookPosition] = newBoardPieces[oldRookPosition];
      newBoardPieces[oldRookPosition] = PieceType.Nothing;
      newAllBitBoard ^= 1ul << newRookPosition;
      newAllBitBoard ^= 1ul << oldRookPosition;
    }

    var enPassantColumn = -1;

    // Only note the enPassant if neighbour square has an enemy pawn to leverage legal transpositions
    if (sourcePiece.pieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2)
    {
      if (BoardPosition.IsInBoard(move.target.col - 1, move.target.row))
      {
        var neighbourPiece = boardPieces[BoardPosition.fromColRow(move.target.col - 1, move.target.row)];
        if (neighbourPiece.IsPawn() && neighbourPiece.IsWhite() != sourcePieceType.IsWhite())
        {
          enPassantColumn = move.source.col;
        }
      }

      if (enPassantColumn == -1 && BoardPosition.IsInBoard(move.target.col + 1, move.target.row))
      {
        var neighbourPiece = boardPieces[BoardPosition.fromColRow(move.target.col + 1, move.target.row)];
        if (neighbourPiece.IsPawn() && neighbourPiece.IsWhite() != sourcePieceType.IsWhite())
        {
          enPassantColumn = move.source.col;
        }
      }
    }

    var whiteKingPosition = this.whiteKingPosition;
    var blackKingPosition = this.blackKingPosition;

    if (sourcePiece.pieceType.IsKing())
    {
      if (sourcePiece.pieceType.IsWhite())
      {
        whiteKingPosition = move.target.index;
      }
      else
      {
        blackKingPosition = move.target.index;
      }
    }

    return new BoardStatePlay(
      new V11BoardState(
        !WhiteTurn,
        newBoardPieces,
        newAllBitBoard,
        castleFlags,
        enPassantColumn,
        whiteKingPosition,
        blackKingPosition
      ),
      sourcePiece,
      killedPiece
    );
  }

  public V11BoardState UndoMove(ReversibleMove reversibleMove)
  {
    var newBoardPieces = (PieceType[])boardPieces.Clone();
    var newAllBitBoard = allBitBoard;
    var sourcePieceType = newBoardPieces[reversibleMove.target.index];

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    var oldSourcePieceType = reversibleMove.promotion != 0 ? sourcePieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : sourcePieceType;
    newBoardPieces[reversibleMove.target.index] = PieceType.Nothing;
    newBoardPieces[reversibleMove.source.index] = oldSourcePieceType;
    newAllBitBoard ^= 1ul << reversibleMove.target.index;
    newAllBitBoard ^= 1ul << reversibleMove.source.index;

    if (reversibleMove.killed != null)
    {
      newBoardPieces[reversibleMove.killed.position.index] = reversibleMove.killed.pieceType;
      newAllBitBoard ^= 1ul << reversibleMove.killed.position.index;
    }

    var castleFlags = this.castleFlags | reversibleMove.lostCastleRights;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      var newRookPosition = 0;
      var oldRookPosition = 0;

      if (reversibleMove.source.index == BoardPosition.fromColRow(4, 0) && reversibleMove.target.index == BoardPosition.fromColRow(6, 0))
      {
        oldRookPosition = BoardPosition.fromColRow(7, 0);
        newRookPosition = BoardPosition.fromColRow(5, 0);
      }
      else if (reversibleMove.source.index == BoardPosition.fromColRow(4, 0) && reversibleMove.target.index == BoardPosition.fromColRow(2, 0))
      {
        oldRookPosition = BoardPosition.fromColRow(0, 0);
        newRookPosition = BoardPosition.fromColRow(3, 0);
      }
      else if (reversibleMove.source.index == BoardPosition.fromColRow(4, 7) && reversibleMove.target.index == BoardPosition.fromColRow(6, 7))
      {
        oldRookPosition = BoardPosition.fromColRow(7, 7);
        newRookPosition = BoardPosition.fromColRow(5, 7);
      }
      else if (reversibleMove.source.index == BoardPosition.fromColRow(4, 7) && reversibleMove.target.index == BoardPosition.fromColRow(2, 7))
      {
        oldRookPosition = BoardPosition.fromColRow(0, 7);
        newRookPosition = BoardPosition.fromColRow(3, 7);
      }

      newBoardPieces[oldRookPosition] = newBoardPieces[newRookPosition];
      newBoardPieces[newRookPosition] = PieceType.Nothing;
      newAllBitBoard ^= 1ul << oldRookPosition;
      newAllBitBoard ^= 1ul << newRookPosition;
    }

    var whiteKingPosition = this.whiteKingPosition;
    var blackKingPosition = this.blackKingPosition;

    if (sourcePieceType.IsKing())
    {
      if (sourcePieceType.IsWhite())
      {
        whiteKingPosition = reversibleMove.source.index;
      }
      else
      {
        blackKingPosition = reversibleMove.source.index;
      }
    }

    return new V11BoardState(
      !WhiteTurn,
      newBoardPieces,
      newAllBitBoard,
      castleFlags,
      reversibleMove.oldEnPassantColumn,
      whiteKingPosition,
      blackKingPosition
    );
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public PieceType GetPieceTypeAtPosition(int boardPosition)
  {
    return boardPieces[boardPosition];
  }

  public override bool Equals(object obj)
  {
    var other = (V11BoardState)obj;
    if (castleFlags != other.castleFlags
    || enPassantColumn != other.enPassantColumn
    || allBitBoard != other.allBitBoard
    || WhiteTurn != other.WhiteTurn) return false;

    for (var index = 0; index < pieceIndices.Length; ++index)
    {
      if (boardPieces[pieceIndices[index]] != other.boardPieces[pieceIndices[index]])
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
      hashCode = hashCode * 2 + (WhiteTurn ? 1 : 0);

      for (var index = 0; index < pieceIndices.Length; ++index)
      {
        hashCode = hashCode * 0x1971987 + pieceIndices[index] * 257 + (int)(boardPieces[pieceIndices[index]] + 1);
      }

      return hashCode;
    }
  }
}
