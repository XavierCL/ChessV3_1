
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V5BoardState : BoardStateInterface
{
  public bool WhiteTurn { get; }
  public List<PiecePosition> piecePositions { get; }
  private PieceType[] boardPieces { get; }
  public CastleFlags CastleFlags { get; }
  public int EnPassantColumn { get; }
  public BoardPosition whiteKingPosition { get; }
  public BoardPosition blackKingPosition { get; }

  public V5BoardState()
  {
    WhiteTurn = true;
    CastleFlags = CastleFlags.All;
    EnPassantColumn = -1;

    piecePositions = new List<PiecePosition> {
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
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position;
        }
        else
        {
          blackKingPosition = piecePosition.position;
        }
      }
    }
  }


  public V5BoardState(BoardStateInterface other)
  {
    WhiteTurn = other.WhiteTurn;
    piecePositions = new List<PiecePosition>(other.piecePositions);
    CastleFlags = other.CastleFlags;
    EnPassantColumn = other.EnPassantColumn;

    boardPieces = new PieceType[64];
    foreach (var piecePosition in piecePositions)
    {
      boardPieces[piecePosition.position.index] = piecePosition.pieceType;
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position;
        }
        else
        {
          blackKingPosition = piecePosition.position;
        }
      }
    }
  }

  public V5BoardState(bool whiteTurn, List<PiecePosition> piecePositions, PieceType[] boardPieces, CastleFlags castleFlags, int enPassantColumn, BoardPosition whiteKingPosition, BoardPosition blackKingPosition)
  {
    this.WhiteTurn = whiteTurn;
    this.piecePositions = piecePositions;
    this.boardPieces = boardPieces;
    this.CastleFlags = castleFlags;
    this.EnPassantColumn = enPassantColumn;
    this.whiteKingPosition = whiteKingPosition;
    this.blackKingPosition = blackKingPosition;
  }

  public V5BoardState(bool whiteTurn, List<PiecePosition> piecePositions, CastleFlags castleFlags)
  {
    this.WhiteTurn = whiteTurn;
    this.piecePositions = piecePositions;
    this.CastleFlags = castleFlags;
    EnPassantColumn = -1;

    boardPieces = new PieceType[64];
    foreach (var piecePosition in piecePositions)
    {
      boardPieces[piecePosition.position.index] = piecePosition.pieceType;
      if (piecePosition.pieceType.IsKing())
      {
        if (piecePosition.pieceType.IsWhite())
        {
          whiteKingPosition = piecePosition.position;
        }
        else
        {
          blackKingPosition = piecePosition.position;
        }
      }
    }
  }

  public class BoardStatePlay
  {
    public V5BoardState boardState { get; set; }
    public PiecePosition sourcePiece { get; set; }
    public PiecePosition killedPiece { get; set; }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var newPiecePositions = new List<PiecePosition>(piecePositions);
    var newBoardPieces = (PieceType[])boardPieces.Clone();
    PiecePosition killedPiece = null;

    if (boardPieces[move.target.index] != PieceType.Nothing)
    {
      var killedPieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(move.target));

      if (killedPieceIndex == -1)
      {
        throw new Exception("Board pieces recorded killed piece, but not piece positions");
      }

      killedPiece = newPiecePositions[killedPieceIndex];
      newPiecePositions.RemoveAt(killedPieceIndex);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
    }

    var sourcePieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(move.source));

    if (sourcePieceIndex == -1)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    var sourcePiece = newPiecePositions[sourcePieceIndex];

    newPiecePositions[sourcePieceIndex] = newPiecePositions[sourcePieceIndex].PlayMove(move.target, move.promotion);
    newBoardPieces[move.target.index] = move.promotion == PieceType.Nothing ? newBoardPieces[sourcePiece.position.index] : move.promotion;
    newBoardPieces[move.source.index] = PieceType.Nothing;

    // En passant
    if (sourcePiece.pieceType.IsPawn() && move.source.col != move.target.col && killedPiece == null)
    {
      var killedPieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(move.target.col, move.source.row)));

      if (killedPieceIndex == -1)
      {
        throw new System.Exception("Invalid move, en passant without enemy piece");
      }

      killedPiece = newPiecePositions[killedPieceIndex];
      newPiecePositions.RemoveAt(killedPieceIndex);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
    }

    var lostCastleRights = CastleFlags.Nothing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || sourcePiece.position.Equals(new BoardPosition(7, 0))) lostCastleRights |= CastleFlags.WhiteKing;
    if (sourcePiece.pieceType == PieceType.WhiteKing || sourcePiece.position.Equals(new BoardPosition(0, 0))) lostCastleRights |= CastleFlags.WhiteQueen;
    if (sourcePiece.pieceType == PieceType.BlackKing || sourcePiece.position.Equals(new BoardPosition(7, 7))) lostCastleRights |= CastleFlags.BlackKing;
    if (sourcePiece.pieceType == PieceType.BlackKing || sourcePiece.position.Equals(new BoardPosition(0, 7))) lostCastleRights |= CastleFlags.BlackQueen;

    var castleFlags = this.CastleFlags & ~lostCastleRights;

    // Castling
    if (sourcePiece.pieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      if (move.source.Equals(new BoardPosition(4, 0)) && move.target.Equals(new BoardPosition(6, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(7, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling white king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(5, 0), PieceType.Nothing);
        newBoardPieces[new BoardPosition(7, 0).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(5, 0).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (move.source.Equals(new BoardPosition(4, 0)) && move.target.Equals(new BoardPosition(2, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(0, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling white queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(3, 0), PieceType.Nothing);
        newBoardPieces[new BoardPosition(0, 0).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(3, 0).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(6, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(7, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling black king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(5, 7), PieceType.Nothing);
        newBoardPieces[new BoardPosition(7, 7).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(5, 7).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(2, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(0, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling black queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(3, 7), PieceType.Nothing);
        newBoardPieces[new BoardPosition(0, 7).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(3, 7).index] = newPiecePositions[rookIndex].pieceType;
      }
    }

    var enPassantColumn = sourcePiece.pieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2 ? move.source.col : -1;
    var whiteKingPosition = this.whiteKingPosition;
    var blackKingPosition = this.blackKingPosition;

    if (sourcePiece.pieceType.IsKing())
    {
      if (sourcePiece.pieceType.IsWhite())
      {
        whiteKingPosition = move.target;
      }
      else
      {
        blackKingPosition = move.target;
      }
    }

    return new BoardStatePlay()
    {
      boardState = new V5BoardState(
        !WhiteTurn,
        newPiecePositions,
        newBoardPieces,
        castleFlags,
        enPassantColumn,
        whiteKingPosition,
        blackKingPosition
      ),
      sourcePiece = sourcePiece,
      killedPiece = killedPiece
    };
  }

  public V5BoardState UndoMove(ReversibleMove reversibleMove)
  {
    var newPiecePositions = new List<PiecePosition>(piecePositions);
    var newBoardPieces = (PieceType[])boardPieces.Clone();
    var sourcePieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(reversibleMove.target));

    if (sourcePieceIndex == -1)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    var sourcePiece = newPiecePositions[sourcePieceIndex];
    var oldSourcePieceType = reversibleMove.promotion != 0 ? sourcePiece.pieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : sourcePiece.pieceType;
    newPiecePositions[sourcePieceIndex] = sourcePiece.PlayMove(reversibleMove.source, reversibleMove.promotion != 0 ? sourcePiece.pieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : PieceType.Nothing);
    newBoardPieces[reversibleMove.target.index] = PieceType.Nothing;
    newBoardPieces[reversibleMove.source.index] = oldSourcePieceType;

    if (reversibleMove.killed != null)
    {
      newPiecePositions.Add(reversibleMove.killed);
      newBoardPieces[reversibleMove.killed.position.index] = reversibleMove.killed.pieceType;
    }

    var castleFlags = this.CastleFlags | reversibleMove.lostCastleRights;

    // Castling
    if (sourcePiece.pieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      if (reversibleMove.source.Equals(new BoardPosition(4, 0)) && reversibleMove.target.Equals(new BoardPosition(6, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(5, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling white king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(7, 0), PieceType.Nothing);
        newBoardPieces[new BoardPosition(5, 0).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(7, 0).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 0)) && reversibleMove.target.Equals(new BoardPosition(2, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(3, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling white queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(0, 0), PieceType.Nothing);
        newBoardPieces[new BoardPosition(3, 0).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(0, 0).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(6, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(5, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling black king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(7, 7), PieceType.Nothing);
        newBoardPieces[new BoardPosition(5, 7).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(7, 7).index] = newPiecePositions[rookIndex].pieceType;
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(2, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(3, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling black queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(0, 7), PieceType.Nothing);
        newBoardPieces[new BoardPosition(3, 7).index] = PieceType.Nothing;
        newBoardPieces[new BoardPosition(0, 7).index] = newPiecePositions[rookIndex].pieceType;
      }
    }

    var whiteKingPosition = this.whiteKingPosition;
    var blackKingPosition = this.blackKingPosition;

    if (sourcePiece.pieceType.IsKing())
    {
      if (sourcePiece.pieceType.IsWhite())
      {
        whiteKingPosition = reversibleMove.source;
      }
      else
      {
        blackKingPosition = reversibleMove.source;
      }
    }

    return new V5BoardState(
      !WhiteTurn,
      newPiecePositions,
      newBoardPieces,
      castleFlags,
      reversibleMove.oldEnPassantColumn,
      whiteKingPosition,
      blackKingPosition
    );
  }

  public PieceType GetPieceTypeAtPosition(BoardPosition boardPosition)
  {
    return boardPieces[boardPosition.index];
  }

  public override bool Equals(object obj)
  {
    var other = (V5BoardState)obj;
    if (CastleFlags != other.CastleFlags
    || EnPassantColumn != other.EnPassantColumn
    || WhiteTurn != other.WhiteTurn) return false;

    if (!new HashSet<PiecePosition>(piecePositions).SetEquals(other.piecePositions)) return false;

    return true;
  }

  public override int GetHashCode()
  {
    unchecked
    {
      var hashCode = EnPassantColumn + 2;
      hashCode = hashCode * 17 + (int)CastleFlags + 1;
      hashCode = hashCode * 2 + (WhiteTurn ? 1 : 0);
      hashCode *= 0x1971987;
      hashCode = piecePositions.Select(piece => piece.GetHashCode()).Aggregate(hashCode, (cum, cur) => cum + cur);
      return hashCode;
    }
  }
}
