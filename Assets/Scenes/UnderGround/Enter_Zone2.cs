using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class Enter_Zone2 : MonoBehaviour
{
    public GameObject diaryPanel;       // �ϱ��� �г�
    public GameObject enterUIText;     // "F�� ���� �ϱ⸦ Ȯ���ϼ���" �ؽ�Ʈ
    public GameObject dialoguePanel;   // ��ȭâ Panel
    public UnityEngine.UI.Image characterIllustration; // Unity�� UI �̹���
    public TextMeshProUGUI dialogueText; // ��ȭ �ؽ�Ʈ

    [TextArea(1, 10)]
    public string[] hintDialogues;     // �ϱ����� ���� �� ���
    [TextArea(1, 10)]
    public string[] afterDiaryDialogues; // �ϱ����� ���� �� ���

    private enum InteractionState { Idle, DialogueHint, DiaryOpen, DialogueAfterDiary }
    private InteractionState currentState = InteractionState.Idle; // ���� ����

    private bool isPlayerInZone = false; // �÷��̾ ������ �ִ��� ����
    private int currentDialogueIndex = 0;  // ���� ��ȭ �ε���

    private TechStudentController playerMovement; // �÷��̾� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody; // �÷��̾� Rigidbody2D ����
    private Animator playerAnimator; // �÷��̾� �ִϸ����� ����

    private void Start()
    {
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(false); // ���� �� �ϱ��� �г� ��Ȱ��ȭ
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // ���� �� ��ȭâ ��Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // ���� �� ĳ���� �̹��� ��Ȱ��ȭ
        }

        if (enterUIText != null)
        {
            enterUIText.SetActive(false); // "F�� ���� �ϱ⸦ Ȯ���ϼ���" �ؽ�Ʈ ��Ȱ��ȭ
        }

        // �÷��̾� ���� ������Ʈ ����
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
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            HandleFKeyInput();
        }

        if ((currentState == InteractionState.DialogueHint || currentState == InteractionState.DialogueAfterDiary)
            && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void HandleFKeyInput()
    {
        switch (currentState)
        {
            case InteractionState.Idle:
                StartDialogue(hintDialogues, InteractionState.DialogueHint); // ��Ʈ ��� ����
                break;
            case InteractionState.DiaryOpen:
                CloseDiary(); // �ϱ����� �ݰ� �ļ� ��ȭ ����
                break;
        }
    }

    private void StartDialogue(string[] dialogues, InteractionState nextState)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true);
        }

        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        currentState = nextState;

        DisablePlayerMovement(); // ��ȭ ���� �� �÷��̾� ���� ��Ȱ��ȭ
    }

    private void ShowNextDialogue()
    {
        if (currentState == InteractionState.DialogueHint)
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < hintDialogues.Length)
            {
                dialogueText.text = hintDialogues[currentDialogueIndex];
            }
            else
            {
                EndDialogue();
                OpenDiary(); // ��Ʈ ��ȭ�� ������ �ϱ��� ����
            }
        }
        else if (currentState == InteractionState.DialogueAfterDiary)
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < afterDiaryDialogues.Length)
            {
                dialogueText.text = afterDiaryDialogues[currentDialogueIndex];
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        if (currentState == InteractionState.DialogueHint)
        {
            currentState = InteractionState.Idle; // ��ȭ ���� �� Idle�� ����
        }
        else if (currentState == InteractionState.DialogueAfterDiary)
        {
            currentState = InteractionState.Idle; // �ļ� ��ȭ ���� �� Idle�� ����
            EnablePlayerMovement(); // �÷��̾� ���� Ȱ��ȭ

            // GameManager�� DiaryOpen ������ 1�� ����
            if (GameManager.instance != null && DiaryController.instance.currentPage == DiaryController.instance.pageTexts.Length - 1)
            {
                GameManager.instance.DiaryOpen = 1;
            }
        }
    }


    private void OpenDiary()
    {
        // ��ȭâ �ݱ�
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // �ϱ��� ����
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(true);
        }

        currentState = InteractionState.DiaryOpen; // ���¸� DiaryOpen���� ����
    }

    private void CloseDiary()
    {
        // �ϱ��� �ݱ�
        if (diaryPanel != null)
        {
            diaryPanel.SetActive(false);
        }

        StartDialogue(afterDiaryDialogues, InteractionState.DialogueAfterDiary); // �ϱ����� ���� �� ��ȭ ����
    }

    private void DisablePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle");
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;

            if (enterUIText != null)
            {
                enterUIText.SetActive(true); // "F�� ���� �ϱ⸦ Ȯ���ϼ���" �ؽ�Ʈ ǥ��
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;

            if (enterUIText != null)
            {
                enterUIText.SetActive(false);
            }

            if (diaryPanel != null)
            {
                diaryPanel.SetActive(false);
            }

            currentState = InteractionState.Idle; // ���� �ʱ�ȭ
            EnablePlayerMovement(); // �÷��̾� ���� Ȱ��ȭ
        }
    }
}

