using UnityEngine;

/// <summary>
/// Script ini mengontrol bidak pada papan,
/// </summary>
public class Piece : MonoBehaviour
{
    /// <summary>Jenis bidak ini.</summary>
    public PieceData.PieceType type = PieceData.PieceType.WhitePawn;

    /// <summary>Kotak saat ini yang diisi oleh instance ini</summary>
    public Square square;

    /// <summary>Apakah ini bidak putih?</summary>
    public bool isWhite { get { return (type > 0); } }

    private SpriteRenderer spriteRenderer;

    private void Awake(){
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Set jenis bidak ini, ubah sprite-nya yang sesuai. Berguna ketika promote pawns.
    /// </summary>
    /// <param name="toType">Jenis yang mau diubah.</param>
    public void SetType(PieceData.PieceType toType)
    {
        this.gameObject.name = toType.ToString();
        spriteRenderer.sprite = PieceData.Instance.GetSpriteFromType(toType);
        type = toType;
    }

    /// <summary> Pindah ke kotak baru</summary>
    /// <param name="newSquare">Kotak yang baru.</summary>
    public void moveToSquare(Square newSquare)
    {
        this.transform.position = new Vector3(
            newSquare.transform.position.x,
            newSquare.transform.position.y,
            newSquare.transform.position.z
        );
        square = newSquare;
    }

    /// <summary>Set bidak yang diserang menjadi mati, dan keluarkan dari papan.</summary>
    /// <param name="toLocation">lokasi tempat menaruh bidak.<param>
    public void SetDeadPiece(Vector3 toLocation)
    {
        square = null;
        this.transform.position = toLocation;
    }
}
