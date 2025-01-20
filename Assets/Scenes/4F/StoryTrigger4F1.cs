using UnityEngine;
using TMPro;

public class StoryTrigger4F1 : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro UI 텍스트 컴포넌트
    public GameObject dialogueUI; // 대사 UI (Canvas)
    public string[] dialogues; // 표시할 대사 배열
    public GameObject player; // 캐릭터 오브젝트
    private TechStudentController playerMovement; // 캐릭터 움직임 스크립트
    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator; // 캐릭터 애니메이터 참조

    private int currentDialogueIndex = 0; // 현재 대사 인덱스
    private bool dialogueActive = false; // 대화 활성화 여부
    private bool hasTriggered = false; // 트리거가 이미 발동했는지 확인

    private void Start()
    {
        // 캐릭터 이동 스크립트, Rigidbody2D, Animator 참조
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // 시작 시 대화창 비활성화
        dialogueUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // GameManager의 Story4Ffirst가 0일 때만 작동
        if (!hasTriggered && other.gameObject == player && GameManager.instance.Story4Ffirst == 0)
        {
            hasTriggered = true; // 트리거가 한 번만 작동
            GameManager.instance.Story4Ffirst = 1; // Story4Ffirst 값을 1로 변경
            StartDialogue();
        }
    }

    private void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void StartDialogue()
    {
        // 대화 UI 활성화
        dialogueUI.SetActive(true);
        dialogueActive = true;

        // 캐릭터 이동 즉시 멈춤
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // 이동 속도 제거
            playerRigidbody.angularVelocity = 0f;   // 회전 속도 제거
        }

        // 걷는 모션 중지 및 Idle 애니메이션 강제 설정
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle 상태로 전환
        }

        // 캐릭터 이동 스크립트 비활성화
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 첫 번째 대사 표시
        currentDialogueIndex = 0;
        dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 표시
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // 대화 UI 비활성화
        dialogueUI.SetActive(false);
        dialogueActive = false;

        // 캐릭터 이동 스크립트 활성화
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro에서 여러 줄로 표시되도록 포맷팅
        return dialogue.Replace("\\n", "\n");
    }
}
