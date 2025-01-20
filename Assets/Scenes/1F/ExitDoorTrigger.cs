using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ExitDoorTrigger : MonoBehaviour
{
    public string sceneName;                // �̵��� ���� �̸�
    public Vector2 playerStartPosition;     // ���� ������ ĳ���Ͱ� ������ ��ġ
    public GameObject dialogueUI;           // ��ȭ UI (Canvas)
    public TextMeshProUGUI dialogueText;    // TextMeshPro ��ȭ �ؽ�Ʈ
    public Image characterIllustration;     // ĳ���� �Ϸ���Ʈ �̹���
    [TextArea(1, 10)]
    public string[] dialogues;              // ����� ��ȭ ���

    private bool isPlayerInRange = false;   // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool isDialogueActive = false;  // ��ȭ ���� ����
    private int currentDialogueIndex = 0;   // ���� ��� �ε���

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody;         // �÷��̾� Rigidbody2D
    private Animator playerAnimator;              // ĳ���� �ִϸ�����

    private void Start()
    {
        // ��ȭ UI�� ĳ���� �Ϸ���Ʈ ��Ȱ��ȭ
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // �÷��̾� ��ũ��Ʈ�� �ִϸ����� ����
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
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // ����: is_1F_Locked == 0 && is_1F_Start == 1
            if (GameManager.instance.is_1F_Locked == 0 && GameManager.instance.is_1F_Start == 1)
            {
                StartDialogue(); // ��ȭ ����
            }
            // ����: is_1F_Locked == 1 && has_Exit_Key == 1
            else if (GameManager.instance.is_1F_Locked == 1 && GameManager.instance.has_Exit_Key == 1)
            {
                LoadSceneByName(); // �� �ε�
            }
        }

        // ��ȭ �� Space Ű�� ���� ��� ����
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // ��ȭ UI Ȱ��ȭ
            isDialogueActive = true;

            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex]; // ù ��° ��� ���
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // ĳ���� �̹��� Ȱ��ȭ
        }

        DisablePlayerMovement(); // �÷��̾� ������ ����
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // ��ȭ ����
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // ĳ���� �̹��� ��Ȱ��ȭ
        }

        EnablePlayerMovement(); // ��ȭ ���� �� �÷��̾� ������ Ȱ��ȭ

        // `is_1F_Locked`�� 1�� ����
        GameManager.instance.is_1F_Locked = 1;

        isDialogueActive = false;
    }

    private void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SavePlayerStartPosition();
            FadeManager.Instance.LoadSceneWithFade(sceneName);
        }
    }

    private void SavePlayerStartPosition()
    {
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(sceneName, playerStartPosition);
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
