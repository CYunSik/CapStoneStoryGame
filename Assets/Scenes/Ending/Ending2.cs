using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending2 : MonoBehaviour
{
    public GameObject dialogueUI;          // ��� UI ������Ʈ
    public TextMeshProUGUI dialogueText;   // ��� �ؽ�Ʈ
    [TextArea(1, 10)]
    public string[] dialogues;             // ��� �ؽ�Ʈ �迭
    private int currentDialogueIndex = 0;  // ���� ��� �ε���
    private bool dialogueActive = false;   // ��ȭ Ȱ��ȭ ����

    public string nextSceneName;           // ���� ���� �̸�

    private void Start()
    {
        // UI �ʱ�ȭ
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // ��� UI Ȱ��ȭ
        }

        // ù ��° ��� ǥ��
        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        dialogueActive = true;
    }

    private void Update()
    {
        // ��� UI�� Ȱ��ȭ�� ��� �����̽� Ű�� ��� ����
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
            // ���� ��� ǥ��
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // ��ȭ ���� �� ���� ������ �̵�
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ��� UI ��Ȱ��ȭ
        }

        dialogueActive = false;

        // ���� ������ �̵�
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
        }
    }
}
