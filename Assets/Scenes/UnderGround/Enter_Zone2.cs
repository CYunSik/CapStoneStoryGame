using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class Enter_Zone2 : MonoBehaviour
{
    public GameObject diaryPanel;       // 일기장 패널
    public GameObject enterUIText;     // "F를 눌러 일기를 확인하세요" 텍스트
    public GameObject dialoguePanel;   // 대화창 Panel
    public UnityEngine.UI.Image characterIllustration; // Unity의 UI 이미지
    public TextMeshProUGUI dialogueText; // 대화 텍스트

    [TextArea(1, 10)]
    public string[] hintDialogues;     // 일기장을 열기 전 대사
    [TextArea(1, 10)]
    public string[] afterDiaryDialogues; // 일기장을 닫은 후 대사

    private enum InteractionState { Idle, DialogueHint, DiaryOpen, DialogueAfterDiary }
    private InteractionState currentState = InteractionState.Idle; // 현재 상태

    private bool isPlayerInZone = false; // 플레이어가 영역에 있는지 여부
    private int currentDialogueIndex = 0;  // 현재 대화 인덱스

    private TechStudentController playerMovement; // 플레이어 이동 스크립트 참조
    private Rigidbody2D playerRigidbody; // 플레이어 Rigidbody2D 참조
    private Animator playerAnimator; // 플레이어 애니메이터 참조

    private void Start()
    {
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(false); // 시작 시 일기장 패널 비활성화
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // 시작 시 대화창 비활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // 시작 시 캐릭터 이미지 비활성화
        }

        if (enterUIText != null)
        {
            enterUIText.SetActive(false); // "F를 눌러 일기를 확인하세요" 텍스트 비활성화
        }

        // 플레이어 관련 컴포넌트 참조
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
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            HandleFKeyInput();
        }

        if ((currentState == InteractionState.DialogueHint || currentState == InteractionState.DialogueAfterDiary)
            && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void HandleFKeyInput()
    {
        switch (currentState)
        {
            case InteractionState.Idle:
                StartDialogue(hintDialogues, InteractionState.DialogueHint); // 힌트 대사 시작
                break;
            case InteractionState.DiaryOpen:
                CloseDiary(); // 일기장을 닫고 후속 대화 시작
                break;
        }
    }

    private void StartDialogue(string[] dialogues, InteractionState nextState)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true);
        }

        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        currentState = nextState;

        DisablePlayerMovement(); // 대화 시작 시 플레이어 조작 비활성화
    }

    private void ShowNextDialogue()
    {
        if (currentState == InteractionState.DialogueHint)
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < hintDialogues.Length)
            {
                dialogueText.text = hintDialogues[currentDialogueIndex];
            }
            else
            {
                EndDialogue();
                OpenDiary(); // 힌트 대화가 끝나면 일기장 열기
            }
        }
        else if (currentState == InteractionState.DialogueAfterDiary)
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < afterDiaryDialogues.Length)
            {
                dialogueText.text = afterDiaryDialogues[currentDialogueIndex];
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        if (currentState == InteractionState.DialogueHint)
        {
            currentState = InteractionState.Idle; // 대화 종료 후 Idle로 복귀
        }
        else if (currentState == InteractionState.DialogueAfterDiary)
        {
            currentState = InteractionState.Idle; // 후속 대화 종료 후 Idle로 복귀
            EnablePlayerMovement(); // 플레이어 조작 활성화

            // GameManager의 DiaryOpen 변수를 1로 설정
            if (GameManager.instance != null && DiaryController.instance.currentPage == DiaryController.instance.pageTexts.Length - 1)
            {
                GameManager.instance.DiaryOpen = 1;
            }
        }
    }


    private void OpenDiary()
    {
        // 대화창 닫기
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // 일기장 열기
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(true);
        }

        currentState = InteractionState.DiaryOpen; // 상태를 DiaryOpen으로 설정
    }

    private void CloseDiary()
    {
        // 일기장 닫기
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(false);
        }

        StartDialogue(afterDiaryDialogues, InteractionState.DialogueAfterDiary); // 일기장을 닫은 후 대화 시작
    }

    private void DisablePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle");
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            if (enterUIText != null)
            {
                enterUIText.SetActive(true); // "F를 눌러 일기를 확인하세요" 텍스트 표시
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (enterUIText != null)
            {
                enterUIText.SetActive(false);
            }

            if (diaryPanel != null)
            {
                diaryPanel.SetActive(false);
            }

            currentState = InteractionState.Idle; // 상태 초기화
            EnablePlayerMovement(); // 플레이어 조작 활성화
        }
    }
}

