using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MonsterLevel
{
    RedSlime,
    Dog
}

public class MonsterType : MonoBehaviour, IPointerDownHandler
{
    GameManager gameManager;
    DCanvas Dcanvas;
    BattleSystem battleSystem;
    Animator anim;

    [System.Serializable]
    public struct MonsterSound
    {
        public AudioSource Slime_sound;
        public AudioSource Dog_sound;
    }

    public MonsterLevel MonsterLV;
    public MonsterSound Monstersound;

    #region MonsterSetting
    void PlaySound()
    {
        switch(MonsterLV)
        {
            case MonsterLevel.RedSlime:
                Monstersound.Slime_sound.Play();
                break;
            case MonsterLevel.Dog:
                Monstersound.Dog_sound.Play();
                break;
        }
    }

    void HurtMotion()
    {
        battleSystem.sound.AttackSound.Play();

        switch(MonsterLV)
        {
            case MonsterLevel.RedSlime:
                anim.SetTrigger("Hurt_RedSlime");
                break;
            case MonsterLevel.Dog:
                anim.SetTrigger("Hurt_Dog");
                break;
        }
    }

    void DeathMotion()
    {
        battleSystem.sound.DeathSound.Play();

        switch(MonsterLV)
        {
            case MonsterLevel.RedSlime:
                anim.SetTrigger("Death_RedSlime");
                break;
            case MonsterLevel.Dog:
                anim.SetTrigger("Death_Dog");
                break;
        }
    }
    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        if (battleSystem.isPlay == true)
        {
            // 마우스 위치에 대한 PointerEventData 생성
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            // 결과를 저장할 리스트 생성
            List<RaycastResult> results = new List<RaycastResult>();

            // GraphicRaycaster로 Raycast 실행
            EventSystem.current.RaycastAll(pointerData, results);

            // 결과 처리
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Monster") && gameManager.MonsterStatus.MonsterCurrentHP > 0)
                {
                    HurtMotion();
                    GameManager.Instance.MonsterStatus.MonsterCurrentHP -= gameManager.status.ATP;

                    if (GameManager.Instance.MonsterStatus.MonsterCurrentHP <= 0)
                    {
                        GameManager.Instance.coin.Gold += gameManager.MonsterStatus.MonsterCoin;
                        StartCoroutine(DeathMonster(result.gameObject));
                        Invoke("resetMonster", 1.2f);
                    }
                }
            }
        }
        else { battleSystem.sound.ClickSound.Play(); }
    }

    IEnumerator DeathMonster(GameObject monster)
    {
        DeathMotion();
        yield return new WaitForSeconds(1f);
        monster.SetActive(false);
    }

    void resetMonster() { battleSystem.ResetMonster(); }

    void Awake()
    {
        gameManager = GameManager.Instance;
        battleSystem = FindObjectOfType<BattleSystem>();
        anim = GetComponent<Animator>();
    }
}
