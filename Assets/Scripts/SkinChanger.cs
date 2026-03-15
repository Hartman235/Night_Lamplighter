using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    public Skin[] info;
    private bool[] StockCheck;

    public Button buyBttn;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI coinsText;
    public Transform player;
    public int index;

    public int coins;

    private void Awake()
    {
        coins = PlayerPrefs.GetInt("coins");
        index = PlayerPrefs.GetInt("chosenSkin");
        coinsText.text = coins.ToString();

        StockCheck = new bool[info.Length];
        if (PlayerPrefs.HasKey("StockArray"))
            StockCheck = PlayerPrefsX.GetBoolArray("StockArray");

        else
            StockCheck[0] = true;

        info[index].isChosen = true;

        for (int i = 0; i < info.Length; i++)
        {
            info[i].inStock = StockCheck[i];
            if (i == index)
                player.GetChild(i).gameObject.SetActive(true);
            else
                player.GetChild(i).gameObject.SetActive(false);
        }

        priceText.text = "Выбрано";
        buyBttn.interactable = false;
    }

    public void Save()
    {
        PlayerPrefsX.SetBoolArray("StockArray",StockCheck);
    }

    public void ScrollRight()
    {
        if (index < player.childCount - 1) // -1 потому что индексы с 0
        {
            index++;

            if (info[index].inStock && info[index].isChosen)
            {
                priceText.text = "Выбрано";
                buyBttn.interactable = false;
            }
            else if (!info[index].inStock)
            {
                priceText.text = info[index].cost.ToString();
                buyBttn.interactable = true;
            }
            else if (info[index].inStock && !info[index].isChosen)
            {
                priceText.text = "Выбрать";
                buyBttn.interactable = true;
            }

            for (int i = 0; i < player.childCount; i++)
                player.GetChild(i).gameObject.SetActive(false);

            player.GetChild(index).gameObject.SetActive(true);
        }
    }

    public void ScrollLeft()
    {
        if (index > 0)
        {
            index--;

            if (info[index].inStock && info[index].isChosen)
            {
                priceText.text = "Выбрано";
                buyBttn.interactable = false;
            }
            else if (!info[index].inStock)
            {
                priceText.text = info[index].cost.ToString();
                buyBttn.interactable = true;
            }
            else if (info[index].inStock && !info[index].isChosen)
            {
                priceText.text = "Выбрать";
                buyBttn.interactable = true;
            }

            for (int i = 0; i < player.childCount; i++)
                player.GetChild(i).gameObject.SetActive(false);

            player.GetChild(index).gameObject.SetActive(true);
        }
    }

    public void BuyButtonAction()
    {
        if (buyBttn.interactable && !info[index].inStock)
        {
            if (coins > int.Parse(priceText.text))
            {
                coins -= int.Parse(priceText.text);
                coinsText.text = coins.ToString();
                PlayerPrefs.SetInt("coins", coins);
                
                // Обновляем массивы
                StockCheck[index] = true;
                info[index].inStock = true;
                
                // Снимаем isChosen со всех скинов
                for (int i = 0; i < info.Length; i++)
                {
                    info[i].isChosen = false;
                }
                
                // Устанавливаем isChosen на текущий
                info[index].isChosen = true;
                
                // Сохраняем выбранный скин
                PlayerPrefs.SetInt("chosenSkin", index);
                
                // Отключаем все скины и включаем текущий
                for (int i = 0; i < player.childCount; i++)
                {
                    player.GetChild(i).gameObject.SetActive(i == index);
                }
                
                // Меняем текст на CHOSEN и блокируем кнопку
                priceText.text = "Выбрано";
                buyBttn.interactable = false;
                
                Save();
            }
        } 

        if (buyBttn.interactable && !info[index].isChosen && info[index].inStock) 
        {
            // Снимаем isChosen со всех скинов
            for (int i = 0; i < info.Length; i++)
            {
                info[i].isChosen = false;
            }
            
            // Устанавливаем isChosen на текущий
            info[index].isChosen = true;
            
            // Сохраняем выбор
            PlayerPrefs.SetInt("chosenSkin", index);
            
            // Отключаем все скины и включаем текущий
            for (int i = 0; i < player.childCount; i++)
            {
                player.GetChild(i).gameObject.SetActive(i == index);
            }
            
            buyBttn.interactable = false;
            priceText.text = "Выбрано";
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}


[System.Serializable]
public class Skin
{
    public int cost;
    public bool inStock;
    public bool isChosen;
}