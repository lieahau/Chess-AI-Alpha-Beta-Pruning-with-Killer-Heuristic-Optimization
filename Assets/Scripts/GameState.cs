using System.Diagnostics;

/// <summary>Berisi kumpulan enum dan static variables yang merepresentasikan status game.</summary>
public static class GameState
{
    public enum Status{
        Normal = 0,
        Check = 1,
        Checkmate = 2,
        Draw = 3
    }

    public enum DrawVariance{
        None = 0,
        StaleMate = 1, // no legal move
        PerpetualCheck = 2, // check repetition 3 times
        ThreefoldRepetition = 3, // position repetition 3 times
        NotEnoughMaterials = 4, // not enough materials to checkmate
    }

    public enum BoardMode {
        AlphaBetaVSAlphaBeta,
        AlphaBetaVSKillerHeuristic,
        KillerHeuristicVSAlphaBeta,
        KillerHeuristicVSKillerHeuristic
    }
    /// <summary>Mode papan saat ini.</summary>
    public static BoardMode boardMode = BoardMode.AlphaBetaVSAlphaBeta;
    /// <summary>Status papan saat ini.</summary>
    public static Status boardStatus = Status.Normal;
    /// <summary>jenis draw yang terjadi apabila status pertandingan diakhiri dengan draw.</summary>
    public static DrawVariance drawVariance = DrawVariance.None;

    public static int totalTurn;

    /// <summary>Tingkat kedalaman pencarian algoritma.</summary>
    public static int whiteSearchDepth = 2, blackSearchDepth = 2;

    /// <summary>Waktu rata-rata setiap move dalam detik dalam 1 pertandingan.</summary>
    public static float whiteAvgTime, blackAvgTime;

    /// <summary>Waktu total dalam detik dalam 1 pertandingan.</summary>
    public static float whiteTotalTime, blackTotalTime;

    /// <summary>Nodes rata-rata setiap move dalam 1 pertandingan.</summary>
    public static ulong whiteAvgNodes, blackAvgNodes;

    /// <summary>Nodes total dalam 1 pertandingan.</summary>
    public static ulong whiteTotalNodes, blackTotalNodes;

    /// <summary>Digunakan untuk menghitung waktu.</summary>
    public static Stopwatch stopwatch;

    public static void RestartValue()
    {
        boardStatus = Status.Normal;
        drawVariance = DrawVariance.None;

        totalTurn = 0;

        whiteAvgTime = 0f;
        whiteTotalTime = 0f;
        whiteAvgNodes = 0;
        whiteTotalNodes = 0;
        
        blackAvgTime = 0f;
        blackTotalTime = 0f;
        blackAvgNodes = 0;
        blackTotalNodes = 0;

        stopwatch = new Stopwatch();
    }

    /// <summary>Update nilai waktu pertandingan</summary>
    /// <param name="asWhite">warna yang ingin di-update waktunya.</param>
    public static void UpdateTimeValue(bool asWhite)
    {
        if(asWhite)
        {
            whiteTotalTime += (float)(stopwatch.Elapsed.TotalMilliseconds / 1000d);
            whiteAvgTime = whiteTotalTime / totalTurn;
        }
        else
        {
            blackTotalTime += (float)(stopwatch.Elapsed.TotalMilliseconds / 1000d);
            blackAvgTime = blackTotalTime / totalTurn;
        }
    }

    /// <summary>Update nilai node pertandingan</summary>
    /// <param name="asWhite">warna yang ingin di-update node-nya.</param>
    public static void UpdateNodeValue(bool asWhite)
    {
        if(asWhite)
        {
            whiteTotalNodes++;
            whiteAvgNodes = whiteTotalNodes / (ulong)totalTurn;
        }
        else
        {
            blackTotalNodes++;
            blackAvgNodes = blackTotalNodes / (ulong)totalTurn;
        }
    }
}
