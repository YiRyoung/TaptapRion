using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    #region Variables
    [System.Serializable]
    public struct Background
    {
        public GameObject BattleField1;
        public GameObject BattleField2;
    }

    [System.Serializable]
    public struct Sounds
    {
        public AudioSource BattleField1;
        public AudioSource BattleField2;

        [Space(10f)]
        public AudioSource ClickSound;
        public AudioSource AttackSound;
        public AudioSource DeathSound;
        public AudioSource CoinSound;
    }

    [System.Serializable]
    public struct TMP
    {
        public TextMeshProUGUI ATPText;
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI TaxText;
        public TextMeshProUGUI Gold_RestText;
    }

    [System.Serializable]
    public struct MonsterUI
    {
        public GameObject RedSlime;
        public GameObject Dog;
    }

    GameManager gameManager;
    DCanvas Dcanvas;
    ShopSystem shopSystem;
    public GameObject ReceiveScreen;
    public Background background;
    public Sounds sound;
    public TMP tmp;
    public bool isPlay;

    [Space(10f)]
    [Header("Monster")]
    public GameObject MonsterHP;
    public Image MonsterHPBar;
    public MonsterUI MonsterImage;
    #endregion

    #region MonsterSetting
    public void MonsterBalance()
    {
        switch (gameManager.status.ATP)
        {
            case 1:
                GameManager.Instance.MonsterStatus.MonsterMaxHP = 15f;
                GameManager.Instance.MonsterStatus.MonsterCoin = 1f;
                MonsterImage.RedSlime.SetActive(true);
                break;
            case 2:
                GameManager.Instance.MonsterStatus.MonsterMaxHP = 60f;
                GameManager.Instance.MonsterStatus.MonsterCoin = 3f;
                MonsterImage.Dog.SetActive(true);
                break;
        }

        /*
        ATP : 3
        MonsterMaxHP = 186f
        MonsterCoin = 5f
        
        ATP : 4
        MonsterMaxHP = 468f
        MonsterCoin = 7f
        
        ATP : 5
        MonsterMaxHP = 1020f
        MonsterCoin = 10f
        */

        if(gameManager.MonsterStatus.MonsterCurrentHP <= 0)
        { GameManager.Instance.MonsterStatus.MonsterCurrentHP = gameManager.MonsterStatus.MonsterMaxHP; }
    }

    public void ResetMonster()
    {
        MonsterBalance();

        switch(gameManager.status.ATP)
        {
            case 1:
                MonsterImage.RedSlime.SetActive(true);
                break;
            case 2:
                MonsterImage.Dog.SetActive(true);
                break;
        }
        
        MonsterHP.SetActive(true);
    }
    #endregion

    #region ReceiveScreen
    void FinishRest()
    {
        if(gameManager.coin.Gold_TotalRest >= 1)
        {
            ReceiveScreen.SetActive(true);
        }
    }

    public void ReceiveButton()
    {
        sound.CoinSound.Play();
        GameManager.Instance.coin.Gold += (int)GameManager.Instance.coin.Gold_TotalRest;
        GameManager.Instance.coin.Gold_TotalRest = 0;
        ReceiveScreen.SetActive(false);
    }
    #endregion

    void PlaySwitch()
    {
        if( Dcanvas.SettingScreen.activeSelf == true || ReceiveScreen.activeSelf == true
            || shopSystem.ShopScreen.activeSelf == true) { isPlay = false; }
        else { isPlay = true; }
    }

    void LoadBackground()
    {
        switch(gameManager.status.ATP)
        {
            case 1:
                background.BattleField1.SetActive(true);
                sound.BattleField1.Play();
                break;
            case 2:
                background.BattleField1.SetActive(false);
                background.BattleField2.SetActive(true);
                sound.BattleField1.Stop();
                sound.BattleField2.Play();
                break;
        }
    }

    void Awake()
    {
        gameManager = GameManager.Instance;
        Dcanvas = DCanvas.Instance;
        shopSystem = FindObjectOfType<ShopSystem>();
        MonsterBalance();
        ReceiveScreen.SetActive(false);
    }

    void Start()
    {
        FinishRest();
        LoadBackground();
    }

    void Update()
    {
        tmp.ATPText.text = gameManager.status.ATP.ToString();
        tmp.GoldText.text = gameManager.coin.Gold.ToString();
        tmp.TaxText.text = gameManager.coin.Gold_Tax.ToString();
        tmp.Gold_RestText.text = ((int)gameManager.coin.Gold_TotalRest).ToString();
        MonsterHPBar.fillAmount =
        GameManager.Instance.MonsterStatus.MonsterCurrentHP / GameManager.Instance.MonsterStatus.MonsterMaxHP;
        PlaySwitch();
    }
}