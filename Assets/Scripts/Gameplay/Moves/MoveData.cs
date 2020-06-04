/// <summary>Menyimpan informasi suatu move.</summary>
[System.Serializable]
public class MoveData
{
    /// <summary>Semua jenis move.</summary>
    public enum MoveType
    {
        Simple = 1, // move biasa yang menggerakkan suatu bidak (termasuk nyerang biasa dan promosi pawn)
        Castle = 2, // castle king dengan rook
        EnPassant = 3, // move serangan khusus yang dimiliki pawn
    }

    /// <summary>Jenis move.</summary>
    public MoveType moveType;
    /// <summary>Jenis bidak.</summary>
    public int pieceType;
    /// <summary>Kotak asal suatu bidak.</summary>
    public int from;
    /// <summary>Kotak tujuan suatu bidak.</summary>
    public int to;
    /// <summary>Kotak asal Rook saat castle</summary>
    public int castleRookFrom;
    /// <summary>Kotak tujuan Rook saat castle</summary>
    public int castleRookTo;
    /// <summary>Kotak pawn musuh saat melakukan EnPassant</summary>
    public int enPassantAttackSquare;
    /// <summary>Apakah move ini mempromosikan pawn?</summary>
    public bool promoted;
    /// <summary>Jenis bidak yang diserang.</summary>
    public int capturedType = 0;
    /// <summary> Nilai yang merepresentasikan seberapa baik move ini sebagai Killer Move.</summary>
    public int weight;

    /// <summary>Simple Move</summary>
    /// <param name="from">Index 0-64 dari kotak asal.</param>
    /// <param name="to">Index 0-64 dari kotak tujuan.</param>
    public MoveData(int piece, int from, int to)
    {
        this.pieceType = piece;
        this.from = from;
        this.to = to;
        this.moveType = MoveType.Simple;
    }

    /// <summary>Castle Move</summary>
    /// <param name="from">Index 0-64 dari kotak asal.</param>
    /// <param name="to">Index 0-64 dari kotak tujuan.</param>
    /// <param name="castleRookFrom">Posisi kotak asal Rook saat castle.</summary>
    /// <param name="castleRookTo">Posisi kotak tujuan Rook saat castle.</summary>
    public MoveData(int piece, int from, int to, int castleRookFrom, int castleRookTo) : this(piece, from, to)
    {
        this.castleRookFrom = castleRookFrom;
        this.castleRookTo = castleRookTo;
        this.moveType = MoveType.Castle;
    }

    /// <summary>EnPassant Move</summary>
    /// <param name="from">Index 0-64 dari kotak asal.</param>
    /// <param name="to">Index 0-64 dari kotak tujuan.</param>
    /// <param name="enPassantSquare">Posisi kotak musuh saat EnPassant.</summary>
    public MoveData(int piece, int from, int to, int enPassantAttackSquare) : this(piece, from, to)
    {
        this.enPassantAttackSquare = enPassantAttackSquare;
        this.moveType = MoveType.EnPassant;
    }

    /// <summary>Duplicate move ini. Digunakan ketika menduplikasi papan.</summary>
    /// <returns>instance MoveData yang baru dengan nilai variabel sama.</returns>
    public MoveData duplicate(){
        MoveData dup;
        if(moveType == MoveType.EnPassant)
            dup = new MoveData(this.pieceType, this.from, this.to, this.enPassantAttackSquare);
        else if(moveType == MoveType.Castle)
            dup = new MoveData(this.pieceType, this.from, this.to, this.castleRookFrom, this.castleRookTo);
        else
            dup = new MoveData(this.pieceType, this.from, this.to);

        dup.promoted = this.promoted;
        dup.capturedType = this.capturedType;

        return dup;
    }

    /// <summary>Membandingkan MoveData ini dengan MoveData lain.</summary>
    /// <param name="other">MoveData lain yang ingin dibandingkan.</param>
    /// <returns>true bila sama, false bila tidak sama.</returns>
    public bool isSameWith(MoveData other)
    {
        return this.moveType == other.moveType && this.pieceType == other.pieceType &&
            this.from == other.from && this.to == other.to &&
            this.enPassantAttackSquare == other.enPassantAttackSquare &&
            this.castleRookFrom == other.castleRookFrom && this.castleRookTo == other.castleRookTo &&
            this.promoted == other.promoted && this.capturedType == other.capturedType;
    }
}
