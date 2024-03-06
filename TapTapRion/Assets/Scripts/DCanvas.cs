using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class DCanvas : MonoBehaviour
{
    #region Singleton
    private static DCanvas instance;
    public static DCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DCanvas>();

                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("DCanvas");
                    instance = gameManagerObject.AddComponent<DCanvas>();
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

    GameManager gameManager;
    [Header("Setting")]
    public GameObject SettingScreen;
    public GameObject FinishScreen;
    public GameObject ResetScreen;

    [Space(10f)]
    public CanvasGroup canvasGroup;
    public bool isFading;
    public AudioSource ClickSound;
    public AudioSource WindowSound;

    #region SettingScreen
    void LoadSettingScreen()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickSound.Play();
            SettingScreen.SetActive(!SettingScreen.activeSelf);
        }
    }

    public void ResetButton()
    {
        ClickSound.Play();
        ResetScreen.SetActive(true);
    }

    public void FinishButton()
    {
        ClickSound.Play();
        FinishScreen.SetActive(true);
    }
    #endregion

    #region FinishScreen
    public void YesButton_FinishScreen()
    {
        ClickSound.Play();
        gameManager.SaveInit();
        Application.Quit();
    }

    public void NoButton_FinishScreen()
    {
        ClickSound.Play();
        FinishScreen.SetActive(false);
    }
    #endregion

    #region ResetScreen
    public void YesButton_ResetScreen()
    {
        ClickSound.Play();
        ResetScreen.SetActive(false);
        SettingScreen.SetActive(false);
        gameManager.ZeroSetting();
        SceneChange();
        GameManager.Instance.loadDirection = LoadDirection.StartMenu;
    }

    public void NoButton_ResetScreen()
    {
        ClickSound.Play();
        ResetScreen.SetActive(false);
    }
    #endregion

    #region FadeEffect
    void FadeInteract()
    {
        if (FinishScreen.activeSelf == true) { canvasGroup.interactable = false; }
        else { canvasGroup.interactable = true; }
    }

    public void FadeIn() { StartCoroutine(FadeEffect(true)); }
    public void FadeOut() { StartCoroutine(FadeEffect(false)); }
    public void SceneChange() { StartCoroutine(Transition()); }

    IEnumerator FadeEffect(bool isFadeIn)
    {
        isFading = true;
        if (isFadeIn)
        {
            canvasGroup.alpha = 1;
            Tween tween = canvasGroup.DOFade(0f, 1f);
            yield return tween.WaitForCompletion();
            canvasGroup.gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(true);
            Tween tween = canvasGroup.DOFade(1f, 1f);
            yield return tween.WaitForCompletion();

        }
        isFading = false;
    }

    IEnumerator Transition()
    {
        WindowSound.Play();
        FadeOut();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("Load");
        FadeIn();
    }
    #endregion

    void Awake()
    {
        SingletonToAwake();
        gameManager = GameManager.Instance;
        canvasGroup.alpha = 0f;
        isFading = false;
        SettingScreen.SetActive(false);
    }

    void Update()
    {
        LoadSettingScreen();
        FadeInteract();
    }
}
