using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending2 : MonoBehaviour
{
    public GameObject dialogueUI;          // 대사 UI 오브젝트
    public TextMeshProUGUI dialogueText;   // 대사 텍스트
    [TextArea(1, 10)]
    public string[] dialogues;             // 대사 텍스트 배열
    private int currentDialogueIndex = 0;  // 현재 대사 인덱스
    private bool dialogueActive = false;   // 대화 활성화 여부

    public string nextSceneName;           // 다음 씬의 이름

    private void Start()
    {
        // UI 초기화
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // 대사 UI 활성화
        }

        // 첫 번째 대사 표시
        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex];
        dialogueActive = true;
    }

    private void Update()
    {
        // 대사 UI가 활성화된 경우 스페이스 키로 대사 진행
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
            // 다음 대사 표시
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            // 대화 종료 후 다음 씬으로 이동
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 대사 UI 비활성화
        }

        dialogueActive = false;

        // 다음 씬으로 이동
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
        }
    }
}
