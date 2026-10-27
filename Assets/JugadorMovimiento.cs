using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class JugadorMovimiento : MonoBehaviour
{
    public float velocidad = 6f;

    private Recolector recolector;

    void Start()
    {
        recolector = GetComponent<Recolector>();
    }

    void Update()
    {
        if (recolector != null && recolector.gameOver) return;

        Vector2 entrada = LeerTeclas();

        float x = entrada.x * velocidad * Time.deltaTime;
        float z = entrada.y * velocidad * Time.deltaTime;

        transform.Translate(x, 0f, z);
    }

    // Lee las teclas con el sistema de input que tenga activo el proyecto.
    Vector2 LeerTeclas()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Keyboard t = Keyboard.current;
        if (t == null) return Vector2.zero;

        float h = 0f;
        float v = 0f;

        if (t.dKey.isPressed || t.rightArrowKey.isPressed) h += 1f;
        if (t.aKey.isPressed || t.leftArrowKey.isPressed) h -= 1f;
        if (t.wKey.isPressed || t.upArrowKey.isPressed) v += 1f;
        if (t.sKey.isPressed || t.downArrowKey.isPressed) v -= 1f;

        return new Vector2(h, v);
#else
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
    }
}
