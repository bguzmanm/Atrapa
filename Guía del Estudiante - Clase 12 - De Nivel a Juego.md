---
tags:
  - dev/guide
  - class
estudiante: Simón Tapia
fecha: 2026-11-10
sesion: 12
tipo: guía de pasos con comprobación
---
# Guía de pasos — Ejercicio de hoy: de nivel a juego

Esta guía te dice **exactamente qué hacer**, en orden, y **exactamente qué debes ver** al terminar cada parte. Síguela con Unity abierto.

Es la continuación de la [[Guía del Estudiante - Clase 11 - Nivel 2D Completo|guía de la clase 11]]. Ahí construiste un nivel que **se puede perder y ganar**. Hoy ese nivel deja de ser un nivel y se convierte en un **juego**:

- un **menú** con un botón **JUGAR**,
- un **segundo nivel**, distinto al primero,
- un botón **SIGUIENTE NIVEL** en la victoria y un botón **REINTENTAR** en el GAME OVER,
- y un **`.exe`** que abre directamente en el menú.

> **Criterio del ejercicio:** el `.exe` abre en el menú, se pueden jugar los dos niveles en orden, perder te devuelve al mismo nivel y ganarlo todo te devuelve al menú. Un juego tiene **principio, recorrido y vuelta al inicio**. Eso es hoy.

Y una buena noticia antes de empezar: **hoy casi no hay C# nuevo.** Se escribe **un** script nuevo de seis líneas y se agregan **dos** métodos cortos a scripts que ya tienes. La idea nueva del día es otra, y es grande:

> **Los mismos scripts sirven en varias escenas, y cada escena los configura distinto desde el Inspector.** El código declara la caja; cada nivel la llena a su manera. Es el patrón `public` + Inspector que usas desde la clase 7, ahora aplicado a escenas enteras.

> **Antes de empezar:** abre tu escena `Nivel2D` y dale Play. Recorre el nivel. Si el jugador **no se mueve**, vuelve a la [[Guía de Pasos - Clase 10 - De 3D a 2D|guía de la clase 10]]. Si se mueve pero **no se puede perder o ganar**, eso se arregla en la Parte A de hoy.

---

## 0. Cómo usar esta guía

1. Haz los pasos en orden. Se numeran así: **A.1, A.2, A.3…**
2. Cada paso termina con una casilla **✅ Comprobación**. Dice qué debes ver.
3. **Compara lo que ves con lo que está escrito.** Si coincide → sigue. Si no → **no avances**: busca tu síntoma en la sección **8. Si algo falla**.
4. **No borres nada ni empieces de cero.** Todo se arregla con ajustes.
5. Guarda seguido:
   - Unity: **Ctrl + S** (Mac: **Cmd + S**) — hoy hay **varias escenas**: guarda la que tengas abierta antes de abrir otra.
   - Editor de código: **Ctrl + S** / **Cmd + S**; Unity recompila solo.

### El reloj de hoy (nuevo)

Las clases duran **2 horas** y las guías anteriores no alcanzaban. Esta sí está diseñada para caber, y trae el presupuesto a la vista:

| Parte | Qué | Tiempo |
| --- | --- | --- |
| A | Puesta a punto (cerrar lo pendiente) | 20 min |
| B | El menú principal | 20 min |
| C | Los dos botones | 30 min |
| D | El nivel 2 | 15 min |
| E | El circuito completo y el `.exe` | 15 min |

**Reglas del reloj:**

- Si una parte se pasa de su tiempo, se **recorta el diseño** (D.3), nunca las conexiones.
- **La Parte E no se corta nunca:** sin el `.exe` la clase no cierra. Si hay que recortar para llegar, se recorta antes.
- Lo que no alcance **no se pierde**: queda como práctica 1 de la semana, y la próxima clase empieza otra vez con 20 minutos de puesta a punto.
- Si un paso te tiene más de **10 minutos pegado**, no lo fuerces: anótalo y pasa al siguiente paso que puedas probar.

### Lo que NO debes hacer hoy

- **No borres** ninguna escena, ningún script, ni el juego 3D de la clase 9. Hoy nada se borra: se **agrega**.
- Hoy se crea **un** script nuevo (`MenuPrincipal`) y se **modifican** dos (`Meta` y `Recolector2D`). Nada más.
- **No dupliques la escena** antes de hacer la Parte C: los botones se copian solos al duplicar. Ese es el truco de hoy.
- **No cambies** el nombre de las escenas después de escribirlas en los campos: el nombre queda escrito en un `string` y **no se actualiza solo**.
- **No uses** `FindObjectOfType`, `GameObject.Find` ni `Buscar`: las referencias se conectan **arrastrando en el Inspector**.
- **No descargues** assets nuevos. El menú funciona con texto y colores.

### Si tu Unity no está en inglés

Todos los menús de esta guía están en inglés:

- Windows: **Edit → Preferences → Localization → English**
- Mac: **Unity → Preferences → Localization → English**

### Si aparece el error de Input

Si en la Consola ves un texto rojo con `You are trying to read Input using the UnityEngine.Input class`, arréglalo **ahora**: `Edit → Project Settings → Player → Other Settings → Active Input Handling = Both`. Unity se reinicia. No sigas con ese error presente.

---

## 1. Qué vas a construir

El circuito completo del juego, de punta a punta:

```
 MENÚ ──JUGAR──► NIVEL 1 ──META──► NIVEL 2 ──META──► MENÚ
                    │                   │
                GAME OVER           GAME OVER
                    │                   │
                REINTENTAR          REINTENTAR
                    ▼                   ▼
                 NIVEL 1              NIVEL 2
```

- **Menú:** escena nueva con el nombre del juego y un botón.
- **Nivel 1:** tu `Nivel2D` de la clase 11, con un botón nuevo en la victoria.
- **Nivel 2:** una **copia** de la escena, editada para que se sienta distinta.
- **Reintentar:** el GAME OVER deja de ser un callejón sin salida.
- **`.exe`:** abre en el menú y se juega el circuito completo, sin Unity.

**Lo que NO cambia:** el movimiento, el salto, la cámara, el enemigo y las vidas quedan **exactamente** como están. Hoy no se toca el gameplay del nivel.

**La idea nueva en una frase:** el script `Meta` es **uno solo** para los dos niveles, pero cada nivel le dice en el Inspector **a dónde sigue**. Código compartido, configuración por escena.

---

## 2. Glosario

| Palabra | Qué es |
| --- | --- |
| **Escena** | Un "cuarto" del juego. `LoadScene` apaga una y prende otra. |
| **`SceneManager`** | El encargado de cambiar de escena. Vive en su propia biblioteca: hay que declarar `using UnityEngine.SceneManagement;`. |
| **`LoadScene("Nombre")`** | Carga la escena que se llama **exactamente** así. Es un `string`: distingue mayúsculas. |
| **`GetActiveScene().name`** | *"dime en qué escena estoy y dame su nombre"*. Una función que **devuelve** un valor. |
| **`public string`** | Caja de **texto** en el Inspector. Cada escena puede llenarla con un valor distinto. |
| **`On Click ()`** | El evento del botón: qué **objeto** y qué **función** se llaman al apretarlo. |
| **Duplicar escena** | Una **foto** completa de la escena: objetos, posiciones y conexiones incluidas. |
| **Scene List** | La lista de escenas que conoce el build (`File → Build Profiles`). El **orden importa**: la primera abre el `.exe`. |
| **`Time.timeScale = 1f`** | Descongela el tiempo. **Siempre antes de `LoadScene`**, o la escena nueva nace muerta (clase 9). |
| **`Application.Quit()`** | Cierra el juego. Solo funciona en el `.exe`; en el editor no hace nada. |

**El nombre de una escena** es el nombre de su **archivo** en la ventana Project, sin la terminación: `Nivel2D_2.unity` se llama `Nivel2D_2`.

---

## 3. Parte A — Puesta a punto: cerrar la clase 11 ⏱️ 20 min

Las guías anteriores quedaron a medias más de una vez. Hoy eso es parte del plan: **20 minutos** para dejar el nivel jugable, no más.

### A.1 Probar y diagnosticar

1. Abre `Assets/Scenes/Nivel2D` y dale **Play**. Recorre el nivel 30 segundos.
2. Compara con esta tabla:

| Debe pasar | Si no pasa, arreglo en la [[Guía del Estudiante - Clase 11 - Nivel 2D Completo\|guía de la clase 11]] |
| --- | --- |
| Camina, salta y **cae** en los huecos | Parte A.6 / fallo F17 |
| La cámara sigue sin temblar | Parte B / fallo F20 |
| El enemigo patrulla y quita vidas | Parte C / fallos F23–F26 |
| A la tercera vida aparece **GAME OVER** congelado | Parte C.5–C.7 / fallo F26 |
| Al llegar a la meta aparece **¡GANASTE!** congelado | Parte D.1–D.4 / fallo F27 |

### A.2 La regla de los 20 minutos

1. Elige **máximo dos** cosas de la tabla para arreglar hoy, con la guía de la clase 11 abierta al lado.
2. **Obligatorio para hoy:** que el nivel se pueda **ganar** (la Meta) y **perder** (el GAME OVER). Si falta alguno de esos dos, ocupa tu cupo de arreglos en él: son la materia prima de hoy.
3. Todo lo demás (cámara, enemigo, tilemap) queda **anotado en la bitácora** como práctica 1 de la semana. No se arregla hoy.
4. Si el juego **arranca congelado**, es `Time.timeScale` en `0`: fallo F26 de la clase 11, arreglo de una línea.

**✅ Comprobación A:** el nivel **se puede ganar y se puede perder**, aunque tenga detalles pendientes. Lo que quedó pendiente está anotado en la bitácora.

---

## 4. Parte B — El menú principal ⏱️ 20 min

### B.1 La escena nueva

1. **File → New Scene** → plantilla **Basic 2D**.
2. **File → Save As…** → guarda en `Assets/Scenes` con el nombre exacto **`MenuPrincipal`**.

**✅ Comprobación B.1:** en Project existe `Assets/Scenes/MenuPrincipal.unity`. La Hierarchy tiene `Main Camera` (y una luz, que en 2D no molesta).

### B.2 El fondo

1. Selecciona **`Main Camera`** → componente **Camera** → sección **Environment**.
2. En **Background Type** elige **`Solid Color`** (en versiones viejas se llama **Clear Flags**).
3. Toca el color de **Background**: algo oscuro, a tu gusto. Es el primer diseño de pantalla "sin juego" que haces: el color manda.

**✅ Comprobación B.2:** en la vista Game se ve el color elegido, llenando toda la pantalla.

### B.3 El título

1. Clic derecho en la Hierarchy → **UI → Text - TextMeshPro**.
2. Escribe el nombre de tu juego (por ejemplo **SALTA Y RECOGE**, como tu carpeta de build de la clase 11).
3. **Font Size** `72`, color claro, centrado.
4. En el **Rect Transform**, ancla al **centro** y sube **Pos Y** a `100`.

> **Ojo:** si la primera vez que creas UI Unity pide importar TMP, ya lo hiciste en la clase 10. No debería pedirlo otra vez.

**✅ Comprobación B.3:** el título se ve centrado y arriba en el Game, sin cortarse.

### B.4 El botón JUGAR

1. Clic derecho en la Hierarchy → **UI → Button - TextMeshPro**.
2. Dentro del botón hay un hijo **Text (TMP)**: cámbiale el texto a **`JUGAR`**, **Font Size** `48`.
3. Ancla el botón al **centro**, **Pos Y** = `-50`. **Width** `400`, **Height** `100`.

**✅ Comprobación B.4:** el botón se ve debajo del título. Al crearlo aparecieron un **Canvas** y un **EventSystem** en la Hierarchy: ambos son necesarios y se crearon solos.

### B.5 El script `MenuPrincipal`

1. En **Project**, clic derecho en `Assets/Scripts` → **Create → C# Script** → nombre exacto: `MenuPrincipal`.
2. Escríbelo **completo**:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Nivel2D");
    }
}
```

3. Guarda (**Ctrl + S** / **Cmd + S**) y vuelve a Unity.

**Qué hace cada línea, literalmente:**

- `using UnityEngine.SceneManagement;` → *"déjame usar la biblioteca de cambiar de escena"*. Ya la habías usado en la clase 9 con el botón Reiniciar: los `using` son las cajas de herramientas disponibles.
- `public void Jugar()` → la función que el botón va a llamar. **Sin parámetros** y **`public`**: un botón no puede llamar a una función privada.
- `SceneManager.LoadScene("Nivel2D")` → *"carga la escena que se llama exactamente así"*. El nombre es un `string`: si escribes `nivel2d`, **no funciona y no avisa**. Es un error silencioso, como el tag mal escrito.

### B.6 El objeto que lleva el script

1. En la **Hierarchy** (panel izquierdo), clic derecho en un espacio vacío → **Create Empty**.
2. Aparecerá un objeto llamado **New GameObject**. Haz clic derecho sobre él → **Rename** (o presiona F2) y escribe: **`Menu`**.
3. Con el objeto **`Menu`** seleccionado, ve al **Inspector** → **Add Component** → busca **`MenuPrincipal`** → selecciónalo.
4. **Verificación:** en la Hierarchy debes ver el objeto **`Menu`** con el componente **MenuPrincipal** en el Inspector.

> **Por qué un objeto aparte y no el botón:** el patrón de siempre. El botón **avisa**; el script **decide**. En la clase 9 el botón `Reanudar` no hacía nada hasta que se arrastró el **objeto correcto** al evento. Hoy el objeto correcto se llama `Menu`.

### B.7 Conectar el botón

1. Selecciona el **botón** (el `Button`, no su texto) en la Hierarchy.
2. En el Inspector, sección **On Click ()** → **`+`**.
3. **Arrastra el objeto `Menu`** desde la Hierarchy y suéltalo en el recuadro que apareció.
4. En el menú desplegable de la derecha: **MenuPrincipal → Jugar**.

**✅ Comprobación B.7:** Play en `MenuPrincipal` → aprieta **JUGAR** → carga el `Nivel2D` y puedes jugar normal. Si el botón no hace nada, mira **F30**; si la Consola dice `couldn't be loaded`, mira **F31**.

---

## 5. Parte C — Los dos botones ⏱️ 30 min

Todo en esta parte se hace **en la escena `Nivel2D`** (el nivel 1), **antes** de duplicar. Así el nivel 2 hereda los dos botones ya conectados.

### C.1 Modificar `Meta.cs`

1. Abre `Meta.cs` y déjalo **exactamente así** (lo nuevo son el `using` de arriba, el `public string` y el método `SiguienteNivel`):

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    public GameObject panelVictoria;
    public string escenaSiguiente;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            panelVictoria.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSiguiente);
    }
}
```

2. Guarda y vuelve a Unity.

**Qué hace cada línea, literalmente:**

- `public string escenaSiguiente;` → una **caja de texto** en el Inspector. La idea del día: el script `Meta` es **uno solo**, pero **cada escena** llena esa caja con un destino distinto. El nivel 1 dirá `Nivel2D_2`; el nivel 2 dirá `MenuPrincipal`.
- `Time.timeScale = 1f;` **antes** de `LoadScene` → la trampa de la clase 9: la victoria congela el tiempo, y si cargas la escena nueva con el tiempo congelado, **nace muerta**. Una línea la evita.
- `public void SiguienteNivel()` → público, sin parámetros: la firma exacta que un botón puede llamar.

> **¿Por qué un botón y no un temporizador** que cambie de nivel solo a los 2 segundos? Porque `Invoke` usa el tiempo **del juego**, y el juego está **congelado** en `timeScale = 0`: nunca se dispararía. Los botones **sí** responden congelados (clase 9). Por eso el cambio de nivel es un clic.

### C.2 Llenar el campo en el nivel 1

1. Selecciona el objeto **`Meta`** de la escena.
2. En el componente **Meta (Script)** apareció el campo nuevo **Escena Siguiente**. Escribe: **`Nivel2D_2`**.

**✅ Comprobación C.2:** el campo **Escena Siguiente** dice `Nivel2D_2`. (Todavía no existe esa escena: se crea en la Parte D. Por eso **no** pruebes este botón todavía.)

### C.3 El botón SIGUIENTE NIVEL en la victoria

1. **Prende** `PanelVictoria` en la Hierarchy (casilla de activo) para poder editarlo. **Al final lo vuelves a apagar.**
2. Clic derecho sobre `PanelVictoria` → **UI → Button - TextMeshPro** → su texto: **`SIGUIENTE NIVEL`**, **Font Size** `40`, color oscuro.
3. Ancla al **centro**, **Pos Y** = `-80` (debajo del texto de ¡GANASTE!).
4. Selecciona el botón → **On Click ()** → **`+`** → **arrastra el objeto `Meta`** de la Hierarchy → menú desplegable: **Meta → SiguienteNivel**.
5. **Apaga** `PanelVictoria` otra vez.

> **Ojo:** al evento se arrastra el objeto **`Meta`**, que es el que **tiene** el script — no el botón ni el panel. Es el mismo error de la clase 9 con `Reanudar`.

**✅ Comprobación C.3:** `PanelVictoria` está apagado, pero al seleccionarlo se ve el botón **SIGUIENTE NIVEL** dentro, y su **On Click ()** dice `Meta → SiguienteNivel`.

### C.4 Modificar `Recolector2D.cs`

1. Abre `Recolector2D.cs`.
2. Arriba de todo, **junto a los `using` que ya tiene**, agrega:

```csharp
using UnityEngine.SceneManagement;
```

3. Dentro de la clase, junto a `OnCollisionEnter2D`, agrega este método **completo**:

```csharp
    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
```

4. Guarda y vuelve a Unity.

**Qué hace cada línea, literalmente:**

- `SceneManager.GetActiveScene()` → una función que **devuelve** un valor: la escena donde estás ahora. Es la primera vez que encadenas así: una función que **entrega** algo, y ese algo se usa **en el mismo renglón**.
- `.name` → de esa escena, su **nombre** (`"Nivel2D"` o `"Nivel2D_2"`, según dónde estés). También es un `string`.
- Ese string entra directo a `LoadScene(...)`. **Por eso este botón funciona en los dos niveles sin duplicar nada.**
- `Time.timeScale = 1f;` primero → misma trampa de C.1: descongelar **antes** de recargar.

### C.5 El botón REINTENTAR en el GAME OVER

1. **Prende** `PanelGameOver` para editarlo.
2. Clic derecho sobre él → **UI → Button - TextMeshPro** → texto **`REINTENTAR`**, **Font Size** `40`.
3. Ancla al **centro**, **Pos Y** = `-80` (debajo de GAME OVER).
4. Selecciona el botón → **On Click ()** → **`+`** → **arrastra `Jugador`** de la Hierarchy (¡es el objeto que tiene el `Recolector2D`!) → menú desplegable: **Recolector2D → Reintentar**.
5. **Apaga** `PanelGameOver`.

**✅ Comprobación C.5:** el botón existe dentro de `PanelGameOver` (apagado junto con el panel), y su **On Click ()** dice `Jugador → Reintentar`.

### C.6 Probar REINTENTAR

1. Guarda la escena (**Ctrl + S**).
2. Play: déjate golpear por el enemigo **tres veces**.
3. Cuando aparezca **GAME OVER**, aprieta **REINTENTAR**.

**✅ Comprobación C.6:** el nivel se recarga fresco: jugador en el inicio, **vidas de vuelta a 3**, monedas de vuelta en su sitio, juego **descongelado**. Si el nivel recarga pero **nace congelado**, mira **F33**. (Que las monedas vuelvan es **normal**: la escena vuelve al estado en que la guardaste, fallo F37.)

> El botón **SIGUIENTE NIVEL** todavía **no se prueba**: la escena `Nivel2D_2` no existe. Se prueba en la Parte E. Si lo apretás ahora, la Consola reclama que la escena no está en el build — es el fallo **F31**, y se arregla más adelante.

---

## 6. Parte D — El nivel 2 ⏱️ 15 min

### D.1 Duplicar la escena

1. Guarda `Nivel2D` (**Ctrl + S**) si no lo hiciste en C.6.
2. En **Project**, dentro de `Assets/Scenes`, clic derecho sobre el archivo **`Nivel2D`** → **Duplicate**.
3. Renombra la copia (F2): **`Nivel2D_2`**.

**Qué acabas de hacer, literalmente:** una **foto completa** de la escena: el terreno, los enemigos, las monedas, el jugador, los paneles **y las conexiones de los On Click ()**. Por eso los botones se agregaron **antes** de duplicar: ya vienen instalados en el nivel 2.

**✅ Comprobación D.1:** existe `Assets/Scenes/Nivel2D_2.unity`. Ábrela con doble clic: es idéntica al nivel 1, con sus dos botones incluidos.

### D.2 El destino de la meta del nivel 2 (el paso que todos olvidan)

1. Con **`Nivel2D_2`** abierta, selecciona el objeto **`Meta`**.
2. En el campo **Escena Siguiente**, **borra** `Nivel2D_2` y escribe: **`MenuPrincipal`**.

> **Trampa anunciada:** la copia hereda el campo del nivel 1, que decía `Nivel2D_2`. Si lo dejas así, la meta del nivel 2 te manda **al nivel 2 otra vez**, para siempre. Es el fallo **F36**.

3. *(Opcional, 2 min):* cambia los textos de esta escena para que se sienta un final: `¡GANASTE!` → **`¡TE PASASTE EL JUEGO!`**, y el texto del botón → **`VOLVER AL MENÚ`**. Estos textos viven **dentro de la escena**, así que el nivel 1 no se entera.

**✅ Comprobación D.2:** en `Nivel2D_2`, el campo **Escena Siguiente** del `Meta` dice `MenuPrincipal`.

### D.3 Rediseño exprés (elige dos, máximo)

Haz que el nivel 2 **se sienta distinto**. Elige **dos** de estas (el rediseño en serio queda para casa):

- **Mover la `Meta`** más lejos.
- **Agregar monedas** (arrastra el prefab desde `Assets/Prefabs`).
- **Duplicar el `Enemigo`** (Ctrl + D) y poner el clon en otra plataforma. Ojo: el clon patrulla los **mismos** `PuntoA` y `PuntoB` que el original; para una ruta nueva, duplica también los puntos y conecta los nuevos en el clon.
- **Abrir o tapar un hueco** en el tilemap (pincel y goma de la clase 11).

**✅ Comprobación D.3:** al abrir una y otra escena, se ven **distintas**. Si tus cambios "no aparecen", editaste la escena equivocada: mira la pestaña abierta arriba (**F35**).

### D.4 Guardar

**Ctrl + S** en `Nivel2D_2`. (Cada escena se guarda por separado: la pestaña abierta es la que se guarda.)

---

## 7. Parte E — El circuito completo y el `.exe` ⏱️ 15 min

### E.1 La lista de escenas (y su orden)

1. **File → Build Profiles** → sección **Scene List**.
2. Deben estar las cuatro: **`MenuPrincipal`**, **`Nivel2D`**, **`Nivel2D_2`** (y la escena 3D de la clase 9, si sigue en la lista).
3. Las que falten: abre la escena y usa **Add Open Scenes** (agrega la escena **abierta**).
4. **Arrastra `MenuPrincipal` al primer lugar de la lista.**

> **Por qué el orden importa:** el primer lugar de la lista es **la escena con la que abre el `.exe`**. Es la única vez en el proyecto que el orden de una lista decide el comportamiento del juego.

**✅ Comprobación E.1:** la Scene List muestra las escenas con `MenuPrincipal` arriba (índice 0), y después `Nivel2D` y `Nivel2D_2`.

### E.2 Probar el circuito en el editor

1. Abre `MenuPrincipal` → Play.
2. Recorre el circuito **entero**: **JUGAR** → llega a la meta → **¡GANASTE!** → **SIGUIENTE NIVEL** → nivel 2 → llega a la meta → **VOLVER AL MENÚ**.
3. En el nivel 2, déjate matar tres veces → **REINTENTAR** → recarga **el nivel 2** (no el 1: eso hace `GetActiveScene()`).

**✅ Comprobación E.2:** el circuito del diagrama de la sección 1 se recorre completo, sin trabarse y sin quedar congelado en ningún punto.

### E.3 El build final

1. **File → Save** (guarda la escena abierta).
2. **Build Profiles** → pestaña **Windows** → **Build** → carpeta **nueva** en el Escritorio: **`SaltaYRecoge_v2`**.
3. Espera (puede tardar unos minutos).
4. Abre el `.exe` **fuera de Unity** y recorre el circuito completo otra vez.

**✅ Comprobación E.3:** el `.exe` **abre en el menú**, el circuito se juega completo, y perder te deja reintentar. Sin Unity abierto.

**🎉 Si llegaste aquí: tu nivel de la clase 11 es ahora un juego con menú, dos niveles y derrotas que se reintentan. Ese es el ejercicio de hoy.**

---

### Parte F — Opcional (solo si te sobra tiempo)

No es obligatorio. El ejercicio ya está terminado.

**F.1 — Música en el menú (sin código, clase 9).**

1. Clic derecho → **Create Empty** → `Audio` → **Add Component → Audio Source**.
2. Arrastra un clip de música, marca **`Play On Awake`** y **`Loop`**, `Volume` ≈ `0.4`, **Spatial Blend** = 0.
3. La música del menú **se corta** al cargar el nivel: normal, cada escena tiene sus propios objetos.

**F.2 — Botón SALIR (una línea).**

1. En `MenuPrincipal.cs`, agrega:

```csharp
    public void Salir()
    {
        Application.Quit();
    }
```

2. Otro botón en el menú → texto `SALIR` → On Click () → arrastra `Menu` → **MenuPrincipal → Salir**.
3. En el editor no hace nada: pruébalo en el `.exe`.

---

## 8. Si algo falla

Busca **el síntoma que ves en pantalla**, aplica el arreglo y vuelve al paso donde estabas.

### F30 — El botón JUGAR no hace nada

1. ¿El **On Click ()** tiene el recuadro lleno con **`Menu`** y la función **`MenuPrincipal.Jugar`** seleccionada? Si el recuadro está vacío, es el cable (B.7).
2. ¿La función es **`public void`**? Un botón no llama funciones privadas.
3. ¿Escribiste `"Nivel2D"` **exactamente**? `nivel2d` o `Nivel2D ` con espacio = error silencioso.
4. ¿Hay **EventSystem** en la Hierarchy? Sin él, ningún botón de la escena responde.

### F31 — La Consola dice `Scene '...' couldn't be loaded because it has not been added to the build settings`

El nombre está bien escrito, pero la escena **no está en la Scene List**. Es el error más esperable del día:

- **File → Build Profiles → Add Open Scenes** con esa escena abierta (E.1).

### F32 — El `.exe` abre en el nivel, no en el menú

No es un error de código: es el **orden** de la Scene List.

- **Build Profiles → Scene List** → arrastra **`MenuPrincipal`** al primer lugar (E.1, paso 4) → vuelve a hacer **Build**.

### F33 — La escena nueva nace congelada

Recargaste o cambiaste de escena **con el tiempo en 0**.

- Cada función que llama `LoadScene` (`Jugar`, `SiguienteNivel`, `Reintentar`) debe empezar con `Time.timeScale = 1f;` **antes** de la línea de `LoadScene`.
- Es la misma trampa de la clase 9 (botón Reiniciar) y del F26 de la clase 11. Tercera vez que aparece: ya es tuya.

### F34 — El botón SIGUIENTE NIVEL no hace nada

1. ¿El campo **Escena Siguiente** del `Meta` está **lleno**? Si quedó vacío, `LoadScene("")` falla sin arruinar nada más.
2. ¿Dice `Nivel2D_2` **exactamente**? (mayúsculas, guion bajo, sin espacios)
3. ¿El **On Click ()** del botón tiene el objeto **`Meta`** y la función **`SiguienteNivel`**? No el botón, no el panel: el objeto `Meta`.
4. ¿`Nivel2D_2` está en la **Scene List**? (E.1 → sino, F31)

### F35 — Mis cambios en el nivel 2 no aparecen (o aparecieron en el nivel 1)

- Estabas editando **la otra escena**: las pestañas de arriba dicen cuál está abierta. En `Nivel2D_2`, el título de la pestaña dice `Nivel2D_2`.
- ¿Guardaste (**Ctrl + S**) antes de cambiar de escena? Unity pregunta al cerrar: no descartes.

### F36 — El nivel 2 se repite infinito

La meta del nivel 2 sigue cargando `Nivel2D_2` (heredado del nivel 1).

- Abre `Nivel2D_2` → objeto `Meta` → campo **Escena Siguiente** → `MenuPrincipal` (D.2).

### F37 — Situaciones que parecen errores y no lo son

| Situación | Explicación |
| --- | --- |
| Al REINTENTAR, las monedas vuelven y las vidas vuelven a 3 | Normal: `LoadScene` devuelve la escena al estado **guardado**. Nadie tiene que resetear nada a mano. |
| El botón JUGAR funciona en el editor aunque el menú no esté en la lista | Normal en el editor. El `.exe` sí necesita la escena en la lista. |
| La música del menú se corta al pasar al nivel | Normal: los objetos (y sus `AudioSource`) pertenecen a **su** escena. |
| `Application.Quit()` no cierra nada en el editor | Normal: solo funciona en el `.exe`. |
| Al duplicar la escena, TODO se duplicó (hasta las monedas) | Normal: es una foto completa. Eso era la idea. |
| El nivel 2 se ve igual al nivel 1 al principio | Correcto: así empieza. Se diferencia en D.3. |

### Prueba de aislamiento (si nada de lo anterior funcionó)

1. Abre `MenuPrincipal` → Play → aprieta JUGAR.
   - **Si carga el nivel** → el menú está bien: el problema está en `Meta` o en `Recolector2D` (F33, F34).
   - **Si no carga nada y la Consola está limpia** → es el cable del botón (F30) o falta el **EventSystem**.
2. En el nivel 1, muere tres veces → REINTENTAR.
   - **Si recarga bien** → `Recolector2D` está bien: el problema es la Meta (F34).
3. **Lee la Consola primero, siempre.** Hoy casi todos los fallos dejan mensaje (`couldn't be loaded…`, `NullReferenceException` en `Meta.SiguienteNivel`): el mensaje nombra la escena o el campo exacto que falta.

---

## 9. Fin

**Criterios de terminación.** Marca las **9** casillas. Todas deben cumplirse.

**El menú (Parte B):**

- [ ] `MenuPrincipal` existe, con título y botón **JUGAR**.
- [ ] El botón carga `Nivel2D`.

**El flujo (Partes C y D):**

- [ ] En el nivel 1, la victoria muestra el botón **SIGUIENTE NIVEL** y carga `Nivel2D_2`.
- [ ] `Nivel2D_2` existe y se ve **distinto** al nivel 1.
- [ ] En el nivel 2, la meta devuelve al **menú**.
- [ ] El GAME OVER muestra **REINTENTAR**, que recarga el nivel **actual** y funciona en **los dos** niveles.
- [ ] Ninguna escena nueva nace **congelada**.

**El producto (Parte E):**

- [ ] La Scene List tiene las escenas con `MenuPrincipal` **en el primer lugar**.
- [ ] El `.exe` abre en el **menú** y el circuito se juega completo, sin Unity.

**Si las 9 están marcadas: terminaste el ejercicio de hoy.**

---

## 10. Bitácora y práctica de la semana

Guarda (**Ctrl + S** / **Cmd + S**) y anota en tu bitácora:

- **Qué quedó pendiente** de la Parte A (de hoy o de la clase 11): es lo primero de la práctica.
- **Qué error te salió** y **cómo lo encontraste** (la Consola, hoy, nombra escenas y campos exactos).
- **El nombre exacto** que escribiste en cada campo `Escena Siguiente`, y por qué cada nivel tiene el suyo.

**Práctica diaria (15–20 min):**

1. **Cerrar lo pendiente:** lo anotado en la Parte A de hoy (y de la clase 11, si quedó algo).
2. **Rediseñar el nivel 2 de verdad:** más difícil que el 1. Al menos **dos enemigos**, una sección difícil nueva, y un hueco que exija un salto preciso.
3. **Botón SALIR** en el menú con `Application.Quit()` (Parte F.2), probado en el `.exe`.
4. **Música en el menú** (Parte F.1, sin código).
5. **Repaso:** leer en voz alta `MenuPrincipal`, `Meta` y el `Reintentar` de `Recolector2D`, explicando **cada línea**. Si puedes explicar `SceneManager.LoadScene(SceneManager.GetActiveScene().name)` sin mirar, entendiste el día.

---

**Cuando termines:** guarda todo, prueba el `.exe` una vez más de punta a punta, y revisa que el juego 3D de la clase 9 siga intacto. Hoy escribiste **una** línea de lógica nueva por botón y conectaste el resto: un script compartido por dos escenas que **cada una configura distinto**. Eso es reutilizar sistemas en vez de repetir código — y un circuito jugable de menú, dos niveles y reintentos es, por primera vez, **un juego** entregable de punta a punta.
