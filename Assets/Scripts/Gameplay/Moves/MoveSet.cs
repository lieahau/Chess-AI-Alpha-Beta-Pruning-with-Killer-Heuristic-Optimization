using System.Collections.Generic;

/// <summary>Menyimpan informasi tentang semua kemungkinan moves suatu bidak pada suatu posisi di papan.</summary>
[System.Serializable]
public class MoveSet
{
   /// <summary>Index location suatu bidak pada papan.</summary>
   public int from = 0;

   /// <summary>Semua kemungkinan moves yang dapat dilakukan bidak ini pada lokasi ini.</summary>
   public List<int> moves;

    /// <summary>Jenis bidak ini.</summary>
    public PieceData.PieceType type;

    /// <summary>Buat move set untuk suatu jenis bidak pada lokasi index yang spesifik.</summary>
    /// <param name="move">Semua kemungkinan moves.</param>
    /// <param name="fromIdx">Dari lokasi ini.</param>
    /// <param name="pieceType">Jenis bidak.</param>
    public MoveSet(List<int> move, int fromIdx, PieceData.PieceType pieceType)
    {
        moves = new List<int>(move);
        from = fromIdx;
        type = pieceType;
    }
}
