using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Keypad4FLock : MonoBehaviour
{
    public string correctPassword = "1234";        // 설정할 비밀번호
    private string inputCode = "";                 // 입력된 코드 저장
    private bool isUnlocked = false;               // 잠금 해제 여부 확인
    private bool isPlayerNearby = false;           // 플레이어가 문 앞에 있는지 확인
    private bool isTransitioning = false;          // 씬 전환 중인지 확인
    private bool dialogueActive = false;           // 대화 UI가 활성화되었는지 확인
    private bool canAdvance = true;                // 대사 진행 가능 여부

    public UnityEngine.UI.Text displayText;        // 키패드 입력을 표시하는 UI 텍스트
    public GameObject keypadUI;                    // 키패드 UI 오브젝트
    public GameObject dialogueUI;                  // 대사 UI 오브젝트
    public TMPro.TMP_Text dialogueText;            // 대사 텍스트
    public string[] dialogues;                     // 표시할 대사 배열
    public MonoBehaviour playerControllerScript;   // 캐릭터 움직임 스크립트
    public Rigidbody2D playerRigidbody;            // 캐릭터의 Rigidbody2D
    public Animator playerAnimator;                // 캐릭터의 Animator

    public string nextSceneName;                   // 이동할 다음 씬의 이름
    public Vector2 playerStartPosition;            // 다음 씬에서 캐릭터가 시작할 위치

    public AudioClip unlockSound;                  // 비밀번호 맞췄을 때 사운드
    public AudioClip errorSound;                   // 비밀번호 틀렸을 때 사운드
    private AudioSource audioSource;               // 오디오 소스

    private const string UnlockedKey = "IsDoorUnlocked"; // 잠금 해제 상태 저장을 위한 키
    private int currentDialogueIndex = 0;          // 현재 대사 인덱스

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
        // 에디터에서 실행 시 항상 잠금 해제 초기화
        if (EditorApplication.isPlaying)
        {
            PlayerPrefs.SetInt(UnlockedKey, 0);
            PlayerPrefs.Save();
        }
#endif

        // 잠금 해제 상태를 불러옴 (에디터에서는 초기화됨)
        if (PlayerPrefs.GetInt(UnlockedKey, 0) == 1)
        {
            isUnlocked = true;
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 시작 시 대사 UI 비활성화
        }
    }

    public void PressButton(string number)
    {
        if (isUnlocked) return; // 이미 잠금이 해제된 상태라면 추가 입력 불필요

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
            PlaySound(unlockSound); // 비밀번호 맞췄을 때만 사운드 재생
        }
        else
        {
            PlaySound(errorSound); // 비밀번호 틀렸을 때 사운드 재생
            inputCode = "";
            displayText.text = ""; // 실패 시 입력 초기화
        }
    }

    public void ClearInput()
    {
        inputCode = "";
        displayText.text = "";
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

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(nextSceneName, playerStartPosition); // 다음 씬 이름을 직접 전달
        }
    }

    public void InteractWithDoor()
    {
        if (isUnlocked)
        {
            OpenDoor(); // 잠금이 해제된 경우 바로 문을 엽니다.
        }
        else
        {
            keypadUI.SetActive(true); // 잠금이 해제되지 않은 경우 키패드 UI를 활성화합니다.
        }
    }

    private void ShowDialogueUI()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
            dialogueActive = true;
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex]; // 첫 번째 대사 표시

            DisablePlayerMovement(); // 캐릭터 움직임 비활성화
        }
    }

    private void AdvanceDialogue()
    {
        if (!canAdvance) return; // 대사 진행 불가능하면 리턴

        canAdvance = false; // 입력을 차단하여 연속 입력 방지

        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // 다음 대사 표시
        }
        else
        {
            EndDialogue(); // 대사 종료
        }

        // 일정 시간 후 대사 진행 가능하도록 설정
        StartCoroutine(EnableAdvanceAfterDelay());
    }

    private IEnumerator EnableAdvanceAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // 짧은 대기 시간 추가
        canAdvance = true; // 대사 진행 가능 상태로 변경
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        dialogueActive = false;

        // 대사 종료 후 is_4F_Locked를 1로 설정
        GameManager.instance.is_4F_Locked = 1;

        EnablePlayerMovement(); // 캐릭터 움직임 다시 활성화
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
                keypadUI.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.is_4F_Locked == 0)
            {
                ShowDialogueUI();
            }
            else if (GameManager.instance.is_4F_Locked == 1 && GameManager.instance.has_4F_KeyHint1 == 1)
            {
                InteractWithDoor();
            }
        }

        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
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

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false; // 캐릭터 움직임 스크립트 비활성화
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // 캐릭터 움직임 스크립트 활성화
        }
    }
}
