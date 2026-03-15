using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
