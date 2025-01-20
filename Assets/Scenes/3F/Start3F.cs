using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class Start3F : MonoBehaviour
{
    public Image characterIllustration; // 캐릭터 이미지
    public TextMeshProUGUI monologueText; // TextMeshPro 대화 텍스트
    public GameObject dialoguePanel; // 대화창 Panel

    [TextArea(1, 10)] // 최소 1줄, 최대 10줄로 설정
    public string[] dialogues; // 출력할 대화 목록
    private int dialogueIndex = 0; // 현재 대화 인덱스
    private bool isDialogueActive = false; // 대화 활성화 여부

    private TechStudentController playerMovement; // 캐릭터 이동 스크립트 참조
    private Rigidbody2D playerRigidbody; // 캐릭터 Rigidbody2D 참조
    private Animator playerAnimator; // 캐릭터 애니메이터 참조

    private void Start()
    {
        // 시작 시 대화창 비활성화
        dialoguePanel.SetActive(false);
        characterIllustration.gameObject.SetActive(false);
        monologueText.gameObject.SetActive(false);

        // 캐릭터 이동 스크립트, Rigidbody2D, Animator 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.instance.is_3F_Start == 0)
        {
            GameManager.instance.is_3F_Start = 1;
            StartDialogue();
        }
    }

    private void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            ShowNextDialogue();
        }
    }

    private void StartDialogue()
    {
        // 대화창과 캐릭터 이미지 활성화
        dialoguePanel.SetActive(true);
        characterIllustration.gameObject.SetActive(true);
        monologueText.gameObject.SetActive(true);

        // 캐릭터 이동 즉시 멈춤
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // 이동 속도 제거
            playerRigidbody.angularVelocity = 0f;   // 회전 속도 제거
        }

        // 걷는 모션 중지 및 Idle 애니메이션 강제 설정
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle 상태로 전환
        }

        // 캐릭터 이동 스크립트 비활성화
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 첫 번째 대화 출력
        dialogueIndex = 0;
        monologueText.text = dialogues[dialogueIndex];
        isDialogueActive = true;
    }

    private void ShowNextDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex < dialogues.Length)
        {
            // 다음 대화 출력
            monologueText.text = dialogues[dialogueIndex];
        }
        else
        {
            // 대화 종료
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // 대화창과 이미지 비활성화
        dialoguePanel.SetActive(false);
        characterIllustration.gameObject.SetActive(false);
        monologueText.gameObject.SetActive(false);

        // 캐릭터 이동 스크립트 활성화
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isDialogueActive = false;
    }
}
