using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDiary : MonoBehaviour
{
    public GameObject BDiaryUI; // "게시판 신문" UI 오브젝트
    public AudioSource noteboardsound; // 게시판 여는 오디오 소스
    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool uiActive = false; // UI가 활성화되었는지 확인

    private void Start()
    {
        if (BDiaryUI != null)
        {
            BDiaryUI.SetActive(false); // 시작 시 UI 비활성화
        }

        if (noteboardsound != null)
        {
            noteboardsound.Stop(); // 시작 시 경고음 멈춤
        }
    }

    private void Update()
    {
        // 범위 안에 있을 때만 "F" 키 입력을 체크
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.instance.has_news_paper = 1;

            // UI 상태 전환 및 경고음 재생
            ToggleNoticeboardUI();
        }

        // 플레이어가 범위 밖으로 나갔을 때 UI 비활성화
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
            BDiaryUI.SetActive(uiActive); // UI 상태 전환

            // UI가 활성화될 때만 사운드 재생
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
            isPlayerInRange = true; // 범위 안에 들어왔음을 표시
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 범위 밖으로 나갔음을 표시
        }
    }
}
