using UnityEngine;

// Clase 13 — El salto vive en su propio componente: ESPACIO empuja hacia arriba.
// Se usa ForceMode2D.Impulse para que el empujón sea un salto real (10 m/s)
// aunque la gravedad del Rigidbody2D esté en 3.
public class Salto2D : MonoBehaviour
{
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rb != null)
                Saltar();
        }
    }

    // También lo llama el botón "Salto Manual" de la escena (Parte E).
    public void Saltar()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        rb.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
    }
}
