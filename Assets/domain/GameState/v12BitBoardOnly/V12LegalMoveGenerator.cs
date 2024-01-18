using System.Collections.Generic;

public static class V12LegalMoveGenerator
{
  private static List<Move> emptyMoves = new List<Move>();
  public static HashsetCache<V12BoardState, List<Move>> legalCache;

  public static List<Move> GenerateLegalMoves(this V12GameState gameState)
  {
    if (gameState.StaleTurns >= 100) return emptyMoves;
    if (gameState.snapshots.GetValueOrDefault(gameState.boardState) >= 2) return emptyMoves;

    if (legalCache == null) legalCache = new HashsetCache<V12BoardState, List<Move>>(999_983);

    var cacheEntry = legalCache.Get(gameState.boardState);
    if (cacheEntry != null) return cacheEntry;

    var legalMoves = GenerateBoardLegalMoves(gameState.boardState);
    legalCache.Set(gameState.boardState, legalMoves);
    return legalMoves;
  }

  private static List<Move> GenerateBoardLegalMoves(V12BoardState rawBoardState)
  {
    var boardState = new AugmentedBoardState(rawBoardState);

    // Declare insufficient material draws
    // Two kings
    var pieceCount = boardState.allBitBoard.bitCount();
    if (pieceCount == 2) return emptyMoves;

    var onGoingBitBoards = rawBoardState.bitBoards[V12BoardState.WhitePawn]
          | rawBoardState.bitBoards[V12BoardState.BlackPawn]
          | rawBoardState.bitBoards[V12BoardState.WhiteRook]
          | rawBoardState.bitBoards[V12BoardState.BlackRook]
          | rawBoardState.bitBoards[V12BoardState.WhiteQueen]
          | rawBoardState.bitBoards[V12BoardState.BlackQueen];

    if (onGoingBitBoards == 0)
    {
      // Two kings and a single bishop or knight
      if (pieceCount == 3) return emptyMoves;

      // Different color same square bishops: draw
      if (pieceCount == 4
        && rawBoardState.bitBoards[V12BoardState.WhiteBishop].bitCount() == 1
        && rawBoardState.bitBoards[V12BoardState.BlackBishop].bitCount() == 1
        && rawBoardState.bitBoards[V12BoardState.WhiteBishop].lsbUnchecked().IsWhiteSquare() == rawBoardState.bitBoards[V12BoardState.BlackBishop].lsbUnchecked().IsWhiteSquare())
      {
        return emptyMoves;
      }
    }

    var pseudoLegalMoves = GeneratePseudoLegalMoves(boardState);
    var legalMoves = new List<Move>(pseudoLegalMoves.Count);

    for (var index = 0; index < pseudoLegalMoves.Count; ++index)
    {
      var move = pseudoLegalMoves[index];
      if (!CanKingDieAfterMove(rawBoardState, move, rawBoardState.whiteTurn))
      {
        legalMoves.Add(move);
      }
    }

    return legalMoves;
  }

  public static bool CanOwnKingDie(V12GameState gameState)
  {
    return CanKingDie(new AugmentedBoardState(gameState.boardState), gameState.BoardState.WhiteTurn);
  }

  private static bool CanKingDieAfterMove(V12BoardState boardState, Move ownMove, bool whiteKing)
  {
    var kingMightBeInCheckState = new AugmentedBoardState(boardState.PlayMove(ownMove).boardState);
    return CanKingDie(kingMightBeInCheckState, whiteKing);
  }

  private static bool CanKingDie(AugmentedBoardState boardState, bool whiteKing)
  {
    var kingPosition = (whiteKing ? boardState.boardState.bitBoards[V12BoardState.WhiteKing] : boardState.boardState.bitBoards[V12BoardState.BlackKing]).lsbUnchecked();

    // Enemy king
    if ((V12Precomputed.kingBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V12BoardState.BlackKing] : boardState.boardState.bitBoards[V12BoardState.WhiteKing])) != 0) return true;

    // Enemy knights
    if ((V12Precomputed.knightBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V12BoardState.BlackKnight] : boardState.boardState.bitBoards[V12BoardState.WhiteKnight])) != 0) return true;

    // Enemy pawns
    if ((whiteKing ? V12Precomputed.whitePawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V12BoardState.BlackPawn] : V12Precomputed.blackPawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V12BoardState.WhitePawn]) != 0) return true;

    // Linear rays
    var linearRayBitBoard = 0ul;
    var rayPiece1 = GetFirstPiecePositionAtRay(-1, 0, boardState.allBitBoard, kingPosition);
    var rayPiece2 = GetFirstPiecePositionAtRay(1, 0, boardState.allBitBoard, kingPosition);
    var rayPiece3 = GetFirstPiecePositionAtRay(0, -1, boardState.allBitBoard, kingPosition);
    var rayPiece4 = GetFirstPiecePositionAtRay(0, 1, boardState.allBitBoard, kingPosition);

    if (rayPiece1 != -1) linearRayBitBoard |= rayPiece1.toBitBoard();
    if (rayPiece2 != -1) linearRayBitBoard |= rayPiece2.toBitBoard();
    if (rayPiece3 != -1) linearRayBitBoard |= rayPiece3.toBitBoard();
    if (rayPiece4 != -1) linearRayBitBoard |= rayPiece4.toBitBoard();

    var linearEnemyBitBoard = whiteKing
      ? boardState.boardState.bitBoards[V12BoardState.BlackQueen] | boardState.boardState.bitBoards[V12BoardState.BlackRook]
      : boardState.boardState.bitBoards[V12BoardState.WhiteQueen] | boardState.boardState.bitBoards[V12BoardState.WhiteRook];

    if ((linearEnemyBitBoard & linearRayBitBoard) != 0) return true;

    // Diagonal rays
    var diagonalRayBitBoard = 0ul;
    var rayPiece5 = GetFirstPiecePositionAtRay(-1, -1, boardState.allBitBoard, kingPosition);
    var rayPiece6 = GetFirstPiecePositionAtRay(-1, 1, boardState.allBitBoard, kingPosition);
    var rayPiece7 = GetFirstPiecePositionAtRay(1, -1, boardState.allBitBoard, kingPosition);
    var rayPiece8 = GetFirstPiecePositionAtRay(1, 1, boardState.allBitBoard, kingPosition);

    if (rayPiece5 != -1) diagonalRayBitBoard |= rayPiece5.toBitBoard();
    if (rayPiece6 != -1) diagonalRayBitBoard |= rayPiece6.toBitBoard();
    if (rayPiece7 != -1) diagonalRayBitBoard |= rayPiece7.toBitBoard();
    if (rayPiece8 != -1) diagonalRayBitBoard |= rayPiece8.toBitBoard();

    var diagonalEnemyBitBoard = whiteKing
      ? boardState.boardState.bitBoards[V12BoardState.BlackQueen] | boardState.boardState.bitBoards[V12BoardState.BlackBishop]
      : boardState.boardState.bitBoards[V12BoardState.WhiteQueen] | boardState.boardState.bitBoards[V12BoardState.WhiteBishop];

    if ((diagonalEnemyBitBoard & diagonalRayBitBoard) != 0) return true;

    return false;
  }

  private static List<Move> GeneratePseudoLegalMoves(AugmentedBoardState boardState)
  {
    if (boardState.boardState.whiteTurn)
    {
      var pawnPositions = boardState.boardState.bitBoards[V12BoardState.WhitePawn].extractIndices();
      var rookPositions = boardState.boardState.bitBoards[V12BoardState.WhiteRook].extractIndices();
      var knightPositions = boardState.boardState.bitBoards[V12BoardState.WhiteKnight].extractIndices();
      var bishopPositions = boardState.boardState.bitBoards[V12BoardState.WhiteBishop].extractIndices();
      var queenPositions = boardState.boardState.bitBoards[V12BoardState.WhiteQueen].extractIndices();
      var kingPositions = boardState.boardState.bitBoards[V12BoardState.WhiteKing].extractIndices();

      var pseudoLegalMoves = new List<Move>(2 * (pawnPositions.Length + rookPositions.Length + knightPositions.Length + bishopPositions.Length + queenPositions.Length + kingPositions.Length));

      for (var index = 0; index < pawnPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalPawnMoves(boardState, pawnPositions[index]));
      }

      for (var index = 0; index < rookPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalRookMoves(boardState, rookPositions[index]));
      }

      for (var index = 0; index < knightPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalKnightMoves(boardState, knightPositions[index]));
      }

      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalBishopMoves(boardState, bishopPositions[index]));
      }

      for (var index = 0; index < queenPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalQueenMoves(boardState, queenPositions[index]));
      }

      for (var index = 0; index < kingPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalKingMoves(boardState, kingPositions[index]));
      }

      return pseudoLegalMoves;
    }
    else
    {
      var pawnPositions = boardState.boardState.bitBoards[V12BoardState.BlackPawn].extractIndices();
      var rookPositions = boardState.boardState.bitBoards[V12BoardState.BlackRook].extractIndices();
      var knightPositions = boardState.boardState.bitBoards[V12BoardState.BlackKnight].extractIndices();
      var bishopPositions = boardState.boardState.bitBoards[V12BoardState.BlackBishop].extractIndices();
      var queenPositions = boardState.boardState.bitBoards[V12BoardState.BlackQueen].extractIndices();
      var kingPositions = boardState.boardState.bitBoards[V12BoardState.BlackKing].extractIndices();

      var pseudoLegalMoves = new List<Move>(2 * (pawnPositions.Length + rookPositions.Length + knightPositions.Length + bishopPositions.Length + queenPositions.Length + kingPositions.Length));

      for (var index = 0; index < pawnPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalPawnMoves(boardState, pawnPositions[index]));
      }

      for (var index = 0; index < rookPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalRookMoves(boardState, rookPositions[index]));
      }

      for (var index = 0; index < knightPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalKnightMoves(boardState, knightPositions[index]));
      }

      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalBishopMoves(boardState, bishopPositions[index]));
      }

      for (var index = 0; index < queenPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalQueenMoves(boardState, queenPositions[index]));
      }

      for (var index = 0; index < kingPositions.Length; ++index)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalKingMoves(boardState, kingPositions[index]));
      }

      return pseudoLegalMoves;
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(AugmentedBoardState boardState, int position)
  {
    var increment = boardState.boardState.whiteTurn ? 1 : -1;
    var ownPawnStartingY = boardState.boardState.whiteTurn ? 1 : 6;
    var enemyFourthRow = boardState.boardState.whiteTurn ? 4 : 3;
    var stopCondition = boardState.boardState.whiteTurn ? 7 : 0;
    var ownRow = position.getRow();

    var targetBitBoard = 0ul;

    // One up
    var oneUpBitBoard = position.toBitBoard().shiftRow(increment) & ~boardState.allBitBoard;
    targetBitBoard |= oneUpBitBoard;

    // Two up
    if (oneUpBitBoard != 0 && ownRow == ownPawnStartingY)
    {
      targetBitBoard |= oneUpBitBoard.shiftRow(increment) & ~boardState.allBitBoard;
    }

    // Captures
    targetBitBoard |= boardState.boardState.whiteTurn ? V12Precomputed.whitePawnCaptureBitBoards[position] & boardState.blackBitBoard : V12Precomputed.blackPawnCaptureBitBoards[position] & boardState.whiteBitBoard;

    // En passant
    if (ownRow == enemyFourthRow && boardState.boardState.EnPassantColumn != -1)
    {
      targetBitBoard |= (boardState.boardState.whiteTurn ? V12Precomputed.whitePawnCaptureBitBoards : V12Precomputed.blackPawnCaptureBitBoards)[position] & (BitBoard.firstColumn << boardState.boardState.EnPassantColumn);
    }

    if (ownRow + increment != stopCondition)
    {
      return TargetBitBoardToMoves(position, targetBitBoard);
    }

    var extractedTargets = targetBitBoard.extractIndices();
    var sourceBoardPosition = position.toBoardPosition();
    var promotionMoves = new List<Move>(extractedTargets.Length * 4);
    var promotions = boardState.boardState.whiteTurn
        ? new PieceType[] { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new PieceType[] { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    // Multiply moves by promotions
    for (var moveIndex = 0; moveIndex < extractedTargets.Length; ++moveIndex)
    {
      for (var promotionIndex = 0; promotionIndex < promotions.Length; ++promotionIndex)
      {
        promotionMoves.Add(new Move(sourceBoardPosition, extractedTargets[moveIndex].toBoardPosition(), promotions[promotionIndex]));
      }
    }

    return promotionMoves;
  }

  private static List<Move> GetPseudoLegalRookMoves(AugmentedBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, -1, ownBitBoard, boardState.allBitBoard, position);

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static List<Move> GetPseudoLegalKnightMoves(AugmentedBoardState boardState, int position)
  {
    var validJumps = V12Precomputed.knightBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard);
    return TargetBitBoardToMoves(position, validJumps);
  }

  private static List<Move> GetPseudoLegalBishopMoves(AugmentedBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(-1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, 1, ownBitBoard, boardState.allBitBoard, position);

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static List<Move> GetPseudoLegalQueenMoves(AugmentedBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, 1, ownBitBoard, boardState.allBitBoard, position);

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static List<Move> GetPseudoLegalKingMoves(AugmentedBoardState boardState, int position)
  {
    var targetBitBoard = V12Precomputed.kingBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard);

    for (var castleIndex = 0; castleIndex < castles.Length; ++castleIndex)
    {
      var castle = castles[castleIndex];
      if (!boardState.boardState.CastleFlags.HasFlag(castle.castle)) continue;
      if (boardState.boardState.whiteTurn != castle.castle.IsWhite()) continue;
      if ((boardState.allBitBoard & castle.emptyPositions) != 0) continue;

      var lastCheckedBoardState = boardState.boardState;
      var lastKingPosition = position;

      if (CanKingDie(boardState, boardState.boardState.whiteTurn)) continue;

      bool kingIsNeverInCheck = true;
      for (var noCheckIndex = 0; noCheckIndex < castle.noCheckPositions.Length; ++noCheckIndex)
      {
        var noCheckPosition = castle.noCheckPositions[noCheckIndex];
        lastCheckedBoardState = lastCheckedBoardState.PlayMove(new Move(lastKingPosition.toBoardPosition(), noCheckPosition.toBoardPosition(), PieceType.Nothing)).boardState;
        lastKingPosition = noCheckPosition;
        if (CanKingDie(new AugmentedBoardState(lastCheckedBoardState), boardState.boardState.whiteTurn))
        {
          kingIsNeverInCheck = false;
          break;
        }
      }

      if (!kingIsNeverInCheck) continue;

      targetBitBoard |= castle.noCheckPositions[^1].toBitBoard();
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static int GetFirstPiecePositionAtRay(int colIncrement, int rowIncrement, ulong allPieces, int position)
  {
    var values = V12Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position] & allPieces;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0) return values.msb();
    return values.lsb();
  }

  private static ulong GetTargetRay(int colIncrement, int rowIncrement, ulong ownPieces, ulong allPieces, int position)
  {
    var rayBitBoard = V12Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position];
    var piecesOnRay = rayBitBoard & allPieces;
    if (piecesOnRay == 0) return rayBitBoard;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0)
    {
      var firstOfRay = piecesOnRay.lsbUnchecked();
      var ignoredBits = firstOfRay + (((ownPieces & firstOfRay.toBitBoard()) != 0) ? 1 : 0);
      return (rayBitBoard >> ignoredBits) << ignoredBits;
    }
    else
    {
      var firstOfRay = piecesOnRay.lsbUnchecked();
      var ignoredBits = 63 - firstOfRay + (((ownPieces & firstOfRay.toBitBoard()) != 0) ? 1 : 0);
      return (rayBitBoard << ignoredBits) >> ignoredBits;
    }
  }

  private static List<Move> TargetBitBoardToMoves(int sourcePosition, ulong targetBitBoard)
  {
    var extractedIndices = targetBitBoard.extractIndices();
    var moves = new List<Move>(extractedIndices.Length);
    var sourceBoardPosition = sourcePosition.toBoardPosition();

    for (var index = 0; index < extractedIndices.Length; ++index)
    {
      moves.Add(new Move(sourceBoardPosition, extractedIndices[index].toBoardPosition(), PieceType.Nothing));
    }

    return moves;
  }

  private class AugmentedBoardState
  {
    public readonly ulong allBitBoard;
    public readonly ulong whiteBitBoard;
    public readonly ulong blackBitBoard;
    public readonly V12BoardState boardState;
    public AugmentedBoardState(V12BoardState boardState)
    {
      this.boardState = boardState;
      allBitBoard = 0ul;

      for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
      {
        allBitBoard |= boardState.bitBoards[bitBoardIndex];
      }

      whiteBitBoard = boardState.bitBoards[V12BoardState.WhitePawn]
        | boardState.bitBoards[V12BoardState.WhiteRook]
        | boardState.bitBoards[V12BoardState.WhiteKnight]
        | boardState.bitBoards[V12BoardState.WhiteBishop]
        | boardState.bitBoards[V12BoardState.WhiteQueen]
        | boardState.bitBoards[V12BoardState.WhiteKing];

      blackBitBoard = boardState.bitBoards[V12BoardState.BlackPawn]
        | boardState.bitBoards[V12BoardState.BlackRook]
        | boardState.bitBoards[V12BoardState.BlackKnight]
        | boardState.bitBoards[V12BoardState.BlackBishop]
        | boardState.bitBoards[V12BoardState.BlackQueen]
        | boardState.bitBoards[V12BoardState.BlackKing];
    }
  }

  private class CastleMove
  {
    public readonly CastleFlags castle;
    public readonly ulong emptyPositions;
    public readonly int[] noCheckPositions;

    public CastleMove(CastleFlags castle, ulong emptyPositions, int[] noCheckPositions)
    {
      this.castle = castle;
      this.emptyPositions = emptyPositions;
      this.noCheckPositions = noCheckPositions;
    }
  }

  private static CastleMove[] castles = new CastleMove[] {
    new CastleMove(CastleFlags.WhiteKing, BoardPosition.fromColRow(5, 0).toBitBoard() | BoardPosition.fromColRow(6, 0).toBitBoard(), new int[] { BoardPosition.fromColRow(5, 0), BoardPosition.fromColRow(6, 0) }),
    new CastleMove(CastleFlags.WhiteQueen, BoardPosition.fromColRow(3, 0).toBitBoard() | BoardPosition.fromColRow(2, 0).toBitBoard() | BoardPosition.fromColRow(1, 0).toBitBoard(), new int[] { BoardPosition.fromColRow(3, 0), BoardPosition.fromColRow(2, 0) }),
    new CastleMove(CastleFlags.BlackKing, BoardPosition.fromColRow(5, 7).toBitBoard() | BoardPosition.fromColRow(6, 7).toBitBoard(), new int[] { BoardPosition.fromColRow(5, 7), BoardPosition.fromColRow(6, 7) }),
    new CastleMove(CastleFlags.BlackQueen, BoardPosition.fromColRow(3, 7).toBitBoard() | BoardPosition.fromColRow(2, 7).toBitBoard() | BoardPosition.fromColRow(1, 7).toBitBoard(), new int[] { BoardPosition.fromColRow(3, 7), BoardPosition.fromColRow(2, 7) }),
  };
}