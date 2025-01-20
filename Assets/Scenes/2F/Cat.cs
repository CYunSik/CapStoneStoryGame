using System.Collections;
using UnityEngine;
using TMPro;

public class Cat : MonoBehaviour
{
    public Animator targetAnimator;         // Cat ������Ʈ�� Animator ������Ʈ
    public GameObject catObject;            // Cat ������Ʈ
    public GameObject keyringUI;            // ������ Ű�� UI ������Ʈ
    public GameObject dialogueUI;           // ��� UI ������Ʈ
    public TextMeshProUGUI dialogueText;    // ��� �ؽ�Ʈ
    [TextArea(1, 10)]
    public string[] dialogues;              // ����� ��� �迭
    public float hideDelay = 2.0f;          // ����� ������Ʈ�� ����� �������� ���� �ð�
    private bool uiActive = false;          // UI�� Ȱ��ȭ�Ǿ����� Ȯ��
    private bool isPlayerInRange = false;   // �÷��̾ Ʈ���� ���� �ȿ� �ִ��� Ȯ��

    private bool isAnimationPlaying = false;
    private bool hasShownMonologue = false; // ���� ��� ǥ�� ����

    public AudioSource catSound;            // ����̰� ��Ÿ�� �� ����� �Ҹ�
    public AudioSource keyringSound;        // Ű�� UI ǥ�� �� ����� �Ҹ�
    private int currentDialogueIndex = 0;   // ���� ��� �ε���
    private bool dialogueActive = false;    // ��ȭ Ȱ��ȭ ����

    private TechStudentController playerMovement; // ĳ���� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody;          // ĳ���� Rigidbody2D ����
    private Animator playerAnimator;              // ĳ���� �ִϸ����� ����

    private void Start()
    {
        // UI �ʱ�ȭ
        if (catObject != null) catObject.SetActive(false);
        if (keyringUI != null) keyringUI.SetActive(false);
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
        // Ʈ���� ���� �ȿ� ���� ���� �۵�
        if (isPlayerInRange)
        {
            // ����� ������Ʈ Ȱ��ȭ ����
            if (Input.GetKeyDown(KeyCode.F) && GameManager.instance.is_Cat == 0
                && GameManager.instance.is_Today_Time == 1)
            {
                if (!isAnimationPlaying && !hasShownMonologue)
                {
                    catObject.SetActive(true);         // ����� ������Ʈ Ȱ��ȭ
                    isAnimationPlaying = true;         // �ִϸ��̼� ���� ����
                    GameManager.instance.is_Cat = 1;

                    // ����̰� ��Ÿ�� �� �Ҹ� ���
                    if (catSound != null)
                    {
                        catSound.Play();
                    }

                    DisablePlayerMovement(); // ĳ���� ������ ��Ȱ��ȭ
                    StartCoroutine(HideCatAfterDelay());
                    hasShownMonologue = true;          // ���� ǥ�� ���� ����
                }
            }

            // F Ű�� ���� Ű�� UI ���
            if (Input.GetKeyDown(KeyCode.F) && GameManager.instance.has_Keyring == 1)
            {
                ToggleKeyringUI();
            }

            // ��ȭ �� �����̽��ٷ� ��ȭ ����
            if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceDialogue();
            }
        }
    }

    private void ToggleKeyringUI()
    {
        if (keyringUI != null)
        {
            uiActive = !uiActive;              // UI ���� ��ȯ
            keyringUI.SetActive(uiActive);     // UI Ȱ��ȭ ���� ����
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            // ���� ��� ���
            dialogueText.text = dialogues[currentDialogueIndex];

            // �� ��° ������ Ű�� UI ǥ��
            if (currentDialogueIndex == 2 && keyringUI != null)
            {
                keyringUI.SetActive(true);
                uiActive = true;

                // Ű�� ȹ�� ���� ����
                GameManager.instance.has_Keyring = 1;

                // Ű�� UI ǥ�� �� ���� ���
                if (keyringSound != null)
                {
                    keyringSound.Play();
                }
            }
        }
        else
        {
            // ��ȭ ����
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null) dialogueUI.SetActive(false); // ��� UI ��Ȱ��ȭ
        dialogueActive = false;

        EnablePlayerMovement(); // ĳ���� ������ Ȱ��ȭ
    }

    private IEnumerator HideCatAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        OnAnimationEnd();
    }

    public void OnAnimationEnd()
    {
        if (catObject != null) catObject.SetActive(false); // ����� ������Ʈ �����
        isAnimationPlaying = false; // �ִϸ��̼� ���� �ʱ�ȭ

        // ����� �ִϸ��̼��� ���� �� ��� UI ���
        if (dialogueUI != null && GameManager.instance.has_Keyring == 0)
        {
            dialogueUI.SetActive(true);
            dialogueActive = true;

            // ù ��° ��� ���
            currentDialogueIndex = 0;
            dialogueText.text = dialogues[currentDialogueIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // �÷��̾ ���� �ȿ� ������ true�� ����
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // �÷��̾ Ʈ���� ������ ����� false�� ����
            if (uiActive)             // UI�� Ȱ��ȭ�� ���
            {
                if (keyringUI != null) keyringUI.SetActive(false); // UI ��Ȱ��ȭ
                uiActive = false;           // UI ���� �ʱ�ȭ
            }
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
