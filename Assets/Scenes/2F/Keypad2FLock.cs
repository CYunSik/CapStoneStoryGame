using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Keypad2FLocked : MonoBehaviour
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

    public AudioClip unlockSound;                  // ��й�ȣ ������ �� ����
    public AudioClip errorSound;                   // ��й�ȣ Ʋ���� �� ����
    private AudioSource audioSource;               // ����� �ҽ�

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // GameManager�� is_2F_Locked�� Ȯ���Ͽ� ��� ���� �ʱ�ȭ
        if (GameManager.instance.is_2F_Locked == 0)
        {
            isUnlocked = false; // ��� ���·� �ʱ�ȭ
        }
        else
        {
            isUnlocked = true; // ��� ���� ���·� �ʱ�ȭ
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
            GameManager.instance.is_2F_Locked = 1; // GameManager�� ���� ������Ʈ
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
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            isTransitioning = true; // �� ��ȯ�� ���۵Ǿ����� ǥ��
            SavePlayerStartPosition(); // ��ȣ�ۿ����� �� �̵��� �߻����� ���� ��ġ ����
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // ���� ���
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
        // GameManager ���¸� Ȯ���Ͽ� �� ��� ���¸� ����
        if (GameManager.instance.is_2F_Locked == 0)
        {
            isUnlocked = false; // �׻� ����ִ� ���·� ����
        }

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
            InteractWithDoor();
        }
    }
}
