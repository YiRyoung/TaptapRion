using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    GameManager gameManager;
    DCanvas Dcanvas;
    BattleSystem battleSystem;

    [System.Serializable]
    public struct Sound
    {
        public AudioSource ClickSound;
        public AudioSource BuySound;
        public AudioSource ErrorSound;
    }

    [System.Serializable]
    public struct Items
    {
        public GameObject Sword;
        public GameObject Shield;
        public GameObject Armor;
    }

    public GameObject ShopScreen;
    public GameObject ATPUpMessage;
    public Sound sound;
    public Items items;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    void Product()
    {
        switch(gameManager.status.ATP)
        {
            case 1:
                items.Sword.SetActive(true);
                itemNameText.text = "°­Ã¶ °Ë";
                break;
            case 2:
                items.Sword.SetActive(false);
                items.Shield.SetActive(true);
                itemNameText.text = "°­Ã¶ °©¿Ê";
                break;
            case 3:
                items.Shield.SetActive(false);
                items.Armor.SetActive(true);
                itemNameText.text = "°­Ã¶ Åõ±¸";
                break;
                
        }

        itemPriceText.text = gameManager.coin.Gold_LvUp.ToString();
    }

    public void ShopButton()
    {
        sound.ClickSound.Play();

        if (battleSystem.ReceiveScreen.activeSelf == false && Dcanvas.SettingScreen.activeSelf == false)
        {
            ShopScreen.SetActive(true);
        }
    }

    public void BuyButton()
    {
        if (gameManager.coin.Gold >= gameManager.coin.Gold_LvUp)
        {
            sound.BuySound.Play();
            GameManager.Instance.coin.Gold -= gameManager.coin.Gold_LvUp;
            GameManager.Instance.status.ATP += 1;
            GameManager.Instance.MonsterStatus.MonsterCurrentHP = 0;
            StartCoroutine(LevelUp());
        }
        else { sound.ErrorSound.Play(); }
    }

    IEnumerator LevelUp()
    {
        ATPUpMessage.SetActive(true);
        yield return new WaitForSeconds(1f);
        ATPUpMessage.SetActive(false);
        GameManager.Instance.loadDirection = LoadDirection.Dialogue;
        Dcanvas.SceneChange();
    }

    public void ExitButton()
    {
        sound.ClickSound.Play();
        ShopScreen.SetActive(false);
    }


    void Awake()
    {
        gameManager = GameManager.Instance;
        Dcanvas = DCanvas.Instance;
        battleSystem = FindObjectOfType<BattleSystem>();
        ShopScreen.SetActive(false);
        ATPUpMessage.SetActive(false);
        itemNameText.text = " ";
        itemPriceText.text = " ";
    }

    void Update()
    {
        if (ShopScreen.activeSelf == true) { Product(); }
    }
}
