using System;
using System.Collections.Generic;
using System.Linq;

public static class V17LegalMoveGenerator
{
  public static Move[] emptyMoveArray = new Move[0];
  public static MapCache<V17BoardState.Hashable, CacheEntry> legalCache;

  private static void InitializeCache()
  {
    if (legalCache == null) legalCache = new MapCache<V17BoardState.Hashable, CacheEntry>(999_983);
  }

  public static bool IsGameStateDraw(this V17GameState gameState)
  {
    if (gameState.staleTurns >= 100) return true;
    if (gameState.snapshots.TryGetValue(gameState.boardState.GetHashable(), out var snapshotCount) && snapshotCount >= 2) return true;
    return false;
  }

  private static bool DrawByInsufficientMaterial(V17BoardState boardState)
  {
    var pieceCount = boardState.allPiecesBitBoard.bitCountLimit(5);

    // Two kings
    if (pieceCount <= 2) return true;
    if (pieceCount > 4) return false;

    var onGoingBitBoards = boardState.bitBoards[V17BoardState.WhitePawn]
      | boardState.bitBoards[V17BoardState.BlackPawn]
      | boardState.bitBoards[V17BoardState.WhiteRook]
      | boardState.bitBoards[V17BoardState.BlackRook]
      | boardState.bitBoards[V17BoardState.WhiteQueen]
      | boardState.bitBoards[V17BoardState.BlackQueen];

    if (onGoingBitBoards != 0) return false;

    // Two kings and a single bishop or knight
    if (pieceCount <= 3) return true;

    // Different color same square bishops: draw
    if (boardState.bitBoards[V17BoardState.WhiteBishop] != 0
      && boardState.bitBoards[V17BoardState.BlackBishop] != 0
      && boardState.bitBoards[V17BoardState.WhiteBishop].lsbUnchecked().IsWhiteSquare() == boardState.bitBoards[V17BoardState.BlackBishop].lsbUnchecked().IsWhiteSquare())
    {
      return true;
    }

    return false;
  }

  public static IReadOnlyList<Move> GenerateLegalMoves(this V17GameState gameState)
  {
    InitializeCache();

    var cacheEntry = legalCache.Get(gameState.boardState.GetHashable());
    if (cacheEntry != null && cacheEntry.type >= CacheEntryType.FullList) return cacheEntry.moves;

    var legalMoves = GenerateBoardLegalMoves(gameState.boardState, CacheEntryType.FullList);
    legalCache.Set(gameState.boardState.GetHashable(), legalMoves);
    return legalMoves.moves;
  }

  public static bool GenerateHasLegalMoves(this V17GameState gameState)
  {
    InitializeCache();

    var cacheEntry = legalCache.Get(gameState.boardState.GetHashable());
    if (cacheEntry != null) return cacheEntry.moves.Count > 0;

    var legalMoves = GenerateBoardLegalMoves(gameState.boardState, CacheEntryType.HasMove);
    legalCache.Set(gameState.boardState.GetHashable(), legalMoves);
    return legalMoves.moves.Count > 0;
  }

  private static CacheEntry GenerateBoardLegalMoves(V17BoardState boardState, CacheEntryType entryType)
  {
    if (DrawByInsufficientMaterial(boardState)) return CacheEntry.empty;
    var checkInfoBoardState = new CheckInfoBoardState(boardState);

    var kingPosition = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteKing : V17BoardState.BlackKing].lsbUnchecked();
    var kingMoves = GetConstrainedLegalKingMoves(checkInfoBoardState, kingPosition, entryType);

    // Two attackers or more, king has to move normally
    if (checkInfoBoardState.attackers.bitCountLimit(2) > 1)
    {
      return new CacheEntry(kingMoves, entryType);
    }

    if (entryType == CacheEntryType.HasMove)
    {
      if (kingMoves.Length > 0) return new CacheEntry(kingMoves, CacheEntryType.HasMove);

      var pawnPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhitePawn : V17BoardState.BlackPawn].extractIndices();
      for (var index = 0; index < pawnPositions.Length; ++index)
      {
        var pieceMoves = GetConstrainedLegalPawnMoves(checkInfoBoardState, pawnPositions[index]);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var bishopPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteBishop : V17BoardState.BlackBishop].extractIndices();
      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalBishopMoves(checkInfoBoardState, bishopPositions[index]);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var knightPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteKnight : V17BoardState.BlackKnight].extractIndices();
      for (var index = 0; index < knightPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalKnightMoves(checkInfoBoardState, knightPositions[index]);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var rookPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteRook : V17BoardState.BlackRook].extractIndices();
      for (var index = 0; index < rookPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalRookMoves(checkInfoBoardState, rookPositions[index]);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var queenPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteQueen : V17BoardState.BlackQueen].extractIndices();
      for (var index = 0; index < queenPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalQueenMoves(checkInfoBoardState, queenPositions[index]);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      return new CacheEntry(emptyMoveArray, CacheEntryType.FullList);
    }
    else
    {
      var pawnPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhitePawn : V17BoardState.BlackPawn].extractIndices();
      var rookPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteRook : V17BoardState.BlackRook].extractIndices();
      var knightPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteKnight : V17BoardState.BlackKnight].extractIndices();
      var bishopPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteBishop : V17BoardState.BlackBishop].extractIndices();
      var queenPositions = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteQueen : V17BoardState.BlackQueen].extractIndices();

      var legalMoves = new Move[
        12 * pawnPositions.Length +
        14 * rookPositions.Length +
        8 * knightPositions.Length +
        14 * bishopPositions.Length +
        28 * queenPositions.Length +
        kingMoves.Length
      ];
      var legalMoveCount = 0;

      for (var index = 0; index < pawnPositions.Length; ++index)
      {
        var pieceMoves = GetConstrainedLegalPawnMoves(checkInfoBoardState, pawnPositions[index]);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalBishopMoves(checkInfoBoardState, bishopPositions[index]);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < knightPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalKnightMoves(checkInfoBoardState, knightPositions[index]);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < rookPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalRookMoves(checkInfoBoardState, rookPositions[index]);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < queenPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalQueenMoves(checkInfoBoardState, queenPositions[index]);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      Array.Copy(kingMoves, 0, legalMoves, legalMoveCount, kingMoves.Length);
      legalMoveCount += kingMoves.Length;

      return new CacheEntry(new ArraySegment<Move>(legalMoves, 0, legalMoveCount), entryType);
    }
  }

  public static bool CanOwnKingDie(V17GameState gameState)
  {
    return CanKingDie(gameState.boardState, gameState.boardState.WhiteTurn);
  }

  private static bool CanKingDie(V17BoardState boardState, bool whiteKing)
  {
    var kingPosition = (whiteKing ? boardState.bitBoards[V17BoardState.WhiteKing] : boardState.bitBoards[V17BoardState.BlackKing]).lsbUnchecked();

    // Enemy king
    if ((V17Precomputed.kingBitBoards[kingPosition] & (whiteKing ? boardState.bitBoards[V17BoardState.BlackKing] : boardState.bitBoards[V17BoardState.WhiteKing])) != 0) return true;

    // Enemy knights
    if ((V17Precomputed.knightBitBoards[kingPosition] & (whiteKing ? boardState.bitBoards[V17BoardState.BlackKnight] : boardState.bitBoards[V17BoardState.WhiteKnight])) != 0) return true;

    // Enemy pawns
    if ((whiteKing ? V17Precomputed.whitePawnCaptureBitBoards[kingPosition] & boardState.bitBoards[V17BoardState.BlackPawn] : V17Precomputed.blackPawnCaptureBitBoards[kingPosition] & boardState.bitBoards[V17BoardState.WhitePawn]) != 0) return true;

    // Linear rays
    var linearRayBitBoard = 0ul;
    var rayPiece1 = GetFirstPiecePositionAtRay(-1, 0, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece2 = GetFirstPiecePositionAtRay(1, 0, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece3 = GetFirstPiecePositionAtRay(0, -1, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece4 = GetFirstPiecePositionAtRay(0, 1, boardState.allPiecesBitBoard, kingPosition);

    if (rayPiece1 != -1) linearRayBitBoard |= rayPiece1.toBitBoard();
    if (rayPiece2 != -1) linearRayBitBoard |= rayPiece2.toBitBoard();
    if (rayPiece3 != -1) linearRayBitBoard |= rayPiece3.toBitBoard();
    if (rayPiece4 != -1) linearRayBitBoard |= rayPiece4.toBitBoard();

    var linearEnemyBitBoard = whiteKing
      ? boardState.bitBoards[V17BoardState.BlackQueen] | boardState.bitBoards[V17BoardState.BlackRook]
      : boardState.bitBoards[V17BoardState.WhiteQueen] | boardState.bitBoards[V17BoardState.WhiteRook];

    if ((linearEnemyBitBoard & linearRayBitBoard) != 0) return true;

    // Diagonal rays
    var diagonalRayBitBoard = 0ul;
    var rayPiece5 = GetFirstPiecePositionAtRay(-1, -1, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece6 = GetFirstPiecePositionAtRay(-1, 1, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece7 = GetFirstPiecePositionAtRay(1, -1, boardState.allPiecesBitBoard, kingPosition);
    var rayPiece8 = GetFirstPiecePositionAtRay(1, 1, boardState.allPiecesBitBoard, kingPosition);

    if (rayPiece5 != -1) diagonalRayBitBoard |= rayPiece5.toBitBoard();
    if (rayPiece6 != -1) diagonalRayBitBoard |= rayPiece6.toBitBoard();
    if (rayPiece7 != -1) diagonalRayBitBoard |= rayPiece7.toBitBoard();
    if (rayPiece8 != -1) diagonalRayBitBoard |= rayPiece8.toBitBoard();

    var diagonalEnemyBitBoard = whiteKing
      ? boardState.bitBoards[V17BoardState.BlackQueen] | boardState.bitBoards[V17BoardState.BlackBishop]
      : boardState.bitBoards[V17BoardState.WhiteQueen] | boardState.bitBoards[V17BoardState.WhiteBishop];

    if ((diagonalEnemyBitBoard & diagonalRayBitBoard) != 0) return true;

    return false;
  }

  private class KingCheckInfo
  {
    public readonly ulong attackers;
    public readonly ulong middleRay;
    public readonly ulong pins;
    public KingCheckInfo(ulong attackers, ulong middleRay, ulong pins)
    {
      this.attackers = attackers;
      this.middleRay = middleRay;
      this.pins = pins;
    }
  }

  private static KingCheckInfo GetKingCheckInfo(V17BoardState boardState, bool whiteKing)
  {
    var kingPosition = (whiteKing ? boardState.bitBoards[V17BoardState.WhiteKing] : boardState.bitBoards[V17BoardState.BlackKing]).lsbUnchecked();
    var attackers = 0ul;
    var middleRay = 0ul;
    var pins = 0ul;

    // Enemy king
    var kingAttackers = V17Precomputed.kingBitBoards[kingPosition] & (whiteKing ? boardState.bitBoards[V17BoardState.BlackKing] : boardState.bitBoards[V17BoardState.WhiteKing]);
    attackers |= kingAttackers;
    middleRay |= kingAttackers;

    // Enemy knights
    var knightAttackers = V17Precomputed.knightBitBoards[kingPosition] & (whiteKing ? boardState.bitBoards[V17BoardState.BlackKnight] : boardState.bitBoards[V17BoardState.WhiteKnight]);
    attackers |= knightAttackers;
    middleRay |= knightAttackers;

    // Enemy pawns
    var pawnAttackers = whiteKing ? V17Precomputed.whitePawnCaptureBitBoards[kingPosition] & boardState.bitBoards[V17BoardState.BlackPawn] : V17Precomputed.blackPawnCaptureBitBoards[kingPosition] & boardState.bitBoards[V17BoardState.WhitePawn];
    attackers |= pawnAttackers;
    middleRay |= pawnAttackers;

    // Linear rays
    var linearEnemyBitBoard = whiteKing
      ? boardState.bitBoards[V17BoardState.BlackQueen] | boardState.bitBoards[V17BoardState.BlackRook]
      : boardState.bitBoards[V17BoardState.WhiteQueen] | boardState.bitBoards[V17BoardState.WhiteRook];

    var middleRay1 = GetEnemyMiddleRay(-1, 0, linearEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay1.attacker;
    middleRay |= middleRay1.ray;

    var middleRay2 = GetEnemyMiddleRay(1, 0, linearEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay2.attacker;
    middleRay |= middleRay2.ray;

    var middleRay3 = GetEnemyMiddleRay(0, -1, linearEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay3.attacker;
    middleRay |= middleRay3.ray;

    var middleRay4 = GetEnemyMiddleRay(0, 1, linearEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay4.attacker;
    middleRay |= middleRay4.ray;

    // Diagonal rays
    var diagonalEnemyBitBoard = whiteKing
      ? boardState.bitBoards[V17BoardState.BlackQueen] | boardState.bitBoards[V17BoardState.BlackBishop]
      : boardState.bitBoards[V17BoardState.WhiteQueen] | boardState.bitBoards[V17BoardState.WhiteBishop];

    var middleRay5 = GetEnemyMiddleRay(-1, -1, diagonalEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay5.attacker;
    middleRay |= middleRay5.ray;

    var middleRay6 = GetEnemyMiddleRay(-1, 1, diagonalEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay6.attacker;
    middleRay |= middleRay6.ray;

    var middleRay7 = GetEnemyMiddleRay(1, 1, diagonalEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay7.attacker;
    middleRay |= middleRay7.ray;

    var middleRay8 = GetEnemyMiddleRay(1, -1, diagonalEnemyBitBoard, boardState.allPiecesBitBoard, kingPosition);
    attackers |= middleRay8.attacker;
    middleRay |= middleRay8.ray;

    var ownPieces = boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var enemyPieces = boardState.whiteTurn ? boardState.blackBitBoard : boardState.whiteBitBoard;

    pins |= GetPinRay(-1, 0, linearEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(1, 0, linearEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(0, -1, linearEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(0, 1, linearEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(-1, -1, diagonalEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(-1, 1, diagonalEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(1, 1, diagonalEnemyBitBoard, ownPieces, enemyPieces, kingPosition);
    pins |= GetPinRay(1, -1, diagonalEnemyBitBoard, ownPieces, enemyPieces, kingPosition);

    return new KingCheckInfo(attackers, middleRay, pins);
  }

  private static Move[] GetConstrainedLegalPawnMoves(CheckInfoBoardState boardState, int position)
  {
    var increment = boardState.boardState.whiteTurn ? 1 : -1;
    var ownPawnStartingY = boardState.boardState.whiteTurn ? 1 : 6;
    var enemyFourthRow = boardState.boardState.whiteTurn ? 4 : 3;
    var stopCondition = boardState.boardState.whiteTurn ? 7 : 0;
    var ownRow = position.getRow();
    var bitBoardPosition = position.toBitBoard();

    var targetBitBoard = 0ul;

    // One up
    var oneUpBitBoard = bitBoardPosition.shiftRow(increment) & ~boardState.boardState.allPiecesBitBoard;
    targetBitBoard |= oneUpBitBoard;

    // Two up
    if (oneUpBitBoard != 0 && ownRow == ownPawnStartingY)
    {
      targetBitBoard |= oneUpBitBoard.shiftRow(increment) & ~boardState.boardState.allPiecesBitBoard;
    }

    // Captures
    targetBitBoard |= boardState.boardState.whiteTurn ? V17Precomputed.whitePawnCaptureBitBoards[position] & boardState.boardState.blackBitBoard : V17Precomputed.blackPawnCaptureBitBoards[position] & boardState.boardState.whiteBitBoard;

    // En passant
    if (ownRow == enemyFourthRow && boardState.boardState.enPassantColumn != -1)
    {
      var enPassantColumnBitBoard = BitBoard.firstColumn << boardState.boardState.enPassantColumn;
      var enPassantBitBoard = (boardState.boardState.whiteTurn ? V17Precomputed.whitePawnCaptureBitBoards : V17Precomputed.blackPawnCaptureBitBoards)[position] & enPassantColumnBitBoard;

      // En passant is the only move that can remove two pins at once
      if (enPassantBitBoard != 0)
      {
        var ignoredPawnsBitBoard = boardState.boardState.allPiecesBitBoard & ~(enPassantColumnBitBoard | bitBoardPosition);
        var firstLeftPiecePosition = GetFirstPiecePositionAtRay(-1, 0, ignoredPawnsBitBoard, position);
        var firstRightPiecePosition = GetFirstPiecePositionAtRay(1, 0, ignoredPawnsBitBoard, position);

        var linearEnemyBitBoard = boardState.boardState.whiteTurn
          ? boardState.boardState.bitBoards[V17BoardState.BlackQueen] | boardState.boardState.bitBoards[V17BoardState.BlackRook]
          : boardState.boardState.bitBoards[V17BoardState.WhiteQueen] | boardState.boardState.bitBoards[V17BoardState.WhiteRook];

        if ((firstLeftPiecePosition != boardState.preservingKingPosition || (linearEnemyBitBoard & firstRightPiecePosition.toBitBoard()) == 0)
          && (firstRightPiecePosition != boardState.preservingKingPosition || (linearEnemyBitBoard & firstLeftPiecePosition.toBitBoard()) == 0))
        {
          targetBitBoard |= enPassantBitBoard;
        }
      }
    }

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & bitBoardPosition) != 0)
    {
      targetBitBoard &= V17Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    if (ownRow + increment != stopCondition || targetBitBoard == 0)
    {
      return TargetBitBoardToMoves(position, targetBitBoard);
    }

    var extractedTargets = targetBitBoard.extractIndices();
    var sourceBoardPosition = position.toBoardPosition();
    var promotionMoves = new Move[extractedTargets.Length * 4];
    var promotions = boardState.boardState.whiteTurn ? whitePawnPromotions : blackPawnPromotions;

    // Multiply moves by promotions
    for (var moveIndex = 0; moveIndex < extractedTargets.Length; ++moveIndex)
    {
      for (var promotionIndex = 0; promotionIndex < promotions.Length; ++promotionIndex)
      {
        promotionMoves[moveIndex * 4 + promotionIndex] = new Move(sourceBoardPosition, extractedTargets[moveIndex].toBoardPosition(), promotions[promotionIndex]);
      }
    }

    return promotionMoves;
  }

  private static Move[] GetPseudoLegalRookMoves(CheckInfoBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.boardState.whiteBitBoard : boardState.boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(1, 0, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(-1, 0, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(0, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(0, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position);

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V17Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalKnightMoves(CheckInfoBoardState boardState, int position)
  {
    var targetBitBoard = V17Precomputed.knightBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.boardState.whiteBitBoard : boardState.boardState.blackBitBoard);

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0) return emptyMoveArray;

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalBishopMoves(CheckInfoBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.boardState.whiteBitBoard : boardState.boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(-1, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(-1, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(1, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(1, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position);

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V17Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalQueenMoves(CheckInfoBoardState boardState, int position)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.boardState.whiteBitBoard : boardState.boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(1, 0, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(-1, 0, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(0, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(0, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(-1, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(-1, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(1, -1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position)
      | GetTargetRay(1, 1, ownBitBoard, boardState.boardState.allPiecesBitBoard, position);

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V17Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetConstrainedLegalKingMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var normalMoveTargetBitBoard = V17Precomputed.kingBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.boardState.whiteBitBoard : boardState.boardState.blackBitBoard);
    var normalMovesNotSteppingInMiddleRay = normalMoveTargetBitBoard & (~boardState.middleRay | boardState.attackers);
    var pseudoLegalTargetPositions = normalMovesNotSteppingInMiddleRay.extractIndices();
    var targetBitBoard = 0ul;

    for (var targetPositionIndex = 0; targetPositionIndex < pseudoLegalTargetPositions.Length; ++targetPositionIndex)
    {
      var targetPosition = pseudoLegalTargetPositions[targetPositionIndex];
      var singleTargetBitBoard = targetPosition.toBitBoard();
      var killedPiece = boardState.boardState.FastPlayKing(boardState.boardState.whiteTurn, position, targetPosition, PieceType.Nothing);
      var canKingDie = CanKingDie(boardState.boardState, boardState.boardState.whiteTurn);
      boardState.boardState.FastPlayKing(boardState.boardState.whiteTurn, targetPosition, position, killedPiece);
      if (canKingDie) continue;
      if (entryType == CacheEntryType.HasMove) return TargetBitBoardToMoves(position, singleTargetBitBoard);
      targetBitBoard |= singleTargetBitBoard;
    }

    // Can't castle if king is under attack
    if (boardState.attackers != 0 || entryType != CacheEntryType.FullList) return TargetBitBoardToMoves(position, targetBitBoard);

    for (var castleIndex = 0; castleIndex < castles.Length; ++castleIndex)
    {
      var castle = castles[castleIndex];
      if (boardState.boardState.whiteTurn != castle.castle.IsWhite()) continue;
      if (!boardState.boardState.castleFlags.HasFlag(castle.castle)) continue;
      if ((boardState.boardState.allPiecesBitBoard & castle.emptyPositions) != 0) continue;

      bool kingIsNeverInCheck = true;
      for (var noCheckIndex = 0; noCheckIndex < castle.noCheckPositions.Length; ++noCheckIndex)
      {
        var noCheckPosition = castle.noCheckPositions[noCheckIndex];
        var killedPiece = boardState.boardState.FastPlayKing(boardState.boardState.whiteTurn, position, noCheckPosition, PieceType.Nothing);
        var canKingDie = CanKingDie(boardState.boardState, boardState.boardState.whiteTurn);
        boardState.boardState.FastPlayKing(boardState.boardState.whiteTurn, noCheckPosition, position, killedPiece);
        if (canKingDie)
        {
          kingIsNeverInCheck = false;
          break;
        }
      }

      if (!kingIsNeverInCheck) continue;

      targetBitBoard |= castle.finalPosition;
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static int GetFirstPiecePositionAtRay(int colIncrement, int rowIncrement, ulong allPieces, int position)
  {
    var values = V17Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position] & allPieces;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0) return values.msb();
    return values.lsb();
  }

  private static ulong GetTargetRay(int colIncrement, int rowIncrement, ulong ownPieces, ulong allPieces, int position)
  {
    var rayBitBoard = V17Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position];
    var piecesOnRay = rayBitBoard & allPieces;
    if (piecesOnRay == 0) return rayBitBoard;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0)
    {
      var firstOfRay = piecesOnRay.msbUnchecked();
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

  private class EnemyMiddleRay
  {
    public readonly ulong ray;
    public readonly ulong attacker;
    public EnemyMiddleRay(ulong ray, ulong attacker)
    {
      this.ray = ray;
      this.attacker = attacker;
    }

    public static EnemyMiddleRay empty = new EnemyMiddleRay(0ul, 0ul);
  }

  private static EnemyMiddleRay GetEnemyMiddleRay(int colIncrement, int rowIncrement, ulong enemyAttackers, ulong allPieces, int position)
  {
    var rayBitBoard = V17Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position];
    var piecesOnRay = rayBitBoard & allPieces;
    if (piecesOnRay == 0) return EnemyMiddleRay.empty;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0)
    {
      var firstOfRay = piecesOnRay.msbUnchecked();
      var firstOfRayBitBoard = firstOfRay.toBitBoard();
      if ((firstOfRay.toBitBoard() & enemyAttackers) == 0) return EnemyMiddleRay.empty;
      return new EnemyMiddleRay((rayBitBoard >> firstOfRay) << firstOfRay, firstOfRayBitBoard);
    }
    else
    {
      var firstOfRay = piecesOnRay.lsbUnchecked();
      var firstOfRayBitBoard = firstOfRay.toBitBoard();
      if ((firstOfRayBitBoard & enemyAttackers) == 0) return EnemyMiddleRay.empty;
      var ignoredBits = 63 - firstOfRay;
      return new EnemyMiddleRay((rayBitBoard << ignoredBits) >> ignoredBits, firstOfRayBitBoard);
    }
  }

  private static ulong GetPinRay(int colIncrement, int rowIncrement, ulong enemyAttackers, ulong ownPieces, ulong enemyPieces, int position)
  {
    var middleRay = GetEnemyMiddleRay(colIncrement, rowIncrement, enemyAttackers, enemyPieces, position).ray;
    var ownPiecesOnMiddleRay = middleRay & ownPieces;
    if (ownPiecesOnMiddleRay.bitCountLimit(2) != 1) return 0ul;
    return ownPiecesOnMiddleRay;
  }

  private static Move[] TargetBitBoardToMoves(int sourcePosition, ulong targetBitBoard)
  {
    if (targetBitBoard == 0) return emptyMoveArray;
    var bitCount = targetBitBoard.bitCount();
    var moves = new Move[bitCount];

    for (var index = 0; index < bitCount; ++index)
    {
      var nextTarget = targetBitBoard.lsbUnchecked();
      targetBitBoard ^= nextTarget.toBitBoard();
      moves[index] = V17Precomputed.moves[sourcePosition][nextTarget];
    }

    return moves;
  }

  private class CheckInfoBoardState
  {
    public readonly ulong attackers;
    public readonly ulong middleRay;
    public readonly ulong pins;
    public readonly int preservingKingPosition;
    public readonly V17BoardState boardState;
    public CheckInfoBoardState(V17BoardState boardState)
    {
      this.boardState = boardState;

      var checkInfo = GetKingCheckInfo(boardState, boardState.whiteTurn);
      this.attackers = checkInfo.attackers;
      this.middleRay = checkInfo.middleRay;
      this.pins = checkInfo.pins;
      this.preservingKingPosition = boardState.bitBoards[boardState.whiteTurn ? V17BoardState.WhiteKing : V17BoardState.BlackKing].lsbUnchecked();
    }
  }

  private class CastleMove
  {
    public readonly CastleFlags castle;
    public readonly ulong finalPosition;
    public readonly int[] noCheckPositions;
    public readonly ulong emptyPositions;

    public CastleMove(CastleFlags castle, int[] noCheckPositions, int[] emptyPositions)
    {
      this.castle = castle;
      this.noCheckPositions = noCheckPositions;
      this.finalPosition = noCheckPositions[^1].toBitBoard();
      this.emptyPositions = emptyPositions.Aggregate(0ul, (acc, cur) => acc | cur.toBitBoard());
    }
  }

  private static CastleMove[] castles = new CastleMove[] {
    new CastleMove(CastleFlags.WhiteKing, new int[] { BoardPosition.fromColRow(5, 0), BoardPosition.fromColRow(6, 0) }, new int [] { BoardPosition.fromColRow(5, 0), BoardPosition.fromColRow(6, 0) }),
    new CastleMove(CastleFlags.WhiteQueen, new int[] { BoardPosition.fromColRow(3, 0), BoardPosition.fromColRow(2, 0) }, new int [] { BoardPosition.fromColRow(3, 0), BoardPosition.fromColRow(2, 0), BoardPosition.fromColRow(1, 0) }),
    new CastleMove(CastleFlags.BlackKing, new int[] { BoardPosition.fromColRow(5, 7), BoardPosition.fromColRow(6, 7) }, new int [] { BoardPosition.fromColRow(5, 7), BoardPosition.fromColRow(6, 7) }),
    new CastleMove(CastleFlags.BlackQueen, new int[] { BoardPosition.fromColRow(3, 7), BoardPosition.fromColRow(2, 7) }, new int [] { BoardPosition.fromColRow(3, 7), BoardPosition.fromColRow(2, 7), BoardPosition.fromColRow(1, 7) }),
  };

  public enum CacheEntryType
  {
    HasMove = 0,
    FullList = 1,
  }

  public class CacheEntry
  {
    public readonly static CacheEntry empty = new CacheEntry(emptyMoveArray, CacheEntryType.FullList);
    public readonly IReadOnlyList<Move> moves;
    public readonly CacheEntryType type;

    public CacheEntry(IReadOnlyList<Move> moves, CacheEntryType type)
    {
      this.moves = moves;
      this.type = type;
    }
  }

  private static PieceType[] whitePawnPromotions = new PieceType[] { PieceType.WhiteQueen, PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop };
  private static PieceType[] blackPawnPromotions = new PieceType[] { PieceType.BlackQueen, PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop };
}