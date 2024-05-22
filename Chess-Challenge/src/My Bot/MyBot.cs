using ChessChallenge.API;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

public class MyBot : IChessBot
{

    // scores based off of https://www.chessprogramming.org/Simplified_Evaluation_Function
    private static int PAWN_BASE_SCORE = 100;
    private static int KNIGHT_BASE_SCORE = 320;
    private static int BISHOP_BASE_SCORE = 330;
    private static int ROOK_BASE_SCORE = 500;
    private static int QUEEN_BASE_SCORE = 900;
    private static int KING_BASE_SCORE = 20000;

    private static int BASE_BOARD_SCORE = PAWN_BASE_SCORE * 8 + KNIGHT_BASE_SCORE * 2 + BISHOP_BASE_SCORE * 2 + ROOK_BASE_SCORE * 2 + QUEEN_BASE_SCORE + KING_BASE_SCORE;


    private Board board;
    private int movesConsidered;

    // offsets based off of https://www.chessprogramming.org/Simplified_Evaluation_Function
    private int[] whitePawnOffset = new int[]
    {
        0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
         5,  5, 10, 25, 25, 10,  5,  5,
         0,  0,  0, 20, 20,  0,  0,  0,
         5, -5,-10,  0,  0,-10, -5,  5,
         5, 10, 10,-20,-20, 10, 10,  5,
         0,  0,  0,  0,  0,  0,  0,  0
    };

    private int[] blackPawnOffset = new int[]
    {
         0,  0,  0,  0,  0,  0,  0,  0, 
         5, 10, 10,-20,-20, 10, 10,  5, 
         5, -5,-10,  0,  0,-10, -5,  5, 
         0,  0,  0, 20, 20,  0,  0,  0, 
         5,  5, 10, 25, 25, 10,  5,  5, 
        10, 10, 20, 30, 30, 20, 10, 10, 
        50, 50, 50, 50, 50, 50, 50, 50, 
        0,  0,  0,  0,  0,  0,  0,  0,  
    };

    private int[] knightOffset = new int[]
    {
       -50,-40,-30,-30,-30,-30,-40,-50,
       -40,-20,  0,  0,  0,  0,-20,-40,
       -30,  0, 10, 15, 15, 10,  0,-30,
       -30,  5, 15, 20, 20, 15,  5,-30,
       -30,  0, 15, 20, 20, 15,  0,-30,
       -30,  5, 10, 15, 15, 10,  5,-30,
       -40,-20,  0,  5,  5,  0,-20,-40,
       -50,-40,-30,-30,-30,-30,-40,-50
    };

    private int[] bishopOffset = new int[]
    {
        -20,-10,-10,-10,-10,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5, 10, 10,  5,  0,-10,
        -10,  5,  5, 10, 10,  5,  5,-10,
        -10,  0, 10, 10, 10, 10,  0,-10,
        -10, 10, 10, 10, 10, 10, 10,-10,
        -10,  5,  0,  0,  0,  0,  5,-10,
        -20,-10,-10,-10,-10,-10,-10,-20,
    };

    private int[] rookOffset = new int[]
    {
          0,  0,  0,  0,  0,  0,  0,  0,
          5, 10, 10, 10, 10, 10, 10,  5,
         -5,  0,  0,  0,  0,  0,  0, -5,
         -5,  0,  0,  0,  0,  0,  0, -5,
         -5,  0,  0,  0,  0,  0,  0, -5,
         -5,  0,  0,  0,  0,  0,  0, -5,
         -5,  0,  0,  0,  0,  0,  0, -5,
          0,  0,  0,  5,  5,  0,  0,  0
    };

    private int[] queenOffset = new int[]
    {
        -20,-10,-10, -5, -5,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5,  5,  5,  5,  0,-10,
         -5,  0,  5,  5,  5,  5,  0, -5,
          0,  0,  5,  5,  5,  5,  0, -5,
        -10,  5,  5,  5,  5,  5,  0,-10,
        -10,  0,  5,  0,  0,  0,  0,-10,
        -20,-10,-10, -5, -5,-10,-10,-20
    };

    private int[] kingMidgameOffset = new int[]
    {
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -30,-40,-40,-50,-50,-40,-40,-30,
       -20,-30,-30,-40,-40,-30,-30,-20,
       -10,-20,-20,-20,-20,-20,-20,-10,
        20, 20,  0,  0,  0,  0, 20, 20,
        20, 30, 10,  0,  0, 10, 30, 20
    };

    private int[] kingEndgameOffset = new int[]
    {
       -50,-40,-30,-20,-20,-30,-40,-50,
       -30,-20,-10,  0,  0,-10,-20,-30,
       -30,-10, 20, 30, 30, 20,-10,-30,
       -30,-10, 30, 40, 40, 30,-10,-30,
       -30,-10, 30, 40, 40, 30,-10,-30,
       -30,-10, 20, 30, 30, 20,-10,-30,
       -30,-30,  0,  0,  0,  0,-30,-30,
       -50,-30,-30,-30,-30,-30,-30,-50
    };

    public Move Think(Board board, Timer timer)
    {
        this.board = board;
        movesConsidered = 0;
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        float bestScore = float.NegativeInfinity;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float score = -AlphaBetaNegaMax(3, float.NegativeInfinity, float.PositiveInfinity);
            board.UndoMove(move);
            if (score > bestScore)
            {
                bestMove = move;
                bestScore = score;
            }
        }

        Console.WriteLine(movesConsidered);
        return bestMove;
    }

    private float AlphaBetaNegaMax(int depth, float alpha, float beta)
    {
        if (depth == 0)
            return Quiesce(alpha, beta);

        Move[] moves = board.GetLegalMoves();
        if (moves.Length == 0)
        {
            return board.IsInCheck() ? float.NegativeInfinity : 0;
        }

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float evaluation = -AlphaBetaNegaMax(depth - 1, -beta, -alpha);
            board.UndoMove(move);
            if (evaluation >= beta)
                return beta;
            alpha = Math.Max(alpha, evaluation);
        }
        return alpha;
    }

    private float Quiesce(float alpha, float beta)
    {
        float standPat = EvaluateBoard();
        if (standPat >= beta)
            return beta;
        alpha = Math.Max(alpha, standPat);

        Move[] moves = board.GetLegalMoves(true); // Only capture moves
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float score = -Quiesce(-beta, -alpha);
            board.UndoMove(move);
            if (score >= beta)
                return beta;
            alpha = Math.Max(alpha, score);
        }
        return alpha;
    }

    private float EvaluateBoard()
    {
        float whiteScore = EvaluateSide(true);
        float blackScore = EvaluateSide(false);
        return board.IsWhiteToMove ? (whiteScore - blackScore) : (blackScore - whiteScore);
    }

    private float EvaluateSide(bool isWhite)
    {
        movesConsidered++;
        float score = 0;
        PieceList[] pieceLists = board.GetAllPieceLists();
        int pieceCount = 0;

        int start = isWhite ? 0 : 6;
        int stop = isWhite ? 6 : 12;
        foreach (PieceList list in pieceLists)
            pieceCount += list.Count;

        for (int i = start; i < stop; i++)
        {
            foreach (Piece piece in pieceLists[i])
            {
                int index = piece.Square.Index;
                switch (piece.PieceType)
                {
                    case PieceType.Pawn:
                        score += PAWN_BASE_SCORE + (isWhite ? whitePawnOffset[index] : blackPawnOffset[index]);
                        break;
                    case PieceType.Knight:
                        score += KNIGHT_BASE_SCORE + knightOffset[index];
                        break;
                    case PieceType.Bishop:
                        score += BISHOP_BASE_SCORE + bishopOffset[index];
                        break;
                    case PieceType.Rook:
                        score += ROOK_BASE_SCORE + rookOffset[index];
                        break;
                    case PieceType.Queen:
                        score += QUEEN_BASE_SCORE + queenOffset[index];
                        break;
                    case PieceType.King:
                        score += KING_BASE_SCORE + (pieceCount >= 4 ? kingMidgameOffset[index] : kingEndgameOffset[index]);
                        break;
                }
            }
        }
        return score;
    }
}