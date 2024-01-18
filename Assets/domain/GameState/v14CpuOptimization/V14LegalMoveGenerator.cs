using System;
using System.Collections.Generic;
using System.Linq;

public static class V14LegalMoveGenerator
{
  public static Move[] emptyMoveArray = new Move[0];
  public static MapCache<V14BoardState, CacheEntry> legalCache;

  private static void InitializeCache()
  {
    if (legalCache == null) legalCache = new MapCache<V14BoardState, CacheEntry>(999_983);
  }

  public static bool IsGameStateDraw(this V14GameState gameState)
  {
    if (gameState.StaleTurns >= 100) return true;
    if (gameState.snapshots.TryGetValue(gameState.boardState, out var snapshotCount) && snapshotCount >= 2) return true;
    return false;
  }

  private static bool DrawByInsufficientMaterial(AugmentedBoardState boardState)
  {
    var pieceCount = boardState.allBitBoard.bitCountLimit(5);

    // Two kings
    if (pieceCount <= 2) return true;
    if (pieceCount > 4) return false;

    var onGoingBitBoards = boardState.boardState.bitBoards[V14BoardState.WhitePawn]
      | boardState.boardState.bitBoards[V14BoardState.BlackPawn]
      | boardState.boardState.bitBoards[V14BoardState.WhiteRook]
      | boardState.boardState.bitBoards[V14BoardState.BlackRook]
      | boardState.boardState.bitBoards[V14BoardState.WhiteQueen]
      | boardState.boardState.bitBoards[V14BoardState.BlackQueen];

    if (onGoingBitBoards != 0) return false;

    // Two kings and a single bishop or knight
    if (pieceCount <= 3) return true;

    // Different color same square bishops: draw
    if (boardState.boardState.bitBoards[V14BoardState.WhiteBishop] != 0
      && boardState.boardState.bitBoards[V14BoardState.BlackBishop] != 0
      && boardState.boardState.bitBoards[V14BoardState.WhiteBishop].lsbUnchecked().IsWhiteSquare() == boardState.boardState.bitBoards[V14BoardState.BlackBishop].lsbUnchecked().IsWhiteSquare())
    {
      return true;
    }

    return false;
  }

  public static IReadOnlyList<Move> GenerateLegalMoves(this V14GameState gameState)
  {
    InitializeCache();

    var cacheEntry = legalCache.Get(gameState.boardState);
    if (cacheEntry != null && cacheEntry.type >= CacheEntryType.FullList) return cacheEntry.moves;

    var legalMoves = GenerateBoardLegalMoves(gameState.boardState, CacheEntryType.FullList);
    legalCache.Set(gameState.boardState, legalMoves);
    return legalMoves.moves;
  }

  public static bool GenerateHasLegalMoves(this V14GameState gameState)
  {
    InitializeCache();

    var cacheEntry = legalCache.Get(gameState.boardState);
    if (cacheEntry != null) return cacheEntry.moves.Count > 0;

    var legalMoves = GenerateBoardLegalMoves(gameState.boardState, CacheEntryType.HasMove);
    legalCache.Set(gameState.boardState, legalMoves);
    return legalMoves.moves.Count > 0;
  }

  private static CacheEntry GenerateBoardLegalMoves(V14BoardState rawBoardState, CacheEntryType entryType)
  {
    var boardState = new AugmentedBoardState(rawBoardState);
    if (DrawByInsufficientMaterial(boardState)) return CacheEntry.empty;
    var checkInfoBoardState = new CheckInfoBoardState(boardState);

    var kingPosition = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteKing : V14BoardState.BlackKing].lsbUnchecked();
    var kingMoves = GetConstrainedLegalKingMoves(checkInfoBoardState, kingPosition, entryType);

    // Two attackers or more, king has to move normally
    if (checkInfoBoardState.attackers.bitCountLimit(2) > 1)
    {
      return new CacheEntry(kingMoves, entryType);
    }

    if (entryType == CacheEntryType.HasMove)
    {
      if (kingMoves.Length > 0) return new CacheEntry(kingMoves, CacheEntryType.HasMove);

      var pawnPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhitePawn : V14BoardState.BlackPawn].extractIndices();
      for (var index = 0; index < pawnPositions.Length; ++index)
      {
        var pieceMoves = GetConstrainedLegalPawnMoves(checkInfoBoardState, pawnPositions[index], entryType);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var bishopPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteBishop : V14BoardState.BlackBishop].extractIndices();
      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalBishopMoves(checkInfoBoardState, bishopPositions[index], entryType);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var knightPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteKnight : V14BoardState.BlackKnight].extractIndices();
      for (var index = 0; index < knightPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalKnightMoves(checkInfoBoardState, knightPositions[index], entryType);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var rookPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteRook : V14BoardState.BlackRook].extractIndices();
      for (var index = 0; index < rookPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalRookMoves(checkInfoBoardState, rookPositions[index], entryType);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      var queenPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteQueen : V14BoardState.BlackQueen].extractIndices();
      for (var index = 0; index < queenPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalQueenMoves(checkInfoBoardState, queenPositions[index], entryType);
        if (pieceMoves.Length > 0) return new CacheEntry(pieceMoves, CacheEntryType.HasMove);
      }

      return new CacheEntry(emptyMoveArray, CacheEntryType.FullList);
    }
    else
    {
      var pawnPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhitePawn : V14BoardState.BlackPawn].extractIndices();
      var rookPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteRook : V14BoardState.BlackRook].extractIndices();
      var knightPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteKnight : V14BoardState.BlackKnight].extractIndices();
      var bishopPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteBishop : V14BoardState.BlackBishop].extractIndices();
      var queenPositions = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteQueen : V14BoardState.BlackQueen].extractIndices();

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
        var pieceMoves = GetConstrainedLegalPawnMoves(checkInfoBoardState, pawnPositions[index], entryType);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < bishopPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalBishopMoves(checkInfoBoardState, bishopPositions[index], entryType);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < knightPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalKnightMoves(checkInfoBoardState, knightPositions[index], entryType);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < rookPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalRookMoves(checkInfoBoardState, rookPositions[index], entryType);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      for (var index = 0; index < queenPositions.Length; ++index)
      {
        var pieceMoves = GetPseudoLegalQueenMoves(checkInfoBoardState, queenPositions[index], entryType);
        Array.Copy(pieceMoves, 0, legalMoves, legalMoveCount, pieceMoves.Length);
        legalMoveCount += pieceMoves.Length;
      }

      Array.Copy(kingMoves, 0, legalMoves, legalMoveCount, kingMoves.Length);
      legalMoveCount += kingMoves.Length;

      return new CacheEntry(new ArraySegment<Move>(legalMoves, 0, legalMoveCount), entryType);
    }
  }

  public static bool CanOwnKingDie(V14GameState gameState)
  {
    return CanKingDie(new AugmentedBoardState(gameState.boardState), gameState.BoardState.WhiteTurn);
  }

  private static bool CanKingDie(AugmentedBoardState boardState, bool whiteKing)
  {
    var kingPosition = (whiteKing ? boardState.boardState.bitBoards[V14BoardState.WhiteKing] : boardState.boardState.bitBoards[V14BoardState.BlackKing]).lsbUnchecked();

    // Enemy king
    if ((V14Precomputed.kingBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V14BoardState.BlackKing] : boardState.boardState.bitBoards[V14BoardState.WhiteKing])) != 0) return true;

    // Enemy knights
    if ((V14Precomputed.knightBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V14BoardState.BlackKnight] : boardState.boardState.bitBoards[V14BoardState.WhiteKnight])) != 0) return true;

    // Enemy pawns
    if ((whiteKing ? V14Precomputed.whitePawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V14BoardState.BlackPawn] : V14Precomputed.blackPawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V14BoardState.WhitePawn]) != 0) return true;

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
      ? boardState.boardState.bitBoards[V14BoardState.BlackQueen] | boardState.boardState.bitBoards[V14BoardState.BlackRook]
      : boardState.boardState.bitBoards[V14BoardState.WhiteQueen] | boardState.boardState.bitBoards[V14BoardState.WhiteRook];

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
      ? boardState.boardState.bitBoards[V14BoardState.BlackQueen] | boardState.boardState.bitBoards[V14BoardState.BlackBishop]
      : boardState.boardState.bitBoards[V14BoardState.WhiteQueen] | boardState.boardState.bitBoards[V14BoardState.WhiteBishop];

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

  private static KingCheckInfo GetKingCheckInfo(AugmentedBoardState boardState, bool whiteKing)
  {
    var kingPosition = (whiteKing ? boardState.boardState.bitBoards[V14BoardState.WhiteKing] : boardState.boardState.bitBoards[V14BoardState.BlackKing]).lsbUnchecked();
    var attackers = 0ul;
    var middleRay = 0ul;
    var pins = 0ul;

    // Enemy king
    var kingAttackers = V14Precomputed.kingBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V14BoardState.BlackKing] : boardState.boardState.bitBoards[V14BoardState.WhiteKing]);
    attackers |= kingAttackers;
    middleRay |= kingAttackers;

    // Enemy knights
    var knightAttackers = V14Precomputed.knightBitBoards[kingPosition] & (whiteKing ? boardState.boardState.bitBoards[V14BoardState.BlackKnight] : boardState.boardState.bitBoards[V14BoardState.WhiteKnight]);
    attackers |= knightAttackers;
    middleRay |= knightAttackers;

    // Enemy pawns
    var pawnAttackers = whiteKing ? V14Precomputed.whitePawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V14BoardState.BlackPawn] : V14Precomputed.blackPawnCaptureBitBoards[kingPosition] & boardState.boardState.bitBoards[V14BoardState.WhitePawn];
    attackers |= pawnAttackers;
    middleRay |= pawnAttackers;

    // Linear rays
    var linearEnemyBitBoard = whiteKing
      ? boardState.boardState.bitBoards[V14BoardState.BlackQueen] | boardState.boardState.bitBoards[V14BoardState.BlackRook]
      : boardState.boardState.bitBoards[V14BoardState.WhiteQueen] | boardState.boardState.bitBoards[V14BoardState.WhiteRook];

    var middleRay1 = GetEnemyMiddleRay(-1, 0, linearEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay1.attacker;
    middleRay |= middleRay1.ray;

    var middleRay2 = GetEnemyMiddleRay(1, 0, linearEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay2.attacker;
    middleRay |= middleRay2.ray;

    var middleRay3 = GetEnemyMiddleRay(0, -1, linearEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay3.attacker;
    middleRay |= middleRay3.ray;

    var middleRay4 = GetEnemyMiddleRay(0, 1, linearEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay4.attacker;
    middleRay |= middleRay4.ray;

    // Diagonal rays
    var diagonalEnemyBitBoard = whiteKing
      ? boardState.boardState.bitBoards[V14BoardState.BlackQueen] | boardState.boardState.bitBoards[V14BoardState.BlackBishop]
      : boardState.boardState.bitBoards[V14BoardState.WhiteQueen] | boardState.boardState.bitBoards[V14BoardState.WhiteBishop];

    var middleRay5 = GetEnemyMiddleRay(-1, -1, diagonalEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay5.attacker;
    middleRay |= middleRay5.ray;

    var middleRay6 = GetEnemyMiddleRay(-1, 1, diagonalEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay6.attacker;
    middleRay |= middleRay6.ray;

    var middleRay7 = GetEnemyMiddleRay(1, 1, diagonalEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay7.attacker;
    middleRay |= middleRay7.ray;

    var middleRay8 = GetEnemyMiddleRay(1, -1, diagonalEnemyBitBoard, boardState.allBitBoard, kingPosition);
    attackers |= middleRay8.attacker;
    middleRay |= middleRay8.ray;

    var ownPieces = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var enemyPieces = boardState.boardState.whiteTurn ? boardState.blackBitBoard : boardState.whiteBitBoard;

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

  private static Move[] GetConstrainedLegalPawnMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var increment = boardState.boardState.whiteTurn ? 1 : -1;
    var ownPawnStartingY = boardState.boardState.whiteTurn ? 1 : 6;
    var enemyFourthRow = boardState.boardState.whiteTurn ? 4 : 3;
    var stopCondition = boardState.boardState.whiteTurn ? 7 : 0;
    var ownRow = position.getRow();
    var bitBoardPosition = position.toBitBoard();

    var targetBitBoard = 0ul;

    // En passant
    if (ownRow == enemyFourthRow && boardState.boardState.EnPassantColumn != -1)
    {
      var enPassantColumnBitBoard = BitBoard.firstColumn << boardState.boardState.EnPassantColumn;
      var enPassantBitBoard = (boardState.boardState.whiteTurn ? V14Precomputed.whitePawnCaptureBitBoards : V14Precomputed.blackPawnCaptureBitBoards)[position] & enPassantColumnBitBoard;

      // En passant is the only move that can remove two pins at once
      if (enPassantBitBoard != 0)
      {
        var ignoredPawnsBitBoard = boardState.allBitBoard & ~(enPassantColumnBitBoard | bitBoardPosition);
        var firstLeftPiecePosition = GetFirstPiecePositionAtRay(-1, 0, ignoredPawnsBitBoard, position);
        var firstRightPiecePosition = GetFirstPiecePositionAtRay(1, 0, ignoredPawnsBitBoard, position);

        var linearEnemyBitBoard = boardState.boardState.whiteTurn
          ? boardState.boardState.bitBoards[V14BoardState.BlackQueen] | boardState.boardState.bitBoards[V14BoardState.BlackRook]
          : boardState.boardState.bitBoards[V14BoardState.WhiteQueen] | boardState.boardState.bitBoards[V14BoardState.WhiteRook];

        if ((firstLeftPiecePosition != boardState.preservingKingPosition || (linearEnemyBitBoard & firstRightPiecePosition.toBitBoard()) == 0)
          && (firstRightPiecePosition != boardState.preservingKingPosition || (linearEnemyBitBoard & firstLeftPiecePosition.toBitBoard()) == 0))
        {
          targetBitBoard |= enPassantBitBoard;
        }
      }
    }

    // Captures
    targetBitBoard |= boardState.boardState.whiteTurn ? V14Precomputed.whitePawnCaptureBitBoards[position] & boardState.blackBitBoard : V14Precomputed.blackPawnCaptureBitBoards[position] & boardState.whiteBitBoard;

    // One up
    var oneUpBitBoard = bitBoardPosition.shiftRow(increment) & ~boardState.allBitBoard;
    targetBitBoard |= oneUpBitBoard;

    // Two up
    if (oneUpBitBoard != 0 && ownRow == ownPawnStartingY)
    {
      targetBitBoard |= oneUpBitBoard.shiftRow(increment) & ~boardState.allBitBoard;
    }

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & bitBoardPosition) != 0)
    {
      targetBitBoard &= V14Precomputed.pinRays[position][boardState.preservingKingPosition];
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

  private static Move[] GetPseudoLegalRookMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 0, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(0, -1, ownBitBoard, boardState.allBitBoard, position);

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V14Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalKnightMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var targetBitBoard = V14Precomputed.knightBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard);

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0) return emptyMoveArray;

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalBishopMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var ownBitBoard = boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard;
    var targetBitBoard = GetTargetRay(-1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(-1, 1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, -1, ownBitBoard, boardState.allBitBoard, position)
      | GetTargetRay(1, 1, ownBitBoard, boardState.allBitBoard, position);

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V14Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetPseudoLegalQueenMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
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

    // Kill attacker or block ray
    if (boardState.middleRay != 0)
    {
      targetBitBoard &= boardState.middleRay;
    }

    // Stay pinned
    if ((boardState.pins & position.toBitBoard()) != 0)
    {
      targetBitBoard &= V14Precomputed.pinRays[position][boardState.preservingKingPosition];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static Move[] GetConstrainedLegalKingMoves(CheckInfoBoardState boardState, int position, CacheEntryType entryType)
  {
    var normalMoveTargetBitBoard = V14Precomputed.kingBitBoards[position] & ~(boardState.boardState.whiteTurn ? boardState.whiteBitBoard : boardState.blackBitBoard);
    var normalMovesNotSteppingInMiddleRay = normalMoveTargetBitBoard & (~boardState.middleRay | boardState.attackers);
    var pseudoLegalTargetPositions = normalMovesNotSteppingInMiddleRay.extractIndices();
    var boardStateCopy = new V14BoardState(boardState.boardState);
    var ownBitBoard = position.toBitBoard();
    var ownKingBitBoardIndex = boardState.boardState.whiteTurn ? V14BoardState.WhiteKing : V14BoardState.BlackKing;
    var targetBitBoard = 0ul;

    for (var targetPositionIndex = 0; targetPositionIndex < pseudoLegalTargetPositions.Length; ++targetPositionIndex)
    {
      var targetPosition = pseudoLegalTargetPositions[targetPositionIndex];
      var singleTargetBitBoard = targetPosition.toBitBoard();
      var xorMove = singleTargetBitBoard | ownBitBoard;
      boardStateCopy.bitBoards[ownKingBitBoardIndex] ^= xorMove;
      var canKingDie = CanKingDie(new AugmentedBoardState(boardStateCopy), boardState.boardState.whiteTurn);
      boardStateCopy.bitBoards[ownKingBitBoardIndex] ^= xorMove;
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
      if (!boardState.boardState.CastleFlags.HasFlag(castle.castle)) continue;
      if ((boardState.allBitBoard & castle.emptyPositions) != 0) continue;

      bool kingIsNeverInCheck = true;
      for (var noCheckIndex = 0; noCheckIndex < castle.noCheckPositions.Length; ++noCheckIndex)
      {
        var noCheckPosition = castle.noCheckPositions[noCheckIndex];
        var xorMove = noCheckPosition | ownBitBoard;
        boardStateCopy.bitBoards[ownKingBitBoardIndex] ^= xorMove;
        var canKingDie = CanKingDie(new AugmentedBoardState(boardStateCopy), boardState.boardState.whiteTurn);
        boardStateCopy.bitBoards[ownKingBitBoardIndex] ^= xorMove;
        if (canKingDie)
        {
          kingIsNeverInCheck = false;
          break;
        }
      }

      if (!kingIsNeverInCheck) continue;

      targetBitBoard |= castle.noCheckPositions[^1];
    }

    return TargetBitBoardToMoves(position, targetBitBoard);
  }

  private static int GetFirstPiecePositionAtRay(int colIncrement, int rowIncrement, ulong allPieces, int position)
  {
    var values = V14Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position] & allPieces;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0) return values.msb();
    return values.lsb();
  }

  private static ulong GetTargetRay(int colIncrement, int rowIncrement, ulong ownPieces, ulong allPieces, int position)
  {
    var rayBitBoard = V14Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position];
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
    var rayBitBoard = V14Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position];
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
      moves[index] = V14Precomputed.moves[sourcePosition][nextTarget];
    }

    return moves;
  }

  private class AugmentedBoardState
  {
    public readonly ulong allBitBoard;
    public readonly ulong whiteBitBoard;
    public readonly ulong blackBitBoard;
    public readonly V14BoardState boardState;
    public AugmentedBoardState(V14BoardState boardState)
    {
      this.boardState = boardState;

      whiteBitBoard = boardState.bitBoards[V14BoardState.WhitePawn]
        | boardState.bitBoards[V14BoardState.WhiteRook]
        | boardState.bitBoards[V14BoardState.WhiteKnight]
        | boardState.bitBoards[V14BoardState.WhiteBishop]
        | boardState.bitBoards[V14BoardState.WhiteQueen]
        | boardState.bitBoards[V14BoardState.WhiteKing];

      blackBitBoard = boardState.bitBoards[V14BoardState.BlackPawn]
        | boardState.bitBoards[V14BoardState.BlackRook]
        | boardState.bitBoards[V14BoardState.BlackKnight]
        | boardState.bitBoards[V14BoardState.BlackBishop]
        | boardState.bitBoards[V14BoardState.BlackQueen]
        | boardState.bitBoards[V14BoardState.BlackKing];

      allBitBoard = whiteBitBoard | blackBitBoard;
    }
  }

  private class CheckInfoBoardState
  {
    public readonly ulong allBitBoard;
    public readonly ulong whiteBitBoard;
    public readonly ulong blackBitBoard;
    public readonly ulong attackers;
    public readonly ulong middleRay;
    public readonly ulong pins;
    public readonly int preservingKingPosition;
    public readonly V14BoardState boardState;
    public CheckInfoBoardState(AugmentedBoardState boardState)
    {
      this.allBitBoard = boardState.allBitBoard;
      this.whiteBitBoard = boardState.whiteBitBoard;
      this.blackBitBoard = boardState.blackBitBoard;
      this.boardState = boardState.boardState;

      var checkInfo = GetKingCheckInfo(boardState, boardState.boardState.whiteTurn);
      this.attackers = checkInfo.attackers;
      this.middleRay = checkInfo.middleRay;
      this.pins = checkInfo.pins;
      this.preservingKingPosition = boardState.boardState.bitBoards[boardState.boardState.whiteTurn ? V14BoardState.WhiteKing : V14BoardState.BlackKing].lsbUnchecked();
    }
  }

  private class CastleMove
  {
    public readonly CastleFlags castle;
    public readonly ulong[] noCheckPositions;
    public readonly ulong emptyPositions;

    public CastleMove(CastleFlags castle, int[] noCheckPositions, int[] emptyPositions)
    {
      this.castle = castle;
      this.noCheckPositions = noCheckPositions.Select(x => x.toBitBoard()).ToArray();
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