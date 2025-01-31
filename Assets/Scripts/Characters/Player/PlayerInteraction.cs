using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private int raycastDistance;
    [SerializeField] LayerMask raycastLayer;

    private RaycastHit hitInfo;
    private bool isRaycastHit;

    private void Update()
    {
        if ((NotesManager.isWin == false && EnemyAI.isGameOver == false) && PauseController.isPause == false)
        {
            HandleInteractionInput();
        }
    }

    private void HandleInteractionInput()
    {
        // Raycast desde la cámara hacia la posición del ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Realizar el raycast
        isRaycastHit = Physics.Raycast(ray, out hitInfo, raycastDistance, raycastLayer);

        if (isRaycastHit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Usar TryGetComponent para obtener la interfaz IInteraction
                if (hitInfo.collider.TryGetComponent<IInteraction>(out var interaction))
                {
                    // Si el componente implementa IInteraction, llamar al método OnCollect
                    interaction.OnCollect();
                }
            }
            NotesManager.Instance.ShowPickupMessage("Recoger nota");
        }
        else
        {
            NotesManager.Instance.ShowPickupMessage("");
        }
    }
}