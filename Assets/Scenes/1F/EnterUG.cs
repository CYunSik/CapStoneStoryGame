using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUG : MonoBehaviour
{
    public string sceneName; // �̵��� ���� �̸��� Inspector���� ���� ����
    public Vector2 playerStartPosition; // ���� ������ ĳ���Ͱ� ������ ��ġ�� Inspector���� ���� ����

    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��

    void Update()
    {
        // �÷��̾ ���� �ȿ� �ְ� "F" Ű�� ��������, GameManager�� enterUG�� 1�� �� �� ��ȯ
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && GameManager.instance != null && GameManager.instance.PlayCat_1F == 1)
        {
            LoadSceneByName();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // �÷��̾�� �浹 ����
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName)) // �� �̸��� ������� ������ Ȯ��
        {
            SavePlayerStartPosition(); // ��ȣ�ۿ����� �� �̵��� �߻����� ���� ��ġ ����
            FadeManager.Instance.LoadSceneWithFade(sceneName); // �� �̸����� �� �ε�
        }
    }

    private void SavePlayerStartPosition()
    {
        // ĳ������ ���� ��ġ�� PlayerPrefs�� ����
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        // GameManager�� ���� �� �̸��� ��ġ ���� ������Ʈ
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(sceneName, playerStartPosition); // ���� �� �̸��� ���� ����
        }
    }
}