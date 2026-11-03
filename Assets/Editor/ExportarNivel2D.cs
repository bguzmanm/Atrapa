using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Clase 11 — Actividad 4, paso 6: exportar el nivel 2D a macOS.
// Menú: Tools > Atrapa > Exportar Nivel 2D (macOS)
public static class ExportarNivel2D
{
    const string Escena = "Assets/Scenes/Nivel2D.unity";
    const string NombreBuild = "SaltaYRecoge.app";
    const string NombreCarpeta = "SaltaYRecoge_Build";

    [MenuItem("Tools/Atrapa/Exportar Nivel 2D (macOS)", priority = 10)]
    public static void ExportarMac()
    {
        ConstruirMac(true);
    }

    public static bool ConstruirMac(bool revelar)
    {
        string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string carpeta = Path.Combine(escritorio, NombreCarpeta);
        string salida = Path.Combine(carpeta, NombreBuild);

        Directory.CreateDirectory(carpeta);

        var opciones = new BuildPlayerOptions
        {
            scenes = new[] { Escena },
            locationPathName = salida,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };

        BuildReport reporte = BuildPipeline.BuildPlayer(opciones);
        BuildSummary resumen = reporte.summary;

        if (resumen.result == BuildResult.Succeeded)
        {
            Debug.Log("Build 2D listo: " + salida + " (" + resumen.totalSize + " bytes)");

            if (revelar)
                EditorUtility.RevealInFinder(salida);

            return true;
        }

        Debug.LogError("Build 2D falló: " + resumen.result +
                       " — " + resumen.totalErrors + " errores. Ver la Consola.");
        return false;
    }
}
