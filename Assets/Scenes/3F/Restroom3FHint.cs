using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���

public class Restroom3FHint : MonoBehaviour
{
    public GameObject toiletUI; // "ȭ�����Ʈ" UI ������Ʈ
    public AudioSource toiletpapersound; // �Խ��� ���� ����� �ҽ�
    public GameObject dialogueCanvas; // ��ȭâ UI
    public GameObject characterPanel; // ĳ���� �̹��� �г�
    public GameObject textPanel; // �ؽ�Ʈ �г�
    public TextMeshProUGUI storyText; // �ؽ�Ʈ ǥ�ÿ� TextMeshPro
    [TextArea(1, 10)]
    public string[] dialogues; // ��ȭ �ؽ�Ʈ �迭

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool dialogueActive = false; // ��ȭ Ȱ��ȭ ����
    private bool uiActive = false; // ȭ��� UI Ȱ��ȭ ����
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

        // ��� UI �ʱ�ȭ
        if (toiletUI != null)
            toiletUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (toiletpapersound != null)
            toiletpapersound.Stop();
    }

    private void Update()
    {
        // ���� �ȿ� ���� ���� F Ű �Է� ó��
        if (isPlayerInRange)
        {
            if (GameManager.instance != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (uiActive)
                    {
                        HideAllUI(); // UI ��Ȱ��ȭ
                    }
                    else
                    {
                        if (GameManager.instance.has_3F_Hint == 0 && GameManager.instance.is_3F_Locked == 1)
                        {
                            ShowToiletUIFirst(); // ó������ ȭ��� ��Ʈ UI�� ǥ��
                        }
                        else if (GameManager.instance.has_3F_Hint == 1)
                        {
                            ShowToiletUIOnly(); // ȭ��� UI�� Ȱ��ȭ
                        }
                    }
                }
            }
        }

        // ��ȭ �� Space Ű �Է� ó��
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowToiletUIFirst()
    {
        if (toiletUI != null)
        {
            toiletUI.SetActive(true); // ȭ��� ��Ʈ UI�� Ȱ��ȭ
            uiActive = true;

            if (toiletpapersound != null)
                toiletpapersound.Play(); // ���� ���

            // ĳ���� �̵� ����
            DisablePlayerMovement();

            // 1�� �� ������ UI�� ǥ��
            Invoke(nameof(StartDialogueWithToiletUI), 1f);
        }
    }

    private void StartDialogueWithToiletUI()
    {
        if (dialogueCanvas != null && dialogues.Length > 0)
        {
            dialogueActive = true;
            currentDialogueIndex = 0;

            // ��ȭâ�� ������ UI Ȱ��ȭ
            dialogueCanvas.SetActive(true);
            characterPanel.SetActive(true);
            textPanel.SetActive(true);

            // ù ��° ��ȭ ���
            storyText.text = dialogues[currentDialogueIndex];
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
        // ��ȭâ ��Ȱ��ȭ
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        dialogueActive = false;

        // ���� ����
        GameManager.instance.has_3F_Hint = 1; // ��ȭ �� ��Ʈ ���� ����

        // ĳ���� �̵� ����
        EnablePlayerMovement();
    }

    private void ShowToiletUIOnly()
    {
        // ��ȭâ ���� ȭ��� ��Ʈ UI�� Ȱ��ȭ
        if (toiletUI != null)
            toiletUI.SetActive(true);

        if (toiletpapersound != null)
            toiletpapersound.Play(); // ���� ���

        // ĳ���� �̵� ����
        DisablePlayerMovement();

        uiActive = true; // UI�� Ȱ��ȭ���� ǥ��
    }

    private void HideAllUI()
    {
        // ��� UI�� ��Ȱ��ȭ
        if (toiletUI != null)
            toiletUI.SetActive(false);

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (characterPanel != null)
            characterPanel.SetActive(false);

        if (textPanel != null)
            textPanel.SetActive(false);

        if (toiletpapersound != null)
            toiletpapersound.Stop(); // ���� ����

        dialogueActive = false;
        uiActive = false;

        // ĳ���� �̵� ����
        EnablePlayerMovement();
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
            isPlayerInRange = true; // ���� �ȿ� �������� ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // ���� ������ �������� ǥ��

            // ������ ����� ��� UI ��Ȱ��ȭ �� �̵� ����
            HideAllUI();
        }
    }
}
