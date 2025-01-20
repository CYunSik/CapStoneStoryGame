using UnityEngine;
using TMPro;

public class StoryTrigger4F1 : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro UI �ؽ�Ʈ ������Ʈ
    public GameObject dialogueUI; // ��� UI (Canvas)
    public string[] dialogues; // ǥ���� ��� �迭
    public GameObject player; // ĳ���� ������Ʈ
    private TechStudentController playerMovement; // ĳ���� ������ ��ũ��Ʈ
    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator; // ĳ���� �ִϸ����� ����

    private int currentDialogueIndex = 0; // ���� ��� �ε���
    private bool dialogueActive = false; // ��ȭ Ȱ��ȭ ����
    private bool hasTriggered = false; // Ʈ���Ű� �̹� �ߵ��ߴ��� Ȯ��

    private void Start()
    {
        // ĳ���� �̵� ��ũ��Ʈ, Rigidbody2D, Animator ����
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // ���� �� ��ȭâ ��Ȱ��ȭ
        dialogueUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // GameManager�� Story4Ffirst�� 0�� ���� �۵�
        if (!hasTriggered && other.gameObject == player && GameManager.instance.Story4Ffirst == 0)
        {
            hasTriggered = true; // Ʈ���Ű� �� ���� �۵�
            GameManager.instance.Story4Ffirst = 1; // Story4Ffirst ���� 1�� ����
            StartDialogue();
        }
    }

    private void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void StartDialogue()
    {
        // ��ȭ UI Ȱ��ȭ
        dialogueUI.SetActive(true);
        dialogueActive = true;

        // ĳ���� �̵� ��� ����
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // �̵� �ӵ� ����
            playerRigidbody.angularVelocity = 0f;   // ȸ�� �ӵ� ����
        }

        // �ȴ� ��� ���� �� Idle �ִϸ��̼� ���� ����
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle ���·� ��ȯ
        }

        // ĳ���� �̵� ��ũ��Ʈ ��Ȱ��ȭ
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // ù ��° ��� ǥ��
        currentDialogueIndex = 0;
        dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��� ǥ��
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);
        }
        else
        {
            // ��ȭ ����
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // ��ȭ UI ��Ȱ��ȭ
        dialogueUI.SetActive(false);
        dialogueActive = false;

        // ĳ���� �̵� ��ũ��Ʈ Ȱ��ȭ
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro���� ���� �ٷ� ǥ�õǵ��� ������
        return dialogue.Replace("\\n", "\n");
    }
}
