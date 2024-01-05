using System;
using System.Collections.Generic;
using System.Linq;

public class PieceIdBoardStateAdapter : BoardStateInterface
{
  private GameStateInterface underlying;
  public List<PiecePosition> piecePositions { get; }

  public PieceIdBoardStateAdapter(GameStateInterface underlying, List<PiecePosition> piecePositions)
  {
    this.underlying = underlying;
    this.piecePositions = piecePositions;
  }

  public static PieceIdBoardStateAdapter FromStartingPosition(GameStateInterface underlying)
  {
    return new PieceIdBoardStateAdapter(underlying, new List<PiecePosition> {
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
    });
  }

  public static PieceIdBoardStateAdapter FromMiddlegame(GameStateInterface underlying)
  {
    return new PieceIdBoardStateAdapter(
      underlying,
      underlying
        .BoardState
        .piecePositions
        .Select((piecePosition, index) => new PiecePosition(
          pieceIds[index],
          piecePosition.pieceType,
          piecePosition.position
        ))
        .ToList()
    );
  }

  public bool WhiteTurn => underlying.BoardState.WhiteTurn;

  public CastleFlags castleFlags => underlying.BoardState.castleFlags;

  public int enPassantColumn => underlying.BoardState.enPassantColumn;

  public class BoardStatePlay
  {
    public PieceIdBoardStateAdapter boardState { get; set; }
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

    return new BoardStatePlay()
    {
      boardState = new PieceIdBoardStateAdapter(underlying, newPiecePositions),
      sourcePiece = sourcePiece,
      killedPiece = killedPiece
    };
  }

  public PieceIdBoardStateAdapter UndoMove(ReversibleMove reversibleMove)
  {
    var newPiecePositions = new List<PiecePosition>(piecePositions);
    var sourcePieceIndex = newPiecePositions.FindIndex(piece => piece.position.Equals(reversibleMove.target));

    if (sourcePieceIndex == -1)
    {
      throw new System.Exception("Invalid undo, no piece at target position");
    }

    var sourcePiece = newPiecePositions[sourcePieceIndex];
    newPiecePositions[sourcePieceIndex] = sourcePiece.PlayMove(reversibleMove.source, reversibleMove.promotion != 0 ? sourcePiece.pieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : PieceType.Nothing);

    if (reversibleMove.killed != null)
    {
      newPiecePositions.Add(reversibleMove.killed);
    }

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

    return new PieceIdBoardStateAdapter(underlying, newPiecePositions);
  }

  private static string[] pieceIds = new string[] {
    "a1",
    "a2",
    "a6",
    "a7",
    "b1",
    "b2",
    "b6",
    "b7",
    "c1",
    "c2",
    "c6",
    "c7",
    "d1",
    "d2",
    "d6",
    "d7",
    "e1",
    "e2",
    "e6",
    "e7",
    "f1",
    "f2",
    "f6",
    "f7",
    "g1",
    "g2",
    "g6",
    "g7",
    "h1",
    "h2",
    "h6",
    "h7"
  };
}