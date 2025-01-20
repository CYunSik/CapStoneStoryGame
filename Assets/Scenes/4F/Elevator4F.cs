using UnityEngine;
using TMPro;

public class Elevator4F : MonoBehaviour
{
    public GameObject elevatorUI; // "��� ����" UI ������Ʈ
    public AudioSource warningSound; // ����� ����� �ҽ�
    public TMP_Text dialogueText; // TextMeshPro ��ȭ �ؽ�Ʈ
    public GameObject dialogueUI; // ��ȭ UI (Canvas)
    public string[] dialogues; // ǥ���� ��� �迭
    public MonoBehaviour playerControllerScript; // ĳ���� ������ ��ũ��Ʈ

    private Rigidbody2D playerRigidbody; // ĳ������ Rigidbody2D
    private Animator playerAnimator; // ĳ������ Animator

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool uiActive = false; // "��� ����" UI�� Ȱ��ȭ�Ǿ����� Ȯ��
    private bool dialogueActive = false; // ��ȭ ���� ����
    private bool hasUIShown = false; // "��� ���� UI"�� �� �� Ȱ��ȭ�Ǿ����� Ȯ��
    private bool isActiveTrigger = false; // ���� Ʈ���Ű� Ȱ��ȭ �������� Ȯ��

    private int currentDialogueIndex = 0; // ���� ��� �ε���

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        if (elevatorUI != null)
        {
            elevatorUI.SetActive(false); // ���� �� "��� ����" UI ��Ȱ��ȭ
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ���� �� ��ȭ UI ��Ȱ��ȭ
        }

        if (warningSound != null)
        {
            warningSound.Stop(); // ���� �� ����� ����
        }
    }

    private void Update()
    {
        // ���� �ȿ� ���� ���� "F" Ű �Է��� üũ
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (GameManager.instance.has_Interacted_Elevator == 0)
            {
                // ó�� ��ȣ�ۿ� �� ��ȭ ����
                StartDialogue();
            }
            else if (GameManager.instance.has_Interacted_Elevator == 1)
            {
                // ���� ��ȣ�ۿ� �� "��� ����" UI Ȱ��ȭ/��Ȱ��ȭ ��ȯ
                ToggleElevatorUI();
            }
        }

        // ��ȭ �� �����̽��ٷ� ��� ����
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }

        // �÷��̾ ���� ������ ������ �� ��� UI ��Ȱ��ȭ
        if (!isPlayerInRange)
        {
            DisableAllUI();
        }
    }

    private void ToggleElevatorUI()
    {
        if (elevatorUI != null)
        {
            uiActive = !uiActive; // UI ���� ��ȯ
            elevatorUI.SetActive(uiActive);

            // ������� UI Ȱ��ȭ �ÿ��� ���
            if (uiActive && warningSound != null)
            {
                warningSound.Play();
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // ��ȭ UI Ȱ��ȭ
            dialogueActive = true;
            isActiveTrigger = true; // ���� Ʈ���� Ȱ��ȭ

            currentDialogueIndex = 0; // ��ȭ ���� �ε��� �ʱ�ȭ
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]); // ù ��° ��� ǥ��

            // ĳ���� ������ ��Ȱ��ȭ
            DisablePlayerMovement();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]); // ���� ��� ǥ��

            // 3��° ����� �� "��� ����" UI Ȱ��ȭ
            if (currentDialogueIndex == 2 && !hasUIShown)
            {
                ShowElevatorUI(); // "��� ����" UI ǥ��
                hasUIShown = true; // �� �� Ȱ��ȭ�Ǿ����� ���
            }
        }
        else
        {
            EndDialogue(); // ��ȭ ����
        }
    }

    private void ShowElevatorUI()
    {
        if (elevatorUI != null)
        {
            uiActive = true;
            elevatorUI.SetActive(true); // "��� ����" UI Ȱ��ȭ

            // ����� ���
            if (warningSound != null)
            {
                warningSound.Play();
            }
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
        }

        dialogueActive = false;
        isActiveTrigger = false;

        // GameManager ���� ������Ʈ
        GameManager.instance.has_Interacted_Elevator = 1;

        // ĳ���� ������ �ٽ� Ȱ��ȭ
        EnablePlayerMovement();
    }

    private void DisableAllUI()
    {
        if (elevatorUI != null)
        {
            elevatorUI.SetActive(false); // "��� ����" UI ��Ȱ��ȭ
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
        }

        uiActive = false;
        dialogueActive = false;

        if (isActiveTrigger) // ���� Ʈ���Ű� Ȱ��ȭ ������ ���� �̵� ����
        {
            EnablePlayerMovement();
            isActiveTrigger = false; // ���� Ʈ���� ��Ȱ��ȭ
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro���� ���� �ٷ� ǥ�õǵ��� ������
        return dialogue.Replace("\\n", "\n");
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
            DisableAllUI(); // ���� ������ ������ ��� UI ��Ȱ��ȭ
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
            playerControllerScript.enabled = false; // �̵� ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // �̵� ��ũ��Ʈ Ȱ��ȭ
        }
    }
}
