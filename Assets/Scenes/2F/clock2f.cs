using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���

public class clock2f : MonoBehaviour
{
    public GameObject clockUI; // "�ð�" UI ������Ʈ
    public GameObject dialogueUI; // ��ȭ UI ������Ʈ
    public TextMeshProUGUI dialogueText; // ��ȭ �ؽ�Ʈ
    [TextArea(1, 10)]
    public string[] dialogues; // ����� ��� �迭

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool uiActive = false; // UI�� Ȱ��ȭ�Ǿ����� Ȯ��
    private bool dialogueActive = false; // ��ȭ ���� ����
    private int currentDialogueIndex = 0; // ���� ��� �ε���

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody; // ĳ���� Rigidbody2D ����
    private Animator playerAnimator; // ĳ���� �ִϸ����� ����

    private void Start()
    {
        // UI �ʱ�ȭ
        if (clockUI != null) clockUI.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);

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
        // ���� �ȿ� �ְ� F Ű�� ������ �� ó��
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.is_Today_Time == 0)
            {
                StartDialogueWithClockUI(); // �ð� UI�� ��ȭ UI ���� Ȱ��ȭ
            }
            else if (GameManager.instance.is_Today_Time == 1)
            {
                ToggleClockUI(); // �ð� UI�� ���
            }
        }

        // ��ȭ �� �����̽��ٷ� ��ȭ ����
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }

        // ���� ������ ������ �� UI ��Ȱ��ȭ
        if (!isPlayerInRange && uiActive)
        {
            DisableAllUI();
        }
    }

    private void StartDialogueWithClockUI()
    {
        if (clockUI != null) clockUI.SetActive(true); // �ð� UI Ȱ��ȭ
        if (dialogueUI != null) dialogueUI.SetActive(true); // ��ȭ UI Ȱ��ȭ
        uiActive = true;
        dialogueActive = true;

        // ĳ���� �̵� ��Ȱ��ȭ
        DisablePlayerMovement();
        GameManager.instance.is_Today_Time = 1; // ���� ������Ʈ

        // ù ��° ��ȭ ���
        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
    }

    private void ToggleClockUI()
    {
        if (clockUI != null)
        {
            uiActive = !uiActive;
            clockUI.SetActive(uiActive);
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��ȭ ���
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // ��ȭ ����
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
        dialogueActive = false;

        // ĳ���� �̵� �ٽ� Ȱ��ȭ
        EnablePlayerMovement();
    }

    private void DisableAllUI()
    {
        if (clockUI != null) clockUI.SetActive(false); // �ð� UI ��Ȱ��ȭ
        if (dialogueUI != null) dialogueUI.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
        uiActive = false;
        dialogueActive = false;

        // ĳ���� �̵� ����
        EnablePlayerMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // ���� �ȿ� �������� ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // ���� ������ �������� ǥ��
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
