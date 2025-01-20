using UnityEngine;
using TMPro;

public class StartTrigger : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro UI �ؽ�Ʈ ������Ʈ
    public GameObject dialogueUI; // ��� UI (Canvas)
    public GameObject clockUI; // �ð� UI
    public string[] dialogues; // ǥ���� ��� �迭
    public GameObject player; // ĳ���� ������Ʈ
    public MonoBehaviour playerControllerScript; // ĳ���� ������ ��ũ��Ʈ

    private int currentDialogueIndex = 0; // ���� ��� �ε���
    private bool dialogueActive = false; // ��ȭ Ȱ��ȭ ����
    private bool hasTriggered = false; // Ʈ���Ű� �̹� �ߵ��ߴ��� Ȯ��

    private void OnTriggerEnter2D(Collider2D other)
    {
        // GameManager�� GameStart�� 0�� ���� �۵�
        if (!hasTriggered && other.gameObject == player && GameManager.instance.GameStart == 0)
        {
            hasTriggered = true; // Ʈ���Ű� �� ���� �۵�
            GameManager.instance.GameStart = 1; // GameStart ���� 1�� ����
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

        // ù ��° ��� ǥ��
        currentDialogueIndex = 0;
        dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);

        // ĳ���� ������ ��Ȱ��ȭ
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        // �ð� UI �ʱ� ���� ��Ȱ��ȭ
        if (clockUI != null)
        {
            clockUI.SetActive(false);
        }
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // Ư�� ��翡�� �ð� UI Ȱ��ȭ
            if (currentDialogueIndex == 2 && clockUI != null)
            {
                clockUI.SetActive(true);
            }

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

        // ĳ���� ������ �ٽ� Ȱ��ȭ
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true;
        }

        // �ð� UI ��Ȱ��ȭ
        if (clockUI != null)
        {
            clockUI.SetActive(false);
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro���� ���� �ٷ� ǥ�õǵ��� ������
        return dialogue.Replace("\\n", "\n");
    }
}