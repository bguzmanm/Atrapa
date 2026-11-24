using UnityEngine;

// Clase 10 — Movimiento horizontal con Rigidbody2D. La física manda: no se usa
// transform.Translate. Clase 11 — el candado cuenta contactos con el piso.
// Clase 13 — el salto (ESPACIO) lo hace Salto2D; aquí solo se conserva la Y
// para que la gravedad y el salto no se peleen con el movimiento en X.
public class MovimientoJugador2D : MonoBehaviour
{
    public float velocidad = 6f;

    public Animator anim;
    public SpriteRenderer sprite;

    private Rigidbody2D rb2d;
    private int contactosPiso = 0;
    private bool enElPiso = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // No arrancamos "en el piso": el primer salto solo funciona si toca suelo.
        contactosPiso = 0;
        enElPiso = false;
        SincronizarAnimador();
    }

    void Update()
    {
        if (rb2d == null) return;

        float horizontal = Input.GetAxisRaw("Horizontal");

        // "Mi velocidad es esta". La Y se conserva porque la gravedad la maneja ella.
        rb2d.linearVelocity = new Vector2(horizontal * velocidad, rb2d.linearVelocity.y);

        // El Animator no decide: obedece al bool que pone el código.
        if (anim != null)
            anim.SetBool("corriendo", horizontal != 0);

        if (sprite != null)
            sprite.flipX = horizontal < 0;
    }

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Piso"))
        {
            contactosPiso++;
            ActualizarPiso();
        }
    }

    void OnCollisionExit2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Piso"))
        {
            contactosPiso = Mathf.Max(0, contactosPiso - 1);
            ActualizarPiso();
        }
    }

    void ActualizarPiso()
    {
        enElPiso = contactosPiso > 0;
        SincronizarAnimador();
    }

    void SincronizarAnimador()
    {
        if (anim != null)
            anim.SetBool("enElPiso", enElPiso);
    }

    // Se llama al reaparecer: el jugador vuelve a caer y debe poder saltar
    // solo cuando toque el piso otra vez.
    public void ReiniciarEstadoPiso()
    {
        contactosPiso = 0;
        ActualizarPiso();
    }
}
