using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonHandler : MonoBehaviour, IPointerDownHandler
{
    private PauseMenu pauseMenu;

    private void Start()
    {
        if (pauseMenu == null)
            pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pauseMenu != null)
            pauseMenu.TogglePause();
    }
}