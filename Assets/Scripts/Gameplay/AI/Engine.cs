using System;
using System.Collections.Generic;

/// <summary>The AI.</summary>
public class Engine
{
    /// <summary>Move untuk putih atau hitam yang ingin di generate.</summary>
    private bool moveAsWhite = true;

    /// <summary>Seberapa dalam engine mencari move.</summary>
    private int searchDepth;

    /// <summary>Best move hasil analisis.</summary>
    private MoveData bestMove;

    /// <summary>Move yang dianggap sebagai Killer Move.</summary>
    private List<MoveData>[] killerMoves;

    /// <summary>Maksimum move yang digunakan untuk menammpung killer move.</summary>
    private int maxKillerMove = 2;

    private Board board;

    public Engine(){}

    #region Alpha Beta Algorithm
    /// <summary>Dipanggil ketika engine harus generate sebuah move menggunakan Alpha Beta algorithm.</summary>
    /// <param name="board">Status papan saat ini.</param>
    /// <param name="moveAsWhite">Move sebagai putih atau hitam.</param>
    /// <param name="depth">Tingkat kedalaman pencarian algoritma.</param>
    public MoveData makeMoveAlphaBeta(Board board, bool moveAsWhite, int depth)
    {
        this.board = board;
        this.moveAsWhite = moveAsWhite;
        this.searchDepth = depth;
        this.bestMove = null;

        alphaBeta(searchDepth, int.MinValue, int.MaxValue, moveAsWhite);

        return bestMove;
    }

    /// <summary>Alpha beta algorithm.</summary>
    /// <param name="depth">Kedalaman AI untuk mencari nilai.</param>
    /// <param name="alpha">Nilai terbaik untuk pemain Max (putih).</param>
    /// <param name="beta">Nilai terbaik untuk pemain Min (hitam).</param>
    /// <param name="maximizing">Depth saat ini giliran siapa? true = putih, false = hitam.</param>
    /// <returns>Nilai dari node yang diberikan.</returns>
    private int alphaBeta(int depth, int alpha, int beta, bool maximizing)
    {
        GameState.UpdateNodeValue(moveAsWhite); // update status node.

        List<MoveData> legalMoves = board.findAllLegalMoves(maximizing);
        if(depth == 0 || legalMoves.Count == 0) // base case, pencarian sudah mencapai max depth atau terminal state. 
            return board.Evaluate(depth == this.searchDepth-1); // return score evaluasi papan saat ini.

        if(maximizing)
        {
            int bestVal = int.MinValue;
            foreach(MoveData move in legalMoves)
            {
                board.move(move); // jalankan move yang ingin di-tes.
                int candidate = alphaBeta(depth - 1, alpha, beta, !maximizing);
                board.revert(); // revert move

                if(candidate > bestVal){
                    bestVal = candidate;
                    if(this.moveAsWhite && depth == this.searchDepth)
                        this.bestMove = move;
                }
                alpha = Math.Max(alpha, bestVal);
                if(beta <= alpha)
                    break;
            }
            return bestVal;
        }
        else
        {
            int bestVal = int.MaxValue;
            foreach(MoveData move in legalMoves)
            {
                board.move(move);
                int candidate = alphaBeta(depth - 1, alpha, beta, !maximizing);
                board.revert();

                if(candidate < bestVal){
                    bestVal = candidate;
                    if(!this.moveAsWhite && depth == this.searchDepth)
                        this.bestMove = move;
                }
                beta = Math.Min(beta, bestVal);
                if(beta <= alpha)
                    break;
            }
            return bestVal;
        }
    }
    #endregion


    #region Killer Heuristic Algorithm
    /// <summary>Dipanggil ketika engine harus generate sebuah move menggunakan Killer Heuristic algorithm.</summary>
    /// <param name="board">Status papan saat ini.</param>
    /// <param name="moveAsWhite">Move sebagai putih atau hitam</param>
    /// <param name="depth">Tingkat kedalaman pencarian algoritma.</param>
    public MoveData makeMoveKillerHeuristic(Board board, bool moveAsWhite, int depth)
    {
        this.board = board;
        this.moveAsWhite = moveAsWhite;
        this.searchDepth = depth;
        this.bestMove = null;
        this.killerMoves = new List<MoveData>[searchDepth+2];
        for(int i = 0; i < searchDepth+2; i++)
            killerMoves[i] = new List<MoveData>();

        killerHeuristic(searchDepth, int.MinValue, int.MaxValue, moveAsWhite, 0);

        return bestMove;
    }

    /// <summary>Killer Heuristic algorithm.</summary>
    /// <param name="node">Move yang dianalisis.</param>
    /// <param name="depth">Kedalaman AI untuk mencari nilai.</param>
    /// <param name="alpha">Nilai terbaik untuk pemain Max (putih).</param>
    /// <param name="beta">Nilai terbaik untuk pemain Min (hitam).</param>
    /// <param name="maximizing">Depth saat ini giliran siapa? true = putih, false = hitam.</param>
    /// <param name="ply">Distance depth saat ini dengan root.</param>
    /// <returns>Nilai dari node yang diberikan.</returns>
    private int killerHeuristic(int depth, int alpha, int beta, bool maximizing, int ply)
    {
        GameState.UpdateNodeValue(moveAsWhite); // update status node

        List<MoveData> legalMoves = GetKillerHeuristicMoves(ply, maximizing);
        if(depth == 0 || legalMoves.Count == 0) // base case, pencarian sudah mencapai max depth atau terminal state.
            return board.Evaluate(depth == this.searchDepth-1); // return score evaluasi papan saat ini.

        if(maximizing)
        {
            int bestVal = int.MinValue;
            foreach(MoveData move in legalMoves)
            {
                board.move(move); // jalankan move yang ingin di-tes.
                int candidate = killerHeuristic(depth - 1, alpha, beta, !maximizing, ply+1);
                board.revert(); // revert move

                if(candidate > bestVal){
                    bestVal = candidate;
                    if(this.moveAsWhite && depth == this.searchDepth)
                        this.bestMove = move;
                }

                alpha = Math.Max(alpha, bestVal);
                if(beta <= alpha)
                {
                    if(!killerMoves[ply].Contains(move))
                        killerMoves[ply].Add(move);
                    else
                        move.weight++;
                    
                    break;
                }
                else if(killerMoves[ply].Contains(move))
                    move.weight--;
            }
            return bestVal;
        }
        else
        {
            int bestVal = int.MaxValue;
            foreach(MoveData move in legalMoves)
            {
                board.move(move);
                int candidate = killerHeuristic(depth - 1, alpha, beta, !maximizing, ply+1);
                board.revert();

                if(candidate < bestVal){
                    bestVal = candidate;
                    if(!this.moveAsWhite && depth == this.searchDepth)
                        this.bestMove = move;
                }

                beta = Math.Min(beta, bestVal);
                if(beta <= alpha)
                {
                    if(!killerMoves[ply].Contains(move))
                        killerMoves[ply].Add(move);
                    else
                        move.weight++;
                    
                    break;
                }
                else if(killerMoves[ply].Contains(move))
                    move.weight--;
            }
            return bestVal;
        }
    }

    /// <summary>Killer Heuristic algorithm.</summary>
    /// <param name="ply">Jarak root dengan node saat ini.</param>
    /// <param name="asWhite">Pada kedalaman ini, giliran putih atau hitam?</param>
    /// <returns>Semua legal moves yang telah diurutkan berdasarkan killer moves.</returns>
    private List<MoveData> GetKillerHeuristicMoves(int ply, bool asWhite)
    {
        // dapatkan legal moves
        List<MoveData> orderingMoves = board.findAllLegalMoves(asWhite);
        if(orderingMoves.Count == 0) // reach terminal state
            return orderingMoves;

        // sort descending killer moves berdasarkan weight dan hapus jika melebihi maksimum killer move
        killerMoves[ply].Sort((x, y) => y.weight.CompareTo(x.weight));
        if(killerMoves[ply].Count > maxKillerMove)
            for(int i = killerMoves[ply].Count; i > maxKillerMove; i--)
                killerMoves[ply].RemoveAt(killerMoves[ply].Count - 1);

        // insert killer move ke indeks awal legal moves agar di periksa terlebih dahulu oleh AI (move ordering)
        for(int slot = killerMoves[ply].Count - 1; slot >= 0; slot--){
            int idx = orderingMoves.FindIndex(x => x.isSameWith(killerMoves[ply][slot]));
            if(idx != -1) // jika ada, maka bisa diasumsikan killer move ini termasuk move yang legal.
            {
                // pindahkan move tersebut ke index awal
                orderingMoves.RemoveAt(idx);
                orderingMoves.Insert(0, killerMoves[ply][slot]);
            }
        }

        return orderingMoves;
    }
    #endregion
}
