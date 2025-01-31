using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;    // Referencia al Dropdown de resoluciones
    public TMP_Dropdown fullscreenDropdown;     // Referencia al Dropdown de modo pantalla
    private List<Resolution> uniqueResolutions = new List<Resolution>(); // Lista de resoluciones únicas
    private Resolution currentResolution; // Resolución actual

    private const string ResolutionKey = "Resolution"; // Clave para almacenar la resolución
    private const string FullscreenKey = "Fullscreen"; // Clave para almacenar el modo pantalla

    void Awake()
    {
        // Cargar las configuraciones guardadas (si existen)
        LoadSettings();

        // Obtener las resoluciones disponibles y mostrarlas en el Dropdown
        LoadResolutions();
    }

    private void Start()
    {
        // Asignar los listeners para cuando el valor cambie
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChanged);
    }

    // Cargar las configuraciones guardadas
    void LoadSettings()
    {
        // Resolución guardada, si no existe usa la resolución actual
        if (PlayerPrefs.HasKey(ResolutionKey))
        {
            string resolutionString = PlayerPrefs.GetString(ResolutionKey);
            string[] resolutionParts = resolutionString.Split('x');
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);

            currentResolution = new Resolution { width = width, height = height };
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
        else
        {
            currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }

        // Modo pantalla guardado
        if (PlayerPrefs.HasKey(FullscreenKey))
        {
            // En este caso, PlayerPrefs almacena 1 para pantalla completa y 0 para modo ventana
            bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey) == 1;
            Screen.fullScreen = isFullscreen;

            // Si es pantalla completa (1), el dropdown debe tener valor 0, si es ventana (0), debe tener valor 1
            fullscreenDropdown.value = isFullscreen ? 0 : 1;  // 0 = Pantalla Completa, 1 = Modo Ventana
        }
        else
        {
            // Si no existe la clave, se asume pantalla completa por defecto
            Screen.fullScreen = true;
            fullscreenDropdown.value = 0; // Pantalla Completa

            // Guardamos el valor por defecto en PlayerPrefs
            PlayerPrefs.SetInt(FullscreenKey, 1); // Guardamos 1 para pantalla completa
        }
    }

    // Cargar las resoluciones disponibles sin repeticiones
    void LoadResolutions()
    {
        // Limpiar la lista de resoluciones previas
        uniqueResolutions.Clear();

        // Obtener todas las resoluciones disponibles
        Resolution[] resolutions = Screen.resolutions;

        foreach (var res in resolutions)
        {
            // Agregar solo resoluciones únicas
            if (!uniqueResolutions.Exists(r => r.width == res.width && r.height == res.height))
            {
                uniqueResolutions.Add(res);
            }
        }

        // Llenar el Dropdown con las resoluciones disponibles
        List<string> resolutionOptions = new List<string>();
        foreach (var res in uniqueResolutions)
        {
            resolutionOptions.Add(res.width + "x" + res.height);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);

        // Preseleccionar la resolución actual
        int currentIndex = resolutionOptions.IndexOf(currentResolution.width + "x" + currentResolution.height);
        if (currentIndex != -1)
        {
            resolutionDropdown.value = currentIndex;
        }
    }

    // Cambiar la resolución cuando el usuario selecciona una opción
    void OnResolutionChanged(int index)
    {
        Resolution selectedResolution = uniqueResolutions[index];
        currentResolution = selectedResolution;
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        // Guardar la resolución
        PlayerPrefs.SetString(ResolutionKey, selectedResolution.width + "x" + selectedResolution.height);

        // Cambiar el modo de pantalla a ventana cuando se cambie la resolución
        Screen.fullScreen = false;
        fullscreenDropdown.value = 1; // Modo ventana
    }

    // Cambiar el modo de pantalla cuando el usuario selecciona una opción
    void OnFullscreenChanged(int value)
    {
        if (value == 0) // Pantalla Completa
        {
            Screen.fullScreen = true;
            PlayerPrefs.SetInt(FullscreenKey, 1);
        }
        else // Modo Ventana
        {
            Screen.fullScreen = false;
            PlayerPrefs.SetInt(FullscreenKey, 0);
        }
    }
}