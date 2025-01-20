using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint4FInteract : MonoBehaviour
{
    private bool isPlayerNearby = false; // 플레이어가 상호작용 범위 안에 있는지 확인
    public GameObject dialogueUI;       // 대사 UI 오브젝트
    public TextMeshProUGUI dialogueText; // 대사 텍스트
    public GameObject diaryUI;          // 실제 쪽지 UI
    public AudioSource diarySound;      // 다이어리 UI 활성화 시 출력할 사운드

    [TextArea(1, 10)]
    public string[] dialogues;          // 대사 텍스트 배열
    private int currentDialogueIndex = 0; // 현재 대사 인덱스
    private bool dialogueActive = false; // 대화 활성화 여부
    private bool uiActive = false;       // 다이어리 UI 활성화 여부

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody; // 캐릭터 Rigidbody2D 참조
    private Animator playerAnimator;    // 캐릭터 애니메이터 참조

    private void Start()
    {
        // UI 초기화
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (diaryUI != null) diaryUI.SetActive(false);

        // 캐릭터 컴포넌트 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // 대사 UI가 활성화된 경우, Space 키로 대사 진행
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
        // F 키를 눌러 대사 시작 또는 다이어리 UI 토글
        else if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.has_4F_KeyHint1 == 0 && !dialogueActive)
                {
                    CheckInteractionConditions(); // 조건 충족 시 대사 시작
                }
                else if (GameManager.instance.has_4F_KeyHint1 == 1)
                {
                    ToggleDiaryUI(); // 다이어리 UI 토글
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // 범위 안에 들어왔음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // 범위 밖으로 나갔음을 표시
            if (uiActive) HideDiaryUI(); // 다이어리 UI가 활성화된 경우 숨김
        }
    }

    private void CheckInteractionConditions()
    {
        // GameManager 조건이 충족될 때만 이벤트 발생
        if (GameManager.instance != null &&
            GameManager.instance.has_Interacted_Elevator == 1 &&
            GameManager.instance.is_4F_Locked == 1)
        {
            StartDialogue(); // 조건을 충족하면 대사 시작
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null && dialogues.Length > 0)
        {
            // 대사 UI 활성화
            dialogueUI.SetActive(true);
            dialogueActive = true;

            // 첫 번째 대사 출력
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex];

            // 캐릭터 움직임 비활성화
            DisablePlayerMovement();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // 대화 종료 후 다이어리 UI 표시
            EndDialogue();
            ShowDiaryUI();

            // GameManager 상태 변경
            if (GameManager.instance != null)
            {
                GameManager.instance.has_4F_KeyHint1 = 1; // 상태 업데이트
            }
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // 대사 UI 비활성화
        dialogueActive = false;

        // 캐릭터 움직임 다시 활성화
        EnablePlayerMovement();
    }

    private void ShowDiaryUI()
    {
        if (diaryUI != null)
        {
            diaryUI.SetActive(true); // 다이어리 UI 활성화
            uiActive = true;

            // 다이어리 사운드 재생
            PlayDiarySound();
        }
    }

    private void ToggleDiaryUI()
    {
        if (diaryUI != null)
        {
            uiActive = !uiActive;
            diaryUI.SetActive(uiActive); // 다이어리 UI 토글

            // 다이어리 UI가 활성화될 때만 사운드 재생
            if (uiActive)
            {
                PlayDiarySound();
            }
        }
    }

    private void HideDiaryUI()
    {
        if (diaryUI != null) diaryUI.SetActive(false); // 다이어리 UI 숨기기
        uiActive = false;
    }

    private void PlayDiarySound()
    {
        if (diarySound != null)
        {
            diarySound.Play(); // 다이어리 사운드 재생
        }
    }

    private void DisablePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // 이동 속도 제거
            playerRigidbody.angularVelocity = 0f;   // 회전 속도 제거
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle 상태로 전환
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false; // 이동 스크립트 비활성화
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true; // 이동 스크립트 활성화
        }
    }
}
