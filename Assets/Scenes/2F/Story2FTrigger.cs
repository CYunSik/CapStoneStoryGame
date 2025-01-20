using System.Collections;
using UnityEngine;
using TMPro;

public class Story2FTrigger : MonoBehaviour
{
    public GameObject dialogueUI;            // 대사 UI 오브젝트
    public TextMeshProUGUI dialogueText;     // 대사 텍스트
    public GameObject diaryUI;               // 쪽지 UI 오브젝트
    public AudioSource triggerSound;         // 트리거 사운드
    [TextArea(1, 10)]
    public string[] dialogues;               // 대사 텍스트 배열
    private int currentDialogueIndex = 0;    // 현재 대사 인덱스
    private bool dialogueActive = false;     // 대화 활성화 여부
    private bool isTriggerActivated = false; // 트리거가 이미 작동했는지 확인
    private bool diaryShown = false;         // 쪽지 UI가 이미 표시되었는지 확인

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody;          // 캐릭터 Rigidbody2D 참조
    private Animator playerAnimator;              // 캐릭터 애니메이터 참조

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.instance.is_2FHint == 1 && GameManager.instance.is_2FStoryTrigger == 0 && !isTriggerActivated)
        {
            isTriggerActivated = true; // 트리거가 작동했음을 기록
            PlayTriggerSound();        // 사운드 재생 및 후속 작업 시작
        }
    }

    private void PlayTriggerSound()
    {
        if (triggerSound != null)
        {
            DisablePlayerMovement(); // 캐릭터 움직임 비활성화
            triggerSound.Play();
            StartCoroutine(WaitForSoundToEnd(triggerSound.clip.length)); // 사운드가 끝날 때까지 대기
        }
        else
        {
            StartDialogue(); // 사운드가 없으면 바로 대화 시작
        }
    }

    private IEnumerator WaitForSoundToEnd(float duration)
    {
        yield return new WaitForSeconds(duration); // 사운드 재생 시간 대기
        StartDialogue();                           // 사운드가 끝난 후 대화 시작
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
        }
    }

    private void Update()
    {
        // 대화 UI가 활성화된 경우 Space 키로 대사 진행
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = dialogues[currentDialogueIndex];

            // 다섯 번째 대사에서 쪽지 UI 표시
            if (currentDialogueIndex == 4 && diaryUI != null && !diaryShown)
            {
                ShowDiaryUI();
            }
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void ShowDiaryUI()
    {
        diaryShown = true; // 쪽지 UI가 표시되었음을 기록
        diaryUI.SetActive(true); // 쪽지 UI 활성화
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // 대사 UI 비활성화
        if (diaryUI != null) diaryUI.SetActive(false);       // 쪽지 UI 비활성화
        dialogueActive = false;

        EnablePlayerMovement(); // 캐릭터 움직임 활성화

        // GameManager 상태 업데이트
        if (GameManager.instance != null)
        {
            GameManager.instance.is_2FStoryTrigger = 1; // 트리거 완료 상태로 설정
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
