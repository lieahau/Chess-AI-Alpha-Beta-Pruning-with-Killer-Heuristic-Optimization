using UnityEngine;

/// <summary>Menyimpan informasi dasar mengenai jenis-jenis bidak.</summary>
public class PieceData : MonoBehaviour
{
    private static PieceData _instance;
    public static PieceData Instance { get { return _instance; } }

    /// <summary>Semua jenis bidak</summary>
    public enum PieceType
    {
        None = 0,
    
        WhitePawn = 1,
        WhiteRook = 2,
        WhiteKnight = 3,
        WhiteBishop = 4,
        WhiteQueen = 5,
        WhiteKing = 6,

        BlackPawn = -1,
        BlackRook = -2,
        BlackKnight = -3,
        BlackBishop = -4,
        BlackQueen = -5,
        BlackKing = -6
    }

    /// <summary>Sprite bidak putih.</summary>
    public Sprite whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing;

    /// <summary>Sprite setiap hitam.</summary>
    public Sprite blackPawn, blackRook, blackKnight, blackBishop, blackQueen, blackKing;

    private void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    /// <summary>Mengembalikkan Sprite berdasarkan jenis bidaknya.</summary>
    public Sprite GetSpriteFromType(PieceType type)
    {
        switch(type)
        {
            case PieceType.WhitePawn: return whitePawn;
            case PieceType.WhiteRook: return whiteRook;
            case PieceType.WhiteKnight: return whiteKnight;
            case PieceType.WhiteBishop: return whiteBishop;
            case PieceType.WhiteQueen: return whiteQueen;
            case PieceType.WhiteKing: return whiteKing;

            case PieceType.BlackPawn: return blackPawn;
            case PieceType.BlackRook: return blackRook;
            case PieceType.BlackKnight: return blackKnight;
            case PieceType.BlackBishop: return blackBishop;
            case PieceType.BlackQueen: return blackQueen;
            case PieceType.BlackKing: return blackKing;

            default: return null;
        }
    }
}
