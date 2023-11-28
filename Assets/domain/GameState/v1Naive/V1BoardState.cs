
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V1BoardState : BoardStateInterface
{
  public List<PiecePosition> piecePositions { get; }
  public bool whiteCastleKingSide { get; }
  public bool whiteCastleQueenSide { get; }
  public bool blackCastleKingSide { get; }
  public bool blackCastleQueenSide { get; }
  public int enPassantColumn { get; }

  public V1BoardState()
  {
    whiteCastleKingSide = true;
    whiteCastleQueenSide = true;
    blackCastleKingSide = true;
    blackCastleQueenSide = true;
    enPassantColumn = -1;

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
  }


  public V1BoardState(BoardStateInterface other)
  {
    piecePositions = new List<PiecePosition>(other.piecePositions);
    whiteCastleKingSide = other.whiteCastleKingSide;
    whiteCastleQueenSide = other.whiteCastleQueenSide;
    blackCastleKingSide = other.blackCastleKingSide;
    blackCastleQueenSide = other.blackCastleQueenSide;
    enPassantColumn = other.enPassantColumn;
  }

  public V1BoardState(List<PiecePosition> piecePositions, bool whiteCastleKingSide, bool whiteCastleQueenSide, bool blackCastleKingSide, bool blackCastleQueenSide, int enPassantColumn)
  {
    this.piecePositions = piecePositions;
    this.whiteCastleKingSide = whiteCastleKingSide;
    this.whiteCastleQueenSide = whiteCastleQueenSide;
    this.blackCastleKingSide = blackCastleKingSide;
    this.blackCastleQueenSide = blackCastleQueenSide;
    this.enPassantColumn = enPassantColumn;
  }

  public class BoardStatePlay
  {
    public V1BoardState boardState { get; set; }
    public PiecePosition sourcePiece { get; set; }
    public PiecePosition killedPiece { get; set; }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var newPiecePositions = new List<PiecePosition>(piecePositions);
    var killedPieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(move.target));
    PiecePosition killedPiece = null;

    if (killedPieceIndex != -1)
    {
      killedPiece = newPiecePositions[killedPieceIndex];
      newPiecePositions.RemoveAt(killedPieceIndex);
    }

    var sourcePieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(move.source));

    if (sourcePieceIndex == -1)
    {
      throw new System.Exception("Invalid move, no piece at source position");
    }

    var sourcePiece = newPiecePositions[sourcePieceIndex];

    newPiecePositions[sourcePieceIndex] = newPiecePositions[sourcePieceIndex].PlayMove(move.target, move.promotion);

    // En passant
    if (sourcePiece.pieceType.IsPawn() && move.source.col != move.target.col && killedPieceIndex == -1)
    {
      killedPieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(move.target.col, move.source.row)));

      if (killedPieceIndex == -1)
      {
        throw new System.Exception("Invalid move, en passant without enemy piece");
      }

      killedPiece = newPiecePositions[killedPieceIndex];
      newPiecePositions.RemoveAt(killedPieceIndex);
    }

    var lostWhiteKingCastleRight = this.whiteCastleKingSide && (sourcePiece.pieceType == PieceType.WhiteKing || sourcePiece.position.Equals(new BoardPosition(7, 0)));
    var lostWhiteQueenCastleRight = this.whiteCastleQueenSide && (sourcePiece.pieceType == PieceType.WhiteKing || sourcePiece.position.Equals(new BoardPosition(0, 0)));
    var lostBlackKingCastleRight = this.blackCastleKingSide && (sourcePiece.pieceType == PieceType.BlackKing || sourcePiece.position.Equals(new BoardPosition(7, 7)));
    var lostBlackQueenCastleRight = this.blackCastleQueenSide && (sourcePiece.pieceType == PieceType.BlackKing || sourcePiece.position.Equals(new BoardPosition(0, 7)));

    var whiteCastleKingSide = this.whiteCastleKingSide && !lostWhiteKingCastleRight;
    var whiteCastleQueenSide = this.whiteCastleQueenSide && !lostWhiteQueenCastleRight;
    var blackCastleKingSide = this.blackCastleKingSide && !lostBlackKingCastleRight;
    var blackCastleQueenSide = this.blackCastleQueenSide && !lostBlackQueenCastleRight;

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
      }
      else if (move.source.Equals(new BoardPosition(4, 0)) && move.target.Equals(new BoardPosition(2, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(0, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling white queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(3, 0), PieceType.Nothing);
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(6, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(7, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling black king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(5, 7), PieceType.Nothing);
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(2, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(0, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Could not find rook while castling black queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(3, 7), PieceType.Nothing);
      }
    }

    var enPassantColumn = sourcePiece.pieceType.IsPawn() && Math.Abs(move.source.row - move.target.row) == 2 ? move.source.col : -1;

    return new BoardStatePlay()
    {
      boardState = new V1BoardState(newPiecePositions, whiteCastleKingSide, whiteCastleQueenSide, blackCastleKingSide, blackCastleQueenSide, enPassantColumn),
      sourcePiece = sourcePiece,
      killedPiece = killedPiece
    };
  }

  public V1BoardState UndoMove(ReversibleMove reversibleMove)
  {
    var newPiecePositions = new List<PiecePosition>(piecePositions);
    var sourcePieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(reversibleMove.target));

    if (sourcePieceIndex == -1)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    var sourcePiece = newPiecePositions[sourcePieceIndex];
    newPiecePositions[sourcePieceIndex] = sourcePiece.PlayMove(reversibleMove.source, reversibleMove.pawnPromoted ? sourcePiece.pieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : PieceType.Nothing);

    if (reversibleMove.killed != null)
    {
      newPiecePositions.Add(reversibleMove.killed);
    }

    var whiteCastleKingSide = this.whiteCastleKingSide;
    var whiteCastleQueenSide = this.whiteCastleQueenSide;
    var blackCastleKingSide = this.blackCastleKingSide;
    var blackCastleQueenSide = this.blackCastleQueenSide;

    if (reversibleMove.lostWhiteKingCastleRight) whiteCastleKingSide = true;
    if (reversibleMove.lostWhiteQueenCastleRight) whiteCastleQueenSide = true;
    if (reversibleMove.lostBlackKingCastleRight) blackCastleKingSide = true;
    if (reversibleMove.lostBlackQueenCastleRight) blackCastleQueenSide = true;

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
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 0)) && reversibleMove.target.Equals(new BoardPosition(2, 0)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(3, 0)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling white queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(0, 0), PieceType.Nothing);
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(6, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(5, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling black king");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(7, 7), PieceType.Nothing);
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(2, 7)))
      {
        var rookIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(3, 7)));

        if (rookIndex == -1)
        {
          throw new Exception("Invalid undo, Could not find rook while castling black queen");
        }

        newPiecePositions[rookIndex] = newPiecePositions[rookIndex].PlayMove(new BoardPosition(0, 7), PieceType.Nothing);
      }
    }

    return new V1BoardState(newPiecePositions, whiteCastleKingSide, whiteCastleQueenSide, blackCastleKingSide, blackCastleQueenSide, reversibleMove.oldPassantColumn);
  }

  public bool HasKing(bool white)
  {
    return piecePositions.Any(piecePosition => white ? piecePosition.pieceType == PieceType.WhiteKing : piecePosition.pieceType == PieceType.BlackKing);
  }

  public override bool Equals(object obj)
  {
    var other = (V1BoardState)obj;
    if (whiteCastleKingSide != other.whiteCastleKingSide
    || whiteCastleQueenSide != other.whiteCastleQueenSide
    || blackCastleKingSide != other.blackCastleKingSide
    || blackCastleQueenSide != other.blackCastleQueenSide
    || enPassantColumn != other.enPassantColumn) return false;

    if (!new HashSet<PiecePosition>(piecePositions).SetEquals(other.piecePositions)) return false;

    return true;
  }

  public override int GetHashCode()
  {
    unchecked
    {
      var hashCode = enPassantColumn + 2;
      hashCode = hashCode * 2 + (whiteCastleKingSide ? 1 : 0);
      hashCode = hashCode * 2 + (whiteCastleQueenSide ? 1 : 0);
      hashCode = hashCode * 2 + (blackCastleKingSide ? 1 : 0);
      hashCode = hashCode * 2 + (blackCastleQueenSide ? 1 : 0);
      hashCode *= 0x1971987;
      hashCode = piecePositions.Select(piece => piece.GetHashCode()).Aggregate(hashCode, (cum, cur) => cum + cur);
      return hashCode;
    }
  }
}
