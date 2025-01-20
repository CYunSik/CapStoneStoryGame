using System.Collections;
using UnityEngine;
using TMPro;

public class Cat : MonoBehaviour
{
    public Animator targetAnimator;         // Cat 오브젝트의 Animator 컴포넌트
    public GameObject catObject;            // Cat 오브젝트
    public GameObject keyringUI;            // 곰돌이 키링 UI 오브젝트
    public GameObject dialogueUI;           // 대사 UI 오브젝트
    public TextMeshProUGUI dialogueText;    // 대사 텍스트
    [TextArea(1, 10)]
    public string[] dialogues;              // 출력할 대사 배열
    public float hideDelay = 2.0f;          // 고양이 오브젝트가 사라질 때까지의 지연 시간
    private bool uiActive = false;          // UI가 활성화되었는지 확인
    private bool isPlayerInRange = false;   // 플레이어가 트리거 범위 안에 있는지 확인

    private bool isAnimationPlaying = false;
    private bool hasShownMonologue = false; // 독백 대사 표시 여부

    public AudioSource catSound;            // 고양이가 나타날 때 재생할 소리
    public AudioSource keyringSound;        // 키링 UI 표시 시 재생할 소리
    private int currentDialogueIndex = 0;   // 현재 대사 인덱스
    private bool dialogueActive = false;    // 대화 활성화 여부

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody;          // 캐릭터 Rigidbody2D 참조
    private Animator playerAnimator;              // 캐릭터 애니메이터 참조

    private void Start()
    {
        // UI 초기화
        if (catObject != null) catObject.SetActive(false);
        if (keyringUI != null) keyringUI.SetActive(false);
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
        // 트리거 영역 안에 있을 때만 작동
        if (isPlayerInRange)
        {
            // 고양이 오브젝트 활성화 조건
            if (Input.GetKeyDown(KeyCode.F) && GameManager.instance.is_Cat == 0
                && GameManager.instance.is_Today_Time == 1)
            {
                if (!isAnimationPlaying && !hasShownMonologue)
                {
                    catObject.SetActive(true);         // 고양이 오브젝트 활성화
                    isAnimationPlaying = true;         // 애니메이션 상태 설정
                    GameManager.instance.is_Cat = 1;

                    // 고양이가 나타날 때 소리 재생
                    if (catSound != null)
                    {
                        catSound.Play();
                    }

                    DisablePlayerMovement(); // 캐릭터 움직임 비활성화
                    StartCoroutine(HideCatAfterDelay());
                    hasShownMonologue = true;          // 독백 표시 상태 설정
                }
            }

            // F 키를 눌러 키링 UI 토글
            if (Input.GetKeyDown(KeyCode.F) && GameManager.instance.has_Keyring == 1)
            {
                ToggleKeyringUI();
            }

            // 대화 중 스페이스바로 대화 진행
            if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceDialogue();
            }
        }
    }

    private void ToggleKeyringUI()
    {
        if (keyringUI != null)
        {
            uiActive = !uiActive;              // UI 상태 전환
            keyringUI.SetActive(uiActive);     // UI 활성화 상태 적용
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = dialogues[currentDialogueIndex];

            // 세 번째 대사부터 키링 UI 표시
            if (currentDialogueIndex == 2 && keyringUI != null)
            {
                keyringUI.SetActive(true);
                uiActive = true;

                // 키링 획득 상태 설정
                GameManager.instance.has_Keyring = 1;

                // 키링 UI 표시 시 사운드 재생
                if (keyringSound != null)
                {
                    keyringSound.Play();
                }
            }
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // 대사 UI 비활성화
        dialogueActive = false;

        EnablePlayerMovement(); // 캐릭터 움직임 활성화
    }

    private IEnumerator HideCatAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        OnAnimationEnd();
    }

    public void OnAnimationEnd()
    {
        if (catObject != null) catObject.SetActive(false); // 고양이 오브젝트 숨기기
        isAnimationPlaying = false; // 애니메이션 상태 초기화

        // 고양이 애니메이션이 끝난 후 대사 UI 출력
        if (dialogueUI != null && GameManager.instance.has_Keyring == 0)
        {
            dialogueUI.SetActive(true);
            dialogueActive = true;

            // 첫 번째 대사 출력
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // 플레이어가 범위 안에 들어오면 true로 설정
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // 플레이어가 트리거 범위를 벗어나면 false로 설정
            if (uiActive)             // UI가 활성화된 경우
            {
                if (keyringUI != null) keyringUI.SetActive(false); // UI 비활성화
                uiActive = false;           // UI 상태 초기화
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
}
