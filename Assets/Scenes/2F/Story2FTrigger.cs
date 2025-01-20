using System.Collections;
using UnityEngine;
using TMPro;

public class Story2FTrigger : MonoBehaviour
{
    public GameObject dialogueUI;            // ��� UI ������Ʈ
    public TextMeshProUGUI dialogueText;     // ��� �ؽ�Ʈ
    public GameObject diaryUI;               // ���� UI ������Ʈ
    public AudioSource triggerSound;         // Ʈ���� ����
    [TextArea(1, 10)]
    public string[] dialogues;               // ��� �ؽ�Ʈ �迭
    private int currentDialogueIndex = 0;    // ���� ��� �ε���
    private bool dialogueActive = false;     // ��ȭ Ȱ��ȭ ����
    private bool isTriggerActivated = false; // Ʈ���Ű� �̹� �۵��ߴ��� Ȯ��
    private bool diaryShown = false;         // ���� UI�� �̹� ǥ�õǾ����� Ȯ��

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody;          // ĳ���� Rigidbody2D ����
    private Animator playerAnimator;              // ĳ���� �ִϸ����� ����

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.instance.is_2FHint == 1 && GameManager.instance.is_2FStoryTrigger == 0 && !isTriggerActivated)
        {
            isTriggerActivated = true; // Ʈ���Ű� �۵������� ���
            PlayTriggerSound();        // ���� ��� �� �ļ� �۾� ����
        }
    }

    private void PlayTriggerSound()
    {
        if (triggerSound != null)
        {
            DisablePlayerMovement(); // ĳ���� ������ ��Ȱ��ȭ
            triggerSound.Play();
            StartCoroutine(WaitForSoundToEnd(triggerSound.clip.length)); // ���尡 ���� ������ ���
        }
        else
        {
            StartDialogue(); // ���尡 ������ �ٷ� ��ȭ ����
        }
    }

    private IEnumerator WaitForSoundToEnd(float duration)
    {
        yield return new WaitForSeconds(duration); // ���� ��� �ð� ���
        StartDialogue();                           // ���尡 ���� �� ��ȭ ����
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
        }
    }

    private void Update()
    {
        // ��ȭ UI�� Ȱ��ȭ�� ��� Space Ű�� ��� ����
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = dialogues[currentDialogueIndex];

            // �ټ� ��° ��翡�� ���� UI ǥ��
            if (currentDialogueIndex == 4 && diaryUI != null && !diaryShown)
            {
                ShowDiaryUI();
            }
        }
        else
        {
            // ��ȭ ����
            EndDialogue();
        }
    }

    private void ShowDiaryUI()
    {
        diaryShown = true; // ���� UI�� ǥ�õǾ����� ���
        diaryUI.SetActive(true); // ���� UI Ȱ��ȭ
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // ��� UI ��Ȱ��ȭ
        if (diaryUI != null) diaryUI.SetActive(false);       // ���� UI ��Ȱ��ȭ
        dialogueActive = false;

        EnablePlayerMovement(); // ĳ���� ������ Ȱ��ȭ

        // GameManager ���� ������Ʈ
        if (GameManager.instance != null)
        {
            GameManager.instance.is_2FStoryTrigger = 1; // Ʈ���� �Ϸ� ���·� ����
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
