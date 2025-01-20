using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Diagnostics;

public class Ending1 : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro ������Ʈ
    public string[] dialogues; // ǥ���� ��� �迭
    public string nextSceneName; // ���� �� �̸�

    private int currentDialogueIndex = 0; // ���� ǥ�� ���� ����� �ε���
    private bool canAdvanceDialogue = true; // ��縦 �ѱ� �� �ִ� ���� Ȯ��

    private void Start()
    {
        if (dialogues.Length > 0)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // ù ��° ��� ǥ��
        }
        else
        {

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canAdvanceDialogue)
        {
            StartCoroutine(AdvanceDialogueWithDelay());
        }
    }

    private IEnumerator AdvanceDialogueWithDelay()
    {
        canAdvanceDialogue = false; // ��� ���� �Ұ� ���·� ��ȯ
        AdvanceDialogue(); // ��� ����
        yield return new WaitForSeconds(1f); // 1�� ���
        canAdvanceDialogue = true; // ��� ���� ���� ���·� ����
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // ���� ��� ǥ��
        }
        else
        {
            // ��� ��簡 ���� ��� ���� ������ �̵�
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // ���� ���
        }
    }
}