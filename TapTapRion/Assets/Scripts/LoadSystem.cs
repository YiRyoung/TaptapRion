using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSystem : MonoBehaviour
{
    GameManager gameManager;
    public GameObject LoadingUI;
    public Slider ProgressBar;
    public TextMeshProUGUI textSpot;


    #region LoadScene by Async
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        LoadingUI.SetActive(true);
        

        // �񵿱�� �� �ε带 �����ϰ� �ε� �۾��� AsyncOperation ������ ����
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // �ε��� �Ϸ�� ������ �ݺ�
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            ProgressBar.value = progress;
            yield return null;
        }

        LoadingUI.SetActive(false);
        Debug.Log("�ε� �Ϸ�");
    }

    public void SceneSwitch()
    {
        switch (gameManager.loadDirection)
        {
            case LoadDirection.StartMenu:
                ChangeScene("StartMenu");
                GameManager.Instance.loadDirection = LoadDirection.Pause;
                break;
            case LoadDirection.Dialogue:
                ChangeScene("Dialogue");
                GameManager.Instance.loadDirection = LoadDirection.Pause;
                break;
            case LoadDirection.Battle:
                SceneManager.LoadSceneAsync("Battle");
                GameManager.Instance.loadDirection = LoadDirection.Pause;
                break;
        }
    }
    #endregion

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Start()
    {
        SceneSwitch();
    }
}
