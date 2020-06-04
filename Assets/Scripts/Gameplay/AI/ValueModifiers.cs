/// <summary>
/// menyimpan nilai nilai yang menentukan bagaimana AI bekerja dengan baik.
/// </summary>
public static class ValueModifiers
{
    #region Piece values
    // Andrew Soltis: Rethinking the Chess Pieces, page 6, Another popular chart/system
    public const int Value_Pawn = 100;
    public const int Value_Rook = 500;
    public const int Value_Knight = 300;
    public const int Value_Bishop = 300;
    public const int Value_Queen = 900;
    public const int Value_King = 100000;
    #endregion

    public const int Value_End_Game_Phase = 1300; // Jonathan Speelman: Endgame Preparation, di Introduction nya.

    #region Positional Value White Pieces
    // Positional value untuk bidak putih di setiap kotak

    public static int[] Position_White_Pawn =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        20, 26, 26, 28, 28, 26, 26, 20,
        12, 14, 16, 21, 21, 16, 14, 12,
        8, 10, 12, 18, 18, 12, 10, 8,
        4, 6, 8, 16, 16, 8, 6, 4,
        2, 2, 4, 6, 6, 4, 2, 2,
        0, 0, 0, -4, -4, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static int[] Position_White_Knight =
    {
        -40, -10, -5, -5, -5, -5, -10, -40,
        -5, 5, 5, 5, 5, 5, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 8, 8, 8, 8, 5, -5,
        -5, 0, 5, 5, 5, 5, 0, -5,
        -50, -20, -10, -10, -10, -10, -20, -50
    };

    public static int[] Position_White_Bishop =
    {
        -40, -20, -15, -15, -15, -15, -20, -40,
        0, 5, 5, 5, 5, 5, 5, 0,
        0, 10, 10, 18, 18, 10, 10, 0,
        0, 10, 10, 18, 18, 10, 10, 0,
        0, 5, 10, 18, 18, 10, 5, 0,
        0, 0, 5, 5, 5, 5, 0, 0,
        0, 5, 0, 0, 0, 0, 5, 0,
        -50, -20, -10, -20, -20, -10, -20, -50
    };

    public static int[] Position_White_Rook =
    {
        10, 10, 10, 10, 10, 10, 10, 10,
        5, 5, 5, 10, 10, 5, 5, 5,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
    };

    public static int[] Position_White_Queen =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 10, 10, 10, 10, 0, 0,
        0, 0, 10, 15, 15, 10, 0, 0,
        0, 0, 10, 15, 15, 10, 0, 0,
        0, 0, 10, 10, 10, 10, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static int[] Position_White_King_Start_Game_Phase =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        12, 8, 4, 0, 0, 4, 8, 12,
        16, 12, 8, 4, 4, 8, 12, 16,
        24, 20, 16, 12, 12, 16, 20, 24,
        24, 24, 24, 16, 16, 6, 32, 32
    };

    public static int[] Position_White_King_End_Game_Phase =
    {
        -30, -5, 0, 0, 0, 0, -5, -30,
        -5, 0, 0, 0, 0, 0, 0, -5,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 5, 5, 0, 0, 0,
        0, 0, 0, 5, 5, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        -10, 0, 0, 0, 0, 0, 0, -10,
        -40, -10, -5, -5, -5, -5, -10, -40
    };
    #endregion

    #region Positional Value Black Pieces
    // Positional value untuk bidak hitam di setiap kotak

    public static int[] Position_Black_Pawn =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, -4, -4, 0, 0, 0,
        2, 2, 4, 6, 6, 4, 2, 2,
        4, 6, 8, 16, 16, 8, 6, 4,
        8, 10, 12, 18, 18, 12, 10, 8,
        12, 14, 16, 21, 21, 16, 14, 12,
        20, 26, 26, 28, 28, 26, 26, 20,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static int[] Position_Black_Knight =
    {
        -50, -20, -10, -10, -10, -10, -20, -50,
        -5, 0, 5, 5, 5, 5, 0, -5,
        -5, 5, 8, 8, 8, 8, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 10, 15, 15, 10, 5, -5,
        -5, 5, 5, 5, 5, 5, 5, -5,
        -40, -10, -5, -5, -5, -5, -10, -40,
    };

    public static int[] Position_Black_Bishop =
    {
        -50, -20, -10, -20, -20, -10, -20, -50,
        0, 5, 0, 0, 0, 0, 5, 0,
        0, 0, 5, 5, 5, 5, 0, 0,
        0, 5, 10, 18, 18, 10, 5, 0,
        0, 10, 10, 18, 18, 10, 10, 0,
        0, 10, 10, 18, 18, 10, 10, 0,
        0, 5, 5, 5, 5, 5, 5, 0,
        -40, -20, -15, -15, -15, -15, -20, -40,
    };

    public static int[] Position_Black_Rook =
    {
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        5, 5, 5, 10, 10, 5, 5, 5,
        10, 10, 10, 10, 10, 10, 10, 10,
    };

    public static int[] Position_Black_Queen =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 10, 10, 10, 10, 0, 0,
        0, 0, 10, 15, 15, 10, 0, 0,
        0, 0, 10, 15, 15, 10, 0, 0,
        0, 0, 10, 10, 10, 10, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static int[] Position_Black_King_Start_Game_Phase =
    {
        24, 24, 24, 16, 16, 6, 32, 32,
        24, 20, 16, 12, 12, 16, 20, 24,
        16, 12, 8, 4, 4, 8, 12, 16,
        12, 8, 4, 0, 0, 4, 8, 12,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public static int[] Position_Black_King_End_Game_Phase =
    {
        -40, -10, -5, -5, -5, -5, -10, -40,
        -10, 0, 0, 0, 0, 0, 0, -10,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 5, 5, 0, 0, 0,
        0, 0, 0, 5, 5, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -30, -5, 0, 0, 0, 0, -5, -30,
    };

    #endregion
}
