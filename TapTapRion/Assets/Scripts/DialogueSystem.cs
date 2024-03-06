using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using KoreanTyper;

public class DialogueSystem : MonoBehaviour, IPointerDownHandler
{
    GameManager gameManager;
    DCanvas Dcanvas;

    #region JsonData
    [System.Serializable]
    public struct DialogueData
    {
        public int StoryNum;
        public Log[] Logs;
    }

    [System.Serializable]
    public struct Log
    {
        public int ID;
        public string log;
    }

    [System.Serializable]
    public struct DialogueCollection
    {
        public DialogueData[] Dialogue;
    }

    public Dictionary<int, DialogueData> dialogueDictionary;

    void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogue");
        if (jsonFile != null)
        {
            // Json �����͸� DialogueCollection���� ��ȯ -> Dialogue[] ������ Dialogue
            DialogueCollection dialogueCollection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
            dialogueDictionary = new Dictionary<int, DialogueData>();
            foreach (DialogueData dialogueData in dialogueCollection.Dialogue)
            {
                dialogueDictionary.Add(dialogueData.StoryNum, dialogueData);
            }
        }
        else { Debug.LogError("Json ������ ã�� �� �����ϴ�."); }
    }
    #endregion

    #region Variables
    [System.Serializable]
    public struct Background
    {
        public GameObject CastlePath;
        public GameObject CastleBallroom;
        public GameObject BattleField;
    }

    [System.Serializable]
    public struct Character
    {
        public GameObject Rion;
        public GameObject Kai;
        public GameObject Lena_Right;
        public GameObject Lena_Left;
    }

    [System.Serializable]
    public struct Dialogue
    {
        public GameObject MessageBox;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI MessageText;

        public bool isTyping;
        [HideInInspector] public int WhoSpeaking;
        [HideInInspector] public int currentLineIndex;
        [HideInInspector] public string currentSentence;
    }

    [System.Serializable]
    public struct Sounds
    {
        public AudioSource ClickSound;

        [Space(10f)]
        public AudioSource BadEndingSound;
        public AudioSource CastleRoadSound;
        public AudioSource CastleSound;
        public AudioSource BattleFieldSound;
        public AudioSource FunnySound;
    }

    public GameObject BattleUI;
    public GameObject StorySkip;

    [Space(10f)]
    public Background background;
    public Character character;
    public Sounds sound;
    public Dialogue dialogue;

    Color ShadowColor = new Color(150 / 255f, 150 / 255f, 150 / 255f);
    Color OrientColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
    #endregion

    #region Dialogue
    // ��ȭ ����
    void StartDialogue()
    {
        if (gameManager.status.ScriptNum == 3)
        {
            BattleUI.SetActive(true);
            StorySkip.SetActive(false);
        }

        dialogue.currentLineIndex = 0;
        dialogue.currentSentence = dialogueDictionary[gameManager.status.ScriptNum].Logs[dialogue.currentLineIndex].log;
        dialogue.isTyping = true;
        StartCoroutine(StartTyping());
        LoadBackGround(gameManager.status.ScriptNum);
        SetSpeaker();
    }

    // ���� �ٷ� �Ѿ��
    void NextLine()
    {
        dialogue.currentLineIndex++;
        if (dialogue.currentLineIndex < dialogueDictionary[gameManager.status.ScriptNum].Logs.Length)
        {
            dialogue.currentSentence = dialogueDictionary[gameManager.status.ScriptNum].Logs[dialogue.currentLineIndex].log;
            dialogue.isTyping = true;
            StartCoroutine(StartTyping());
            SetSpeaker();
        }
        else { EndDialogue(); }
    }

    // ��ȭ ��
    void EndDialogue()
    {
        dialogue.NameText.text = null;
        dialogue.MessageText.text = null;
        Off();

        if (gameManager.status.ScriptNum == 3) { BattleUI.SetActive(false); StorySkip.SetActive(false); }

        GameManager.Instance.status.ScriptNum++;
        NextCommand();
    }

    // ��ȭ�� ���� �� ����Ǵ� ����
    void NextCommand()
    {
        Dcanvas.SceneChange();

        switch (gameManager.status.ScriptNum)
        {
            case -2:
            case -1:
                gameManager.ZeroSetting();
                GameManager.Instance.loadDirection = LoadDirection.StartMenu;
                break;
            case 1:
            case 2:
            case 3:
                GameManager.Instance.loadDirection = LoadDirection.Dialogue;
                break;
            case 4:
            case 5:
                GameManager.Instance.loadDirection = LoadDirection.Battle;
                break;
        }
    }

    public void SkipButton()
    {
        if (Dcanvas.SettingScreen.activeSelf == false)
        EndDialogue();
    }
    #endregion

    #region UI
    // ��� �ҷ�����
    void LoadBackGround(int storyNum)
    {
        switch (storyNum)
        {
            case 0:
                background.CastlePath.SetActive(true);
                break;
            case 1:
                background.CastleBallroom.SetActive(true);
                break;
            case 2:
                background.CastleBallroom.SetActive(true);
                break;
            case 3:
            case 4:
                background.BattleField.SetActive(true);
                break;
        }
    }

    // ĳ���� �ҷ�����
    void ProfileActive(int whoSpeaking)
    {
        switch (whoSpeaking)
        {
            case 0:
                dialogue.NameText.text = " ";
                Off_Characters();
                break;
            case 1:
                dialogue.NameText.text = "����";
                if (character.Rion.activeSelf == false)
                {
                    character.Rion.SetActive(true);
                    character.Rion.GetComponent<Image>().color = OrientColor;
                }
                else { character.Rion.GetComponent<Image>().color = OrientColor; }
                break;
            case 2:
                dialogue.NameText.text = "ī��";
                if (character.Kai.activeSelf == false)
                {
                    character.Kai.SetActive(true);
                    character.Kai.GetComponent<Image>().color = OrientColor;
                }
                else { character.Kai.GetComponent<Image>().color = OrientColor; }
                break;
            case 3:
                dialogue.NameText.text = "����";
                if (character.Lena_Right.activeSelf == false)
                {
                    character.Lena_Right.SetActive(true);
                    character.Lena_Right.GetComponent<Image>().color = OrientColor;
                }
                else { character.Lena_Right.GetComponent<Image>().color = OrientColor; }
                break;
            case 4:
                dialogue.NameText.text = "����";
                if (character.Lena_Left.activeSelf == false)
                {
                    character.Lena_Left.SetActive(true);
                    character.Lena_Left.GetComponent<Image>().color = OrientColor;
                }
                else { character.Lena_Left.GetComponent<Image>().color = OrientColor; }
                break;
            case 5:
                dialogue.NameText.text = " ";
                if (character.Rion.activeSelf == false)
                {
                    character.Rion.SetActive(true);
                    character.Rion.GetComponent<Image>().color = OrientColor;
                }
                else { character.Rion.GetComponent<Image>().color = OrientColor; }
                break;
            case 6:
                dialogue.NameText.text = " ";
                if (character.Kai.activeSelf == false)
                {
                    character.Kai.SetActive(true);
                    character.Kai.GetComponent<Image>().color = OrientColor;
                }
                else { character.Kai.GetComponent<Image>().color = OrientColor; }
                break;
            case 7:
                dialogue.NameText.text = " ";
                if (character.Lena_Right.activeSelf == false)
                {
                    character.Lena_Right.SetActive(true);
                    character.Lena_Right.GetComponent<Image>().color = OrientColor;
                }
                else { character.Lena_Right.GetComponent<Image>().color = OrientColor; }
                break;
            case 8:
                dialogue.NameText.text = " ";
                if (character.Lena_Left.activeSelf == false)
                {
                    character.Lena_Left.SetActive(true);
                    character.Lena_Left.GetComponent<Image>().color = OrientColor;
                }
                else { character.Lena_Left.GetComponent<Image>().color = OrientColor; }
                break;
        }
    }

    // ��� ���� ����
    void Off_BackGrounds()
    {
        background.CastlePath.SetActive(false);
        background.CastleBallroom.SetActive(false);
        background.BattleField.SetActive(false);
    }

    // ĳ���� ���� ����
    void Off_Characters()
    {
        character.Rion.SetActive(false);
        character.Kai.SetActive(false);
        character.Lena_Right.SetActive(false);
        character.Lena_Left.SetActive(false);
    }

    // ���� ��ȭ�ϴ��� (�ι� ������ �� ������ ȿ�� ����)
    void SetSpeaker()
    {
        Shadow();
        ProfileActive(dialogueDictionary[gameManager.status.ScriptNum].Logs[dialogue.currentLineIndex].ID);
    }

    // ��� UI ����
    void Off()
    {
        Off_Characters();
        Off_BackGrounds();
    }
    #endregion

    #region Effect
    // ���� ��ȭ ����� �ƴ� ��� �׸��� ȿ��
    void Shadow()
    {
        if (character.Rion.activeSelf == true) { character.Rion.GetComponent<Image>().color = ShadowColor; }
        if (character.Kai.activeSelf == true) { character.Kai.GetComponent<Image>().color = ShadowColor; }
        if (character.Lena_Right.activeSelf == true) { character.Lena_Right.GetComponent<Image>().color = ShadowColor; }
        if (character.Lena_Left.activeSelf == true) { character.Lena_Left.GetComponent<Image>().color = ShadowColor; }
    }

    // Ÿ���� ȿ��
    IEnumerator StartTyping()
    {
        int typingLength = dialogue.currentSentence.GetTypingLength();
        int typingIndex = 0;
        dialogue.MessageText.text = "";

        while (typingIndex <= typingLength)
        {
            if (!dialogue.isTyping) { break; }

            dialogue.MessageText.text = dialogue.currentSentence.Typing(typingIndex);
            typingIndex++;
            yield return new WaitForSeconds(0.03f);
        }
    }

    // Ÿ���� �� Ŭ������ ���
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
        if (Dcanvas.isFading == false && Dcanvas.SettingScreen.activeSelf == false)
        {
            if (clickedObject == null)
            {
                if (dialogue.isTyping)
                {
                    sound.ClickSound.Play();
                    dialogue.MessageText.text = dialogue.currentSentence;
                    dialogue.isTyping = false;
                }
                else { sound.ClickSound.Play(); NextLine(); }
            }
        }
    }

    void BackGroundSoundManager()
    {
        switch (gameManager.status.ScriptNum)
        {
            case -2:
            case -3:
                sound.BadEndingSound.Play();
                break;
            case 0:
                OffSound();
                sound.CastleRoadSound.Play();
                break;
            case 1:
            case 2:
                OffSound();
                sound.CastleSound.Play();
                break;
            case 3:
                OffSound();
                sound.BattleFieldSound.Play();
                break;
            case 4:
                OffSound();
                sound.FunnySound.Play();
                break;
        }
    }

    void OffSound()
    {
        sound.CastleRoadSound.Stop();
        sound.CastleSound.Stop();
        sound.BattleFieldSound.Stop();
    }
    #endregion

    void Awake()
    {
        gameManager = GameManager.Instance;
        Dcanvas = DCanvas.Instance;
        LoadDialogueData();
    }

    void Start()
    {
        dialogue.isTyping = false;
        BattleUI.SetActive(false);
        StorySkip.SetActive(true);
        StartDialogue();
        OffSound();
        BackGroundSoundManager();
    }
}
