using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class UIManager : MonoBehaviour
{
    public Recolector recolector;

    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVidas;
    public GameObject textoGameOver;
    public GameObject botonReiniciar;
    public GameObject pauseMenu;

    void Update()
    {
        if (recolector == null) return;

        textoPuntos.text = "Puntos: " + recolector.puntos;
        textoVidas.text = "Vidas: " + recolector.vidas;
        textoGameOver.SetActive(recolector.gameOver);

        if (botonReiniciar != null)
            botonReiniciar.SetActive(recolector.gameOver);

        if (pauseMenu != null && !recolector.gameOver && PausaPresionada())
        {
            bool pausado = !pauseMenu.activeSelf;
            pauseMenu.SetActive(pausado);
            Time.timeScale = pausado ? 0f : 1f;
        }
    }

    bool PausaPresionada()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.P);
#endif
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Reanudar()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }
}
