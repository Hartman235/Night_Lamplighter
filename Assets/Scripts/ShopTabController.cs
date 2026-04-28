using UnityEngine;

public class ShopTabController : MonoBehaviour
{
    public GameObject skinsPanel;
    public GameObject upgradesPanel;

    public void ShowSkins()
    {
        skinsPanel.SetActive(true);
        upgradesPanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        skinsPanel.SetActive(false);
        upgradesPanel.SetActive(true);
    }
}