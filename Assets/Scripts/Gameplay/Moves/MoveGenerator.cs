using System.Collections.Generic;

/// <summary>
/// Generate semua possible moves untuk semua bidak disini (tidak termasuk blocking bidak).
/// Untuk menghemat komputasi AI dalam menelusuri setiap move.
/// </summary>
[System.Serializable]
public class MoveGenerator
{
    /*
        index array papan:
        0  1  2  3  4  5  6  7
        8  9  10 11 12 13 14 15
        16 17 18 19 20 21 22 23
        24 25 26 27 28 29 30 31
        32 33 34 35 36 37 38 39 
        40 41 42 43 44 45 46 47
        48 49 50 51 52 53 54 55
        56 57 58 59 60 61 62 63
    */

    #region Directions
    // Angka yang digunakan berdasarkan penambahan atau pengurangan index pada array suatu Board 
    private int UP = -8;
    private int RIGHT_UP = -7;
    private int RIGHT = 1;
    private int RIGHT_DOWN = 9;
    private int DOWN = 8;
    private int LEFT_DOWN = 7;
    private int LEFT = -1;
    private int LEFT_UP = -9;
    private List<int> leftDir = new List<int>() { -9, -1, 7 };
    private List<int> rightDir = new List<int>() { -7, 1, 9 };
    #endregion

    private List<int> rightBorder = new List<int>() { 7, 15, 23, 31, 39, 47, 55, 63 };
    private List<int> leftBorder = new List<int>() { 0, 8, 16, 24, 32, 40, 48, 56 };

    private List<int> whitePawnRow = new List<int>() { 48, 49, 50, 51, 52, 53, 54, 55 };
    private List<int> blackPawnRow = new List<int>() { 8, 9, 10, 11, 12, 13, 14, 15 };
    private int maxCellIndex = 64;

    public MoveGenerator(){}

    #region Generator
    // method-method yang meng-generate moves setiap jenis bidak.

    /// <summary>Generate semua kemungkinan movement pawn dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <param name="isWhite">apakah pawn ini putih?</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GeneratePawnMoves(int pos, bool isWhite)
    {
        List<int> moves = new List<int>();
        List<int> directions = new List<int>();

        if(isWhite)
        {
            moves.Add(pos + UP);
            if(whitePawnRow.Contains(pos)){ // double move jika masih di posisi awal
                moves.Add(pos + UP + UP);
                moves.Add(-1); // -1 untuk menandai akhir dari ray;
            }
            // add arah attack move.
            directions.Add(LEFT_UP);
            directions.Add(RIGHT_UP);
        }
        else
        {
            moves.Add(pos + DOWN);
            if(blackPawnRow.Contains(pos)){
                moves.Add(pos + DOWN + DOWN);
                moves.Add(-1);
            }
            directions.Add(LEFT_DOWN);
            directions.Add(RIGHT_DOWN);
        }

        moves.Add(-2); // -2 untuk menandai pawn bisa attack
        foreach(int dir in directions)
        {
            int nextIndex = pos + dir;

            // jika sudah ada di border dan dir kearah border yang sama, maka tidak perlu dilanjutkan
            bool leftMost = leftBorder.Contains(pos) && leftDir.Contains(dir);
            bool rightMost = rightBorder.Contains(pos) && rightDir.Contains(dir);
            if(nextIndex >= 0 && nextIndex < maxCellIndex && !leftMost && !rightMost)
                moves.Add(nextIndex);
        }
        return moves;
    }

    /// <summary>Generate semua kemungkinan movement bishop dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GenerateKingMoves(int pos)
    {
        List<int> moves = new List<int>();
        List<int> directions = new List<int>() { UP, RIGHT_UP, RIGHT, RIGHT_DOWN, DOWN, LEFT_DOWN, LEFT, LEFT_UP };
        foreach (int dir in directions)
        {
            int nextIndex = pos + dir;
            // jika sudah ada di border dan dir kearah border yang sama, maka tidak perlu dilanjutkan
            bool leftMost = leftBorder.Contains(pos) && leftDir.Contains(dir);
            bool rightMost = rightBorder.Contains(pos) && rightDir.Contains(dir);
            if(nextIndex >= 0 && nextIndex < maxCellIndex && !leftMost && !rightMost)
                moves.Add(nextIndex);
        }
        return moves;
    }

    /// <summary>Generate semua kemungkinan movement knight dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GenerateKnightMoves(int pos)
    {
        List<int> moves = new List<int>();

        // setiap pattern merepresentasikan 1 kemungkinan move untuk knight
        List<int> pattern1 = new List<int>() { UP, UP, LEFT };
        List<int> pattern2 = new List<int>() { UP, UP, RIGHT };
        List<int> pattern3 = new List<int>() { LEFT, LEFT, UP };
        List<int> pattern4 = new List<int>() { LEFT, LEFT, DOWN };
        List<int> pattern5 = new List<int>() { DOWN, DOWN, LEFT };
        List<int> pattern6 = new List<int>() { DOWN, DOWN, RIGHT };
        List<int> pattern7 = new List<int>() { RIGHT, RIGHT, UP };
        List<int> pattern8 = new List<int>() { RIGHT, RIGHT, DOWN };

        List<List<int>> patterns = new List<List<int>>() { pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8 };

        foreach(List<int> pattern in patterns)
        {
            int prevIndex = pos;
            int nextIndex = pos;
            int idx = 0;

            bool leftMost = leftBorder.Contains(prevIndex) && leftDir.Contains(pattern[idx]);
            bool rightMost = rightBorder.Contains(prevIndex) && rightDir.Contains(pattern[idx]);
            while(nextIndex >= 0 && nextIndex < maxCellIndex && !leftMost && !rightMost && idx < pattern.Count + 1)
            {
                if(idx == pattern.Count)
                {
                    // reach arah terakhir dalam pattern, karena loop blm selesai,
                    // bisa diasumsikan kotaknya tidak melewati border, jadi moves nya eligible.
                    moves.Add(nextIndex);
                    break;
                }
                prevIndex = nextIndex;
                nextIndex = nextIndex + pattern[idx];
                leftMost = leftBorder.Contains(prevIndex) && leftDir.Contains(pattern[idx]);
                rightMost = rightBorder.Contains(prevIndex) && rightDir.Contains(pattern[idx]);
                idx++;
            }
        }
        return moves;
    }

    /// <summary>Generate semua kemungkinan movement bishop dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GenerateBishopMoves(int pos)
    {
        List<int> directions = new List<int>() { LEFT_UP, RIGHT_UP, LEFT_DOWN, RIGHT_DOWN };
        return FindPathMoves(directions, pos);
    }

    /// <summary>Generate semua kemungkinan movement rook dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GenerateRookMoves(int pos)
    {
        List<int> directions = new List<int>() { UP, LEFT, DOWN, RIGHT };
        return FindPathMoves(directions, pos);
    }

    /// <summary>Generate semua kemungkinan movement queen dari suatu index posisi.</summary>
    /// <param name="pos">posisi asal berupa index 0-63</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.</returns>
    public List<int> GenerateQueenMoves(int pos)
    {
        List<int> directions = new List<int>() { UP, LEFT, DOWN, RIGHT, LEFT_UP, RIGHT_UP, LEFT_DOWN, RIGHT_DOWN };
        return FindPathMoves(directions, pos);
    }

    /// <summary>Cari move di setiap direction sejauh mungkin tanpa melewati border papan.</summary>
    /// <param name="directions">arah mana saja yang ingin dicari.</param>
    /// <param name="pos">posisi awal</param>
    /// <returns>List of int yang berisi semua kemungkinan moves.<returns>
    private List<int> FindPathMoves(List<int> directions, int pos)
    {
        List<int> moves = new List<int>();
        
        foreach(int dir in directions)
        {
            int prevIndex = pos;
            int nextIndex = pos + dir;

            // jika sudah ada di border dan dir kearah border yang sama, maka tidak perlu dilanjutkan
            bool leftMost = leftBorder.Contains(prevIndex) && leftDir.Contains(dir);
            bool rightMost = rightBorder.Contains(prevIndex) && rightDir.Contains(dir);

            while(nextIndex >= 0 && nextIndex < maxCellIndex && !leftMost && !rightMost)
            {
                moves.Add(nextIndex);
                prevIndex = nextIndex;
                nextIndex = nextIndex + dir;
                leftMost = leftBorder.Contains(prevIndex) && leftDir.Contains(dir);
                rightMost = rightBorder.Contains(prevIndex) && rightDir.Contains(dir);
            }
            // nilai -1 untuk menandai akhir dari ray (path dengan arah sama yang lebih dari 1 kotak)
            moves.Add(-1);
        }
        return moves;
    }
    #endregion
}
