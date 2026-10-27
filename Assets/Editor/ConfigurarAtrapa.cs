using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Construye la escena completa del juego "Atrapa y esquiva" siguiendo
// las guías 7, 8, 8.1 y 9. Se ejecuta una sola vez, sola, y también se
// puede repetir desde el menú  Tools > Atrapa > Construir escena completa.
public static class ConfigurarAtrapa
{
    const string EscenaPath = "Assets/Scenes/SampleScene.unity";
    const string LogPath = "/tmp/atrapa_build.log";

    [InitializeOnLoadMethod]
    static void AutoEjecutar()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => { };
                return;
            }

            if (EscenaYaConstruida())
            {
                EscribirLog("La escena ya tiene los objetos. No se reconstruye.\n");
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EscribirLog("Unity está en Play. Sal de Play y vuelve a ejecutar Tools > Atrapa.\n");
                return;
            }

            Build();
        };
    }

    static bool EscenaYaConstruida()
    {
        try
        {
            if (!File.Exists(EscenaPath)) return false;
            string texto = File.ReadAllText(EscenaPath);

            return texto.Contains("m_Name: Jugador")
                && texto.Contains("m_Name: Generador")
                && texto.Contains("m_Name: TextoPuntos")
                && texto.Contains("m_Name: PauseMenu")
                && texto.Contains("m_Name: BotonReiniciar");
        }
        catch { return false; }
    }

    [MenuItem("Tools/Atrapa/Construir escena completa", priority = 0)]
    public static void Build()
    {
        var log = new StringBuilder();
        log.AppendLine("== Construyendo escena Atrapa ==");

        try
        {
            AssetDatabase.Refresh();
            AsegurarCarpetas(log);
            AsegurarTMP(log);
            AsegurarTags(log, "Moneda", "Obstaculo");

            Material matMoneda = MaterialDe("Assets/Materiales/Moneda.mat", new Color(1f, 0.82f, 0.1f));
            Material matObstaculo = MaterialDe("Assets/Materiales/Obstaculo.mat", new Color(0.85f, 0.15f, 0.15f));
            Material matJugador = MaterialDe("Assets/Materiales/Jugador.mat", new Color(0.2f, 0.5f, 0.95f));
            log.AppendLine("Materiales listos.");

            // ---------- Escena ----------
            try { EditorSceneManager.SaveOpenScenes(); } catch { }
            var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var camara = Object.FindAnyObjectByType<Camera>();
            if (camara != null)
            {
                camara.transform.position = new Vector3(0f, 12f, -12f);
                camara.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            }

            var luz = Object.FindAnyObjectByType<Light>();
            if (luz != null && luz.type == LightType.Directional)
                luz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // ---------- Prefabs ----------
            GameObject prefMoneda = CrearPrefabMoneda(matMoneda);
            GameObject prefObstaculo = CrearPrefabObstaculo(matObstaculo);
            log.AppendLine("Prefabs: " + prefMoneda + " / " + prefObstaculo);

            // ---------- Piso ----------
            var piso = GameObject.CreatePrimitive(PrimitiveType.Plane);
            piso.name = "Piso";
            piso.transform.position = Vector3.zero;
            piso.transform.localScale = Vector3.one;

            // ---------- Jugador ----------
            var jugador = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jugador.name = "Jugador";
            jugador.tag = "Untagged";
            jugador.transform.position = new Vector3(0f, 0.5f, 0f);
            jugador.GetComponent<MeshRenderer>().sharedMaterial = matJugador;

            var rb = jugador.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezePositionY
                           | RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationY
                           | RigidbodyConstraints.FreezeRotationZ;

            var recolector = jugador.AddComponent<Recolector>();
            var movimiento = jugador.AddComponent<JugadorMovimiento>();
            movimiento.velocidad = 6f;

            // ---------- Audio ----------
            var audioGO = new GameObject("Audio");
            var bgm = audioGO.AddComponent<AudioSource>();
            bgm.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/musica.wav");
            bgm.loop = true;
            bgm.playOnAwake = true;
            bgm.volume = 0.4f;
            bgm.spatialBlend = 0f;

            var sfx = audioGO.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            sfx.loop = false;
            sfx.volume = 0.8f;
            sfx.spatialBlend = 0f;

            recolector.sfxSource = sfx;
            recolector.clipMoneda = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/moneda.wav");
            recolector.clipChoque = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/choque.wav");
            recolector.clipGameOver = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gameover.wav");
            log.AppendLine("Audio listo.");

            // ---------- Generador ----------
            var generadorGO = new GameObject("Generador");
            var generador = generadorGO.AddComponent<GeneradorDeObjetos>();
            generador.monedaPrefab = prefMoneda;
            generador.obstaculoPrefab = prefObstaculo;
            generador.jugador = jugador.transform;
            generador.recolector = recolector;
            generador.intervalo = 2f;

            // ---------- UI ----------
            var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var eventSystem = new GameObject("EventSystem");
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif

            TMP_FontAsset fuente = FuenteTMP(log);

            var textoPuntos = CrearTexto("TextoPuntos", canvasGO.transform, "Puntos: 0", 48f, Color.white, fuente);
            var rtPuntos = textoPuntos.rectTransform;
            rtPuntos.anchorMin = new Vector2(0f, 1f);
            rtPuntos.anchorMax = new Vector2(0f, 1f);
            rtPuntos.pivot = new Vector2(0f, 1f);
            rtPuntos.anchoredPosition = new Vector2(30f, -30f);
            rtPuntos.sizeDelta = new Vector2(500f, 80f);

            var textoVidas = CrearTexto("TextoVidas", canvasGO.transform, "Vidas: 3", 48f, Color.white, fuente);
            var rtVidas = textoVidas.rectTransform;
            rtVidas.anchorMin = new Vector2(0f, 1f);
            rtVidas.anchorMax = new Vector2(0f, 1f);
            rtVidas.pivot = new Vector2(0f, 1f);
            rtVidas.anchoredPosition = new Vector2(30f, -120f);
            rtVidas.sizeDelta = new Vector2(500f, 80f);

            var textoGameOver = CrearTexto("TextoGameOver", canvasGO.transform, "GAME OVER", 120f, new Color(1f, 0.15f, 0.15f), fuente);
            Centrar(textoGameOver.rectTransform, new Vector2(900f, 200f));
            textoGameOver.alignment = TextAlignmentOptions.Center;
            textoGameOver.gameObject.SetActive(false);

            var botonReiniciar = CrearBoton("BotonReiniciar", canvasGO.transform, "REINICIAR", 32f, fuente);
            var rtBoton = (RectTransform)botonReiniciar.transform;
            rtBoton.anchorMin = new Vector2(0.5f, 0f);
            rtBoton.anchorMax = new Vector2(0.5f, 0f);
            rtBoton.pivot = new Vector2(0.5f, 0.5f);
            rtBoton.anchoredPosition = new Vector2(0f, 60f);
            rtBoton.sizeDelta = new Vector2(300f, 90f);
            botonReiniciar.SetActive(false);

            // ---------- Menú de pausa (clase 9) ----------
            var pauseMenu = new GameObject("PauseMenu", typeof(RectTransform));
            pauseMenu.transform.SetParent(canvasGO.transform, false);
            var rtPausa = pauseMenu.GetComponent<RectTransform>();
            rtPausa.anchorMin = Vector2.zero;
            rtPausa.anchorMax = Vector2.one;
            rtPausa.offsetMin = Vector2.zero;
            rtPausa.offsetMax = Vector2.zero;

            var fondo = new GameObject("Fondo", typeof(RectTransform), typeof(Image));
            fondo.transform.SetParent(pauseMenu.transform, false);
            var rtFondo = fondo.GetComponent<RectTransform>();
            rtFondo.anchorMin = Vector2.zero;
            rtFondo.anchorMax = Vector2.one;
            rtFondo.offsetMin = Vector2.zero;
            rtFondo.offsetMax = Vector2.zero;
            fondo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            var textoPausa = CrearTexto("TextoPausa", pauseMenu.transform, "PAUSA", 100f, Color.white, fuente);
            Centrar(textoPausa.rectTransform, new Vector2(900f, 200f));
            textoPausa.alignment = TextAlignmentOptions.Center;

            var botonReanudar = CrearBoton("BotonReanudar", pauseMenu.transform, "REANUDAR", 32f, fuente);
            var rtReanudar = (RectTransform)botonReanudar.transform;
            rtReanudar.anchorMin = new Vector2(0.5f, 0.5f);
            rtReanudar.anchorMax = new Vector2(0.5f, 0.5f);
            rtReanudar.pivot = new Vector2(0.5f, 0.5f);
            rtReanudar.anchoredPosition = new Vector2(0f, -150f);
            rtReanudar.sizeDelta = new Vector2(300f, 90f);

            pauseMenu.SetActive(false);

            // ---------- Objeto UI con el UIManager ----------
            var uiGO = new GameObject("UI");
            var ui = uiGO.AddComponent<UIManager>();
            ui.recolector = recolector;
            ui.textoPuntos = textoPuntos;
            ui.textoVidas = textoVidas;
            ui.textoGameOver = textoGameOver.gameObject;
            ui.botonReiniciar = botonReiniciar;
            ui.pauseMenu = pauseMenu;

            UnityEventTools.AddPersistentListener(botonReiniciar.GetComponent<Button>().onClick, ui.Reiniciar);
            UnityEventTools.AddPersistentListener(botonReanudar.GetComponent<Button>().onClick, ui.Reanudar);
            log.AppendLine("UI y botones conectados.");

            // ---------- Guardar ----------
            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena, EscenaPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EscribirLog(log.ToString() + "LISTO: escena guardada en " + EscenaPath + "\n");
            Debug.Log("Atrapa: escena construida y guardada.");
        }
        catch (System.Exception e)
        {
            EscribirLog(log.ToString() + "ERROR: " + e + "\n");
            Debug.LogError("Atrapa: falló la construcción -> " + e);
        }
    }

    // ---------------- utilidades ----------------

    static void EscribirLog(string texto)
    {
        try { File.WriteAllText(LogPath, texto); } catch { }
        Debug.Log("Atrapa log: " + texto);
    }

    static void AsegurarCarpetas(StringBuilder log)
    {
        foreach (var c in new[] { "Assets/Materiales", "Assets/Prefabs" })
        {
            if (!AssetDatabase.IsValidFolder(c))
            {
                string padre = Path.GetDirectoryName(c).Replace('\\', '/');
                string nombre = Path.GetFileName(c);
                AssetDatabase.CreateFolder(padre, nombre);
                log.AppendLine("Carpeta creada: " + c);
            }
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

    static bool TMPListo()
    {
        try { return TMP_Settings.defaultFontAsset != null; }
        catch { return false; }
    }

    static void AsegurarTMP(StringBuilder log)
    {
        if (TMPListo())
        {
            log.AppendLine("TMP ya está importado.");
            return;
        }

        try
        {
            var pkg = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
            if (pkg == null) { log.AppendLine("TMP: no se encontró el paquete."); return; }

            string ruta = Path.Combine(pkg.resolvedPath, "Package Resources", "TMP Essential Resources.unitypackage");
            if (!File.Exists(ruta))
            {
                log.AppendLine("TMP: no existe " + ruta);
                return;
            }

            AssetDatabase.ImportPackage(ruta, false);
            AssetDatabase.Refresh();
            log.AppendLine("TMP Essentials importado.");
        }
        catch (System.Exception e)
        {
            log.AppendLine("TMP: error al importar -> " + e.Message);
        }
    }

    static TMP_FontAsset FuenteTMP(StringBuilder log)
    {
        try
        {
            var fuente = TMP_Settings.defaultFontAsset;
            if (fuente != null)
            {
                log.AppendLine("Fuente TMP (settings): " + fuente.name);
                return fuente;
            }
        }
        catch { }

        var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        TMP_FontAsset primera = null;

        foreach (var g in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(g);
            var f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(ruta);
            if (f == null) continue;

            if (primera == null) primera = f;
            if (f.name.Contains("LiberationSans"))
            {
                log.AppendLine("Fuente TMP encontrada: " + f.name);
                return f;
            }
        }

        if (primera != null)
            log.AppendLine("Fuente TMP encontrada: " + primera.name);

        if (primera == null)
            log.AppendLine("AVISO: no hay fuente TMP. Los textos pueden verse vacíos.");

        return primera;
    }

    static Material MaterialDe(string ruta, Color color)
    {
        var existente = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (existente != null)
        {
            existente.color = color;
            return existente;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        var mat = new Material(shader);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

        AssetDatabase.CreateAsset(mat, ruta);
        return mat;
    }

    static GameObject CrearPrefabMoneda(Material mat)
    {
        const string ruta = "Assets/Prefabs/Moneda.prefab";
        var temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "Moneda";
        temp.tag = "Moneda";
        temp.transform.localScale = Vector3.one * 0.5f;
        var col = temp.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
        temp.GetComponent<MeshRenderer>().sharedMaterial = mat;

        var prefab = PrefabUtility.SaveAsPrefabAsset(temp, ruta);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static GameObject CrearPrefabObstaculo(Material mat)
    {
        const string ruta = "Assets/Prefabs/Obstaculo.prefab";
        var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temp.name = "Obstaculo";
        temp.tag = "Obstaculo";
        var col = temp.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
        temp.GetComponent<MeshRenderer>().sharedMaterial = mat;

        var prefab = PrefabUtility.SaveAsPrefabAsset(temp, ruta);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    static TextMeshProUGUI CrearTexto(string nombre, Transform padre, string contenido, float tamano, Color color, TMP_FontAsset fuente)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);

        var texto = go.AddComponent<TextMeshProUGUI>();
        if (fuente != null) texto.font = fuente;
        texto.text = contenido;
        texto.fontSize = tamano;
        texto.color = color;
        texto.alignment = TextAlignmentOptions.Left;

        return texto;
    }

    static GameObject CrearBoton(string nombre, Transform padre, string etiqueta, float tamanoTexto, TMP_FontAsset fuente)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);

        go.GetComponent<Image>().color = new Color(0.16f, 0.42f, 0.85f, 1f);

        var etiquetaGO = new GameObject("Text (TMP)", typeof(RectTransform));
        etiquetaGO.transform.SetParent(go.transform, false);

        var texto = etiquetaGO.AddComponent<TextMeshProUGUI>();
        if (fuente != null) texto.font = fuente;
        texto.text = etiqueta;
        texto.fontSize = tamanoTexto;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;

        var rtTexto = texto.rectTransform;
        rtTexto.anchorMin = Vector2.zero;
        rtTexto.anchorMax = Vector2.one;
        rtTexto.offsetMin = Vector2.zero;
        rtTexto.offsetMax = Vector2.zero;

        return go;
    }

    static void Centrar(RectTransform rt, Vector2 tamano)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = tamano;
    }
}

