using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint4FInteract : MonoBehaviour
{
    private bool isPlayerNearby = false; // �÷��̾ ��ȣ�ۿ� ���� �ȿ� �ִ��� Ȯ��
    public GameObject dialogueUI;       // ��� UI ������Ʈ
    public TextMeshProUGUI dialogueText; // ��� �ؽ�Ʈ
    public GameObject diaryUI;          // ���� ���� UI
    public AudioSource diarySound;      // ���̾ UI Ȱ��ȭ �� ����� ����

    [TextArea(1, 10)]
    public string[] dialogues;          // ��� �ؽ�Ʈ �迭
    private int currentDialogueIndex = 0; // ���� ��� �ε���
    private bool dialogueActive = false; // ��ȭ Ȱ��ȭ ����
    private bool uiActive = false;       // ���̾ UI Ȱ��ȭ ����

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody; // ĳ���� Rigidbody2D ����
    private Animator playerAnimator;    // ĳ���� �ִϸ����� ����

    private void Start()
    {
        // UI �ʱ�ȭ
        if (dialogueUI != null) dialogueUI.SetActive(false);
        if (diaryUI != null) diaryUI.SetActive(false);

        // ĳ���� ������Ʈ ����
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
        // ��� UI�� Ȱ��ȭ�� ���, Space Ű�� ��� ����
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
        // F Ű�� ���� ��� ���� �Ǵ� ���̾ UI ���
        else if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.has_4F_KeyHint1 == 0 && !dialogueActive)
                {
                    CheckInteractionConditions(); // ���� ���� �� ��� ����
                }
                else if (GameManager.instance.has_4F_KeyHint1 == 1)
                {
                    ToggleDiaryUI(); // ���̾ UI ���
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // ���� �ȿ� �������� ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // ���� ������ �������� ǥ��
            if (uiActive) HideDiaryUI(); // ���̾ UI�� Ȱ��ȭ�� ��� ����
        }
    }

    private void CheckInteractionConditions()
    {
        // GameManager ������ ������ ���� �̺�Ʈ �߻�
        if (GameManager.instance != null &&
            GameManager.instance.has_Interacted_Elevator == 1 &&
            GameManager.instance.is_4F_Locked == 1)
        {
            StartDialogue(); // ������ �����ϸ� ��� ����
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null && dialogues.Length > 0)
        {
            // ��� UI Ȱ��ȭ
            dialogueUI.SetActive(true);
            dialogueActive = true;

            // ù ��° ��� ���
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex];

            // ĳ���� ������ ��Ȱ��ȭ
            DisablePlayerMovement();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // ��ȭ ���� �� ���̾ UI ǥ��
            EndDialogue();
            ShowDiaryUI();

            // GameManager ���� ����
            if (GameManager.instance != null)
            {
                GameManager.instance.has_4F_KeyHint1 = 1; // ���� ������Ʈ
            }
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // ��� UI ��Ȱ��ȭ
        dialogueActive = false;

        // ĳ���� ������ �ٽ� Ȱ��ȭ
        EnablePlayerMovement();
    }

    private void ShowDiaryUI()
    {
        if (diaryUI != null)
        {
            diaryUI.SetActive(true); // ���̾ UI Ȱ��ȭ
            uiActive = true;

            // ���̾ ���� ���
            PlayDiarySound();
        }
    }

    private void ToggleDiaryUI()
    {
        if (diaryUI != null)
        {
            uiActive = !uiActive;
            diaryUI.SetActive(uiActive); // ���̾ UI ���

            // ���̾ UI�� Ȱ��ȭ�� ���� ���� ���
            if (uiActive)
            {
                PlayDiarySound();
            }
        }
    }

    private void HideDiaryUI()
    {
        if (diaryUI != null) diaryUI.SetActive(false); // ���̾ UI �����
        uiActive = false;
    }

    private void PlayDiarySound()
    {
        if (diarySound != null)
        {
            diarySound.Play(); // ���̾ ���� ���
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
