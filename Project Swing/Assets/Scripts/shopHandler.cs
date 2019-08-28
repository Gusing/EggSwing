using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class shopHandler : MonoBehaviour
{
    public GameObject ShopListContent;
    public ScrollRect ShopScrollList;
    public GameObject ShopItemContainer;

    public Sprite spriteCombo1;
    public Sprite spriteCombo2;
    public Sprite spriteCombo3;
    public Sprite spriteCombo4;
    public Sprite spriteCombo5;

    public Text txtCurrency;

    int[] prices;

    public int endlessRecord;
    public bool[] clearedLevel;
    public bool[] clearedBirdLevel;
    public bool[] unlockedLevel;
    public bool[] unlockedBirdLevel;
    public int[] streakRecord;
    public int[] comboRecord;
    public int streakLevelEndlessRecord;
    public int[] scoreRecord;
    public int[] scoreBirdRecord;
    public int[] rankRecord;
    public int[] rankBirdRecord;
    int currentMaxStreak;
    public int currency;
    public bool[] itemBought;
    public bool[] itemActive;
    public int lastMode;

    FMOD.Studio.EventInstance soundUIClick;
    FMOD.Studio.EventInstance soundUIStart;
    FMOD.Studio.EventInstance soundShopBuy;
    FMOD.Studio.EventInstance soundShopCannotAfford;

    FMOD.Studio.EventInstance musicShop;

    PlayerData data;

    public Text textCurrency;

    readonly int COMBOFLATTEN = 0, COMBOCHARGEPUNCH = 1, COMBORAPIDKICKS = 2, COMBOSUPER = 3, COMBOCOUNTERHIT = 4;

    void Awake()
    {
        data = SaveSystem.LoadPlayer();
    }

    void Start()
    {
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");
        soundShopCannotAfford = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super_fail");
        soundShopBuy = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Buy");

        textCurrency.text = "Munny: " + data.currency;

        data.Init();
        endlessRecord = data.endlessRecord;
        clearedLevel = data.clearedLevel;
        clearedBirdLevel = data.clearedBirdLevel;
        unlockedLevel = data.unlockedLevel;
        unlockedBirdLevel = data.unlockedBirdLevel;
        streakRecord = data.streakRecord;
        comboRecord = data.comboRecord;
        streakLevelEndlessRecord = data.streakLevelEndlessRecord;
        scoreRecord = data.scoreRecord;
        scoreBirdRecord = data.scoreBirdRecord;
        rankRecord = data.rankRecord;
        rankBirdRecord = data.rankBirdRecord;
        currency = data.currency;
        itemBought = data.itemBought;
        itemActive = data.itemActive;
        lastMode = data.lastMode;

        prices = new int[] {
            100,
            200,
            220,
            80,
            140
        };

        // load data
        GameObject ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[0], data.itemActive[0], "Flatten Combo",  spriteCombo1, prices[0], 0);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[3], data.itemActive[3], "Super Attack", spriteCombo4, prices[3], 3);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[4], data.itemActive[4], "Counter Hit", spriteCombo5, prices[4], 4);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[1], data.itemActive[1], "Charge Punch", spriteCombo2, prices[1], 1);

        ShopListItem = Instantiate(ShopItemContainer) as GameObject;
        ShopListItem.SetActive(true);
        ShopListItem.transform.SetParent(ShopListContent.transform, false);
        ShopListItem.GetComponent<ShopItemContainerHandler>().Init(data.itemBought[2], data.itemActive[2], "Rapid Kicks", spriteCombo3, prices[2], 2);
    }

    void Update()
    {
        // scroll with input
        // scroll with input
        if (Input.GetAxis("NavigateRight") > 0.5f)
        {
            ShopScrollList.verticalNormalizedPosition -= 2.5f * Time.deltaTime;
        }
        if (Input.GetAxis("NavigateLeft") > 0.5f)
        {
            ShopScrollList.verticalNormalizedPosition += 2.5f * Time.deltaTime;
        }

        // back to main menu
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Super"))
        {
            BackToPlayMenu();
        }
    }

    public bool ClickBuy(int ID)
    {
        if (!itemBought[ID])
        {
            if (currency >= prices[ID])
            {
                soundShopBuy.start();
                itemBought[ID] = true;
                currency -= prices[ID];
                txtCurrency.text = "MUNNY: " + currency;
                return true;
            }
            else
            {
                soundShopCannotAfford.start();
            }
        }
        else
        {
            soundUIClick.start();
            itemActive[ID] = !itemActive[ID];
            return true;
        }

        return false;
    }

    public void BackToPlayMenu()
    {
        SaveSystem.SavePlayerShop(this);

        soundUIClick.start();
        menuMusicPlayerHandler.Instance.swapShop(false);
        SceneManager.LoadScene("MenuScene");
    }
}
