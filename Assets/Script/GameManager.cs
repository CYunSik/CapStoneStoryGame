using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 상호작용 상태 변수 (각 키나 오브젝트와의 상호작용 여부)
    public int is_Elevator_Disabled = 0;
    public int GameStart = 0;
    public int Story4Ffirst = 0;
    public int has_Interacted_Elevator = 0;
    public int is_4F_Locked = 0;
    public int has_4F_KeyHint1 = 0;

    public int is_3F_Start = 0;
    public int has_news_paper = 0;
    public int is_3F_Locked = 0;
    public int has_3F_Hint = 0;

    public int is_2F_Start = 0;
    public int is_Today_Time = 0;
    public int is_Cat = 0;
    public int has_Keyring = 0;
    public int is_2F_Locked = 0;
    public int is_2FHint = 0;
    public int is_2FStoryTrigger = 0;

    public int is_1F_Start = 0;
    public int is_1F_Locked = 0;
    public int PlayCat_1F = 0;
    public int DiaryOpen = 0;
    public int is_B1_Start = 0;
    public int B1Cat = 0;
    public int has_Exit_Key = 0;


    // 캐릭터의 마지막 씬과 위치 정보
    public string currentSceneName;
    public Vector2 currentPosition;

    private void Awake()
    {
        // Singleton 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 상호작용 상태 업데이트 함수
    public void UpdateInteraction(string item, int state)
    {
        switch (item)
        {
            case "Elevateor_Disabled":
                is_Elevator_Disabled = state;
                break;
            case "GameStart":
                GameStart = state;
                break;
            case "Story4Ffirst":
                Story4Ffirst = state;
                break;
            case "Interacted_Elevator":
                has_Interacted_Elevator = state;
                break;
            case "4F_Locked":
                is_4F_Locked = state;
                break;
            case "4F_KeyHint1":
                has_4F_KeyHint1 = state;
                break;


            case "3F_Start":
                is_3F_Start = state;
                break;
            case "3F_Locked":
                is_3F_Locked = state;
                break;
            case "news_paper":
                has_news_paper = state;
                break;
            case "3F_Hint":
                has_3F_Hint = state;
                break;


            case "2F_Start":
                is_2F_Start = state;
                break;
            case "Today_Time":
                is_Today_Time = state;
                break;
            case "Cat":
                is_Cat = state;
                break;
            case "Keyring":
                has_Keyring = state;
                break;
            case "2fHint":
                is_2FHint = state;
                break;
            case "2fLocked":
                is_2F_Locked = state;
                break;
            case "2fStoryTrigger":
                is_2FStoryTrigger = state;
                break;

            case "1F_Start":
                is_1F_Start = state;
                break;
            case "1F_Locked":
                is_1F_Locked = state;
                break;
            case "Exit_Key":
                has_Exit_Key = state;
                break;
            case "catplay":
                PlayCat_1F = state;
                break;
            case "DiaryOpen":
                DiaryOpen = state;
                break;
            case "B1Start":
                is_B1_Start = state;
                break;
            case "B1cat":
                B1Cat = state;
                break;
        }
    }

    // 현재 위치 정보 저장 함수
    public void UpdatePosition(string sceneName, Vector2 position)
    {
        currentSceneName = sceneName;
        currentPosition = position;
    }
}