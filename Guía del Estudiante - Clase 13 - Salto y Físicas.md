---
tags:
  - dev/guide
  - class
  - salto
  - físicas
estudiante: Simón Tapia
fecha: 2026-11-17
sesion: 13
tipo: guía de pasos con comprobación
---

# Guía de pasos — Ejercicio de hoy: Salto y Físicas

Esta guía te dice **exactamente qué hacer**, en orden, y **exactamente qué debes ver** al terminar cada parte. Síguela con Unity y Visual Studio abiertos.

## De dónde venimos

Es la continuación de la [[Guía del Estudiante - Clase 12 - De Nivel a Juego|clase 12]]. Ahí construiste un circuito con menú, dos niveles, SIGUIENTE NIVEL y REINTENTAR. Hoy agregamos el **salto**: el jugador ya camina, pero ahora también **salta** y puede saltar por encima de plataformas.

---

## 0. El reto de hoy

El jugador **salta** con ESPACIO y **cae** como en la vida real: velocidad vertical que varía con el tiempo. Al caer en una plataforma, **se detiene** hasta saltar de nuevo.

**Criterio del ejercicio:** ESPACIO pone el jugador en el aire, la velocidad vertical cambia, el movimiento horizontal no se ve alterado, al tocar tierra la velocidad vertical se va a 0. Esa es la lógica de salto de hoy.

---

## 1. Qué vas a construir (diagrama)

```
NIVEL 1 ──┬─ ESPACIO ──► JUGADOR en el aire
         │
         ▼
    (cae y) ▼
         └─ toca piso ──► velocidad = 0
```

**Qué aparece hoy:**

- El componente **Rigidbody2D** en el jugador
- El script **`Salto2D`** con dos líneas nuevas
- La detección del salto: `Input.GetKeyDown(KeyCode.Space)`

**Qué NO aparece hoy:**

- **No hay físicas complejas**: colisiones con rotación, rebotar en paredes, fricción del suelo. Eso viene después (Clase 14).
- **No hay detección de suelo automática**: hoy el salto funciona "a ciegas": pones velocidad, caes, al tocar tierra te detienes (lo haremos en la Clase 14).

---

## 2. Glosario

| Palabra | Qué es |
| --- | --- |
| **Rigidbody2D** | Componente que le da "peso" y colisiones a un GameObject en 2D. Hoy es el único ingrediente de físicas que usaremos. |
| **`simulated = true`** | Hace que Unity mueva el objeto como si tuviera gravedad. **Siempre `true`** en el jugador de hoy. |
| **`gravityScale = 3f`** | Qué tan rápido cae cuando no hay fuerza hacia arriba. |
| **`force = new Vector2(0, -10)`** | Empujón hacia abajo en el eje Y cada frame. El signo menos es hacia abajo. |
| **`Input.GetKeyDown(KeyCode.Space)`** | Detecta que ESPACIO se presionó **por primera vez** en el frame actual. Solo da un frame de salto. |
| **`Mathf.Abs(val)`** | Valor absoluto: elimina el signo. Sirve para comparar si una velocidad es positiva o negativa sin preocuparse del signo. |
| **`velocity.y`** | La velocidad vertical del Rigidbody (unidad: metros por segundo, aunque en 2D no es muy intuitivo). |

> **Ojo:** `Input.GetKeyDown` **NO** es "estoy presionando" — es "esto fue presionado ahora". Diferente de `GetKey` (que se mantiene apretado). El salto debe dispararse **una sola vez** al presionar, no mientras mantienes.

---

## 3. Parte A — El componente Rigidbody2D ⏱️ 15 min

En Unity el GameObject **Jugador** debe tener el componente **Rigidbody2D** con los valores correctos.

### A.1 Agregar Rigidbody2D

1. Selecciona el **Jugador** en la Hierarchy.
2. **Component** → busca **Rigidbody2D** en la lista alfabética.

### A.2 Configurar el componente

1. En el Inspector, sección **Rigidbody2D**:

| Propiedad | Valor |
| --- | --- |
| **Simulation Mode** | **Fixed** (por ahora) |
| **Gravity Scale** | **3** |
| **Linear Drag** | **0.1** (fricción suave) |
| **Angular Drag** | **0** |
| **Rotation Speed Limit** | **20** (opcional) |
| **Constraints** | **Rotation Locked (x + z)** → bloquea que gire |

**Qué hacen las propiedades, literalmente:**

- **Fixed** → Unity calcula colisiones y movimiento en frames fijos (estable para juegos de plataforma).
- **Gravity Scale 3** → cae a velocidad media. Si lo subes, cae más rápido. Si lo pones en 0, no cae (flota).
- **Linear Drag 0.1** → lo frena lentamente. En 2D, valores entre 0 y 1 suenan bien.
- **Rotation Locked** → el jugador no gire (evitas que se vuelque).

### A.3 Ver en el Game

1. Dale **Play** en la escena `Nivel2D`.
2. Observa el cuadrado **cayendo** con el tiempo (aunque no lo lancemos por el aire todavía).

**✅ Comprobación A:** el Rigidbody2D aparece, el jugador **cae** poco a poco (velocidad de caída = gravedad × tiempo).

---

## 4. Parte B — La función de salto ⏱️ 10 min

Aquí escribimos el código. Solo **dos líneas**.

### B.1 Abrir el script Salto2D

1. En **Project**, abre el script `Salto2D.cs`. Si no existe:

```csharp
using UnityEngine;

public class Salto2D : MonoBehaviour
{
    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0, 10));
        }
    }
}
```

2. Si el script ya existe, solo debes agregar las dos líneas entre `Update()` y `Close Brace`, o mejor, dentro del `if`:

```csharp
// Agrega ESTAS dos líneas:

if (Input.GetKeyDown(KeyCode.Space))
{
    rb.AddForce(new Vector2(0, 10));
}
```

### B.2 Conectar el script al Jugador

Si el script no está en el objeto Jugador:

1. En la Hierarchy, selecciona **Jugador**.
2. **Add Component** → busca **Salto2D** → selecciona la versión que acabas de crear.

### B.3 Probar el salto

1. Dale **Play**.
2. Presiona **ESPACIO**.

**✅ Comprobación B:** el jugador **salta** hacia arriba al presionar ESPACIO, luego **cae** de vuelta con la gravedad. No puedes seguir presionando para saltar otra vez en el mismo frame (solo da un "impulso").

---

## 5. Parte C — La velocidad de caída ⏱️ 5 min

Aquí conectamos la **gravedad** del Rigidbody con la **velocidad vertical** que ya tenías calculada en la clase 11.

> **En este proyecto (Simón):** el script del movimiento es **`MovimientoJugador2D.cs`** (y el jugador también tiene **`Recolector2D.cs`**). No existe `Jugador.cs`. Si en la guía ves `Jugador.cs`, ábrelo como **`MovimientoJugador2D.cs`**. Además, en Unity 6/tu versión se usa **`rb.linearVelocity`** (no `rb.velocity`).

### C.1 Unificar la velocidad con el Rigidbody (ajuste para tu proyecto)

1. Abre **`Assets/MovimientoJugador2D.cs`**.
2. En el movimiento horizontal, mantén algo como:

```csharp
// Control de movimiento horizontal (tú lo defines), vertical la maneja la física:
rb.linearVelocity = new Vector2(cantidad, rb.linearVelocity.y);
```

3. El salto debe **poner** una velocidad vertical hacia arriba (por ejemplo `fuerzaSalto`), y la gravedad del Rigidbody se encarga del resto:

```csharp
if (Input.GetKeyDown(KeyCode.Space) && EnPiso())
{
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
}
```

(En tu proyecto ya tienes una lógica similar; ajusta si falta.)

### C.2 Probar que el salto es coherente

1. Dale **Play**.
2. Presiona ESPACIO mientras estás en el suelo.

**✅ Comprobación C:** el jugador **sube** unos frames, luego **cae** y, al tocar el suelo, la velocidad vertical se controla por tu lógica de piso. No rebotas para siempre.

---

## 6. Parte D — El tiempo del salto (profundidad) ⏱️ 5 min

Hoy el salto es **físicamente correcto**: sube y baja, pero la **altura** y el **tiempo** de salto dependen de la gravedad y la fuerza. Si quieres que el salto sea muy alto o muy corto, puedes ajustar esos dos valores.

### D.1 Probar con distintos valores

1. En el Rigidbody, cambia **Gravity Scale** a **6**, **1.5**, **0.1**, **0** (sin gravedad).
2. Presiona ESPACIO y observa **cuán alto** subes y **cuánto tiempo** estás en el aire.

**✅ Comprobación D:** Gravity Scale 6 = salto bajo y corto; Gravity Scale 0 = flotas para siempre. Entre 2 y 4 es cómodo para plataformas pequeñas.

---

## 7. Parte E — Control total de la velocidad vertical ⏱️ 5 min

Hoy el jugador tiene **control total** sobre la velocidad vertical: tú decides saltar, y tú decides cuándo caer más rápido o más lento. Esto te prepara para la Clase 14: **detectar suelo y detener la caída**.

### E.1 Agregar botón en el nivel

1. En la escena `Nivel2D`, en el objeto **Jugador**, agrega un **Button** en la UI:
   - **UI → Button - TextMeshPro**
   - **Text** = "Salto Manual"
   - **On Click** → arrastras el objeto **Jugador** → función que ya existe

Esto te da una forma de saltar sin teclado en el juego si quieres probar.

### E.2 Probar el circuito completo

1. Carga el menú, juega el nivel 1, llega a la meta → SIGUIENTE NIVEL → nivel 2.
2. Juega con **ESPACIO** y **W/S** (o flechas) saltando plataformas.
3. Si te caes, te mueres y llega a **GAME OVER** → **REINTENTAR**.

**✅ Comprobación E:** el circuito de niveles funciona como en la clase 12, pero ahora el jugador puede **saltar** para esquivar o cruzar plataformas.

---

## 8. Si algo falla

### B1 — El script no aparece o el código no hace nada

1. ¿El script tiene el nombre exacto: `Salto2D`? Las funciones no se heredan si el nombre no coincide.
2. ¿Hay un **Rigidbody2D** en el jugador? Sin él, `GetComponent<Rigidbody2D>()` devuelve null.
3. ¿El script está **conectado** al objeto **Jugador** en el Inspector?

**Arreglo:** revisa el Inspector del objeto Jugador: debe tener ambos componentes visibles.

---

### B2 — El salto no funciona o no hay gravedad

1. ¿El Rigidbody tiene **Simulation Mode = Fixed** (o **Dynamic**) y **Gravity Scale > 0**?
2. ¿**`simulated`** está en **true**? Si está en **false**, el físico no hace nada.

**Arreglo:** en el Rigidbody, cambia **Simulation Mode** a **Fixed** y **simulated** a **true**.

---

### C — La velocidad vertical no se conecta con la del Rigidbody

1. ¿Estás modificando `velocity` del **Rigidbody** o la del **Transform**? Solo el Rigidbody cuenta para físicas.
2. ¿Hay **Constraints** bloqueando la rotación? En 2D, a veces da problemas si intentas modificar `velocity` manualmente.

**Arreglo:** desactiva el movimiento vertical en el script de movimiento, y deja que el input lo controle solo:

```csharp
// Solo el salto afecta verticalmente
rb.AddForce(new Vector2(0, 10)); // hacia arriba
```

---

### E1 — El jugador no tiene input

1. ¿Tienes un **InputSystem** configurado (Botón1, Botón2)? No confundas con **Input.GetKey**.
2. Si usaste el InputSystem, cambia la línea a `InputSystem.GetButtonDown("Salto")`.

**Arreglo:** mira el script **InputHandler** o **JugadorInput**: usa el input correcto para tu proyecto.

---

## 9. Fin

**Criterios de terminación.** Marca las **5** casillas. Todas deben cumplirse.

- [ ] Rigidbody2D en el jugador con **Fixed**, **Gravity Scale = 3**, **Rotation Locked**.
- [ ] Script **Salto2D** conectado con `AddForce` al presionar **ESPACIO**.
- [ ] El jugador **salta** y luego **cae** por gravedad, sin rebotar infinito.
- [ ] La velocidad vertical se controla sin colisiones extrañas.
- [ ] El circuito del juego (menú → niveles → game over) sigue funcionando con el salto.

**Si las 5 están marcadas: terminaste el salto.**

---

## 10. Bitácora y práctica de la semana

Guarda y anota en tu bitácora:

- **Qué queda pendiente** de hoy: ¿el jugador cae a los agujeros? ¿El salto es muy bajo o muy alto?
- **Qué error te salió** y **cómo lo encontraste** (consola Unity).
- **Los valores** que probaste para **Gravity Scale** y **Fuerza de salto**: qué combinación te gusta más.

**Práctica diaria (15–20 min):**

1. **Experimenta con la gravedad:** cámbiala y mide qué tan alto salta el jugador.
2. **Añade una mecánica de salto doble salto** (te lleva al suelo, saltas de nuevo): requiere `isGrounded`, que aprenderemos en la Clase 14.
3. **Salto de mayor duración:** aumenta la fuerza del salto o disminuye la gravedad, pero cuida que no caiga demasiado rápido después.

---

**Cuando termines:** prueba que puedes esquivar plataformas y saltar huecos en ambos niveles del circuito. Recuerda: hoy aprendiste que el **salto es solo empujar hacia arriba**, y la gravedad es la fuerza que siempre tira hacia abajo. El equilibrio entre ambos es lo que da la sensación de "peso" del jugador.

**Clave de la clase:** en Unity, el salto no es "animación": es **velocidad variable**. El jugador sube, la gravedad lo tira, el jugador sube más fuerte. Es un baile de fuerzas que se equilibra. Mañana eso se **controla**: aprenderemos a detectar el suelo y detener la caída cuando toca tierra.
