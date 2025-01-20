using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Unity�� UI ���ӽ����̽�
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // Singleton Instance
    public UnityEngine.UI.Image fadeImage; // ���̵� ȿ���� ���� �̹���
    public float fadeDuration = 1.0f; // ���̵� �ð�

    public bool IsFading { get; private set; } // ���̵� ���� Ȯ�� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �� �ε� �̺�Ʈ ����
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
        IsFading = true; // ���̵� ����
        yield return new WaitForSeconds(0.5f); // 0.5�� ���

        fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ
        Color color = fadeImage.color;
        color.a = 1f; // ������ ��ο� ���·� ����
        fadeImage.color = color;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); // ���� ����
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false); // ���̵� �̹��� ��Ȱ��ȭ
        IsFading = false; // ���̵� ����
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        IsFading = true; // ���̵� ����

        fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ
        Color color = fadeImage.color;
        color.a = 0f; // ������ ���¿��� ����
        fadeImage.color = color;

        float timer = 0f;
        float fadeOutDuration = 0.5f; // ���̵� �ƿ� �ð��� 0.5�ʷ� ����

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeOutDuration); // ���� ����
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // �� �̵�
        SceneManager.LoadScene(sceneName);
        IsFading = false; // ���̵� ����
    }
}
