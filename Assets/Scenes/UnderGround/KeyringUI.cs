using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyringUIController : MonoBehaviour
{
    public GameObject keyringUI;          // 키링 UI 오브젝트
    public GameObject player;            // 플레이어 오브젝트
    public GameObject dialoguePanel;     // 대화창 Panel
    public Image characterIllustration;  // 캐릭터 이미지
    public TextMeshProUGUI dialogueText; // 대화 텍스트
    [TextArea(1, 10)]
    public string[] exitKeyDialogues;    // 키링 대화 내용
    public float interactionDistance = 2f; // 상호작용 거리

    private bool isUIActive = false;     // 키링 UI가 활성화되어 있는지 여부
    private bool isDialogueActive = false; // 대화가 진행 중인지 여부
    private int currentDialogueIndex = 0;  // 현재 대화 인덱스

    private TechStudentController playerMovement; // 플레이어 이동 스크립트 참조
    private Rigidbody2D playerRigidbody;         // 플레이어 Rigidbody2D 참조
    private Animator playerAnimator;             // 플레이어 애니메이터 참조

    void Start()
    {
        if (keyringUI != null)
        {
            keyringUI.SetActive(false); // 시작 시 키링 UI 비활성화
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // 대화창 비활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // 캐릭터 이미지 비활성화
        }

        // 플레이어 이동 및 애니메이터 참조
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // B1Cat이 1인지 확인 (GameManager에서 조건 확인)
        if (GameManager.instance.B1Cat == 1)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            // F 키로 키링 UI 활성화
            if (!isDialogueActive && !isUIActive && distance <= interactionDistance && Input.GetKeyDown(KeyCode.F))
            {
                ActivateKeyringUI();
            }

            // 대화 진행 중 Space 키로 대화 넘기기
            if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                ShowNextDialogue();
            }
        }
    }

    private void ActivateKeyringUI()
    {
        if (keyringUI != null)
        {
            keyringUI.SetActive(true); // 키링 UI 활성화
            isUIActive = true;

            // 1초 뒤에 대화창 활성화
            Invoke(nameof(StartDialogue), 1f);
        }

        DisablePlayerMovement(); // 키링 UI가 활성화되면 플레이어 조작 비활성화
    }

    private void StartDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true); // 대화창 활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // 캐릭터 이미지 활성화
        }

        currentDialogueIndex = 0;
        dialogueText.text = exitKeyDialogues[currentDialogueIndex]; // 첫 번째 대사 표시
        isDialogueActive = true;
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < exitKeyDialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = exitKeyDialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // 대화 종료
        }
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // 대화창 비활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // 캐릭터 이미지 비활성화
        }

        if (keyringUI != null)
        {
            keyringUI.SetActive(false); // 키링 UI 비활성화
        }

        isDialogueActive = false;
        isUIActive = false;

        // 플레이어 조작 활성화
        EnablePlayerMovement();

        // GameManager의 has_Exit_Key 값을 1로 설정
        if (GameManager.instance != null)
        {
            GameManager.instance.has_Exit_Key = 1;
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
