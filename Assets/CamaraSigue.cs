using UnityEngine;

// Clase 11 — La cámara persigue al jugador, pero siempre después de que
// todos los Update terminaron. Ese "después" es LateUpdate.
public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 5f;
    public Vector3 desfase = new Vector3(0f, 1f, -10f);

    void LateUpdate()
    {
        // Sin objetivo no hay nada que seguir (evita el NullReferenceException).
        if (objetivo == null) return;

        Vector3 destino = objetivo.position + desfase;

        // Lerp acerca la cámara un porcentaje del camino: no se teletransporta.
        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
    }
}
