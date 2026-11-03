using UnityEngine;
using TMPro;

// Clase 10 — El Recolector de la clase 7/8 con tres letras nuevas:
// OnTriggerEnter2D y Collider2D. La lógica es la misma.
// Clase 11 — Además de puntos, ahora administra vidas, respawn y game over.
public class Recolector2D : MonoBehaviour
{
    public int puntos = 0;
    public int valorMoneda = 10;
    public TMP_Text textoPuntos;

    [Header("Vidas y respawn")]
    public int vidas = 3;
    public Transform puntoInicio;
    public GameObject panelGameOver;
    public TMP_Text textoVidas;
    public float limiteCaida = -7f;

    private bool gameOver = false;
    private Rigidbody2D rb2d;
    private MovimientoJugador2D movimiento;

    void Start()
    {
        // Si venimos de una partida congelada, el nivel arranca vivo.
        Time.timeScale = 1f;

        rb2d = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<MovimientoJugador2D>();

        ActualizarTexto();
    }

    void Update()
    {
        // Caer a un hueco también cuesta una vida; si no, el jugador cae para siempre.
        if (!gameOver && transform.position.y < limiteCaida)
            RecibirDaño();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Moneda"))
        {
            puntos += valorMoneda;
            Destroy(other.gameObject);
            ActualizarTexto();
        }
    }

    void OnCollisionEnter2D(Collision2D colision)
    {
        // El candado de la clase 8: una vez perdido, no se sigue restando.
        if (gameOver) return;

        if (colision.gameObject.CompareTag("Enemigo"))
            RecibirDaño();
    }

    void RecibirDaño()
    {
        vidas--;
        ActualizarTexto();

        if (vidas <= 0)
        {
            gameOver = true;

            if (panelGameOver != null)
                panelGameOver.SetActive(true);

            Time.timeScale = 0f;
            return;
        }

        // El respawn no es física: es una decisión del juego.
        if (puntoInicio != null)
            transform.position = puntoInicio.position;

        if (rb2d != null)
            rb2d.linearVelocity = Vector2.zero;

        if (movimiento != null)
            movimiento.ReiniciarEstadoPiso();
    }

    void ActualizarTexto()
    {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;

        if (textoVidas != null)
            textoVidas.text = "Vidas: " + Mathf.Max(0, vidas);
    }
}
