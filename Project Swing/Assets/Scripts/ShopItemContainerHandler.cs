using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemContainerHandler : MonoBehaviour
{
    public Image imgBg;
    public Text txtName;
    public Text txtPrice;
    public Image imgCombo;
    public Text txtButton;
    bool bought;
    bool active;
    int id;
    GameObject mainHandler;
    public Sprite spriteBgGreen;

    void Start()
    {
        mainHandler = GameObject.Find("Main Camera");
    }

    public void Init(bool isBought, bool isActive, string name, Sprite comboImage, int price, int ID)
    {
        bought = isBought;
        active = isActive;
        txtPrice.text = price.ToString();
        if (bought)
        {
            txtPrice.enabled = false;
            imgBg.sprite = spriteBgGreen;
            if (!active)
            {
                active = false;
                txtButton.text = "Inactive";
                txtButton.color = new Color(1f, 0.3f, 0.3f);
            }
            else
            {
                active = true;
                txtButton.text = "Active";
                txtButton.color = new Color(0.7f, 1f, 0.3f);
            }
        }
        txtName.text = name;
        imgCombo.sprite = comboImage;
        id = ID;
    }
    
    void Update()
    {
        
    }

    public void ClickBuy()
    {
        if (mainHandler.GetComponent<shopHandler>().ClickBuy(id))
        {
            if (!bought)
            {
                txtPrice.enabled = false;
                bought = true;
                active = true;
                txtButton.text = "Active";
                imgBg.sprite = spriteBgGreen;
            }
            else if (active)
            {
                active = false;
                txtButton.text = "Inactive";
                txtButton.color = new Color(1f, 0.3f, 0.3f);
            }
            else
            {
                active = true;
                txtButton.text = "Active";
                txtButton.color = new Color(0.7f, 1f, 0.3f);
            }
        }

    }
}
