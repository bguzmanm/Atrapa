using UnityEngine;

public class Recolector : MonoBehaviour
{
    public int puntos = 0;
    public int valorMoneda = 10;
    public int costoObstaculo = 5;

    public int vidas = 3;
    public bool gameOver = false;

    public AudioSource sfxSource;
    public AudioClip clipMoneda;
    public AudioClip clipChoque;
    public AudioClip clipGameOver;

    void OnTriggerEnter(Collider other)
    {
        if (gameOver) return;

        if (other.transform.CompareTag("Moneda"))
        {
            puntos += valorMoneda;
            Debug.Log("Moneda recogida. Puntos: " + puntos);

            if (sfxSource != null && clipMoneda != null)
                sfxSource.PlayOneShot(clipMoneda);

            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (gameOver) return;

        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            puntos -= costoObstaculo;
            if (puntos < 0) puntos = 0;
            vidas -= 1;
            Debug.Log("Choque con obstaculo. Puntos: " + puntos + " Vidas: " + vidas);

            if (sfxSource != null && clipChoque != null)
                sfxSource.PlayOneShot(clipChoque);

            if (vidas <= 0)
            {
                gameOver = true;
                Debug.Log("GAME OVER — puntaje final: " + puntos);

                if (sfxSource != null && clipGameOver != null)
                    sfxSource.PlayOneShot(clipGameOver);
            }
        }
    }
}
