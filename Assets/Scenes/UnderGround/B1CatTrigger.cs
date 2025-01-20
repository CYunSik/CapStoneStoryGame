using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class B1CatTrigger : MonoBehaviour
{
    public GameObject catObject;         // ����� ������Ʈ (�ִϸ��̼� ����)
    public AudioSource catSound;        // ����̰� ��Ÿ�� �� ��µ� ����
    public Animator catAnimator;        // ����� Animator ������Ʈ
    public GameObject dialoguePanel;    // ��ȭâ Panel
    public Image characterIllustration; // ĳ���� �̹���
    public TextMeshProUGUI dialogueText; // ��ȭ �ؽ�Ʈ

    [TextArea(1, 10)]
    public string[] catDialogues;       // ����̿� ���õ� ���

    private bool hasTriggered = false;  // Ʈ���Ű� �̹� �ߵ��Ǿ����� ����
    private int currentDialogueIndex = 0; // ���� ��ȭ �ε���
    private bool isDialogueActive = false; // ��ȭ ���� ����

    private GameManager gameManager;    // GameManager ����
    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody; // �÷��̾� Rigidbody2D ����
    private Animator playerAnimator; // �÷��̾� �ִϸ����� ����

    void Start()
    {
        // GameManager�� �����մϴ�.
        gameManager = FindObjectOfType<GameManager>();

        if (catObject != null)
        {
            catObject.SetActive(false); // ���� �� ����� ������Ʈ ��Ȱ��ȭ
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
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // �÷��̾ Ʈ���ſ� �������� ��
        {
            // GameManager�� ���� �˻�
            if (gameManager != null && gameManager.DiaryOpen == 1 && gameManager.B1Cat == 0)
            {
                ActivateCatTrigger(); // Ʈ���� �ߵ�
            }
        }
    }

    private void ActivateCatTrigger()
    {
        hasTriggered = true; // Ʈ���Ű� �� ���� �ߵ��ǵ��� ����

        DisablePlayerMovement(); // ����� �ִϸ��̼� ���� �� �÷��̾� ���� ��Ȱ��ȭ

        if (catObject != null)
        {
            catObject.SetActive(true); // ����� ������Ʈ Ȱ��ȭ
        }

        if (catAnimator != null)
        {
            
        }
        else
        {
            Debug.LogWarning("Animator�� �����Ǿ� ���� �ʽ��ϴ�.");
        }

        if (catSound != null)
        {
            catSound.Play(); // ���� ���
        }

        StartCoroutine(ShowCatDialogueAfterDelay()); // ����� �ִϸ��̼� �� ��ȭ ����
    }

    private IEnumerator ShowCatDialogueAfterDelay()
    {
        yield return new WaitForSeconds(1f); // ����� �ִϸ��̼� 1�� ���

        if (catObject != null)
        {
            catObject.SetActive(false); // ����� ������Ʈ ��Ȱ��ȭ
        }

        StartDialogue(); // ��ȭ ����
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
        dialogueText.text = catDialogues[currentDialogueIndex]; // ù ��° ��� ǥ��
        isDialogueActive = true;
    }

    private void Update()
    {
        // ��ȭ �� Space Ű�� ��ȭ �ѱ��
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < catDialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = catDialogues[currentDialogueIndex];
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

        isDialogueActive = false;

        // �÷��̾� ���� Ȱ��ȭ
        EnablePlayerMovement();

        // GameManager�� B1Cat ���� 1�� ����
        if (gameManager != null)
        {
            gameManager.B1Cat = 1;
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
