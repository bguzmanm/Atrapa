using UnityEngine;
using UnityEngine.SceneManagement;

// Clase 11 — La meta no empuja: avisa. Es el trigger que sí gana.
public class Meta : MonoBehaviour
{
    public GameObject panelVictoria;
    public string escenaSiguiente;

    private bool ganado = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ganado) return;
        if (!other.CompareTag("Player")) return;

        ganado = true;

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        // La clase 9 otra vez: congelar el mundo para celebrar.
        Time.timeScale = 0f;
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSiguiente);
    }
}
