using UnityEngine;

// Clase 10 — Movimiento con Rigidbody2D y salto con candado "enElPiso".
// Es el mismo movimiento de la clase 7, pero la física manda: no se usa
// transform.Translate porque el salto necesita velocidad y gravedad.
// Clase 11 — El candado cuenta contactos para no fallar con varios colliders.
public class MovimientoJugador2D : MonoBehaviour
{
    public float velocidad = 6f;
    // 8 daba 1.09 m de altura y el enemigo ocupa ~0.72 m de collider:
    // el margen era casi nulo. 10 da ~1.70 m y deja pasar con holgura.
    public float fuerzaSalto = 10f;

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

        // Dos condiciones con &&: hay que apretar salto Y estar en el piso.
        if (Input.GetKeyDown(KeyCode.Space) && enElPiso)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, fuerzaSalto);
        }
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
