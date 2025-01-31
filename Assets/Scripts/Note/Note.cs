using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteraction
{
    public static int collectedNotes = 0;  // Notas ya recogidas
    public static int totalNotes;  // Número total de notas en la escena
    public int noteIndex; // Índice único para cada nota

    private void Start()
    {
        collectedNotes = 0;
    }

    public void OnCollect()
    {
        // Desactivar el objeto cuando se recoja la nota
        gameObject.SetActive(false);

        // Aumentar el contador de notas recogidas
        collectedNotes++;

        // Guardar la nota como recogida en PlayerPrefs
        PlayerPrefs.SetInt("Note_" + noteIndex, 1);  // Guardar que la nota con este índice fue recogida
        PlayerPrefs.Save();

        // Llamar al gestor de UI para actualizar el mensaje
        NotesManager.Instance.UpdateNoteMessage();
    }
}