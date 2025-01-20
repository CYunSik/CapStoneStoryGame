using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro ���

public class Keypad3FLock : MonoBehaviour
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

    public GameObject dialogueCanvas;              // ��ȭâ UI
    public GameObject characterPanel;              // ĳ���� �̹��� �г�
    public GameObject textPanel;                   // �ؽ�Ʈ �г�
    public TextMeshProUGUI storyText;              // �ؽ�Ʈ ǥ�ÿ� TextMeshPro
    [TextArea(1, 10)]
    public string[] dialogues;                     // ��ȭ �ؽ�Ʈ �迭

    private int currentDialogueIndex = 0;          // ���� ��ȭ �ؽ�Ʈ �ε���
    private bool dialogueActive = false;           // ��ȭ Ȱ��ȭ ����

    public AudioClip unlockSound;                  // ��й�ȣ ������ �� ����
    public AudioClip errorSound;                   // ��й�ȣ Ʋ���� �� ����
    private AudioSource audioSource;               // ����� �ҽ�

    private TechStudentController playerMovement;  // ĳ���� �̵� ��ũ��Ʈ
    private Rigidbody2D playerRigidbody;           // ĳ���� Rigidbody2D
    private Animator playerAnimator;               // ĳ���� Animator

    private const string UnlockedKey = "IsDoorUnlocked"; // ��� ���� ���� ������ ���� Ű

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // ĳ���� ������Ʈ ����
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // ���� �� UI ��Ȱ��ȭ
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (keypadUI != null)
            keypadUI.SetActive(false);

        // ��� ���� ���� �ʱ�ȭ
        isUnlocked = false;
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.is_3F_Locked == 0)
            {
                StartDialogue(); // ��ȭâ ����
            }
            else if (GameManager.instance.is_3F_Locked == 1 && !isUnlocked)
            {
                // Ű�е� ��ȣ�ۿ� ���� (��й�ȣ �ʿ�)
                InteractWithDoor();
            }
            else if (GameManager.instance.is_3F_Locked == 1 && isUnlocked)
            {
                // ��й�ȣ �Է� ���� �� �� ����
                OpenDoor();
            }
        }

        // ��ȭ �� Space Ű �Է� ó��
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

            // ĳ���� �̵� ����
            DisablePlayerMovement();
        }
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� �ؽ�Ʈ ���
            storyText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // ��ȭ ����
            EndDialogueSequence();
        }
    }

    private void EndDialogueSequence()
    {
        // ��� ��ȭ UI ��Ȱ��ȭ
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;

        // ���� ����
        GameManager.instance.is_3F_Locked = 1; // ��ȭ �� �� ��� ���·� ����

        // ĳ���� �̵� ����
        EnablePlayerMovement();
    }

    public void InteractWithDoor()
    {
        if (!keypadUI.activeSelf)
        {
            keypadUI.SetActive(true); // Ű�е� UI Ȱ��ȭ
            inputCode = "";           // �Է� �ʱ�ȭ
            displayText.text = "";    // ȭ�� �ʱ�ȭ
        }
    }

    public void PressButton(string number)
    {
        if (isUnlocked) return; // �̹� ����� ������ ���¶�� �Է� ����

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
            isTransitioning = true; // �� ��ȯ�� ���۵Ǿ����� ǥ��
            SavePlayerStartPosition(); // ��ȣ�ۿ����� �� �̵��� �߻����� ���� ��ġ ����
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // ���� ���
        }
    }

    private void SavePlayerStartPosition()
    {
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);
    }

    public void ClearInput()
    {
        inputCode = "";           // �Է� �ʱ�ȭ
        displayText.text = "";    // ȭ�� �ʱ�ȭ
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ �� �տ� �������� ��
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
                keypadUI.SetActive(false); // ������ �־����� UI ��Ȱ��ȭ
                ClearInput();              // �Է� �ʱ�ȭ �߰�
            }
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

        if (playerMovement != null)
        {
            playerMovement.enabled = false; // �̵� ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true; // �̵� ��ũ��Ʈ Ȱ��ȭ
        }
    }
}
