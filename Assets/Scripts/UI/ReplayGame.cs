using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayGame : MonoBehaviour
{
    public void Replay() => StartCoroutine(RG());
    private IEnumerator RG()
    {
        int totalNotes = PlayerPrefs.GetInt("TotalNotes", 0);
        // Borrar todos los PlayerPrefs relacionados con las notas
        for (int i = 0; i < totalNotes; i++)
        {
            PlayerPrefs.DeleteKey("Note_" + i);  // Borrar la clave individual de cada nota
        }

        // Borrar la clave de TotalNotes para asegurar que el número de notas no persista entre sesiones
        PlayerPrefs.DeleteKey("TotalNotes");

        // Borrar la clave FollowRange
        PlayerPrefs.DeleteKey("FollowRange");

        // Borrar la clave Speed
        PlayerPrefs.DeleteKey("Speed");

        // Guardar cualquier otro dato relevante
        PlayerPrefs.Save();  // Asegurarse de guardar los cambios
        Note.collectedNotes = 0;

        // Cargar la escena del juego
        yield return null;
        SceneManager.LoadScene("Game");
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}