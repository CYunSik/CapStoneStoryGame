using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ�

public class Start3F : MonoBehaviour
{
    public Image characterIllustration; // ĳ���� �̹���
    public TextMeshProUGUI monologueText; // TextMeshPro ��ȭ �ؽ�Ʈ
    public GameObject dialoguePanel; // ��ȭâ Panel

    [TextArea(1, 10)] // �ּ� 1��, �ִ� 10�ٷ� ����
    public string[] dialogues; // ����� ��ȭ ���
    private int dialogueIndex = 0; // ���� ��ȭ �ε���
    private bool isDialogueActive = false; // ��ȭ Ȱ��ȭ ����

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody; // ĳ���� Rigidbody2D ����
    private Animator playerAnimator; // ĳ���� �ִϸ����� ����

    private void Start()
    {
        // ���� �� ��ȭâ ��Ȱ��ȭ
        dialoguePanel.SetActive(false);
        characterIllustration.gameObject.SetActive(false);
        monologueText.gameObject.SetActive(false);

        // ĳ���� �̵� ��ũ��Ʈ, Rigidbody2D, Animator ����
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
        if (other.CompareTag("Player") && GameManager.instance.is_3F_Start == 0)
        {
            GameManager.instance.is_3F_Start = 1;
            StartDialogue();
        }
    }

    private void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            ShowNextDialogue();
        }
    }

    private void StartDialogue()
    {
        // ��ȭâ�� ĳ���� �̹��� Ȱ��ȭ
        dialoguePanel.SetActive(true);
        characterIllustration.gameObject.SetActive(true);
        monologueText.gameObject.SetActive(true);

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

        // ù ��° ��ȭ ���
        dialogueIndex = 0;
        monologueText.text = dialogues[dialogueIndex];
        isDialogueActive = true;
    }

    private void ShowNextDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex < dialogues.Length)
        {
            // ���� ��ȭ ���
            monologueText.text = dialogues[dialogueIndex];
        }
        else
        {
            // ��ȭ ����
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // ��ȭâ�� �̹��� ��Ȱ��ȭ
        dialoguePanel.SetActive(false);
        characterIllustration.gameObject.SetActive(false);
        monologueText.gameObject.SetActive(false);

        // ĳ���� �̵� ��ũ��Ʈ Ȱ��ȭ
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isDialogueActive = false;
    }
}
