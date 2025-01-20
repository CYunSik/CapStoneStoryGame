using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Keypad4FLock : MonoBehaviour
{
    public string correctPassword = "1234";        // ������ ��й�ȣ
    private string inputCode = "";                 // �Էµ� �ڵ� ����
    private bool isUnlocked = false;               // ��� ���� ���� Ȯ��
    private bool isPlayerNearby = false;           // �÷��̾ �� �տ� �ִ��� Ȯ��
    private bool isTransitioning = false;          // �� ��ȯ ������ Ȯ��
    private bool dialogueActive = false;           // ��ȭ UI�� Ȱ��ȭ�Ǿ����� Ȯ��
    private bool canAdvance = true;                // ��� ���� ���� ����

    public UnityEngine.UI.Text displayText;        // Ű�е� �Է��� ǥ���ϴ� UI �ؽ�Ʈ
    public GameObject keypadUI;                    // Ű�е� UI ������Ʈ
    public GameObject dialogueUI;                  // ��� UI ������Ʈ
    public TMPro.TMP_Text dialogueText;            // ��� �ؽ�Ʈ
    public string[] dialogues;                     // ǥ���� ��� �迭
    public MonoBehaviour playerControllerScript;   // ĳ���� ������ ��ũ��Ʈ
    public Rigidbody2D playerRigidbody;            // ĳ������ Rigidbody2D
    public Animator playerAnimator;                // ĳ������ Animator

    public string nextSceneName;                   // �̵��� ���� ���� �̸�
    public Vector2 playerStartPosition;            // ���� ������ ĳ���Ͱ� ������ ��ġ

    public AudioClip unlockSound;                  // ��й�ȣ ������ �� ����
    public AudioClip errorSound;                   // ��й�ȣ Ʋ���� �� ����
    private AudioSource audioSource;               // ����� �ҽ�

    private const string UnlockedKey = "IsDoorUnlocked"; // ��� ���� ���� ������ ���� Ű
    private int currentDialogueIndex = 0;          // ���� ��� �ε���

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

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ���� �� ��� UI ��Ȱ��ȭ
        }
    }

    public void PressButton(string number)
    {
        if (isUnlocked) return; // �̹� ����� ������ ���¶�� �߰� �Է� ���ʿ�

        if (inputCode.Length < correctPassword.Length)
        {
            inputCode += number;
            displayText.text = inputCode;
        }
    }

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
            isTransitioning = true; // �� ��ȯ�� ���۵Ǿ����� ǥ��
            SavePlayerStartPosition(); // ��ȣ�ۿ����� �� �̵��� �߻����� ���� ��ġ ����
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // ���� ���
        }
    }

    private void SavePlayerStartPosition()
    {
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(nextSceneName, playerStartPosition); // ���� �� �̸��� ���� ����
        }
    }

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

    private void ShowDialogueUI()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
            dialogueActive = true;
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex]; // ù ��° ��� ǥ��

            DisablePlayerMovement(); // ĳ���� ������ ��Ȱ��ȭ
        }
    }

    private void AdvanceDialogue()
    {
        if (!canAdvance) return; // ��� ���� �Ұ����ϸ� ����

        canAdvance = false; // �Է��� �����Ͽ� ���� �Է� ����

        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // ���� ��� ǥ��
        }
        else
        {
            EndDialogue(); // ��� ����
        }

        // ���� �ð� �� ��� ���� �����ϵ��� ����
        StartCoroutine(EnableAdvanceAfterDelay());
    }

    private IEnumerator EnableAdvanceAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // ª�� ��� �ð� �߰�
        canAdvance = true; // ��� ���� ���� ���·� ����
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        dialogueActive = false;

        // ��� ���� �� is_4F_Locked�� 1�� ����
        GameManager.instance.is_4F_Locked = 1;

        EnablePlayerMovement(); // ĳ���� ������ �ٽ� Ȱ��ȭ
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
            playerRigidbody.velocity = Vector2.zero; // �̵� �ӵ� ����
            playerRigidbody.angularVelocity = 0f;   // ȸ�� �ӵ� ����
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle ���·� ��ȯ
        }

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false; // ĳ���� ������ ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // ĳ���� ������ ��ũ��Ʈ Ȱ��ȭ
        }
    }
}
