# Atrapa y Esquiva

Micro-juego 3D en Unity donde controlas un cubo que **recoge monedas** y **esquiva obstáculos**. El escenario es infinito: los objetos aparecen alrededor del jugador, la dificultad sube con el tiempo y la partida termina cuando te quedas sin vidas.

Proyecto desarrollado a partir de las guías de las clases 7, 8, 8.1 y 9 de **Programación I (UNAB, 2026)**.

- **Autor:** Simón Tapia
- **Motor:** Unity 6000.5.8f1 (URP)
- **Escena principal:** `Assets/Scenes/SampleScene.unity`

---

## Controles

| Acción | Tecla |
|---|---|
| Moverse | `W` `A` `S` `D` o flechas |
| Pausar / reanudar | `P` |
| Reiniciar (tras GAME OVER) | Botón **REINICIAR** |
| Reanudar desde la pausa | Botón **REANUDAR** |

## Reglas del juego

| Evento | Efecto |
|---|---|
| Recoger una moneda | **+10 puntos** (y suena un blip) |
| Chocar con un obstáculo | **−5 puntos** (nunca baja de 0) y **−1 vida** |
| Quedarse sin vidas | **GAME OVER**, se detiene el juego y aparece el botón de reinicio |
| Tiempo entre apariciones | Empieza en 2 s y baja 5 % por ciclo, con un mínimo de 1.2 s |
| Duración de los objetos | Se destruyen solos a los 10 s |

Los objetos nunca aparecen a menos de 3 unidades del jugador.

---

## Estructura del proyecto

### Scripts de juego (`Assets/`)

| Script | Responsabilidad |
|---|---|
| `JugadorMovimiento.cs` | Lee el teclado y traslada al jugador. Se bloquea cuando `gameOver` es verdadero. |
| `Recolector.cs` | Lleva puntos, vidas y `gameOver`; detecta monedas (`OnTriggerEnter`) y obstáculos (`OnCollisionEnter`); reproduce los efectos de sonido. |
| `GeneradorDeObjetos.cs` | Instancia monedas y obstáculos alrededor del jugador, ajusta la dificultad y destruye los clones viejos. Se detiene en GAME OVER. |
| `UIManager.cs` | Actualiza el HUD (puntos y vidas), muestra GAME OVER y el botón de reinicio, y maneja la pausa con `Time.timeScale`. |

Las referencias entre scripts se asignan en el Inspector; no se usa `FindObjectOfType` ni `GameObject.Find` en tiempo de ejecución.

### Escena (`Assets/Scenes/SampleScene.unity`)

```
SampleScene
├── Main Camera            (0, 12, -12) · rotación X 45°
├── Directional Light
├── Piso                   Plano del escenario
├── Jugador                Cubo + Rigidbody + JugadorMovimiento + Recolector
├── Generador              GeneradorDeObjetos
├── Audio                  2 × AudioSource (música en loop + efectos)
├── Canvas                 HUD y menús
│   ├── TextoPuntos        "Puntos: 0"
│   ├── TextoVidas         "Vidas: 3"
│   ├── TextoGameOver      "GAME OVER" (oculto al inicio)
│   ├── BotonReiniciar     "REINICIAR" (oculto al inicio)
│   └── PauseMenu          Fondo + "PAUSA" + BotonReanudar
├── EventSystem            InputSystemUIInputModule
└── UI                     UIManager con todas las referencias conectadas
```

El `Rigidbody` del jugador tiene congelados la posición en Y y las tres rotaciones para que no vuelque al chocar.

### Assets

| Carpeta | Contenido |
|---|---|
| `Assets/Prefabs/` | `Moneda.prefab` (tag `Moneda`, collider trigger) y `Obstaculo.prefab` (tag `Obstaculo`, collider sólido) |
| `Assets/Materiales/` | `Moneda.mat`, `Obstaculo.mat`, `Jugador.mat` (shaders de URP) |
| `Assets/Audio/` | `musica.wav`, `moneda.wav`, `choque.wav`, `gameover.wav` (generados por código, sin assets externos) |
| `Assets/Editor/` | `ConfigurarAtrapa.cs` — constructor de la escena con menú propio |

### Constructor de escena

`Assets/Editor/ConfigurarAtrapa.cs` reconstruye la escena completa usando las APIs de Unity (materiales, prefabs, tags, canvas, eventos de botones y referencias). Se puede ejecutar cuando se quiera desde:

```
Tools → Atrapa → Construir escena completa
```

También se ejecuta automáticamente si detecta que la escena no está construida.

---

## Cómo abrir y jugar

1. Abre el proyecto con **Unity 6000.5.8f1** (Unity Hub → *Add project from disk* → carpeta `Atrapa`).
2. En el panel **Project**, abre `Assets/Scenes/SampleScene.unity`.
3. Pulsa **Play**.
4. Muévete con `WASD` o las flechas, recoge monedas y esquiva los cubos rojos.

El HUD muestra los puntos y las vidas en pantalla; al perder la tercera vida aparece **GAME OVER** y el botón **REINICIAR**.

## Cómo generar el ejecutable

1. `File → Build Profiles` (en versiones anteriores: *Build Settings*).
2. En **Scene List** confirma que está `Assets/Scenes/SampleScene.unity` (si no, **Add Open Scenes**).
3. Elige la plataforma **Windows** (o macOS) → **Build**.
4. Selecciona una **carpeta nueva fuera del proyecto** (por ejemplo, en el Escritorio).

La escena debe estar en la *Scene List* porque es la misma lista que usa `SceneManager.LoadScene` en el botón **REINICIAR**.

---

## Detalles de implementación

- **Input compatible con ambos sistemas:** los scripts compilan con el *Input System* nuevo (`ENABLE_INPUT_SYSTEM`) y con el *Input Manager* clásico (`Input.GetAxisRaw`), usando `#if` para elegir. El proyecto usa el Input System nuevo.
- **Pausa:** `UIManager` alterna `Time.timeScale` entre `0` y `1` solo al pausar o reanudar; el resto del juego no lo toca.
- **Candado de GAME OVER:** jugador y generador se detienen, y `Recolector` ignora colisiones posteriores.
- **Dificultad progresiva:** el intervalo del generador se multiplica por `0.95` en cada ciclo.
- **Audio:** los cuatro `.wav` se generaron por síntesis (no hay assets de terceros); la música se reproduce en loop desde el inicio.

## Verificación realizada

Prueba automatizada en Play Mode sobre la escena final:

- HUD inicial correcto: `Puntos: 0`, `Vidas: 3`, GAME OVER y botón ocultos.
- Movimiento con tecla real: desplazamiento de 5.03 unidades.
- Tres choques: `vidas: 0`, `gameOver: True`, GAME OVER y botón **REINICIAR** visibles.
- Reinicio: vuelve a `Puntos: 0`, `Vidas: 3`, `gameOver: False`.
- Generación de monedas y obstáculos activa, con puntos sumando correctamente.
- **0 errores y 0 excepciones** durante la partida.

## Problemas frecuentes

| Síntoma | Causa / solución |
|---|---|
| No veo los objetos en la Hierarchy | La escena no está abierta: doble clic en `Assets/Scenes/SampleScene.unity`. |
| El botón no responde | Falta el `EventSystem` o el módulo de input de la UI. La escena ya lo incluye. |
| `Scene 'SampleScene' couldn't be loaded` | La escena no está en la *Scene List* de Build Profiles. |
| Los textos se ven vacíos | No están importados los **TMP Essential Resources** (`Window → TextMeshPro → Import TMP Essential Resources`). |
| No se mueve con el teclado | La ventana **Game** debe tener el foco al pulsar Play. |
