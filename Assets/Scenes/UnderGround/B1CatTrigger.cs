using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class B1CatTrigger : MonoBehaviour
{
    public GameObject catObject;         // 고양이 오브젝트 (애니메이션 포함)
    public AudioSource catSound;        // 고양이가 나타날 때 출력될 사운드
    public Animator catAnimator;        // 고양이 Animator 컴포넌트
    public GameObject dialoguePanel;    // 대화창 Panel
    public Image characterIllustration; // 캐릭터 이미지
    public TextMeshProUGUI dialogueText; // 대화 텍스트

    [TextArea(1, 10)]
    public string[] catDialogues;       // 고양이와 관련된 대사

    private bool hasTriggered = false;  // 트리거가 이미 발동되었는지 여부
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    private bool isDialogueActive = false; // 대화 진행 여부

    private GameManager gameManager;    // GameManager 참조
    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody; // 플레이어 Rigidbody2D 참조
    private Animator playerAnimator; // 플레이어 애니메이터 참조

    void Start()
    {
        // GameManager를 참조합니다.
        gameManager = FindObjectOfType<GameManager>();

        if (catObject != null)
        {
            catObject.SetActive(false); // 시작 시 고양이 오브젝트 비활성화
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
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // 플레이어가 트리거에 진입했을 때
        {
            // GameManager의 조건 검사
            if (gameManager != null && gameManager.DiaryOpen == 1 && gameManager.B1Cat == 0)
            {
                ActivateCatTrigger(); // 트리거 발동
            }
        }
    }

    private void ActivateCatTrigger()
    {
        hasTriggered = true; // 트리거가 한 번만 발동되도록 설정

        DisablePlayerMovement(); // 고양이 애니메이션 시작 전 플레이어 조작 비활성화

        if (catObject != null)
        {
            catObject.SetActive(true); // 고양이 오브젝트 활성화
        }

        if (catAnimator != null)
        {
            
        }
        else
        {
            Debug.LogWarning("Animator가 설정되어 있지 않습니다.");
        }

        if (catSound != null)
        {
            catSound.Play(); // 사운드 출력
        }

        StartCoroutine(ShowCatDialogueAfterDelay()); // 고양이 애니메이션 후 대화 시작
    }

    private IEnumerator ShowCatDialogueAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 고양이 애니메이션 1초 대기

        if (catObject != null)
        {
            catObject.SetActive(false); // 고양이 오브젝트 비활성화
        }

        StartDialogue(); // 대화 시작
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
        dialogueText.text = catDialogues[currentDialogueIndex]; // 첫 번째 대사 표시
        isDialogueActive = true;
    }

    private void Update()
    {
        // 대화 중 Space 키로 대화 넘기기
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < catDialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = catDialogues[currentDialogueIndex];
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

        isDialogueActive = false;

        // 플레이어 조작 활성화
        EnablePlayerMovement();

        // GameManager의 B1Cat 값을 1로 설정
        if (gameManager != null)
        {
            gameManager.B1Cat = 1;
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
