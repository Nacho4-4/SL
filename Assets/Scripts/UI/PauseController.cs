using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject menuPause;
    [SerializeField] private GameObject menuOptions;

    public static bool isPause;
    private AudioSource[] audioSources;

    private void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
            {
                isPause = !isPause;
            }
        }

        if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
        {
            panelPause.SetActive(isPause);
        }

        if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
        {
            Time.timeScale = isPause ? 0 : 1;
            Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isPause;
        }

        if (!isPause)
        {
            if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
            {
                menuOptions.SetActive(false);
                if (!menuPause.activeSelf)
                {
                    menuPause.SetActive(true);
                }
            }
        }

        if (EnemyAI.isGameOver == false && NotesManager.isWin == false)
        {
            SetVolume();
        }
    }

    private void SetVolume()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.mute = isPause;
        }
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
