using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyringUIController : MonoBehaviour
{
    public GameObject keyringUI;          // Ű�� UI ������Ʈ
    public GameObject player;            // �÷��̾� ������Ʈ
    public GameObject dialoguePanel;     // ��ȭâ Panel
    public Image characterIllustration;  // ĳ���� �̹���
    public TextMeshProUGUI dialogueText; // ��ȭ �ؽ�Ʈ
    [TextArea(1, 10)]
    public string[] exitKeyDialogues;    // Ű�� ��ȭ ����
    public float interactionDistance = 2f; // ��ȣ�ۿ� �Ÿ�

    private bool isUIActive = false;     // Ű�� UI�� Ȱ��ȭ�Ǿ� �ִ��� ����
    private bool isDialogueActive = false; // ��ȭ�� ���� ������ ����
    private int currentDialogueIndex = 0;  // ���� ��ȭ �ε���

    private TechStudentController playerMovement; // �÷��̾� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody;         // �÷��̾� Rigidbody2D ����
    private Animator playerAnimator;             // �÷��̾� �ִϸ����� ����

    void Start()
    {
        if (keyringUI != null)
        {
            keyringUI.SetActive(false); // ���� �� Ű�� UI ��Ȱ��ȭ
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // ��ȭâ ��Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // ĳ���� �̹��� ��Ȱ��ȭ
        }

        // �÷��̾� �̵� �� �ִϸ����� ����
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // B1Cat�� 1���� Ȯ�� (GameManager���� ���� Ȯ��)
        if (GameManager.instance.B1Cat == 1)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            // F Ű�� Ű�� UI Ȱ��ȭ
            if (!isDialogueActive && !isUIActive && distance <= interactionDistance && Input.GetKeyDown(KeyCode.F))
            {
                ActivateKeyringUI();
            }

            // ��ȭ ���� �� Space Ű�� ��ȭ �ѱ��
            if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                ShowNextDialogue();
            }
        }
    }

    private void ActivateKeyringUI()
    {
        if (keyringUI != null)
        {
            keyringUI.SetActive(true); // Ű�� UI Ȱ��ȭ
            isUIActive = true;

            // 1�� �ڿ� ��ȭâ Ȱ��ȭ
            Invoke(nameof(StartDialogue), 1f);
        }

        DisablePlayerMovement(); // Ű�� UI�� Ȱ��ȭ�Ǹ� �÷��̾� ���� ��Ȱ��ȭ
    }

    private void StartDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true); // ��ȭâ Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // ĳ���� �̹��� Ȱ��ȭ
        }

        currentDialogueIndex = 0;
        dialogueText.text = exitKeyDialogues[currentDialogueIndex]; // ù ��° ��� ǥ��
        isDialogueActive = true;
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < exitKeyDialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = exitKeyDialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // ��ȭ ����
        }
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // ��ȭâ ��Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // ĳ���� �̹��� ��Ȱ��ȭ
        }

        if (keyringUI != null)
        {
            keyringUI.SetActive(false); // Ű�� UI ��Ȱ��ȭ
        }

        isDialogueActive = false;
        isUIActive = false;

        // �÷��̾� ���� Ȱ��ȭ
        EnablePlayerMovement();

        // GameManager�� has_Exit_Key ���� 1�� ����
        if (GameManager.instance != null)
        {
            GameManager.instance.has_Exit_Key = 1;
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
