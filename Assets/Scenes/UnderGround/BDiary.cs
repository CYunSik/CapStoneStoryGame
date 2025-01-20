using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDiary : MonoBehaviour
{
    public GameObject BDiaryUI; // "�Խ��� �Ź�" UI ������Ʈ
    public AudioSource noteboardsound; // �Խ��� ���� ����� �ҽ�
    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool uiActive = false; // UI�� Ȱ��ȭ�Ǿ����� Ȯ��

    private void Start()
    {
        if (BDiaryUI != null)
        {
            BDiaryUI.SetActive(false); // ���� �� UI ��Ȱ��ȭ
        }

        if (noteboardsound != null)
        {
            noteboardsound.Stop(); // ���� �� ����� ����
        }
    }

    private void Update()
    {
        // ���� �ȿ� ���� ���� "F" Ű �Է��� üũ
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.instance.has_news_paper = 1;

            // UI ���� ��ȯ �� ����� ���
            ToggleNoticeboardUI();
        }

        // �÷��̾ ���� ������ ������ �� UI ��Ȱ��ȭ
        if (!isPlayerInRange && uiActive)
        {
            BDiaryUI.SetActive(false);
            uiActive = false;
        }
    }

    private void ToggleNoticeboardUI()
    {
        if (BDiaryUI != null)
        {
            uiActive = !uiActive;
            BDiaryUI.SetActive(uiActive); // UI ���� ��ȯ

            // UI�� Ȱ��ȭ�� ���� ���� ���
            if (uiActive && noteboardsound != null)
            {
                noteboardsound.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // ���� �ȿ� �������� ǥ��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // ���� ������ �������� ǥ��
        }
    }
}
