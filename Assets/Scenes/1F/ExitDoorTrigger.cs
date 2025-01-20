using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ExitDoorTrigger : MonoBehaviour
{
    public string sceneName;                // 이동할 씬의 이름
    public Vector2 playerStartPosition;     // 다음 씬에서 캐릭터가 시작할 위치
    public GameObject dialogueUI;           // 대화 UI (Canvas)
    public TextMeshProUGUI dialogueText;    // TextMeshPro 대화 텍스트
    public Image characterIllustration;     // 캐릭터 일러스트 이미지
    [TextArea(1, 10)]
    public string[] dialogues;              // 출력할 대화 목록

    private bool isPlayerInRange = false;   // 플레이어가 범위 안에 있는지 확인
    private bool isDialogueActive = false;  // 대화 진행 여부
    private int currentDialogueIndex = 0;   // 현재 대사 인덱스

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody;         // 플레이어 Rigidbody2D
    private Animator playerAnimator;              // 캐릭터 애니메이터

    private void Start()
    {
        // 대화 UI와 캐릭터 일러스트 비활성화
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // 플레이어 스크립트와 애니메이터 참조
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
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // 조건: is_1F_Locked == 0 && is_1F_Start == 1
            if (GameManager.instance.is_1F_Locked == 0 && GameManager.instance.is_1F_Start == 1)
            {
                StartDialogue(); // 대화 시작
            }
            // 조건: is_1F_Locked == 1 && has_Exit_Key == 1
            else if (GameManager.instance.is_1F_Locked == 1 && GameManager.instance.has_Exit_Key == 1)
            {
                LoadSceneByName(); // 씬 로드
            }
        }

        // 대화 중 Space 키로 다음 대사 진행
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // 대화 UI 활성화
            isDialogueActive = true;

            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex]; // 첫 번째 대사 출력
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // 캐릭터 이미지 활성화
        }

        DisablePlayerMovement(); // 플레이어 움직임 멈춤
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // 대화 종료
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 대화 UI 비활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // 캐릭터 이미지 비활성화
        }

        EnablePlayerMovement(); // 대화 종료 후 플레이어 움직임 활성화

        // `is_1F_Locked`를 1로 변경
        GameManager.instance.is_1F_Locked = 1;

        isDialogueActive = false;
    }

    private void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SavePlayerStartPosition();
            FadeManager.Instance.LoadSceneWithFade(sceneName);
        }
    }

    private void SavePlayerStartPosition()
    {
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(sceneName, playerStartPosition);
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
