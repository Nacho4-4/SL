using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        // Si hay progreso, habilitamos el bot�n de continuar
        if (CheckProgressSaved())
        {
            SetTextAlpha(255);  // Hacemos el texto completamente visible
            continueButton.enabled = true;  // Habilitamos el bot�n
            continueButton.image.enabled = true;  // Habilitamos el bot�n
        }
        else
        {
            SetTextAlpha(150);  // Hacemos el texto semi-transparente
            continueButton.enabled = false;  // Deshabilitamos el bot�n
            continueButton.image.enabled = false;  // Deshabilitamos el bot�n
        }
    }

    // Funci�n para crear un nuevo juego (borrar PlayerPrefs)
    public void NewGame() => StartCoroutine(NG());

    // Funci�n para continuar el juego si ya hay datos guardados
    public void ContinueGame() => StartCoroutine(Continue());

    private IEnumerator NG()
    {
        int totalNotes = PlayerPrefs.GetInt("TotalNotes", 0);
        // Borrar todos los PlayerPrefs relacionados con las notas
        for (int i = 0; i < totalNotes; i++)
        {
            PlayerPrefs.DeleteKey("Note_" + i);  // Borrar la clave individual de cada nota
        }

        // Borrar la clave de TotalNotes para asegurar que el n�mero de notas no persista entre sesiones
        PlayerPrefs.DeleteKey("TotalNotes");

        // Borrar la clave FollowRange
        PlayerPrefs.DeleteKey("FollowRange");

        // Borrar la clave Speed
        PlayerPrefs.DeleteKey("Speed");

        // Guardar cualquier otro dato relevante
        PlayerPrefs.Save();  // Asegurarse de guardar los cambios

        // Cargar la escena del juego
        yield return null;
        SceneManager.LoadScene("Game");
        Time.timeScale = 1.0f;
    }

    private IEnumerator Continue()
    {
        // Verificar si existe progreso guardado revisando "TotalNotes"
        int totalNotes = PlayerPrefs.GetInt("TotalNotes", 0); // Si no existe, devuelve 0

        if (totalNotes > 0)
        {
            // Verificar si alguna de las notas fue recogida (comprobamos las claves de las notas)
            bool hasProgress = false;
            for (int i = 0; i < totalNotes; i++)
            {
                if (PlayerPrefs.HasKey("Note_" + i))  // Si existe la clave, significa que esa nota fue recogida
                {
                    hasProgress = true;
                    break;
                }
            }

            // Si hay progreso guardado, cargar la escena del juego
            if (hasProgress)
            {
                // Cargar la escena del juego
                yield return null;
                SceneManager.LoadScene("Game");  // Aqu� aseg�rate de que el nombre de la escena es correcto
                Time.timeScale = 1.0f;  // Aseg�rate de que el juego no est� pausado
            }
            else
            {
                // Si no hay progreso guardado, comienza un nuevo juego
                NewGame();  // Aqu� usas el m�todo NewGame para reiniciar los datos y cargar la escena del juego
            }
        }
        else
        {
            // Si no existe "TotalNotes" en PlayerPrefs (nuevo juego sin progreso), inicia uno nuevo
            NewGame();  // Llama a NewGame si no hay datos guardados
        }
    }

    // M�todo para salir del juego
    public void ExitGame() => Application.Quit();

    // M�todo para cambiar el alpha del texto
    private void SetTextAlpha(int alpha)
    {
        // Asegurarse de que el alpha est� en el rango de 0 a 255
        alpha = Mathf.Clamp(alpha, 0, 255);

        // Obtener el color actual del texto
        Color currentColor = continueText.color;

        // Modificar solo el valor de alpha, manteniendo los otros componentes (R, G, B)
        continueText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha / 255f);
    }

    // M�todo que verifica si hay progreso guardado en PlayerPrefs
    private bool CheckProgressSaved()
    {
        // Obtener el n�mero total de notas desde PlayerPrefs
        int totalNotes = PlayerPrefs.GetInt("TotalNotes", 0);  // Si no existe, devuelve 0

        // Verificar si alguna de las notas ha sido recogida
        for (int i = 0; i < totalNotes; i++)
        {
            if (PlayerPrefs.HasKey("Note_" + i))  // Si existe la clave de la nota, significa que esa nota fue recogida
            {
                return true;  // Hay progreso guardado
            }
        }

        // Si no se encuentra progreso, retornar false
        return false;
    }
}