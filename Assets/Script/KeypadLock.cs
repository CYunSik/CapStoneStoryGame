using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class KeypadLock : MonoBehaviour
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

    public AudioClip unlockSound;           // 비밀번호 맞췄을 때 사운드
    public AudioClip errorSound;            // 비밀번호 틀렸을 때 사운드
    private AudioSource audioSource;        // 오디오 소스

    private const string UnlockedKey = "IsDoorUnlocked"; // 잠금 해제 상태 저장을 위한 키

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
    }

    // 각 버튼이 호출하는 함수
    public void PressButton(string number)
    {
        if (isUnlocked) return; // 이미 잠금이 해제된 상태라면 추가 입력 불필요

        if (inputCode.Length < correctPassword.Length)
        {
            inputCode += number;
            displayText.text = inputCode;
        }
    }

    // Enter 버튼이 눌렸을 때 호출되는 함수
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

    // Clear 버튼이 눌렸을 때 호출되는 함수
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

    // 문을 여는 함수
    private void OpenDoor()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) // 씬 이름이 비어있지 않은지 확인
        {
            isTransitioning = true; // 씬 전환이 시작되었음을 표시
            SavePlayerStartPosition(); // 이동할 위치를 저장
            SceneManager.LoadScene(nextSceneName); // 씬 이름으로 씬 로드
        }
    }

    // 플레이어 시작 위치 저장
    private void SavePlayerStartPosition()
    {
        // 캐릭터의 시작 위치를 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(nextSceneName, playerStartPosition); // 다음 씬 이름을 직접 전달
        }
    }

    // 문과 상호작용할 때 호출되는 함수
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

    // 플레이어가 문 앞에 접근했을 때 호출되는 함수
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 캐릭터가 "Player" 태그를 가지고 있을 때
        {
            isPlayerNearby = true;
        }
    }

    // 플레이어가 문 앞에서 벗어났을 때 호출되는 함수
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isTransitioning || other == null) return;
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (keypadUI != null)
            {
                keypadUI.SetActive(false); // 문에서 멀어지면 UI를 숨깁니다.
            }
        }
    }

    // Update 함수에서 F 키 입력 감지
    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithDoor(); // F 키가 눌렸을 때 문과 상호작용
        }
    }
}