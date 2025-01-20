using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryController : MonoBehaviour
{
    public static DiaryController instance; // 싱글톤 인스턴스

    public UnityEngine.UI.Image diaryImage; // 일기장 이미지
    public Button nextButton;              // 다음 페이지 버튼
    public Button prevButton;              // 이전 페이지 버튼
    public AudioSource pageTurnSound;      // 페이지 넘김 사운드
    public TMP_Text diaryText;             // 페이지 텍스트 표시를 위한 TextMeshPro 텍스트

    public int currentPage = 0;           // 현재 페이지 번호
    private int totalPages;                // 전체 페이지 수 (pageTexts 배열에 따라 자동 설정)

    // 일기장 페이지 이미지 배열 (각 페이지에 해당하는 이미지를 설정)
    public Sprite[] pageSprites;

    // 일기장 페이지 텍스트 배열 (각 페이지의 텍스트를 설정)
    [TextArea(1, 10)]
    public string[] pageTexts;

    private void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
        {
            instance = this; // 인스턴스 설정
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    void Start()
    {
        totalPages = pageTexts.Length;      // 전체 페이지 수를 텍스트 배열의 길이로 설정
        UpdatePageButtons();               // 시작 시 버튼 상태 업데이트
        UpdateDiaryContent();              // 시작 시 첫 페이지 내용 업데이트
    }

    public void NextPage()
    {
        if (currentPage < totalPages - 1)  // 마지막 페이지가 아닌 경우에만
        {
            currentPage++;
            PlayPageTurnSound();           // 페이지 넘길 때 사운드 재생
            UpdatePageButtons();
            UpdateDiaryContent();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)               // 첫 페이지가 아닌 경우에만
        {
            currentPage--;
            PlayPageTurnSound();           // 페이지 넘길 때 사운드 재생
            UpdatePageButtons();
            UpdateDiaryContent();
        }
    }

    private void UpdatePageButtons()
    {
        // 첫 페이지나 마지막 페이지일 경우 버튼을 비활성화
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;
    }

    private void UpdateDiaryContent()
    {
        // 현재 페이지에 해당하는 이미지를 diaryImage에 적용
        if (pageSprites != null && pageSprites.Length > currentPage)
        {
            diaryImage.sprite = pageSprites[currentPage];
        }

        // 현재 페이지에 해당하는 텍스트를 diaryText에 적용
        if (pageTexts != null && pageTexts.Length > currentPage)
        {
            diaryText.text = pageTexts[currentPage];
        }
    }

    private void PlayPageTurnSound()
    {
        if (pageTurnSound != null)
        {
            pageTurnSound.Play();          // 페이지 넘길 때 사운드 재생
        }
    }
}