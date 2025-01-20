using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용

public class NoteboardInteract : MonoBehaviour
{
    public GameObject noteboardUI; // 게시판 신문 UI 오브젝트
    public GameObject dialogueCanvas; // 대화창 캔버스
    public GameObject characterPanel; // 캐릭터 이미지 패널
    public GameObject textPanel; // 텍스트 패널
    public TextMeshProUGUI storyText; // Story 텍스트 (TextMeshPro)

    public AudioSource noteboardsound; // 게시판 여는 오디오 소스
    [TextArea(1, 10)]
    public string[] dialogues; // 출력할 대화 텍스트 배열

    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool dialogueActive = false; // 대화창 활성화 여부
    private bool uiActive = false; // 게시판 UI 활성화 여부
    private int currentDialogueIndex = 0; // 현재 대화 텍스트 인덱스

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트
    private Rigidbody2D playerRigidbody; // 캐릭터 Rigidbody2D
    private Animator playerAnimator; // 캐릭터 Animator

    private void Start()
    {
        // 캐릭터 컴포넌트 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // UI 초기화
        if (noteboardUI != null)
            noteboardUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (noteboardsound != null)
            noteboardsound.Stop();
    }

    private void Update()
    {
        // 범위 내에서 F 키 입력을 처리
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.has_news_paper == 0)
            {
                ShowInitialNoteboardUI(); // 첫 번째 UI 표시
            }
            else if (GameManager.instance.has_news_paper == 1)
            {
                ToggleNoticeboardUI(); // 게시판 UI 토글
            }
        }

        // 대화 중 Space 키 입력 처리
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowInitialNoteboardUI()
    {
        if (noteboardUI != null)
        {
            noteboardUI.SetActive(true);
            uiActive = true;

            // 게시판 UI 사운드 재생
            if (noteboardsound != null)
                noteboardsound.Play();

            // 캐릭터 이동 차단
            DisablePlayerMovement();

            // 2초 뒤 나머지 UI 표시
            Invoke(nameof(ShowRemainingUI), 1f);
        }
    }

    private void ShowRemainingUI()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (characterPanel != null)
            characterPanel.SetActive(true);

        if (textPanel != null)
            textPanel.SetActive(true);

        dialogueActive = true; // 대화 활성화
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex == 0)
        {
            // 처음 Space 키 입력 시 텍스트 출력
            storyText.text = dialogues[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 텍스트 출력
            storyText.text = dialogues[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            // 대화 종료
            EndDialogueSequence();
        }
    }

    private void EndDialogueSequence()
    {
        // 모든 UI 비활성화
        if (noteboardUI != null)
            noteboardUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;
        uiActive = false;
        currentDialogueIndex = 0;

        // 캐릭터 이동 복구
        EnablePlayerMovement();

        // `has_news_paper` 상태 변경
        GameManager.instance.has_news_paper = 1;
    }

    private void ToggleNoticeboardUI()
    {
        if (noteboardUI != null)
        {
            uiActive = !uiActive;
            noteboardUI.SetActive(uiActive); // 게시판 UI 활성화/비활성화

            if (uiActive)
            {
                // 게시판 UI 열릴 때 이동 차단
                DisablePlayerMovement();

                if (noteboardsound != null)
                    noteboardsound.Play();
            }
            else
            {
                // 게시판 UI 닫힐 때 이동 복구
                EnablePlayerMovement();
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // 플레이어가 범위 안에 있음
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 플레이어가 범위 밖으로 나감

            // 범위를 벗어나면 UI 비활성화 및 이동 복구
            if (uiActive && noteboardUI != null)
            {
                noteboardUI.SetActive(false);
                EnablePlayerMovement();
            }
        }
    }
}
