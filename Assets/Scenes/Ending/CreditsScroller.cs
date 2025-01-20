using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    public float scrollSpeed = 50f;              // ��ũ�� �ӵ�
    public RectTransform creditsText;           // ũ���� �ؽ�Ʈ�� RectTransform
    public float startYPosition = -500f;        // ũ���� ���� y ��ġ
    public float endPositionY = 1000f;          // ũ������ ���� y ��ġ
    public float delayBeforeExit = 3f;          // ũ���� ���� �� ��� �ð�
    public string nextSceneName;                // ���� ���� �̸� (���� ���� ��� �� �̵�)

    private bool isScrolling = true;

    private void Start()
    {
        // ũ���� ���� ��ġ�� ����
        if (creditsText != null)
        {
            creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, startYPosition);
        }
    }

    private void Update()
    {
        if (isScrolling)
        {
            // ũ���� �ؽ�Ʈ�� ���� �̵�
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // ũ������ �� ��ġ�� �����ϸ� ��ũ�� ����
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
            // ���� ������ �̵�
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            // ���� ���� (UnityEditor������ ������� ����)
            UnityEngine.Application.Quit();
        }
    }
}
