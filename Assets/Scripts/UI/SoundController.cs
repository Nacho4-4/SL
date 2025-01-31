using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] private Slider sfxController;
    [SerializeField] private AudioSource[] sfx;

    private const string sfxKey = "Volume";

    private void Awake()
    {
        // Intentar cargar el volumen guardado. Si no existe, se usa el valor por defecto (1f)
        float savedVolume = PlayerPrefs.GetFloat(sfxKey, 1);

        // Si la clave no existe, el volumen será 1, y el valor de PlayerPrefs debería guardarse
        if (!PlayerPrefs.HasKey(sfxKey))
        {
            savedVolume = 1f; // Establecer el valor por defecto
            PlayerPrefs.SetFloat(sfxKey, savedVolume); // Guardar el valor por defecto
        }

        // Asignar el valor del volumen al slider
        sfxController.value = savedVolume;

        // Aplicar el volumen a los AudioSources
        SetVolume(savedVolume);

        // Asignar el evento del Slider
        sfxController.onValueChanged.AddListener(OnVolumeChanged);
    }

    // Método para cambiar el volumen
    private void OnVolumeChanged(float value)
    {
        SetVolume(value);
        // Guardar el volumen actual en PlayerPrefs
        PlayerPrefs.SetFloat(sfxKey, value);
        PlayerPrefs.Save();
    }

    // Método para ajustar el volumen de todos los AudioSources
    private void SetVolume(float volume)
    {
        foreach (var audioSource in sfx)
        {
            audioSource.volume = volume;
        }
    }
}