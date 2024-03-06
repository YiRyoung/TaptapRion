using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour, IPointerDownHandler
{
    GameManager gameManager;
    DCanvas Dcanvas;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Dcanvas.SettingScreen.activeSelf == false)
        {
            StartingPoint();
            Dcanvas.SceneChange();
        }
    }

    void StartingPoint()
    {
        int Gold_GameOver = Convert.ToInt32(gameManager.coin.Gold_Tax * -3);
        gameManager.coin.Gold -= (gameManager.date.DayDifference * gameManager.coin.Gold_Tax);

        if (gameManager.status.ScriptNum != 0 && gameManager.date.DayDifference <= 5
            && gameManager.coin.Gold >= Gold_GameOver)
        { GameManager.Instance.loadDirection = LoadDirection.Battle; }
        else
        {
            if (gameManager.date.DayDifference > 5)
            { GameManager.Instance.status.ScriptNum = -2; }
            else if (gameManager.coin.Gold < (gameManager.coin.Gold_Tax * -3))
            { GameManager.Instance.status.ScriptNum = -3; }
            GameManager.Instance.loadDirection = LoadDirection.Dialogue;
        }
    }

    void Awake()
    {
        gameManager = GameManager.Instance;
        Dcanvas = DCanvas.Instance;
    }
}
