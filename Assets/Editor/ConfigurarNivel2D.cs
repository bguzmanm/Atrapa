using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Construye la escena Nivel2D completa:
//   Clase 10: movimiento + salto, Animator, monedas y HUD.
//   Clase 11: terreno con Tilemap sólido, cámara que sigue, enemigo que
//             patrulla, vidas/respawn/game over y meta con victoria.
//
// Se ejecuta sola si la escena no existe, o si existe el archivo
// Temp/atrapa_nivel2d.flag (para reconstruir a propósito).
// También está en Tools > Atrapa > Construir Nivel 2D.
//
// No toca la escena 3D (SampleScene): todo lo nuevo vive en Nivel2D.
public static class ConfigurarNivel2D
{
    const string EscenaPath = "Assets/Scenes/Nivel2D.unity";
    const string LogPath = "/tmp/atrapa_nivel2d.log";
    const string MarcaReconstruccion = "Temp/atrapa_nivel2d.flag";

    const string CarpetaArte = "Assets/Arte2D";
    const string CarpetaAnim = "Assets/Animations";
    const string CarpetaPrefabs = "Assets/Prefabs";
    const string CarpetaTiles = "Assets/Tiles";

    const string RutaIdle = "Assets/Animations/Idle.anim";
    const string RutaRun = "Assets/Animations/Run.anim";
    const string RutaControlador = "Assets/Animations/Jugador2D.controller";
    const string RutaMoneda = "Assets/Prefabs/Moneda2D.prefab";
    const string RutaMaterial = "Assets/Materiales/Sprite2D.mat";
    const string RutaTileCesped = "Assets/Tiles/TileCesped.asset";
    const string RutaTileTierra = "Assets/Tiles/TileTierra.asset";

    const string ParamCorriendo = "corriendo";
    const string ParamEnElPiso = "enElPiso";

    // Limites del nivel (en celdas del Tilemap).
    const int NivelIzquierda = -20;
    const int NivelDerecha = 20;
    const int TopePiso = -4;        // la celda de arriba del piso: el piso mide 1 m
    const int TopePlataforma = -3;  // 1 m mas arriba que el piso

    // Paleta pixel art
    static readonly Color32 Tinta = new Color32(28, 43, 82, 255);
    static readonly Color32 Cuerpo = new Color32(74, 134, 250, 255);
    static readonly Color32 CuerpoBrillo = new Color32(126, 178, 255, 255);
    static readonly Color32 Blanco = new Color32(255, 255, 255, 255);
    static readonly Color32 Negro = new Color32(24, 24, 38, 255);
    static readonly Color32 Zapato = new Color32(38, 64, 130, 255);
    static readonly Color32 Cesped = new Color32(76, 175, 80, 255);
    static readonly Color32 CespedClaro = new Color32(120, 210, 110, 255);
    static readonly Color32 Tierra = new Color32(107, 74, 47, 255);
    static readonly Color32 TierraOscura = new Color32(84, 57, 36, 255);
    static readonly Color32 Oro = new Color32(255, 210, 63, 255);
    static readonly Color32 OroOscuro = new Color32(196, 143, 20, 255);
    static readonly Color32 OroBrillo = new Color32(255, 238, 150, 255);
    static readonly Color32 Rojo = new Color32(220, 70, 90, 255);
    static readonly Color32 RojoOscuro = new Color32(150, 35, 55, 255);
    static readonly Color32 RojoClaro = new Color32(255, 120, 130, 255);
    static readonly Color32 Verde = new Color32(60, 200, 110, 255);
    static readonly Color32 VerdeOscuro = new Color32(30, 140, 70, 255);
    static readonly Color32 Metal = new Color32(200, 205, 220, 255);

    [InitializeOnLoadMethod]
    static void AutoEjecutar()
    {
        EditorApplication.delayCall += IntentarAutoBuild;
    }

    static void IntentarAutoBuild()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += IntentarAutoBuild;
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (File.Exists(MarcaReconstruccion))
            {
                EscribirLog("Saliendo de Play para reconstruir Nivel2D...\n");
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += IntentarAutoBuild;
                return;
            }

            EscribirLog("Unity está en Play. Sal de Play y usa Tools > Atrapa > Construir Nivel 2D.\n");
            return;
        }

        bool forzado = File.Exists(MarcaReconstruccion);

        if (!forzado && File.Exists(EscenaPath))
        {
            EscribirLog("La escena Nivel2D ya existe. No se reconstruye.\n");
            return;
        }

        if (forzado)
        {
            try { File.Delete(MarcaReconstruccion); } catch { }
        }

        Build();
    }

    [MenuItem("Tools/Atrapa/Construir Nivel 2D", priority = 1)]
    public static void Build()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Sal de Play antes de reconstruir Nivel2D.");
            return;
        }

        var log = new StringBuilder();
        log.AppendLine("== Construyendo Nivel2D completo (clases 10 y 11) ==");

        try
        {
            AssetDatabase.Refresh();
            AsegurarCarpetas(log);
            AsegurarTags(log, "Piso", "Enemigo", "Meta");
            AsegurarSortingLayers(log);

            // ---------- Sprites (pixel art generado, PPU 32 / 16) ----------
            Sprite[] idle = new Sprite[3];
            for (int i = 0; i < idle.Length; i++)
            {
                int bob = (i == 1) ? 1 : 0;
                bool parpadeo = (i == 2);
                idle[i] = GuardarSprite(
                    CarpetaArte + "/jugador_idle_" + i + ".png",
                    Personaje(bob, -1, parpadeo), 32);
            }

            Sprite[] run = new Sprite[6];
            int[] bobRun = { 0, 1, 0, 0, 1, 0 };
            for (int i = 0; i < run.Length; i++)
            {
                run[i] = GuardarSprite(
                    CarpetaArte + "/jugador_run_" + i + ".png",
                    Personaje(bobRun[i], i, false), 32);
            }

            Sprite piso = GuardarSprite(CarpetaArte + "/piso.png", Piso(), 32);
            Sprite tierra = GuardarSprite(CarpetaArte + "/tierra.png", TierraSola(), 32);
            Sprite moneda = GuardarSprite(CarpetaArte + "/moneda.png", Moneda(), 32);
            Sprite nube = GuardarSprite(CarpetaArte + "/nube.png", Nube(), 16);
            Sprite enemigo = GuardarSprite(CarpetaArte + "/enemigo.png", Enemigo(), 32);
            Sprite meta = GuardarSprite(CarpetaArte + "/meta.png", Bandera(), 32);
            log.AppendLine("Sprites generados: " + (idle.Length + run.Length + 6));

            Material material = MaterialSprite(log);
            GuardarTile(RutaTileCesped, piso);
            GuardarTile(RutaTileTierra, tierra);
            log.AppendLine("Tiles: " + RutaTileCesped + ", " + RutaTileTierra);

            // ---------- Animaciones ----------
            AnimationClip clipIdle = CrearClip("Idle", idle, 4f);
            AnimationClip clipRun = CrearClip("Run", run, 12f);

            // ---------- Animator: dos estados + dos bool ----------
            AnimatorController controlador = CrearControlador(clipIdle, clipRun);
            log.AppendLine("Animator: Idle/Run + parámetros 'corriendo' y 'enElPiso'.");

            // ---------- Escena ----------
            // No se guarda la escena abierta: podría ser la 3D (SampleScene).
            var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ---------- Fondo ----------
            // La cámara mide 5 unidades hacia arriba desde el jugador + 1, así que
            // el borde superior queda en y ≈ 3.6. Las nubes se quedan por debajo.
            CrearDecoracion("Nube1", nube, material, new Vector3(-16f, 1.9f, 0f), new Vector3(1.6f, 1.2f, 1f), "Fondo", -10);
            CrearDecoracion("Nube2", nube, material, new Vector3(-10f, 2.7f, 0f), new Vector3(1.2f, 0.9f, 1f), "Fondo", -10);
            CrearDecoracion("Nube3", nube, material, new Vector3(-4f, 1.7f, 0f), new Vector3(1.1f, 0.8f, 1f), "Fondo", -10);
            CrearDecoracion("Nube4", nube, material, new Vector3(2f, 2.4f, 0f), new Vector3(1.4f, 1.0f, 1f), "Fondo", -10);
            CrearDecoracion("Nube5", nube, material, new Vector3(8f, 1.9f, 0f), new Vector3(1.7f, 1.2f, 1f), "Fondo", -10);
            CrearDecoracion("Nube6", nube, material, new Vector3(14f, 2.8f, 0f), new Vector3(1.2f, 0.9f, 1f), "Fondo", -10);
            CrearDecoracion("Nube7", nube, material, new Vector3(19f, 1.8f, 0f), new Vector3(1.5f, 1.1f, 1f), "Fondo", -10);

            // ---------- Terreno con Tilemap ----------
            var gridGO = new GameObject("Grid", typeof(Grid));
            var terrenoGO = new GameObject("Terreno");
            // El Tilemap necesita un Grid ya asignado cuando se crea; si se
            // agrega antes de emparentarlo, SetTile puede quedar en silencio.
            terrenoGO.transform.SetParent(gridGO.transform);
            terrenoGO.AddComponent<Tilemap>();
            terrenoGO.AddComponent<TilemapRenderer>();
            terrenoGO.tag = "Piso";

            var tilemap = terrenoGO.GetComponent<Tilemap>();
            var tilemapRenderer = terrenoGO.GetComponent<TilemapRenderer>();
            tilemapRenderer.sharedMaterial = material;
            tilemapRenderer.sortingLayerName = "Objetos";
            tilemapRenderer.sortingOrder = 0;

            // El orden importa: primero el collider de tiles, luego el cuerpo
            // estático y por último el Composite que los fusiona.
            var colTilemap = terrenoGO.AddComponent<TilemapCollider2D>();
            colTilemap.compositeOperation = Collider2D.CompositeOperation.Merge;

            var rbTerreno = terrenoGO.AddComponent<Rigidbody2D>();
            rbTerreno.bodyType = RigidbodyType2D.Static;

            var composite = terrenoGO.AddComponent<CompositeCollider2D>();
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            composite.generationType = CompositeCollider2D.GenerationType.Synchronous;

            int tilesPintados = PintarTerreno(tilemap);
            tilemap.RefreshAllTiles();
            tilemap.CompressBounds();

            int tilesUsados = tilemap.GetUsedTilesCount();
            if (tilesUsados == 0)
                throw new System.Exception("El Tilemap quedó vacío: SetTile no pintó nada.");

            // El TilemapCollider2D arma sus formas en un paso aparte; sin esto
            // el Composite se genera antes de que existan y queda vacío.
            colTilemap.ProcessTilemapChanges();
            composite.GenerateGeometry();

            log.AppendLine("Tilemap: " + tilesPintados + " tiles pintados, " +
                           tilesUsados + " tiles usados, composite pathCount=" +
                           composite.pathCount + ".");

            // Paredes invisibles para que el nivel tenga bordes.
            CrearPared("ParedIzquierda", NivelIzquierda - 0.5f);
            CrearPared("ParedDerecha", NivelDerecha + 1.5f);

            // ---------- Jugador ----------
            var jugadorGO = new GameObject("Jugador");
            jugadorGO.tag = "Player";
            jugadorGO.transform.position = new Vector3(-18f, -2.4f, 0f);

            var srJugador = jugadorGO.AddComponent<SpriteRenderer>();
            srJugador.sprite = idle[0];
            srJugador.sharedMaterial = material;
            srJugador.sortingLayerName = "Personajes";
            srJugador.sortingOrder = 10;

            var rb = jugadorGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var colJugador = jugadorGO.AddComponent<BoxCollider2D>();
            colJugador.size = new Vector2(0.6f, 0.8f);
            colJugador.offset = new Vector2(0f, -0.02f);

            var animador = jugadorGO.AddComponent<Animator>();
            animador.runtimeAnimatorController = controlador;
            animador.applyRootMotion = false;

            var movimiento = jugadorGO.AddComponent<MovimientoJugador2D>();
            movimiento.velocidad = 6f;
            // Con 8 el salto llegaba a 1.09 m y el enemigo ocupaba ~0.9 m:
            // no había margen. Con 10 llega a ~1.70 m y se pasa con holgura.
            movimiento.fuerzaSalto = 10f;
            movimiento.anim = animador;
            movimiento.sprite = srJugador;

            var recolector = jugadorGO.AddComponent<Recolector2D>();
            recolector.valorMoneda = 10;
            recolector.vidas = 3;

            // Punto de inicio para el respawn.
            var inicioGO = new GameObject("Inicio");
            inicioGO.transform.position = jugadorGO.transform.position;
            recolector.puntoInicio = inicioGO.transform;

            // ---------- Enemigo que patrulla ----------
            // Patrulla corta y pegada al inicio: el salto cubre 4.08 m, así que
            // una zona de 2 m garantiza que se aterrice fuera de su alcance.
            // Con la zona de 5 m anterior se caía encima del slime casi siempre.
            var puntoAGO = new GameObject("PuntoA");
            puntoAGO.transform.position = new Vector3(-16.5f, -2.6f, 0f);

            var puntoBGO = new GameObject("PuntoB");
            puntoBGO.transform.position = new Vector3(-14.5f, -2.6f, 0f);

            var enemigoGO = new GameObject("Enemigo");
            enemigoGO.tag = "Enemigo";
            enemigoGO.transform.position = puntoAGO.transform.position;
            // Slime más bajo que el jugador: con el salto de 10, los pies pasan
            // el collider con margen incluso si el salto no es perfecto.
            enemigoGO.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            var srEnemigo = enemigoGO.AddComponent<SpriteRenderer>();
            srEnemigo.sprite = enemigo;
            srEnemigo.sharedMaterial = material;
            srEnemigo.sortingLayerName = "Personajes";
            srEnemigo.sortingOrder = 9;

            var rbEnemigo = enemigoGO.AddComponent<Rigidbody2D>();
            rbEnemigo.bodyType = RigidbodyType2D.Kinematic;
            rbEnemigo.gravityScale = 0f;
            rbEnemigo.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbEnemigo.interpolation = RigidbodyInterpolation2D.Interpolate;

            var colEnemigo = enemigoGO.AddComponent<BoxCollider2D>();
            colEnemigo.size = new Vector2(0.8f, 0.8f);

            var patrulla = enemigoGO.AddComponent<EnemigoPatrulla>();
            patrulla.puntoA = puntoAGO.transform;
            patrulla.puntoB = puntoBGO.transform;
            patrulla.velocidad = 1.5f;
            patrulla.sprite = srEnemigo;

            // ---------- Prefab de moneda ----------
            var monedaGO = new GameObject("Moneda");
            monedaGO.tag = "Moneda";
            var srMoneda = monedaGO.AddComponent<SpriteRenderer>();
            srMoneda.sprite = moneda;
            srMoneda.sharedMaterial = material;
            srMoneda.sortingLayerName = "Objetos";
            srMoneda.sortingOrder = 5;

            var colMoneda = monedaGO.AddComponent<CircleCollider2D>();
            colMoneda.isTrigger = true;
            colMoneda.radius = 0.25f;

            GameObject prefabMoneda = PrefabUtility.SaveAsPrefabAsset(monedaGO, RutaMoneda);
            Object.DestroyImmediate(monedaGO);
            log.AppendLine("Prefab: " + RutaMoneda);

            // ---------- Monedas en la escena ----------
            var monedas = new GameObject("Monedas");
            Vector3[] posiciones =
            {
                new Vector3(-17f, -2.2f, 0f),
                new Vector3(-13.5f, -2.2f, 0f),
                new Vector3(-10.5f, -2.2f, 0f),
                new Vector3(-4f, -2.2f, 0f),
                new Vector3(0f, -2.2f, 0f),
                new Vector3(2f, -1.2f, 0f),
                new Vector3(5f, -2.2f, 0f),
                new Vector3(9.5f, -2.2f, 0f),
                new Vector3(12.5f, -1.2f, 0f),
                new Vector3(17f, -2.2f, 0f)
            };

            foreach (Vector3 posicion in posiciones)
            {
                var copia = (GameObject)PrefabUtility.InstantiatePrefab(prefabMoneda);
                copia.transform.SetParent(monedas.transform, false);
                copia.transform.position = posicion;
            }

            // ---------- Meta ----------
            var metaGO = new GameObject("Meta");
            metaGO.tag = "Meta";
            metaGO.transform.position = new Vector3(18.5f, -2f, 0f);

            var srMeta = metaGO.AddComponent<SpriteRenderer>();
            srMeta.sprite = meta;
            srMeta.sharedMaterial = material;
            srMeta.sortingLayerName = "Objetos";
            srMeta.sortingOrder = 6;

            var colMeta = metaGO.AddComponent<BoxCollider2D>();
            colMeta.isTrigger = true;
            colMeta.size = new Vector2(1f, 2f);

            // ---------- HUD ----------
            var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            TMP_Text textoPuntos = CrearTextoHUD(canvasGO.transform, "TextoPuntos",
                "Puntos: 0", TextAlignmentOptions.TopLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(0f, 1f), new Vector2(30f, -30f));

            TMP_Text textoVidas = CrearTextoHUD(canvasGO.transform, "TextoVidas",
                "Vidas: 3", TextAlignmentOptions.TopRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(1f, 1f), new Vector2(-30f, -30f));

            GameObject panelGameOver = CrearPanel(canvasGO.transform, "PanelGameOver",
                "GAME OVER", new Color(0.12f, 0.01f, 0.03f, 0.82f), new Color32(255, 120, 130, 255));

            GameObject panelVictoria = CrearPanel(canvasGO.transform, "PanelVictoria",
                "¡GANASTE!", new Color(0.01f, 0.10f, 0.05f, 0.82f), new Color32(150, 255, 180, 255));

            recolector.textoPuntos = textoPuntos;
            recolector.textoVidas = textoVidas;
            recolector.panelGameOver = panelGameOver;

            var scriptMeta = metaGO.AddComponent<Meta>();
            scriptMeta.panelVictoria = panelVictoria;

            // ---------- Cámara ----------
            var camaraGO = new GameObject("Main Camera");
            camaraGO.tag = "MainCamera";

            var camara = camaraGO.AddComponent<Camera>();
            camara.orthographic = true;
            camara.orthographicSize = 5f;
            camara.clearFlags = CameraClearFlags.SolidColor;
            camara.backgroundColor = new Color(0.09f, 0.12f, 0.20f);
            camaraGO.AddComponent<AudioListener>();

            var seguir = camaraGO.AddComponent<CamaraSigue>();
            seguir.objetivo = jugadorGO.transform;
            seguir.suavizado = 5f;
            seguir.desfase = new Vector3(0f, 1f, -10f);
            camaraGO.transform.position = jugadorGO.transform.position + seguir.desfase;

            // ---------- Guardar escena ----------
            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena, EscenaPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // La escena 2D se agrega a Build Profiles sin sacar la 3D.
            AgregarAEscenasDeBuild(EscenaPath);

            EscribirLog(log.ToString() + "LISTO: escena guardada en " + EscenaPath + "\n");
            Debug.Log("Nivel2D: escena completa construida y guardada.");
        }
        catch (System.Exception e)
        {
            EscribirLog(log.ToString() + "ERROR: " + e + "\n");
            Debug.LogError("Nivel2D: falló la construcción -> " + e);
        }
    }

    // ---------------- utilidades ----------------

    static void EscribirLog(string texto)
    {
        try { File.WriteAllText(LogPath, texto); } catch { }
        Debug.Log("Nivel2D log: " + texto);
    }

    static void AsegurarCarpetas(StringBuilder log)
    {
        foreach (var carpeta in new[] { CarpetaArte, CarpetaAnim, CarpetaPrefabs, CarpetaTiles })
        {
            if (AssetDatabase.IsValidFolder(carpeta)) continue;

            string padre = Path.GetDirectoryName(carpeta).Replace('\\', '/');
            string nombre = Path.GetFileName(carpeta);
            AssetDatabase.CreateFolder(padre, nombre);
            log.AppendLine("Carpeta creada: " + carpeta);
        }
    }

    static void AsegurarTags(StringBuilder log, params string[] tags)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        var so = new SerializedObject(asset);
        var lista = so.FindProperty("tags");

        foreach (var tag in tags)
        {
            bool existe = false;
            for (int i = 0; i < lista.arraySize; i++)
            {
                if (lista.GetArrayElementAtIndex(i).stringValue == tag) { existe = true; break; }
            }

            if (existe) continue;

            lista.InsertArrayElementAtIndex(lista.arraySize);
            lista.GetArrayElementAtIndex(lista.arraySize - 1).stringValue = tag;
            log.AppendLine("Tag creado: " + tag);
        }

        so.ApplyModifiedProperties();
    }

    // Crea Fondo, Objetos y Personajes en ese orden. En Unity, las capas de
    // arriba se dibujan primero: Fondo queda detrás y Personajes adelante.
    static void AsegurarSortingLayers(StringBuilder log)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        var so = new SerializedObject(asset);
        var lista = so.FindProperty("m_SortingLayers");

        var deseadas = new[]
        {
            new KeyValuePair<string, int>("Fondo", 1001),
            new KeyValuePair<string, int>("Objetos", 1002),
            new KeyValuePair<string, int>("Personajes", 1003)
        };

        foreach (var capa in deseadas)
        {
            bool existe = false;
            for (int i = 0; i < lista.arraySize; i++)
            {
                var nombre = lista.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                if (nombre == capa.Key) { existe = true; break; }
            }

            if (existe) continue;

            int indice = (capa.Key == "Fondo") ? 0 : lista.arraySize;
            lista.InsertArrayElementAtIndex(indice);

            var elemento = lista.GetArrayElementAtIndex(indice);
            elemento.FindPropertyRelative("name").stringValue = capa.Key;
            elemento.FindPropertyRelative("uniqueID").intValue = capa.Value;
            elemento.FindPropertyRelative("locked").boolValue = false;

            log.AppendLine("Sorting Layer creada: " + capa.Key);
        }

        so.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();

        // Fuerza a Unity a releer la lista antes de asignar sortingLayerName
        // a los renderers de la escena.
        AssetDatabase.ImportAsset("ProjectSettings/TagManager.asset",
            ImportAssetOptions.ForceUpdate);
    }

    static Material MaterialSprite(StringBuilder log)
    {
        var existente = AssetDatabase.LoadAssetAtPath<Material>(RutaMaterial);
        if (existente != null) return existente;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Transparent");

        var material = new Material(shader);
        material.name = "Sprite2D";
        AssetDatabase.CreateAsset(material, RutaMaterial);
        log.AppendLine("Material de sprites listo.");
        return material;
    }

    [MenuItem("Tools/Atrapa/Agregar Nivel 2D a las escenas de build", priority = 2)]
    public static void AgregarNivel2DABuild()
    {
        AgregarAEscenasDeBuild(EscenaPath);
        EscribirLog("Escenas de build: " + string.Join(", ",
            EditorBuildSettings.scenes.Select(e => e.path).ToArray()) + "\n");
    }

    static void AgregarAEscenasDeBuild(string ruta)
    {
        // En Unity 6 las escenas globales se leen con globalScenes.
        // EditorBuildSettings.scenes puede apuntar a un perfil de build.
        var escenas = EditorBuildSettings.globalScenes.ToList();

        if (!escenas.Any(e => e.path == ruta))
        {
            escenas.Add(new EditorBuildSettingsScene(ruta, true));
            EditorBuildSettings.globalScenes = escenas.ToArray();
        }

        // Si el perfil activo tiene su propia lista, también se agrega ahí.
        var perfil = UnityEditor.Build.Profile.BuildProfile.GetActiveBuildProfile();
        if (perfil != null && perfil.overrideGlobalScenes
            && !perfil.scenes.Any(e => e.path == ruta))
        {
            var delPerfil = perfil.scenes.ToList();
            delPerfil.Add(new EditorBuildSettingsScene(ruta, true));
            perfil.scenes = delPerfil.ToArray();
            EditorUtility.SetDirty(perfil);
        }

        AssetDatabase.SaveAssets();
    }

    static void CrearDecoracion(string nombre, Sprite sprite, Material material,
                                Vector3 posicion, Vector3 escala,
                                string capa, int orden)
    {
        var go = new GameObject(nombre);
        go.transform.position = posicion;
        go.transform.localScale = escala;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sharedMaterial = material;
        sr.sortingLayerName = capa;
        sr.sortingOrder = orden;
    }

    static void CrearPared(string nombre, float x)
    {
        var go = new GameObject(nombre);
        go.transform.position = new Vector3(x, 0f, 0f);

        var col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 20f);
    }

    static TMP_Text CrearTextoHUD(Transform padre, string nombre, string contenido,
                                  TextAlignmentOptions alineacion,
                                  Vector2 anclaMin, Vector2 anclaMax,
                                  Vector2 pivote, Vector2 posicion)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);

        var texto = go.AddComponent<TextMeshProUGUI>();
        TMP_FontAsset fuente = TMP_Settings.defaultFontAsset;
        if (fuente != null) texto.font = fuente;

        texto.text = contenido;
        texto.fontSize = 40f;
        texto.color = Color.white;
        texto.alignment = alineacion;

        var rt = texto.rectTransform;
        rt.anchorMin = anclaMin;
        rt.anchorMax = anclaMax;
        rt.pivot = pivote;
        rt.anchoredPosition = posicion;
        rt.sizeDelta = new Vector2(500f, 80f);

        return texto;
    }

    static GameObject CrearPanel(Transform padre, string nombre, string mensaje,
                                 Color fondo, Color colorTexto)
    {
        var panel = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(padre, false);

        var rtPanel = panel.GetComponent<RectTransform>();
        rtPanel.anchorMin = Vector2.zero;
        rtPanel.anchorMax = Vector2.one;
        rtPanel.offsetMin = Vector2.zero;
        rtPanel.offsetMax = Vector2.zero;

        var imagen = panel.GetComponent<Image>();
        imagen.color = fondo;
        imagen.raycastTarget = false;

        var textoGO = new GameObject("Texto", typeof(RectTransform));
        textoGO.transform.SetParent(panel.transform, false);

        var texto = textoGO.AddComponent<TextMeshProUGUI>();
        TMP_FontAsset fuente = TMP_Settings.defaultFontAsset;
        if (fuente != null) texto.font = fuente;

        texto.text = mensaje;
        texto.fontSize = 110f;
        texto.fontStyle = FontStyles.Bold;
        texto.color = colorTexto;
        texto.alignment = TextAlignmentOptions.Center;

        var rtTexto = texto.rectTransform;
        rtTexto.anchorMin = Vector2.zero;
        rtTexto.anchorMax = Vector2.one;
        rtTexto.offsetMin = Vector2.zero;
        rtTexto.offsetMax = Vector2.zero;

        panel.SetActive(false);
        return panel;
    }

    // ---------------- tilemap ----------------

    static Tile GuardarTile(string ruta, Sprite sprite)
    {
        var tile = AssetDatabase.LoadAssetAtPath<Tile>(ruta);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, ruta);
        }

        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.Grid;
        EditorUtility.SetDirty(tile);
        AssetDatabase.SaveAssets();

        // Asegura que el asset esté importado antes de usarlo en SetTile.
        AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceUpdate);
        return AssetDatabase.LoadAssetAtPath<Tile>(ruta);
    }

    static int PintarTerreno(Tilemap tilemap)
    {
        // Se cargan acá, después de crear la escena: al abrir una escena nueva
        // Unity recarga los assets y las referencias viejas pueden quedar nulas.
        Tile cesped = AssetDatabase.LoadAssetAtPath<Tile>(RutaTileCesped);
        Tile tierra = AssetDatabase.LoadAssetAtPath<Tile>(RutaTileTierra);

        if (cesped == null || tierra == null)
            throw new System.Exception("No se pudieron cargar los tiles del terreno.");

        int pintados = 0;

        for (int x = NivelIzquierda; x <= NivelDerecha; x++)
        {
            if (EsHueco(x)) continue;

            int tope = AlturaTerreno(x);

            // Tres filas de grosor: la de arriba con césped y las de abajo con tierra.
            for (int y = tope; y >= tope - 2; y--)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), (y == tope) ? cesped : tierra);
                pintados++;
            }
        }

        return pintados;
    }

    static bool EsHueco(int x)
    {
        // El primer hueco va después de la zona del enemigo: si queda pegado
        // al final de su patrulla, el jugador salta al enemigo y cae al hueco.
        return (x >= -3 && x <= -2) || (x >= 6 && x <= 7);
    }

    static int AlturaTerreno(int x)
    {
        // Plataformas altas: un escalón de 1 m, alcanzable con cualquier salto.
        if ((x >= 1 && x <= 3) || (x >= 12 && x <= 13))
            return TopePlataforma;

        return TopePiso;
    }

    // ---------------- animación ----------------

    static AnimationClip CrearClip(string nombre, Sprite[] frames, float fps)
    {
        string ruta = CarpetaAnim + "/" + nombre + ".anim";
        AssetDatabase.DeleteAsset(ruta);

        var clip = new AnimationClip();
        clip.name = nombre;
        clip.frameRate = 12f;

        var claves = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            claves[i] = new ObjectReferenceKeyframe
            {
                time = i / fps,
                value = frames[i]
            };
        }

        var binding = new EditorCurveBinding
        {
            path = "",
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, claves);

        var ajustes = AnimationUtility.GetAnimationClipSettings(clip);
        ajustes.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, ajustes);

        AssetDatabase.CreateAsset(clip, ruta);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(ruta);
    }

    static AnimatorController CrearControlador(AnimationClip idle, AnimationClip run)
    {
        AssetDatabase.DeleteAsset(RutaControlador);

        var controlador = AnimatorController.CreateAnimatorControllerAtPath(RutaControlador);
        controlador.AddParameter(ParamCorriendo, AnimatorControllerParameterType.Bool);
        controlador.AddParameter(ParamEnElPiso, AnimatorControllerParameterType.Bool);

        var maquina = controlador.layers[0].stateMachine;

        var estadoIdle = maquina.AddState("Idle");
        estadoIdle.motion = idle;

        var estadoRun = maquina.AddState("Run");
        estadoRun.motion = run;

        maquina.defaultState = estadoIdle;

        // Idle -> Run: sin Has Exit Time, condición corriendo = true.
        var aRun = estadoIdle.AddTransition(estadoRun);
        aRun.hasExitTime = false;
        aRun.duration = 0f;
        aRun.AddCondition(AnimatorConditionMode.If, 0f, ParamCorriendo);

        // Run -> Idle: sin Has Exit Time, condición corriendo = false.
        var aIdle = estadoRun.AddTransition(estadoIdle);
        aIdle.hasExitTime = false;
        aIdle.duration = 0f;
        aIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, ParamCorriendo);

        EditorUtility.SetDirty(controlador);
        AssetDatabase.SaveAssets();
        return controlador;
    }

    // ---------------- pixel art ----------------

    static Sprite GuardarSprite(string ruta, Lienzo lienzo, int ppu)
    {
        var textura = new Texture2D(lienzo.Ancho, lienzo.Alto, TextureFormat.RGBA32, false);
        textura.SetPixels32(lienzo.Datos);
        textura.Apply();

        File.WriteAllBytes(Path.GetFullPath(ruta), textura.EncodeToPNG());
        Object.DestroyImmediate(textura);

        AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceUpdate);

        var importer = (TextureImporter)AssetImporter.GetAtPath(ruta);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Repeat;

        var ajustes = new TextureImporterSettings();
        importer.ReadTextureSettings(ajustes);
        ajustes.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(ajustes);
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }

    static Lienzo Personaje(int bob, int faseRun, bool parpadeo)
    {
        var l = new Lienzo(32, 32);

        int dxIzq = 0, dxDer = 0, altoIzq = 1, altoDer = 1;

        if (faseRun >= 0)
        {
            int[] desplazamiento = { -2, 0, 2, 0, -2, 0 };
            int[] alturas = { 0, 1, 0, 0, 1, 0 };
            int i = Mathf.Clamp(faseRun, 0, 5);

            dxIzq = desplazamiento[i];
            dxDer = -desplazamiento[i];
            altoIzq = alturas[i];
            altoDer = 1 - alturas[i];
        }

        int xIzq = 10 + dxIzq;
        int xDer = 18 + dxDer;

        // Piernas y pies (detrás del cuerpo).
        l.Rect(xIzq, 2 + altoIzq, xIzq + 3, 8 + bob, Tinta);
        l.Rect(xDer, 2 + altoDer, xDer + 3, 8 + bob, Tinta);
        l.Rect(xIzq - 1, 2 + altoIzq, xIzq + 3, 3 + altoIzq, Zapato);
        l.Rect(xDer - 1, 2 + altoDer, xDer + 3, 3 + altoDer, Zapato);

        // Brazos.
        int balanceo = dxIzq / 2;
        l.Rect(6, 14 + bob + balanceo, 7, 20 + bob + balanceo, Tinta);
        l.Rect(24, 14 + bob - balanceo, 25, 20 + bob - balanceo, Tinta);

        // Cuerpo.
        l.Rect(8, 8 + bob, 23, 27 + bob, Tinta);
        l.Rect(9, 9 + bob, 22, 26 + bob, Cuerpo);
        l.Rect(10, 21 + bob, 16, 23 + bob, CuerpoBrillo);

        // Ojos.
        if (parpadeo)
        {
            l.Rect(12, 18 + bob, 14, 18 + bob, Tinta);
            l.Rect(17, 18 + bob, 19, 18 + bob, Tinta);
        }
        else
        {
            l.Rect(12, 17 + bob, 14, 20 + bob, Blanco);
            l.Rect(17, 17 + bob, 19, 20 + bob, Blanco);
            l.Rect(13, 17 + bob, 14, 18 + bob, Negro);
            l.Rect(18, 17 + bob, 19, 18 + bob, Negro);
        }

        // Boca.
        l.Rect(14, 13 + bob, 17, 13 + bob, Tinta);

        return l;
    }

    static Lienzo Piso()
    {
        var l = new Lienzo(32, 32);

        l.Rect(0, 0, 31, 31, Tierra);
        l.Rect(0, 0, 31, 1, TierraOscura);
        l.Rect(0, 27, 31, 31, Cesped);
        l.Rect(0, 31, 31, 31, CespedClaro);

        // Motas de tierra, para que el tile no se vea plano.
        l.Rect(3, 8, 5, 9, TierraOscura);
        l.Rect(10, 14, 12, 15, TierraOscura);
        l.Rect(18, 6, 20, 7, TierraOscura);
        l.Rect(24, 18, 26, 19, TierraOscura);
        l.Rect(6, 22, 8, 23, TierraOscura);
        l.Rect(27, 10, 29, 11, TierraOscura);
        l.Rect(14, 20, 15, 21, TierraOscura);

        return l;
    }

    static Lienzo TierraSola()
    {
        var l = new Lienzo(32, 32);

        l.Rect(0, 0, 31, 31, Tierra);
        l.Rect(0, 0, 31, 1, TierraOscura);

        l.Rect(3, 8, 5, 9, TierraOscura);
        l.Rect(10, 14, 12, 15, TierraOscura);
        l.Rect(18, 6, 20, 7, TierraOscura);
        l.Rect(24, 18, 26, 19, TierraOscura);
        l.Rect(6, 22, 8, 23, TierraOscura);
        l.Rect(27, 10, 29, 11, TierraOscura);
        l.Rect(14, 20, 15, 21, TierraOscura);

        return l;
    }

    static Lienzo Moneda()
    {
        var l = new Lienzo(16, 16);

        l.Disco(8, 8, 7f, OroOscuro);
        l.Disco(8, 8, 6f, Oro);
        l.Rect(7, 4, 8, 11, OroOscuro);
        l.Disco(6, 10, 2f, OroBrillo);

        return l;
    }

    static Lienzo Nube()
    {
        var l = new Lienzo(32, 16);
        var color = new Color32(210, 225, 255, 210);

        l.Rect(3, 4, 28, 9, color);
        l.Disco(9, 9, 5f, color);
        l.Disco(17, 11, 6f, color);
        l.Disco(24, 9, 5f, color);

        return l;
    }

    static Lienzo Enemigo()
    {
        var l = new Lienzo(32, 32);

        // Cuerpo redondeado con base ancha.
        l.Rect(5, 2, 26, 7, RojoOscuro);
        l.Disco(16, 13, 11f, RojoOscuro);
        l.Disco(16, 14, 10f, Rojo);
        l.Rect(6, 4, 25, 7, Rojo);

        // Brillo y ojos.
        l.Disco(10, 22, 3f, RojoClaro);
        l.Rect(10, 15, 13, 19, Blanco);
        l.Rect(18, 15, 21, 19, Blanco);
        l.Rect(11, 15, 13, 17, Negro);
        l.Rect(19, 15, 21, 17, Negro);

        // Boca.
        l.Rect(13, 9, 19, 10, Tinta);

        return l;
    }

    static Lienzo Bandera()
    {
        var l = new Lienzo(32, 64);

        // Poste.
        l.Rect(6, 2, 8, 62, Metal);
        l.Rect(7, 2, 8, 62, new Color32(160, 165, 180, 255));
        l.Rect(3, 2, 11, 4, Metal);

        // Bandera triangular (más ancha al centro).
        for (int y = 38; y <= 61; y++)
        {
            int centro = 50;
            int ancho = 20 - Mathf.Abs(centro - y) / 2;
            int x1 = 9 + ancho;
            l.Rect(9, y, x1, y, (y == 38 || y == 61) ? VerdeOscuro : Verde);
        }

        l.Rect(9, 38, 10, 61, VerdeOscuro);

        return l;
    }

    class Lienzo
    {
        public readonly int Ancho;
        public readonly int Alto;
        readonly Color32[] pixeles;

        public Lienzo(int ancho, int alto)
        {
            Ancho = ancho;
            Alto = alto;
            pixeles = new Color32[ancho * alto];
            var transparente = new Color32(0, 0, 0, 0);
            for (int i = 0; i < pixeles.Length; i++) pixeles[i] = transparente;
        }

        public Color32[] Datos { get { return pixeles; } }

        public void Rect(int x0, int y0, int x1, int y1, Color32 color)
        {
            if (x0 > x1) { int t = x0; x0 = x1; x1 = t; }
            if (y0 > y1) { int t = y0; y0 = y1; y1 = t; }

            for (int y = Mathf.Max(0, y0); y <= Mathf.Min(Alto - 1, y1); y++)
            {
                for (int x = Mathf.Max(0, x0); x <= Mathf.Min(Ancho - 1, x1); x++)
                {
                    pixeles[y * Ancho + x] = color;
                }
            }
        }

        public void Disco(int cx, int cy, float radio, Color32 color)
        {
            for (int y = 0; y < Alto; y++)
            {
                for (int x = 0; x < Ancho; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    if (dx * dx + dy * dy <= radio * radio)
                        pixeles[y * Ancho + x] = color;
                }
            }
        }
    }
}
