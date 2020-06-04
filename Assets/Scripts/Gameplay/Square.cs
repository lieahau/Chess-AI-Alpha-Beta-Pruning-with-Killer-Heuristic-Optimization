using UnityEngine;

/// <summary>
/// Script ini di-attach pada setiap kotak pada papan (64 gameobject), dan harus dinamai sesuai dengan lokasinya (a1 - h8)
/// </summary>
public class Square : MonoBehaviour
{
    /// <summary>Nama yang unik (a1 - h8) digunakan untuk menaruh bidak pada kotak yang benar.</summary>
    public string uniqueName;
    public Color recentMoveColor = Color.red;

    [HideInInspector] public Color startColor;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
    }

    /// <summary>Ubah warna kotak ini.</summary>
    /// <param name="color">Ubah menjadi warna ini.</param>
    public void changeColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
