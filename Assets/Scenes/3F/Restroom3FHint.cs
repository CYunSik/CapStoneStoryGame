using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용

public class Restroom3FHint : MonoBehaviour
{
    public GameObject toiletUI; // "화장실힌트" UI 오브젝트
    public AudioSource toiletpapersound; // 게시판 여는 오디오 소스
    public GameObject dialogueCanvas; // 대화창 UI
    public GameObject characterPanel; // 캐릭터 이미지 패널
    public GameObject textPanel; // 텍스트 패널
    public TextMeshProUGUI storyText; // 텍스트 표시용 TextMeshPro
    [TextArea(1, 10)]
    public string[] dialogues; // 대화 텍스트 배열

    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool dialogueActive = false; // 대화 활성화 여부
    private bool uiActive = false; // 화장실 UI 활성화 여부
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

        // 모든 UI 초기화
        if (toiletUI != null)
            toiletUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (toiletpapersound != null)
            toiletpapersound.Stop();
    }

    private void Update()
    {
        // 범위 안에 있을 때만 F 키 입력 처리
        if (isPlayerInRange)
        {
            if (GameManager.instance != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (uiActive)
                    {
                        HideAllUI(); // UI 비활성화
                    }
                    else
                    {
                        if (GameManager.instance.has_3F_Hint == 0 && GameManager.instance.is_3F_Locked == 1)
                        {
                            ShowToiletUIFirst(); // 처음에는 화장실 힌트 UI만 표시
                        }
                        else if (GameManager.instance.has_3F_Hint == 1)
                        {
                            ShowToiletUIOnly(); // 화장실 UI만 활성화
                        }
                    }
                }
            }
        }

        // 대화 중 Space 키 입력 처리
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowToiletUIFirst()
    {
        if (toiletUI != null)
        {
            toiletUI.SetActive(true); // 화장실 힌트 UI만 활성화
            uiActive = true;

            if (toiletpapersound != null)
                toiletpapersound.Play(); // 사운드 재생

            // 캐릭터 이동 차단
            DisablePlayerMovement();

            // 1초 후 나머지 UI를 표시
            Invoke(nameof(StartDialogueWithToiletUI), 1f);
        }
    }

    private void StartDialogueWithToiletUI()
    {
        if (dialogueCanvas != null && dialogues.Length > 0)
        {
            dialogueActive = true;
            currentDialogueIndex = 0;

            // 대화창과 나머지 UI 활성화
            dialogueCanvas.SetActive(true);
            characterPanel.SetActive(true);
            textPanel.SetActive(true);

            // 첫 번째 대화 출력
            storyText.text = dialogues[currentDialogueIndex];
        }
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 텍스트 출력
            storyText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // 대화 종료
            EndDialogueSequence();
        }
    }

    private void EndDialogueSequence()
    {
        // 대화창 비활성화
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;

        // 상태 변경
        GameManager.instance.has_3F_Hint = 1; // 대화 후 힌트 상태 변경

        // 캐릭터 이동 복구
        EnablePlayerMovement();
    }

    private void ShowToiletUIOnly()
    {
        // 대화창 없이 화장실 힌트 UI만 활성화
        if (toiletUI != null)
            toiletUI.SetActive(true);

        if (toiletpapersound != null)
            toiletpapersound.Play(); // 사운드 재생

        // 캐릭터 이동 차단
        DisablePlayerMovement();

        uiActive = true; // UI가 활성화됨을 표시
    }

    private void HideAllUI()
    {
        // 모든 UI를 비활성화
        if (toiletUI != null)
            toiletUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (toiletpapersound != null)
            toiletpapersound.Stop(); // 사운드 중지

        dialogueActive = false;
        uiActive = false;

        // 캐릭터 이동 복구
        EnablePlayerMovement();
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
            isPlayerInRange = true; // 범위 안에 들어왔음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 범위 밖으로 나갔음을 표시

            // 범위를 벗어나면 모든 UI 비활성화 및 이동 복구
            HideAllUI();
        }
    }
}
