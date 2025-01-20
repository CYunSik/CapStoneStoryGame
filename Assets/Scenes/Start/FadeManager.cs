using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Unity의 UI 네임스페이스
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // Singleton Instance
    public UnityEngine.UI.Image fadeImage; // 페이드 효과를 위한 이미지
    public float fadeDuration = 1.0f; // 페이드 시간

    public bool IsFading { get; private set; } // 페이드 상태 확인 변수

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeIn()
    {
        IsFading = true; // 페이드 시작
        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
        Color color = fadeImage.color;
        color.a = 1f; // 완전히 어두운 상태로 시작
        fadeImage.color = color;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); // 투명도 감소
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false); // 페이드 이미지 비활성화
        IsFading = false; // 페이드 종료
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        IsFading = true; // 페이드 시작

        fadeImage.gameObject.SetActive(true); // 페이드 이미지 활성화
        Color color = fadeImage.color;
        color.a = 0f; // 투명한 상태에서 시작
        fadeImage.color = color;

        float timer = 0f;
        float fadeOutDuration = 0.5f; // 페이드 아웃 시간을 0.5초로 설정

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeOutDuration); // 투명도 증가
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // 씬 이동
        SceneManager.LoadScene(sceneName);
        IsFading = false; // 페이드 종료
    }
}
