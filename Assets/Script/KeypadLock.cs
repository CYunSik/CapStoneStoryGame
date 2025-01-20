using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class KeypadLock : MonoBehaviour
{
    public string correctPassword = "1234";        // ������ ��й�ȣ
    private string inputCode = "";                 // �Էµ� �ڵ� ����
    private bool isUnlocked = false;               // ��� ���� ���� Ȯ��
    private bool isPlayerNearby = false;           // �÷��̾ �� �տ� �ִ��� Ȯ��
    private bool isTransitioning = false;          // �� ��ȯ ������ Ȯ��

    public UnityEngine.UI.Text displayText;        // Ű�е� �Է��� ǥ���ϴ� UI �ؽ�Ʈ
    public GameObject keypadUI;                    // Ű�е� UI ������Ʈ

    public string nextSceneName;                   // �̵��� ���� ���� �̸�
    public Vector2 playerStartPosition;            // ���� ������ ĳ���Ͱ� ������ ��ġ

    public AudioClip unlockSound;           // ��й�ȣ ������ �� ����
    public AudioClip errorSound;            // ��й�ȣ Ʋ���� �� ����
    private AudioSource audioSource;        // ����� �ҽ�

    private const string UnlockedKey = "IsDoorUnlocked"; // ��� ���� ���� ������ ���� Ű

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
        // �����Ϳ��� ���� �� �׻� ��� ���� �ʱ�ȭ
        if (EditorApplication.isPlaying)
        {
            PlayerPrefs.SetInt(UnlockedKey, 0);
            PlayerPrefs.Save();
        }
#endif

        // ��� ���� ���¸� �ҷ��� (�����Ϳ����� �ʱ�ȭ��)
        if (PlayerPrefs.GetInt(UnlockedKey, 0) == 1)
        {
            isUnlocked = true;
        }
    }

    // �� ��ư�� ȣ���ϴ� �Լ�
    public void PressButton(string number)
    {
        if (isUnlocked) return; // �̹� ����� ������ ���¶�� �߰� �Է� ���ʿ�

        if (inputCode.Length < correctPassword.Length)
        {
            inputCode += number;
            displayText.text = inputCode;
        }
    }

    // Enter ��ư�� ������ �� ȣ��Ǵ� �Լ�
    public void CheckPassword()
    {
        if (isUnlocked) return; // �̹� ����� ������ ���¶�� �߰� ���� ���ʿ�

        if (inputCode == correctPassword)
        {
            isUnlocked = true;
            PlayerPrefs.SetInt(UnlockedKey, 1);
            PlayerPrefs.Save();
            displayText.text = "Unlocked";
            keypadUI.SetActive(false);
            PlaySound(unlockSound); // ��й�ȣ ������ ���� ���� ���
        }
        else
        {
            PlaySound(errorSound); // ��й�ȣ Ʋ���� �� ���� ���
            inputCode = "";
            displayText.text = ""; // ���� �� �Է� �ʱ�ȭ
        }
    }

    // Clear ��ư�� ������ �� ȣ��Ǵ� �Լ�
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

    // ���� ���� �Լ�
    private void OpenDoor()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) // �� �̸��� ������� ������ Ȯ��
        {
            isTransitioning = true; // �� ��ȯ�� ���۵Ǿ����� ǥ��
            SavePlayerStartPosition(); // �̵��� ��ġ�� ����
            SceneManager.LoadScene(nextSceneName); // �� �̸����� �� �ε�
        }
    }

    // �÷��̾� ���� ��ġ ����
    private void SavePlayerStartPosition()
    {
        // ĳ������ ���� ��ġ�� PlayerPrefs�� ����
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(nextSceneName, playerStartPosition); // ���� �� �̸��� ���� ����
        }
    }

    // ���� ��ȣ�ۿ��� �� ȣ��Ǵ� �Լ�
    public void InteractWithDoor()
    {
        if (isUnlocked)
        {
            OpenDoor(); // ����� ������ ��� �ٷ� ���� ���ϴ�.
        }
        else
        {
            keypadUI.SetActive(true); // ����� �������� ���� ��� Ű�е� UI�� Ȱ��ȭ�մϴ�.
        }
    }

    // �÷��̾ �� �տ� �������� �� ȣ��Ǵ� �Լ�
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ĳ���Ͱ� "Player" �±׸� ������ ���� ��
        {
            isPlayerNearby = true;
        }
    }

    // �÷��̾ �� �տ��� ����� �� ȣ��Ǵ� �Լ�
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isTransitioning || other == null) return;
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (keypadUI != null)
            {
                keypadUI.SetActive(false); // ������ �־����� UI�� ����ϴ�.
            }
        }
    }

    // Update �Լ����� F Ű �Է� ����
    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithDoor(); // F Ű�� ������ �� ���� ��ȣ�ۿ�
        }
    }
}