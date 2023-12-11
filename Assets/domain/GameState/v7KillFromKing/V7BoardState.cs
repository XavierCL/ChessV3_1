
using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{piecePositions.Count} pieces")]
public class V7BoardState : BoardStateInterface
{
  public bool whiteTurn { get; }
  private List<PiecePosition> _piecePositions;
  public List<PiecePosition> piecePositions
  {
    get
    {
      if (_piecePositions != null) return _piecePositions;

      _piecePositions = new List<PiecePosition>(32);

      for (var index = 0; index < boardPieces.Length; ++index)
      {
        var pieceType = boardPieces[index];
        if (pieceType != PieceType.Nothing)
        {
          _piecePositions.Add(new PiecePosition("", pieceType, new BoardPosition(index)));
        }
      }

      return _piecePositions;
    }
  }

  public readonly PieceType[] boardPieces;
  public bool whiteCastleKingSide { get; }
  public bool whiteCastleQueenSide { get; }
  public bool blackCastleKingSide { get; }
  public bool blackCastleQueenSide { get; }
  public int enPassantColumn { get; }
  public BoardPosition whiteKingPosition { get; }
  public BoardPosition blackKingPosition { get; }

  public V7BoardState()
  {
    whiteTurn = true;
    whiteCastleKingSide = true;
    whiteCastleQueenSide = true;
    blackCastleKingSide = true;
    blackCastleQueenSide = true;
    enPassantColumn = -1;

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


  public V7BoardState(BoardStateInterface other)
  {
    whiteTurn = other.whiteTurn;
    whiteCastleKingSide = other.whiteCastleKingSide;
    whiteCastleQueenSide = other.whiteCastleQueenSide;
    blackCastleKingSide = other.blackCastleKingSide;
    blackCastleQueenSide = other.blackCastleQueenSide;
    enPassantColumn = other.enPassantColumn;

    boardPieces = new PieceType[64];
    foreach (var piecePosition in other.piecePositions)
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

  public V7BoardState(bool whiteTurn, PieceType[] boardPieces, bool whiteCastleKingSide, bool whiteCastleQueenSide, bool blackCastleKingSide, bool blackCastleQueenSide, int enPassantColumn, BoardPosition whiteKingPosition, BoardPosition blackKingPosition)
  {
    this.whiteTurn = whiteTurn;
    this.boardPieces = boardPieces;
    this.whiteCastleKingSide = whiteCastleKingSide;
    this.whiteCastleQueenSide = whiteCastleQueenSide;
    this.blackCastleKingSide = blackCastleKingSide;
    this.blackCastleQueenSide = blackCastleQueenSide;
    this.enPassantColumn = enPassantColumn;
    this.whiteKingPosition = whiteKingPosition;
    this.blackKingPosition = blackKingPosition;
  }

  public V7BoardState(bool whiteTurn, List<PiecePosition> piecePositions, bool whiteCastleKingSide, bool whiteCastleQueenSide, bool blackCastleKingSide, bool blackCastleQueenSide)
  {
    this.whiteTurn = whiteTurn;
    this.whiteCastleKingSide = whiteCastleKingSide;
    this.whiteCastleQueenSide = whiteCastleQueenSide;
    this.blackCastleKingSide = blackCastleKingSide;
    this.blackCastleQueenSide = blackCastleQueenSide;
    enPassantColumn = -1;

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
    public V7BoardState boardState { get; set; }
    public PiecePosition sourcePiece { get; set; }
    public PiecePosition killedPiece { get; set; }
  }

  public BoardStatePlay PlayMove(Move move)
  {
    var newBoardPieces = (PieceType[])boardPieces.Clone();
    PiecePosition killedPiece = null;

    var killedPieceType = boardPieces[move.target.index];
    if (killedPieceType != PieceType.Nothing)
    {
      killedPiece = new PiecePosition("0", killedPieceType, move.target);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
    }

    var sourcePieceType = newBoardPieces[move.source.index];

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new Exception("Invalid move, no piece at source position");
    }

    var sourcePiece = new PiecePosition("0", sourcePieceType, move.source);

    newBoardPieces[move.target.index] = move.promotion == PieceType.Nothing ? newBoardPieces[sourcePiece.position.index] : move.promotion;
    newBoardPieces[move.source.index] = PieceType.Nothing;

    // En passant
    if (sourcePieceType.IsPawn() && move.source.col != move.target.col && killedPiece == null)
    {
      var killedPiecePosition = new BoardPosition(move.target.col, move.source.row);
      killedPiece = new PiecePosition("0", boardPieces[killedPiecePosition.index], killedPiecePosition);
      newBoardPieces[killedPiece.position.index] = PieceType.Nothing;
    }

    var lostWhiteKingCastleRight = this.whiteCastleKingSide && (sourcePiece.pieceType == PieceType.WhiteKing || move.source.Equals(new BoardPosition(7, 0)) || move.target.Equals(new BoardPosition(7, 0)));
    var lostWhiteQueenCastleRight = this.whiteCastleQueenSide && (sourcePiece.pieceType == PieceType.WhiteKing || move.source.Equals(new BoardPosition(0, 0)) || move.target.Equals(new BoardPosition(0, 0)));
    var lostBlackKingCastleRight = this.blackCastleKingSide && (sourcePiece.pieceType == PieceType.BlackKing || move.source.Equals(new BoardPosition(7, 7)) || move.target.Equals(new BoardPosition(7, 7)));
    var lostBlackQueenCastleRight = this.blackCastleQueenSide && (sourcePiece.pieceType == PieceType.BlackKing || move.source.Equals(new BoardPosition(0, 7)) || move.target.Equals(new BoardPosition(0, 7)));

    var whiteCastleKingSide = this.whiteCastleKingSide && !lostWhiteKingCastleRight;
    var whiteCastleQueenSide = this.whiteCastleQueenSide && !lostWhiteQueenCastleRight;
    var blackCastleKingSide = this.blackCastleKingSide && !lostBlackKingCastleRight;
    var blackCastleQueenSide = this.blackCastleQueenSide && !lostBlackQueenCastleRight;

    // Castling
    if (sourcePieceType.IsKing() && Math.Abs(move.target.col - move.source.col) == 2)
    {
      var oldRookPosition = new BoardPosition(0, 0);
      var newRookPosition = new BoardPosition(0, 0);
      if (move.source.Equals(new BoardPosition(4, 0)) && move.target.Equals(new BoardPosition(6, 0)))
      {
        oldRookPosition = new BoardPosition(7, 0);
        newRookPosition = new BoardPosition(5, 0);
      }
      else if (move.source.Equals(new BoardPosition(4, 0)) && move.target.Equals(new BoardPosition(2, 0)))
      {
        oldRookPosition = new BoardPosition(0, 0);
        newRookPosition = new BoardPosition(3, 0);
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(6, 7)))
      {
        oldRookPosition = new BoardPosition(7, 7);
        newRookPosition = new BoardPosition(5, 7);
      }
      else if (move.source.Equals(new BoardPosition(4, 7)) && move.target.Equals(new BoardPosition(2, 7)))
      {
        oldRookPosition = new BoardPosition(0, 7);
        newRookPosition = new BoardPosition(3, 7);
      }

      newBoardPieces[newRookPosition.index] = newBoardPieces[oldRookPosition.index];
      newBoardPieces[oldRookPosition.index] = PieceType.Nothing;
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
      boardState = new V7BoardState(
        !whiteTurn,
        newBoardPieces,
        whiteCastleKingSide,
        whiteCastleQueenSide,
        blackCastleKingSide,
        blackCastleQueenSide,
        enPassantColumn,
        whiteKingPosition,
        blackKingPosition
      ),
      sourcePiece = sourcePiece,
      killedPiece = killedPiece
    };
  }

  public V7BoardState UndoMove(ReversibleMove reversibleMove)
  {
    var newBoardPieces = (PieceType[])boardPieces.Clone();
    var sourcePieceType = newBoardPieces[reversibleMove.target.index];

    if (sourcePieceType == PieceType.Nothing)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    var oldSourcePieceType = reversibleMove.pawnPromoted ? sourcePieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : sourcePieceType;
    newBoardPieces[reversibleMove.target.index] = PieceType.Nothing;
    newBoardPieces[reversibleMove.source.index] = oldSourcePieceType;

    if (reversibleMove.killed != null)
    {
      newBoardPieces[reversibleMove.killed.position.index] = reversibleMove.killed.pieceType;
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
    if (sourcePieceType.IsKing() && Math.Abs(reversibleMove.target.col - reversibleMove.source.col) == 2)
    {
      var newRookPosition = new BoardPosition(0, 0);
      var oldRookPosition = new BoardPosition(0, 0);

      if (reversibleMove.source.Equals(new BoardPosition(4, 0)) && reversibleMove.target.Equals(new BoardPosition(6, 0)))
      {
        oldRookPosition = new BoardPosition(7, 0);
        newRookPosition = new BoardPosition(5, 0);
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 0)) && reversibleMove.target.Equals(new BoardPosition(2, 0)))
      {
        oldRookPosition = new BoardPosition(0, 0);
        newRookPosition = new BoardPosition(3, 0);
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(6, 7)))
      {
        oldRookPosition = new BoardPosition(7, 7);
        newRookPosition = new BoardPosition(5, 7);
      }
      else if (reversibleMove.source.Equals(new BoardPosition(4, 7)) && reversibleMove.target.Equals(new BoardPosition(2, 7)))
      {
        oldRookPosition = new BoardPosition(0, 7);
        newRookPosition = new BoardPosition(3, 7);
      }

      newBoardPieces[oldRookPosition.index] = newBoardPieces[newRookPosition.index];
      newBoardPieces[newRookPosition.index] = PieceType.Nothing;
    }

    var whiteKingPosition = this.whiteKingPosition;
    var blackKingPosition = this.blackKingPosition;

    if (sourcePieceType.IsKing())
    {
      if (sourcePieceType.IsWhite())
      {
        whiteKingPosition = reversibleMove.source;
      }
      else
      {
        blackKingPosition = reversibleMove.source;
      }
    }

    return new V7BoardState(
      !whiteTurn,
      newBoardPieces,
      whiteCastleKingSide,
      whiteCastleQueenSide,
      blackCastleKingSide,
      blackCastleQueenSide,
      reversibleMove.oldPassantColumn,
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
    var other = (V7BoardState)obj;
    if (whiteCastleKingSide != other.whiteCastleKingSide
    || whiteCastleQueenSide != other.whiteCastleQueenSide
    || blackCastleKingSide != other.blackCastleKingSide
    || blackCastleQueenSide != other.blackCastleQueenSide
    || enPassantColumn != other.enPassantColumn
    || whiteTurn != other.whiteTurn) return false;

    for (var index = 0; index < boardPieces.Length; ++index)
    {
      if (boardPieces[index] != other.boardPieces[index]) return false;
    }

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
      hashCode = hashCode * 2 + (whiteTurn ? 1 : 0);

      for (var index = 0; index < boardPieces.Length; ++index)
      {
        hashCode = hashCode * 0x1971987 + (int)(boardPieces[index] + 1);
      }

      return hashCode;
    }
  }
}
