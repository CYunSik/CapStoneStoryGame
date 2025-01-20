using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용

public class clock2f : MonoBehaviour
{
    public GameObject clockUI; // "시계" UI 오브젝트
    public GameObject dialogueUI; // 대화 UI 오브젝트
    public TextMeshProUGUI dialogueText; // 대화 텍스트
    [TextArea(1, 10)]
    public string[] dialogues; // 출력할 대사 배열

    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool uiActive = false; // UI가 활성화되었는지 확인
    private bool dialogueActive = false; // 대화 진행 여부
    private int currentDialogueIndex = 0; // 현재 대사 인덱스

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody; // 캐릭터 Rigidbody2D 참조
    private Animator playerAnimator; // 캐릭터 애니메이터 참조

    private void Start()
    {
        // UI 초기화
        if (clockUI != null) clockUI.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);

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
        // 범위 안에 있고 F 키를 눌렀을 때 처리
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.is_Today_Time == 0)
            {
                StartDialogueWithClockUI(); // 시계 UI와 대화 UI 동시 활성화
            }
            else if (GameManager.instance.is_Today_Time == 1)
            {
                ToggleClockUI(); // 시계 UI만 토글
            }
        }

        // 대화 중 스페이스바로 대화 진행
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }

        // 범위 밖으로 나갔을 때 UI 비활성화
        if (!isPlayerInRange && uiActive)
        {
            DisableAllUI();
        }
    }

    private void StartDialogueWithClockUI()
    {
        if (clockUI != null) clockUI.SetActive(true); // 시계 UI 활성화
        if (dialogueUI != null) dialogueUI.SetActive(true); // 대화 UI 활성화
        uiActive = true;
        dialogueActive = true;

        // 캐릭터 이동 비활성화
        DisablePlayerMovement();
        GameManager.instance.is_Today_Time = 1; // 상태 업데이트

        // 첫 번째 대화 출력
        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
    }

    private void ToggleClockUI()
    {
        if (clockUI != null)
        {
            uiActive = !uiActive;
            clockUI.SetActive(uiActive);
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대화 출력
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // 대화 UI 비활성화
        dialogueActive = false;

        // 캐릭터 이동 다시 활성화
        EnablePlayerMovement();
    }

    private void DisableAllUI()
    {
        if (clockUI != null) clockUI.SetActive(false); // 시계 UI 비활성화
        if (dialogueUI != null) dialogueUI.SetActive(false); // 대화 UI 비활성화
        uiActive = false;
        dialogueActive = false;

        // 캐릭터 이동 복구
        EnablePlayerMovement();
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
