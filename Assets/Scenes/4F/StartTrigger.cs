using UnityEngine;
using TMPro;

public class StartTrigger : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro UI 텍스트 컴포넌트
    public GameObject dialogueUI; // 대사 UI (Canvas)
    public GameObject clockUI; // 시계 UI
    public string[] dialogues; // 표시할 대사 배열
    public GameObject player; // 캐릭터 오브젝트
    public MonoBehaviour playerControllerScript; // 캐릭터 움직임 스크립트

    private int currentDialogueIndex = 0; // 현재 대사 인덱스
    private bool dialogueActive = false; // 대화 활성화 여부
    private bool hasTriggered = false; // 트리거가 이미 발동했는지 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // GameManager의 GameStart가 0일 때만 작동
        if (!hasTriggered && other.gameObject == player && GameManager.instance.GameStart == 0)
        {
            hasTriggered = true; // 트리거가 한 번만 작동
            GameManager.instance.GameStart = 1; // GameStart 값을 1로 변경
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
        // 대화 UI 활성화
        dialogueUI.SetActive(true);
        dialogueActive = true;

        // 첫 번째 대사 표시
        currentDialogueIndex = 0;
        dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);

        // 캐릭터 움직임 비활성화
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        // 시계 UI 초기 상태 비활성화
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
            // 특정 대사에서 시계 UI 활성화
            if (currentDialogueIndex == 2 && clockUI != null)
            {
                clockUI.SetActive(true);
            }

            // 다음 대사 표시
            dialogueText.text = FormatDialogue(dialogues[currentDialogueIndex]);
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // 대화 UI 비활성화
        dialogueUI.SetActive(false);
        dialogueActive = false;

        // 캐릭터 움직임 다시 활성화
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true;
        }

        // 시계 UI 비활성화
        if (clockUI != null)
        {
            clockUI.SetActive(false);
        }
    }

    private string FormatDialogue(string dialogue)
    {
        // TextMeshPro에서 여러 줄로 표시되도록 포맷팅
        return dialogue.Replace("\\n", "\n");
    }
}