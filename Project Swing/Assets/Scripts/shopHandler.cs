using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Analytics;

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

    [HideInInspector] public int endlessRecord;
    [HideInInspector] public bool[] clearedLevel;
    [HideInInspector] public bool[] clearedBirdLevel;
    [HideInInspector] public bool[] clearedHardLevel;
    [HideInInspector] public bool[] unlockedLevel;
    [HideInInspector] public bool[] unlockedBirdLevel;
    [HideInInspector] public bool[] unlockedHardLevel;
    [HideInInspector] public int[] streakRecord;
    [HideInInspector] public int[] comboRecord;
    [HideInInspector] public float[] timeRecord;
    [HideInInspector] public int streakLevelEndlessRecord;
    [HideInInspector] public int[] scoreRecord;
    [HideInInspector] public int[] scoreBirdRecord;
    [HideInInspector] public int[] scoreHardRecord;
    [HideInInspector] public int[] rankRecord;
    [HideInInspector] public int[] rankBirdRecord;
    [HideInInspector] public int[] rankHardRecord;
    [HideInInspector] int currentMaxStreak;
    [HideInInspector] public int currency;
    [HideInInspector] public bool[] itemBought;
    [HideInInspector] public bool[] itemActive;
    [HideInInspector] public int lastMode;
    [HideInInspector] public bool seenTutorial;
    [HideInInspector] public bool seenBirdTutorial;
    [HideInInspector] public int inputSelected;

    EventSystem eventSystem;
    GameObject oldSelected;

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
        menuMusicPlayerHandler.Instance.CheckStarted(true);
        
        soundUIClick = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_klick");
        soundUIStart = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Button_Start");
        soundShopCannotAfford = FMODUnity.RuntimeManager.CreateInstance("event:/Brad/Punch_super_fail");
        soundShopBuy = FMODUnity.RuntimeManager.CreateInstance("event:/Ui/Buy");

        textCurrency.text = data.currency.ToString();

        eventSystem = EventSystem.current;

        data.Init();
        endlessRecord = data.endlessRecord;
        clearedLevel = data.clearedLevel;
        clearedBirdLevel = data.clearedBirdLevel;
        clearedHardLevel = data.clearedHardLevel;
        unlockedLevel = data.unlockedLevel;
        unlockedBirdLevel = data.unlockedBirdLevel;
        unlockedHardLevel = data.unlockedHardLevel;
        streakRecord = data.streakRecord;
        comboRecord = data.comboRecord;
        timeRecord = data.timeRecord;
        streakLevelEndlessRecord = data.streakLevelEndlessRecord;
        scoreRecord = data.scoreRecord;
        scoreBirdRecord = data.scoreBirdRecord;
        scoreHardRecord = data.scoreHardRecord;
        rankRecord = data.rankRecord;
        rankBirdRecord = data.rankBirdRecord;
        rankHardRecord = data.rankHardRecord;
        currency = data.currency;
        itemBought = data.itemBought;
        itemActive = data.itemActive;
        lastMode = data.lastMode;
        seenTutorial = data.seenTutorial;
        seenBirdTutorial = data.seenBirdTutorial;
        inputSelected = data.inputSelected;

        prices = new int[] {
            100,
            200,
            220,
            80,
            140
        };
        
        /*
        prices = new int[] {
            0,
            0,
            0,
            0,
            0
        };
        */
        
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

        oldSelected = eventSystem.currentSelectedGameObject;
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
        
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (eventSystem.currentSelectedGameObject != oldSelected)
            {
                if (eventSystem.currentSelectedGameObject.transform.parent.parent != null && sceneSelectionHandler.Instance.inputIcons != 2)
                {
                    if (eventSystem.currentSelectedGameObject.transform.parent.parent.gameObject == ShopListContent)
                    {
                        if (eventSystem.currentSelectedGameObject.transform.position.y < -3.6f) ShopScrollList.verticalNormalizedPosition = Mathf.Clamp(ShopScrollList.verticalNormalizedPosition - 0.51f, 0, 1);
                        if (eventSystem.currentSelectedGameObject.transform.position.y > 2.6f) ShopScrollList.verticalNormalizedPosition = Mathf.Clamp(ShopScrollList.verticalNormalizedPosition + 0.51f, 0, 1);
                    }
                }
            }
        }

        oldSelected = eventSystem.currentSelectedGameObject;
    }

    public bool ClickBuy(int ID)
    {
        if (!itemBought[ID])
        {
            if (currency >= prices[ID])
            {
                AnalyticsEvent.ItemAcquired(AcquisitionType.Soft, "Shop", 1, "item_ID_" + ID.ToString());
                soundShopBuy.start();
                itemBought[ID] = true;
                currency -= prices[ID];
                txtCurrency.text = currency.ToString();
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
        seenTutorial = true;
        inputSelected = sceneSelectionHandler.Instance.inputIcons;
        SaveSystem.SavePlayerShop(this);

        soundUIClick.start();

        if(menuMusicPlayerHandler.Instance != null)
        menuMusicPlayerHandler.Instance.SwapShop(false);
        SceneManager.LoadScene("MenuScene");
    }
}
