using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";
    public void OpenMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}