using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Unity UI�� Image�� ���

public class TheEndManager : MonoBehaviour
{
    public UnityEngine.UI.Image fadePanel;  // ������ ���̵�ƿ� �г�
    public float fadeDuration = 2f;         // ���̵�ƿ��� �ɸ��� �ð�
    public float waitTime = 5f;             // "The End" ȭ�鿡�� ��� �ð�

    private bool isFading = false;

    private void Start()
    {
        if (fadePanel != null)
        {
            // ���� �� �г� ����ȭ
            fadePanel.color = new Color(0, 0, 0, 0);
        }

        // 5�� �� ���̵�ƿ� ����
        StartCoroutine(ShowTheEndAndFadeOut());
    }

    private IEnumerator ShowTheEndAndFadeOut()
    {
        if (isFading) yield break; // �̹� ���̵尡 ���� ���̶�� �ߺ� ���� ����

        isFading = true; // ���̵� ���� ����
        yield return new WaitForSeconds(waitTime); // ��� �ð�

        float elapsed = 0f;
        Color panelColor = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration); // ������ 0���� 1�� ��ȭ
            panelColor.a = alpha;
            fadePanel.color = panelColor;
            yield return null;
        }

        // ���̵�ƿ��� ���� �� ���� ����
        Application.Quit();

        // ����Ƽ �����Ϳ��� �׽�Ʈ ���� ��� ���� ��� �÷��� ������ ����
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
