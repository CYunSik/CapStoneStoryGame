using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro 사용

public class Keypad3FLock : MonoBehaviour
{
    public string correctPassword = "1234";        // 설정할 비밀번호
    private string inputCode = "";                 // 입력된 코드 저장
    private bool isUnlocked = false;               // 잠금 해제 여부 확인
    private bool isPlayerNearby = false;           // 플레이어가 문 앞에 있는지 확인
    private bool isTransitioning = false;          // 씬 전환 중인지 확인

    public UnityEngine.UI.Text displayText;        // 키패드 입력을 표시하는 UI 텍스트
    public GameObject keypadUI;                    // 키패드 UI 오브젝트

    public string nextSceneName;                   // 이동할 다음 씬의 이름
    public Vector2 playerStartPosition;            // 다음 씬에서 캐릭터가 시작할 위치

    public GameObject dialogueCanvas;              // 대화창 UI
    public GameObject characterPanel;              // 캐릭터 이미지 패널
    public GameObject textPanel;                   // 텍스트 패널
    public TextMeshProUGUI storyText;              // 텍스트 표시용 TextMeshPro
    [TextArea(1, 10)]
    public string[] dialogues;                     // 대화 텍스트 배열

    private int currentDialogueIndex = 0;          // 현재 대화 텍스트 인덱스
    private bool dialogueActive = false;           // 대화 활성화 여부

    public AudioClip unlockSound;                  // 비밀번호 맞췄을 때 사운드
    public AudioClip errorSound;                   // 비밀번호 틀렸을 때 사운드
    private AudioSource audioSource;               // 오디오 소스

    private TechStudentController playerMovement;  // 캐릭터 이동 스크립트
    private Rigidbody2D playerRigidbody;           // 캐릭터 Rigidbody2D
    private Animator playerAnimator;               // 캐릭터 Animator

    private const string UnlockedKey = "IsDoorUnlocked"; // 잠금 해제 상태 저장을 위한 키

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 캐릭터 컴포넌트 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // 시작 시 UI 비활성화
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (keypadUI != null)
            keypadUI.SetActive(false);

        // 잠금 해제 상태 초기화
        isUnlocked = false;
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.is_3F_Locked == 0)
            {
                StartDialogue(); // 대화창 시작
            }
            else if (GameManager.instance.is_3F_Locked == 1 && !isUnlocked)
            {
                // 키패드 상호작용 시작 (비밀번호 필요)
                InteractWithDoor();
            }
            else if (GameManager.instance.is_3F_Locked == 1 && isUnlocked)
            {
                // 비밀번호 입력 성공 시 문 열기
                OpenDoor();
            }
        }

        // 대화 중 Space 키 입력 처리
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogueCanvas != null && dialogues.Length > 0)
        {
            dialogueActive = true;
            currentDialogueIndex = 0;

            dialogueCanvas.SetActive(true);
            characterPanel.SetActive(true);
            textPanel.SetActive(true);

            storyText.text = dialogues[currentDialogueIndex];

            // 캐릭터 이동 차단
            DisablePlayerMovement();
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
        // 모든 대화 UI 비활성화
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;

        // 상태 변경
        GameManager.instance.is_3F_Locked = 1; // 대화 후 문 잠금 상태로 변경

        // 캐릭터 이동 복구
        EnablePlayerMovement();
    }

    public void InteractWithDoor()
    {
        if (!keypadUI.activeSelf)
        {
            keypadUI.SetActive(true); // 키패드 UI 활성화
            inputCode = "";           // 입력 초기화
            displayText.text = "";    // 화면 초기화
        }
    }

    public void PressButton(string number)
    {
        if (isUnlocked) return; // 이미 잠금이 해제된 상태라면 입력 무시

        if (inputCode.Length < correctPassword.Length)
        {
            inputCode += number;
            displayText.text = inputCode;
        }
    }

    public void CheckPassword()
    {
        if (isUnlocked) return; // 이미 잠금이 해제된 상태라면 추가 검증 불필요

        if (inputCode == correctPassword)
        {
            isUnlocked = true;
            PlayerPrefs.SetInt(UnlockedKey, 1);
            PlayerPrefs.Save();
            displayText.text = "Unlocked";
            keypadUI.SetActive(false);
            PlaySound(unlockSound);
        }
        else
        {
            PlaySound(errorSound);
            inputCode = "";
            displayText.text = "";
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OpenDoor()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            isTransitioning = true; // 씬 전환이 시작되었음을 표시
            SavePlayerStartPosition(); // 상호작용으로 씬 이동이 발생했을 때만 위치 저장
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // 변수 사용
        }
    }

    private void SavePlayerStartPosition()
    {
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);
    }

    public void ClearInput()
    {
        inputCode = "";           // 입력 초기화
        displayText.text = "";    // 화면 초기화
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 문 앞에 접근했을 때
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isTransitioning || other == null) return;
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (keypadUI != null)
            {
                keypadUI.SetActive(false); // 문에서 멀어지면 UI 비활성화
                ClearInput();              // 입력 초기화 추가
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
