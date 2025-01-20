using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���

public class NoteboardInteract : MonoBehaviour
{
    public GameObject noteboardUI; // �Խ��� �Ź� UI ������Ʈ
    public GameObject dialogueCanvas; // ��ȭâ ĵ����
    public GameObject characterPanel; // ĳ���� �̹��� �г�
    public GameObject textPanel; // �ؽ�Ʈ �г�
    public TextMeshProUGUI storyText; // Story �ؽ�Ʈ (TextMeshPro)

    public AudioSource noteboardsound; // �Խ��� ���� ����� �ҽ�
    [TextArea(1, 10)]
    public string[] dialogues; // ����� ��ȭ �ؽ�Ʈ �迭

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool dialogueActive = false; // ��ȭâ Ȱ��ȭ ����
    private bool uiActive = false; // �Խ��� UI Ȱ��ȭ ����
    private int currentDialogueIndex = 0; // ���� ��ȭ �ؽ�Ʈ �ε���

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ
    private Rigidbody2D playerRigidbody; // ĳ���� Rigidbody2D
    private Animator playerAnimator; // ĳ���� Animator

    private void Start()
    {
        // ĳ���� ������Ʈ ����
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // UI �ʱ�ȭ
        if (noteboardUI != null)
            noteboardUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (noteboardsound != null)
            noteboardsound.Stop();
    }

    private void Update()
    {
        // ���� ������ F Ű �Է��� ó��
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.has_news_paper == 0)
            {
                ShowInitialNoteboardUI(); // ù ��° UI ǥ��
            }
            else if (GameManager.instance.has_news_paper == 1)
            {
                ToggleNoticeboardUI(); // �Խ��� UI ���
            }
        }

        // ��ȭ �� Space Ű �Է� ó��
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowInitialNoteboardUI()
    {
        if (noteboardUI != null)
        {
            noteboardUI.SetActive(true);
            uiActive = true;

            // �Խ��� UI ���� ���
            if (noteboardsound != null)
                noteboardsound.Play();

            // ĳ���� �̵� ����
            DisablePlayerMovement();

            // 2�� �� ������ UI ǥ��
            Invoke(nameof(ShowRemainingUI), 1f);
        }
    }

    private void ShowRemainingUI()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (characterPanel != null)
            characterPanel.SetActive(true);

        if (textPanel != null)
            textPanel.SetActive(true);

        dialogueActive = true; // ��ȭ Ȱ��ȭ
    }

    private void ShowNextDialogue()
    {
        if (currentDialogueIndex == 0)
        {
            // ó�� Space Ű �Է� �� �ؽ�Ʈ ���
            storyText.text = dialogues[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else if (currentDialogueIndex < dialogues.Length)
        {
            // ���� �ؽ�Ʈ ���
            storyText.text = dialogues[currentDialogueIndex];
            currentDialogueIndex++;
        }
        else
        {
            // ��ȭ ����
            EndDialogueSequence();
        }
    }

    private void EndDialogueSequence()
    {
        // ��� UI ��Ȱ��ȭ
        if (noteboardUI != null)
            noteboardUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;
        uiActive = false;
        currentDialogueIndex = 0;

        // ĳ���� �̵� ����
        EnablePlayerMovement();

        // `has_news_paper` ���� ����
        GameManager.instance.has_news_paper = 1;
    }

    private void ToggleNoticeboardUI()
    {
        if (noteboardUI != null)
        {
            uiActive = !uiActive;
            noteboardUI.SetActive(uiActive); // �Խ��� UI Ȱ��ȭ/��Ȱ��ȭ

            if (uiActive)
            {
                // �Խ��� UI ���� �� �̵� ����
                DisablePlayerMovement();

                if (noteboardsound != null)
                    noteboardsound.Play();
            }
            else
            {
                // �Խ��� UI ���� �� �̵� ����
                EnablePlayerMovement();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // �÷��̾ ���� �ȿ� ����
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // �÷��̾ ���� ������ ����

            // ������ ����� UI ��Ȱ��ȭ �� �̵� ����
            if (uiActive && noteboardUI != null)
            {
                noteboardUI.SetActive(false);
                EnablePlayerMovement();
            }
        }
    }
}
