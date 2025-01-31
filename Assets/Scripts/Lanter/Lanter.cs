using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lanter : MonoBehaviour
{
    [SerializeField] private GameObject lanter;
    [SerializeField] private AudioClip onLanterClip;
    [SerializeField] private AudioClip offLanterClip;

    private AudioSource audioSource;
    private bool isActive;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        lanter.SetActive(isActive);
    }

    private void Update() => ToggleLanter();

    private void ToggleLanter()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Toggle());
            isActive = !isActive;
        }
    }

    private IEnumerator Toggle()
    {
        AudioClip currentClip = isActive ? offLanterClip : onLanterClip;
        audioSource.PlayOneShot(currentClip);
        lanter.SetActive(!isActive);

        yield return new WaitForSeconds(currentClip.length);
    }
}
