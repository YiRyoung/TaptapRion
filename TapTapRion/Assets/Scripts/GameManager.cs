using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoadDirection
{
    Pause,
    StartMenu,
    Dialogue,
    Battle
}

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
                    instance = gameManagerObject.AddComponent<GameManager>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    void SingletonToAwake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Variables
    [System.Serializable]
    public struct Status
    {
        public int ScriptNum;
        public int ATP;
    }

    [System.Serializable]
    public struct Coin
    {
        public float Gold;
        public float Gold_LvUp;

        [Space(10f)]
        public float Gold_Tax;
        public float Gold_Rest;
        public int Gold_TotalRest;
    }

    [System.Serializable]
    public struct dateTime
    {
        [HideInInspector] public string LastDayString;
        [HideInInspector] public long LastDayBinary;
        public int DayDifference;
        public int HourDifference;
    }

    [System.Serializable]
    public struct MonsterStat
    {
        public float MonsterMaxHP;
        public float MonsterCurrentHP;
        public float MonsterCoin;
    }

    [Header("Player")]
    public LoadDirection loadDirection;
    public Status status;
    public Coin coin;
    public dateTime date;
    public DateTime LastDay;

    [Space(10f)]
    [Header("Monster")]
    public MonsterStat MonsterStatus;
    
    #endregion

    #region PlayerPrefs
    public void LoadInit()
    {
        // 데이터 불러오기(& 초기값 설정)
        if (PlayerPrefs.HasKey("ScriptNum")) { status.ScriptNum = PlayerPrefs.GetInt("ScriptNum"); }
        else { status.ScriptNum = 0; }
        if (PlayerPrefs.HasKey("ATP")) { status.ATP = PlayerPrefs.GetInt("ATP"); }
        else { status.ATP = 1; }
        if (PlayerPrefs.HasKey("Gold")) { coin.Gold = PlayerPrefs.GetFloat("Gold"); }
        else { coin.Gold = 0; }
        if (PlayerPrefs.HasKey("LastDayString")) { date.LastDayString = PlayerPrefs.GetString("LastDayString"); }
        else { date.LastDayString = null; }
        if (PlayerPrefs.HasKey("MonsterCurrentHP")) { MonsterStatus.MonsterCurrentHP = PlayerPrefs.GetFloat("MonsterCurrentHP"); }
        else { MonsterStatus.MonsterCurrentHP = 0; }
        if (status.ScriptNum != 0) { CheckOffline(); }
    }

    public void SaveInit()
    {
        // 이전에 저장한 데이터가 존재한다면 이전 데이터 삭제
        if (PlayerPrefs.HasKey("LastDayString")) { PlayerPrefs.DeleteAll(); }

        // 현재 데이터를 저장
        PlayerPrefs.SetInt("ATP", status.ATP);
        PlayerPrefs.SetFloat("Gold", coin.Gold);
        PlayerPrefs.SetInt("ScriptNum", status.ScriptNum);
        PlayerPrefs.SetString("LastDayString", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetFloat("MonsterCurrentHP", MonsterStatus.MonsterCurrentHP);
    }

    void CheckOffline()
    {
        // RestCoin 및 오프라인 일수 변수 관리
        long LastDayBinary = Convert.ToInt64(date.LastDayString);
        LastDay = DateTime.FromBinary(LastDayBinary);

        DateTime NewDay = DateTime.Now;
        TimeSpan Timedifference = NewDay.Subtract(LastDay);
        date.DayDifference = Timedifference.Days;
        date.HourDifference = (Timedifference.Hours / 2);    // 2시간 기준
    }
    #endregion

    #region WorldSetting
    void PlayerBalance()
    {
        switch (status.ATP)
        {
            case 1:
                coin.Gold_LvUp = 25f;
                coin.Gold_Tax = 15f;
                coin.Gold_Rest = 0.25f;
                break;
            case 2:
                coin.Gold_LvUp = 84f;
                coin.Gold_Tax = 30f;
                coin.Gold_Rest = 0.84f;
                break;
            case 3:
                coin.Gold_LvUp = 170f;
                coin.Gold_Tax = 60f;
                coin.Gold_Rest = 1.7f;
                break;
            case 4:
                coin.Gold_LvUp = 294f;
                coin.Gold_Tax = 90f;
                coin.Gold_Rest = 2.94f;
                break;
            case 5:
                coin.Gold_LvUp = 550f;
                coin.Gold_Tax = 120f;
                coin.Gold_Rest = 5.5f;
                break;
        }
    }

    public void ZeroSetting()
    {
        status.ScriptNum = 0;
        status.ATP = 1;
        coin.Gold = 0;
        date.DayDifference = 0;
        date.HourDifference = 0;
        MonsterStatus.MonsterCurrentHP = 0;
    }
    #endregion

    void Awake()
    {
        SingletonToAwake();
        LoadInit();
        loadDirection = LoadDirection.Pause;
    }

    void Update()
    {
        PlayerBalance();
    }
}
