using UnityEngine;

// Clase 11 — Un enemigo no necesita IA: necesita un destino y avanzar hacia él.
// MoveTowards llega exacto al punto y nunca se pasa.
public class EnemigoPatrulla : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public SpriteRenderer sprite;

    private Transform destino;

    void Start()
    {
        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning("EnemigoPatrulla necesita PuntoA y PuntoB.");
            enabled = false;
            return;
        }

        destino = puntoB;

        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (destino == null) return;

        float xAnterior = transform.position.x;

        transform.position = Vector3.MoveTowards(
            transform.position, destino.position, velocidad * Time.deltaTime);

        // Mirar hacia donde camina (el sprite base mira a la derecha).
        if (sprite != null)
        {
            float avance = transform.position.x - xAnterior;
            if (avance > 0.0001f) sprite.flipX = false;
            else if (avance < -0.0001f) sprite.flipX = true;
        }

        // Tolerancia, no igualdad: los decimales nunca son exactos.
        if (Vector2.Distance(transform.position, destino.position) < 0.05f)
            destino = (destino == puntoB) ? puntoA : puntoB;
    }
}
