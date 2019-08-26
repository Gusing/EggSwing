using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class shopHandler : MonoBehaviour
{
    public GameObject ShopListContent;
    public GameObject ShopScrollList;
    public GameObject ShopItemContainer;

    public Sprite spriteCombo1;
    public Sprite spriteCombo2;
    public Sprite spriteCombo3;

    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;

    FMOD.Studio.EventInstance musicShop;

    PlayerData data;

    public Text textCurrency;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");
        //musicShop = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Shop");

        textCurrency.text = "Munny: " + data.currency;

        //musicShop.start();
        
        // load data
        GameObject ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[0], data.itemActive[0], "Flatten Combo", spriteCombo1, 0);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[1], data.itemActive[1], "Charge Punch", spriteCombo2, 1);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[2], data.itemActive[2], "Rapid Kicks", spriteCombo3, 2);
    }

    void Update()
    {
        
    }

    public bool ClickBuy(int ID)
    {




        return true;
    }

    public void BackToPlayMenu()
    {
        soundUIClick.start();
        SceneManager.LoadScene("MenuScene");
    }

    public void Purchase(int ID)
    {

    }
}
