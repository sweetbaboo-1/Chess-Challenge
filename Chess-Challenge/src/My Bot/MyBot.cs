using ChessChallenge.API;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

public class MyBot : IChessBot
{

    private static int PAWN_BASE_SCORE = 100;
    private static int KNIGHT_BASE_SCORE = 320;
    private static int BISHOP_BASE_SCORE = 330;
    private static int ROOK_BASE_SCORE = 500;
    private static int QUEEN_BASE_SCORE = 900;
    private static int KING_BASE_SCORE = 1000;

    private static int BASE_BOARD_SCORE = PAWN_BASE_SCORE * 8 + KNIGHT_BASE_SCORE * 2 + BISHOP_BASE_SCORE * 2 + ROOK_BASE_SCORE * 2 + QUEEN_BASE_SCORE + KING_BASE_SCORE;


    private Board board;

    //private float[] pawnOffsetsArray = new float[]
    //{
    //1.0f,  1.0f,  1.0f, 1.0f,  1.0f,  1.0f, 1.0f,  1.0f,  // 7
    //1.5f,  1.5f,  1.5f, 1.5f,  1.5f,  1.5f, 1.5f,  1.5f,  // 15
    //1.1f,  1.1f,  1.2f, 1.3f,  1.3f,  1.2f, 1.1f,  1.1f,  // 23
    //1.05f, 1.05f, 1.1f, 1.25f, 1.25f, 1.1f, 1.05f, 1.05f, // 31
    //1.0f,  1.0f,  1.0f, 1.2f,  1.2f,  1.0f, 1.0f,  1.0f,  // 39
    //1.05f, 0.95f, 0.9f, 1.0f,  1.0f,  0.9f, 0.95f, 1.05f, // 47
    //1.05f, 1.1f,  1.1f, 0.8f,  0.8f,  1.1f, 1.1f,  1.05f, // 55
    //1.0f,  1.0f,  1.0f, 1.0f,  1.0f,  1.0f, 1.0f,  1.0f   // 63
    //};
    //private float[] knightOffsetsArray = new float[]
    //{
    //0.5f, 0.6f, 0.7f, 0.7f, 0.7f, 0.7f, 0.6f, 0.5f,
    //0.6f, 0.8f, 1.0f, 1.0f, 1.0f, 1.0f, 0.8f, 0.6f,
    //0.7f, 1.0f, 1.1f, 1.15f, 1.15f, 1.1f, 1.0f, 0.7f,
    //0.7f, 1.05f, 1.15f, 1.2f, 1.2f, 1.15f, 1.05f, 0.7f,
    //0.7f, 1.0f, 1.15f, 1.2f, 1.2f, 1.15f, 1.0f, 0.7f,
    //0.7f, 1.05f, 1.1f, 1.15f, 1.15f, 1.1f, 1.05f, 0.7f,
    //0.6f, 0.8f, 1.0f, 1.05f, 1.05f, 1.0f, 0.8f, 0.6f,
    //0.5f, 0.6f, 0.7f, 0.7f, 0.7f, 0.7f, 0.6f, 0.5f
    //};
    //private float[] bishopOffsetsArray = new float[]
    //{
    //0.8f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.8f,
    //0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f,
    //0.9f, 1.0f, 1.05f, 1.1f, 1.1f, 1.05f, 1.0f, 0.9f,
    //0.9f, 1.05f, 1.05f, 1.1f, 1.1f, 1.05f, 1.05f, 0.9f,
    //0.9f, 1.0f, 1.1f, 1.1f, 1.1f, 1.1f, 1.0f, 0.9f,
    //0.9f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 0.9f,
    //0.9f, 1.05f, 1.0f, 1.0f, 1.0f, 1.0f, 1.05f, 0.9f,
    //0.8f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f, 0.8f
    //};
    //private float[] queenOffsetsArray = new float[]
    //{
    //0.8f, 0.9f, 0.9f, 0.95f, 0.95f, 0.9f, 0.9f, 0.8f,
    //0.9f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f,
    //0.9f, 1.0f, 1.05f, 1.05f, 1.05f, 1.05f, 1.0f, 0.9f,
    //0.95f, 1.0f, 1.05f, 1.05f, 1.05f, 1.05f, 1.0f, 0.95f,
    //1.0f, 1.0f, 1.05f, 1.05f, 1.05f, 1.05f, 1.0f, 1.0f,
    //0.9f, 1.05f, 1.05f, 1.05f, 1.05f, 1.05f, 1.0f, 0.9f,
    //0.9f, 1.0f, 1.05f, 1.0f, 1.0f, 1.0f, 1.0f, 0.9f,
    //0.8f, 0.9f, 0.9f, 0.95f, 0.95f, 0.9f, 0.9f, 0.8f
    //};
    //private float[] rookOffsetsArray = new float[]
    //{
    //1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
    //1.05f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f, 1.05f,
    //0.95f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.95f,
    //0.95f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.95f,
    //0.95f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.95f,
    //0.95f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.95f,
    //0.95f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.95f,
    //1.0f, 1.0f, 1.0f, 1.05f, 1.05f, 1.0f, 1.0f, 1.0f
    //};
    //private float[] kingMidGameOffsetsArray = new float[]
    //{
    //0.7f, 0.6f, 0.6f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f,
    //0.7f, 0.6f, 0.6f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f,
    //0.7f, 0.6f, 0.6f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f,
    //0.7f, 0.6f, 0.6f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f,
    //0.8f, 0.7f, 0.7f, 0.6f, 0.6f, 0.7f, 0.7f, 0.8f,
    //0.9f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.9f,
    //1.2f, 1.2f, 1.0f, 1.0f, 1.0f, 1.0f, 1.2f, 1.2f,
    //1.2f, 1.3f, 1.1f, 1.0f, 1.0f, 1.1f, 1.3f, 1.2f
    //};
    //private float[] kingEndGameOffsetsArray = new float[]
    //{
    //0.5f, 0.6f, 0.7f, 0.8f, 0.8f, 0.7f, 0.6f, 0.5f,
    //0.7f, 0.8f, 0.9f, 1.0f, 1.0f, 0.9f, 0.8f, 0.7f,
    //0.7f, 0.9f, 1.2f, 1.3f, 1.3f, 1.2f, 0.9f, 0.7f,
    //0.7f, 0.9f, 1.3f, 1.4f, 1.4f, 1.3f, 0.9f, 0.7f,
    //0.7f, 0.9f, 1.3f, 1.4f, 1.4f, 1.3f, 0.9f, 0.7f,
    //0.7f, 0.9f, 1.2f, 1.3f, 1.3f, 1.2f, 0.9f, 0.7f,
    //0.7f, 0.7f, 1.0f, 1.0f, 1.0f, 1.0f, 0.7f, 0.7f,
    //0.5f, 0.7f, 0.7f, 0.7f, 0.7f, 0.7f, 0.7f, 0.5f
    //};

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
         0,  0,  0,  0,  0,  0,  0,  0, //8
         5, 10, 10,-20,-20, 10, 10,  5, //7
         5, -5,-10,  0,  0,-10, -5,  5, //6
         0,  0,  0, 20, 20,  0,  0,  0, //5
         5,  5, 10, 25, 25, 10,  5,  5, //4
        10, 10, 20, 30, 30, 20, 10, 10, //3
        50, 50, 50, 50, 50, 50, 50, 50, //2
        0,  0,  0,  0,  0,  0,  0,  0,  //1
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

    private int movesConsidered;

    public Move Think(Board board, Timer timer)
    {
        this.board = board;
        movesConsidered = 0;
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        float bestScore = float.NegativeInfinity;
        foreach (Move move in moves)
        {
            float score = AlphaBetaNegaMax(3, float.NegativeInfinity, float.PositiveInfinity);
            if (score > bestScore)
                bestMove = move;
        }
        Console.WriteLine(movesConsidered);
        return bestMove;
    }

    public float AlphaBetaNegaMax(int depth, float alpha, float beta)
    {
        if (depth == 0)
            return Quiesce(alpha, beta);

        Move[] moves = board.GetLegalMoves();

        if (moves.Length == 0)
        {
            if (board.IsInCheck())
                return float.NegativeInfinity;
            return 0;
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
        float evaluation = StaticEvaluation();
        if (evaluation >= beta)
            return beta;
        alpha = Math.Max(alpha, evaluation);

        Move[] moves = board.GetLegalMoves(true);
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            evaluation = -Quiesce(-beta, -alpha);
            board.UndoMove(move);
            if (evaluation >= beta)
                return beta;
            alpha = Math.Max(alpha, evaluation);
        }
        return alpha;
    }

    private float StaticEvaluation()
    {
        //float whiteScore = SumBoard(true);
        //float blackScore = SumBoard(false);
        //return whiteScore - blackScore * (board.IsWhiteToMove ? 1 : -1);
        return CountMaterial() * (board.IsWhiteToMove ? 1 : -1);
    }

    private float CountMaterial()
    {
        movesConsidered++;
        PieceList[] pieceLists = board.GetAllPieceLists();

        int whiteScore = 0;
        int blackScore = 0;
        
        whiteScore += pieceLists[0].Count * PAWN_BASE_SCORE;
        whiteScore += pieceLists[1].Count * KNIGHT_BASE_SCORE;
        whiteScore += pieceLists[2].Count * BISHOP_BASE_SCORE;
        whiteScore += pieceLists[3].Count * ROOK_BASE_SCORE;
        whiteScore += pieceLists[4].Count * QUEEN_BASE_SCORE;
        whiteScore += pieceLists[5].Count * KING_BASE_SCORE;
        
        blackScore += pieceLists[6].Count * PAWN_BASE_SCORE;
        blackScore += pieceLists[7].Count * KNIGHT_BASE_SCORE;
        blackScore += pieceLists[8].Count * BISHOP_BASE_SCORE;
        blackScore += pieceLists[9].Count * ROOK_BASE_SCORE;
        blackScore += pieceLists[10].Count * QUEEN_BASE_SCORE;
        blackScore += pieceLists[11].Count * KING_BASE_SCORE;

        return whiteScore - blackScore;
    }

    private float SumBoard(bool isWhite)
    {
        movesConsidered++;

        int numPieces = 0;

        PieceList[] pieceLists = board.GetAllPieceLists();

        foreach (PieceList list in pieceLists)
            numPieces += list.Count;

        int start, stop;
        if (isWhite)
        {
            start = 0;
            stop = 6;
        }
        else
        {

            start = 6;
            stop = 12;
        }
        float boardScore = 0;
        for (int i = start; i < stop; i++)
        {
            foreach (Piece piece in pieceLists[i])
            {
                int index = piece.Square.Index;
                //if (isWhite)
                //    index = 63 - index;
                switch (piece.PieceType)
                {
                    case PieceType.Pawn:
                        boardScore += PAWN_BASE_SCORE + whitePawnOffset[index];
                        break;
                    case PieceType.Rook:
                        boardScore += ROOK_BASE_SCORE + rookOffset[index];
                        break;
                    case PieceType.Bishop:
                        boardScore += BISHOP_BASE_SCORE + bishopOffset[index];
                        break;
                    case PieceType.Knight:
                        boardScore += KNIGHT_BASE_SCORE + knightOffset[index];
                        break;
                    case PieceType.Queen:
                        boardScore += QUEEN_BASE_SCORE + queenOffset[index];
                        break;
                    case PieceType.King:
                        if (numPieces >= 4)
                            boardScore += KING_BASE_SCORE + kingMidgameOffset[index];
                        else
                            boardScore += KING_BASE_SCORE + kingEndgameOffset[index];
                        break;
                }
            }
        }
        return boardScore;
    }
}
