using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryController : MonoBehaviour
{
    public static DiaryController instance; // �̱��� �ν��Ͻ�

    public UnityEngine.UI.Image diaryImage; // �ϱ��� �̹���
    public Button nextButton;              // ���� ������ ��ư
    public Button prevButton;              // ���� ������ ��ư
    public AudioSource pageTurnSound;      // ������ �ѱ� ����
    public TMP_Text diaryText;             // ������ �ؽ�Ʈ ǥ�ø� ���� TextMeshPro �ؽ�Ʈ

    public int currentPage = 0;           // ���� ������ ��ȣ
    private int totalPages;                // ��ü ������ �� (pageTexts �迭�� ���� �ڵ� ����)

    // �ϱ��� ������ �̹��� �迭 (�� �������� �ش��ϴ� �̹����� ����)
    public Sprite[] pageSprites;

    // �ϱ��� ������ �ؽ�Ʈ �迭 (�� �������� �ؽ�Ʈ�� ����)
    [TextArea(1, 10)]
    public string[] pageTexts;

    private void Awake()
    {
        // �̱��� ����
        if (instance == null)
        {
            instance = this; // �ν��Ͻ� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }

    void Start()
    {
        totalPages = pageTexts.Length;      // ��ü ������ ���� �ؽ�Ʈ �迭�� ���̷� ����
        UpdatePageButtons();               // ���� �� ��ư ���� ������Ʈ
        UpdateDiaryContent();              // ���� �� ù ������ ���� ������Ʈ
    }

    public void NextPage()
    {
        if (currentPage < totalPages - 1)  // ������ �������� �ƴ� ��쿡��
        {
            currentPage++;
            PlayPageTurnSound();           // ������ �ѱ� �� ���� ���
            UpdatePageButtons();
            UpdateDiaryContent();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)               // ù �������� �ƴ� ��쿡��
        {
            currentPage--;
            PlayPageTurnSound();           // ������ �ѱ� �� ���� ���
            UpdatePageButtons();
            UpdateDiaryContent();
        }
    }

    private void UpdatePageButtons()
    {
        // ù �������� ������ �������� ��� ��ư�� ��Ȱ��ȭ
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;
    }

    private void UpdateDiaryContent()
    {
        // ���� �������� �ش��ϴ� �̹����� diaryImage�� ����
        if (pageSprites != null && pageSprites.Length > currentPage)
        {
            diaryImage.sprite = pageSprites[currentPage];
        }

        // ���� �������� �ش��ϴ� �ؽ�Ʈ�� diaryText�� ����
        if (pageTexts != null && pageTexts.Length > currentPage)
        {
            diaryText.text = pageTexts[currentPage];
        }
    }

    private void PlayPageTurnSound()
    {
        if (pageTurnSound != null)
        {
            pageTurnSound.Play();          // ������ �ѱ� �� ���� ���
        }
    }
}