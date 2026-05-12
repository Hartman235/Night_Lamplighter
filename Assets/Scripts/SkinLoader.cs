using UnityEngine;
using System; 

public class SkinLoader : MonoBehaviour
{
    [Header("Player Skins")]
    [SerializeField] private Transform skinsParent; 
    private int chosenIndex;

    public static event Action OnSkinChanged;

    void Start()
    {
        LoadSelectedSkin();
    }

    void LoadSelectedSkin()
    {
        chosenIndex = PlayerPrefs.GetInt("chosenSkin", 0);

        if (chosenIndex < 0 || chosenIndex >= skinsParent.childCount)
        {
            chosenIndex = 0;
        }

        for (int i = 0; i < skinsParent.childCount; i++)
        {
            skinsParent.GetChild(i).gameObject.SetActive(i == chosenIndex);
        }
        
        OnSkinChanged?.Invoke();
    }

    public void ReloadSkin()
    {
        LoadSelectedSkin();
    }
}