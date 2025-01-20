using UnityEngine;
using TMPro;

public class Elevator4F : MonoBehaviour
{
    public GameObject elevatorUI; // "사용 금지" UI 오브젝트
    public AudioSource warningSound; // 경고음 오디오 소스
    public TMP_Text dialogueText; // TextMeshPro 대화 텍스트
    public GameObject dialogueUI; // 대화 UI (Canvas)
    public string[] dialogues; // 표시할 대사 배열
    public MonoBehaviour playerControllerScript; // 캐릭터 움직임 스크립트

    private Rigidbody2D playerRigidbody; // 캐릭터의 Rigidbody2D
    private Animator playerAnimator; // 캐릭터의 Animator

    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool uiActive = false; // "사용 금지" UI가 활성화되었는지 확인
    private bool dialogueActive = false; // 대화 진행 여부
    private bool hasUIShown = false; // "사용 금지 UI"가 한 번 활성화되었는지 확인
    private bool isActiveTrigger = false; // 현재 트리거가 활성화 상태인지 확인

    private int currentDialogueIndex = 0; // 현재 대사 인덱스

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        if (elevatorUI != null)
        {
            elevatorUI.SetActive(false); // 시작 시 "사용 금지" UI 비활성화
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 시작 시 대화 UI 비활성화
        }

        if (warningSound != null)
        {
            warningSound.Stop(); // 시작 시 경고음 멈춤
        }
    }

    private void Update()
    {
        // 범위 안에 있을 때만 "F" 키 입력을 체크
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.has_Interacted_Elevator == 0)
            {
                // 처음 상호작용 시 대화 시작
                StartDialogue();
            }
            else if (GameManager.instance.has_Interacted_Elevator == 1)
            {
                // 이후 상호작용 시 "사용 금지" UI 활성화/비활성화 전환
                ToggleElevatorUI();
            }
        }

        // 대화 중 스페이스바로 대사 진행
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }

        // 플레이어가 범위 밖으로 나갔을 때 모든 UI 비활성화
        if (!isPlayerInRange)
        {
            DisableAllUI();
        }
    }

    private void ToggleElevatorUI()
    {
        if (elevatorUI != null)
        {
            uiActive = !uiActive; // UI 상태 전환
            elevatorUI.SetActive(uiActive);

            // 경고음은 UI 활성화 시에만 재생
            if (uiActive && warningSound != null)
            {
                warningSound.Play();
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // 대화 UI 활성화
            dialogueActive = true;
            isActiveTrigger = true; // 현재 트리거 활성화

            currentDialogueIndex = 0; // 대화 시작 인덱스 초기화
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]); // 첫 번째 대사 표시

            // 캐릭터 움직임 비활성화
            DisablePlayerMovement();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]); // 다음 대사 표시

            // 3번째 대사일 때 "사용 금지" UI 활성화
            if (currentDialogueIndex == 2 && !hasUIShown)
            {
                ShowElevatorUI(); // "사용 금지" UI 표시
                hasUIShown = true; // 한 번 활성화되었음을 기록
            }
        }
        else
        {
            EndDialogue(); // 대화 종료
        }
    }

    private void ShowElevatorUI()
    {
        if (elevatorUI != null)
        {
            uiActive = true;
            elevatorUI.SetActive(true); // "사용 금지" UI 활성화

            // 경고음 재생
            if (warningSound != null)
            {
                warningSound.Play();
            }
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 대화 UI 비활성화
        }

        dialogueActive = false;
        isActiveTrigger = false;

        // GameManager 상태 업데이트
        GameManager.instance.has_Interacted_Elevator = 1;

        // 캐릭터 움직임 다시 활성화
        EnablePlayerMovement();
    }

    private void DisableAllUI()
    {
        if (elevatorUI != null)
        {
            elevatorUI.SetActive(false); // "사용 금지" UI 비활성화
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 대화 UI 비활성화
        }

        uiActive = false;
        dialogueActive = false;

        if (isActiveTrigger) // 현재 트리거가 활성화 상태일 때만 이동 복구
        {
            EnablePlayerMovement();
            isActiveTrigger = false; // 현재 트리거 비활성화
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro에서 여러 줄로 표시되도록 포맷팅
        return dialogue.Replace("\\n", "\n");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // 범위 안에 들어왔음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 범위 밖으로 나갔음을 표시
            DisableAllUI(); // 범위 밖으로 나가면 모든 UI 비활성화
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

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false; // 이동 스크립트 비활성화
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // 이동 스크립트 활성화
        }
    }
}
