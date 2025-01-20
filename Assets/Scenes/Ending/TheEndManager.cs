using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Unity UI의 Image를 사용

public class TheEndManager : MonoBehaviour
{
    public UnityEngine.UI.Image fadePanel;  // 검은색 페이드아웃 패널
    public float fadeDuration = 2f;         // 페이드아웃에 걸리는 시간
    public float waitTime = 5f;             // "The End" 화면에서 대기 시간

    private bool isFading = false;

    private void Start()
    {
        if (fadePanel != null)
        {
            // 시작 시 패널 투명화
            fadePanel.color = new Color(0, 0, 0, 0);
        }

        // 5초 후 페이드아웃 시작
        StartCoroutine(ShowTheEndAndFadeOut());
    }

    private IEnumerator ShowTheEndAndFadeOut()
    {
        if (isFading) yield break; // 이미 페이드가 진행 중이라면 중복 실행 방지

        isFading = true; // 페이드 상태 설정
        yield return new WaitForSeconds(waitTime); // 대기 시간

        float elapsed = 0f;
        Color panelColor = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration); // 투명도를 0에서 1로 변화
            panelColor.a = alpha;
            fadePanel.color = panelColor;
            yield return null;
        }

        // 페이드아웃이 끝난 후 게임 종료
        Application.Quit();

        // 유니티 에디터에서 테스트 중인 경우 종료 대신 플레이 중지를 수행
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
