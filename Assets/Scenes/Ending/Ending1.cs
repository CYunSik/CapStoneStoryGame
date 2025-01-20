using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Diagnostics;

public class Ending1 : MonoBehaviour
{
    public TMP_Text dialogueText; // TextMeshPro 컴포넌트
    public string[] dialogues; // 표시할 대사 배열
    public string nextSceneName; // 다음 씬 이름

    private int currentDialogueIndex = 0; // 현재 표시 중인 대사의 인덱스
    private bool canAdvanceDialogue = true; // 대사를 넘길 수 있는 상태 확인

    private void Start()
    {
        if (dialogues.Length > 0)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // 첫 번째 대사 표시
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
        canAdvanceDialogue = false; // 대사 진행 불가 상태로 전환
        AdvanceDialogue(); // 대사 진행
        yield return new WaitForSeconds(1f); // 1초 대기
        canAdvanceDialogue = true; // 대사 진행 가능 상태로 복구
    }

    private void AdvanceDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[currentDialogueIndex]; // 다음 대사 표시
        }
        else
        {
            // 모든 대사가 끝난 경우 다음 씬으로 이동
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            FadeManager.Instance.LoadSceneWithFade(nextSceneName); // 변수 사용
        }
    }
}