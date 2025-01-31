using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaBarUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private TextMeshProUGUI message;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        float normalizedStamina = Mathf.InverseLerp(0, playerMovement.GetStaminaMaxValue(), 
            playerMovement.CurrentSamina);
        staminaBar.fillAmount = normalizedStamina;

        if (playerMovement.IsRunning && staminaBar.fillAmount == normalizedStamina)
        {
            staminaBar.enabled = true;
        }
        else if (staminaBar.fillAmount == 1)
        {
            staminaBar.enabled = false;
        }

        if (playerMovement.CurrentSamina <= 0 && playerMovement.IsRunning)
        {
            message.text = "Sin energia, no puedes correr.";
        }
        else if (playerMovement.CurrentSamina > 0 && !playerMovement.IsRunning)
        {
            message.text = "";
        }
    }
}
