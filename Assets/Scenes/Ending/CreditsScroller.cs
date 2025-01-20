using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;              // 스크롤 속도
    public RectTransform creditsText;           // 크레딧 텍스트의 RectTransform
    public float startYPosition = -500f;        // 크레딧 시작 y 위치
    public float endPositionY = 1000f;          // 크레딧이 멈출 y 위치
    public float delayBeforeExit = 3f;          // 크레딧 종료 후 대기 시간
    public string nextSceneName;                // 다음 씬의 이름 (게임 종료 대신 씬 이동)

    private bool isScrolling = true;

    private void Start()
    {
        // 크레딧 시작 위치를 설정
        if (creditsText != null)
        {
            creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, startYPosition);
        }
    }

    private void Update()
    {
        if (isScrolling)
        {
            // 크레딧 텍스트를 위로 이동
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // 크레딧이 끝 위치에 도달하면 스크롤 종료
            if (creditsText.anchoredPosition.y >= endPositionY)
            {
                isScrolling = false;
                StartCoroutine(EndCredits());
            }
        }
    }

    private IEnumerator EndCredits()
    {
        yield return new WaitForSeconds(delayBeforeExit);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // 다음 씬으로 이동
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            // 게임 종료 (UnityEditor에서는 종료되지 않음)
            UnityEngine.Application.Quit();
        }
    }
}
