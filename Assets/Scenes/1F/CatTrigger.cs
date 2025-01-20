using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용

public class CatTrigger : MonoBehaviour
{
    public GameObject catObject;          // 고양이 오브젝트
    public Transform catFollowTarget;     // 카메라가 따라갈 고양이 위치
    public Transform playerFollowTarget;  // 카메라가 따라갈 플레이어 위치
    public GameObject dialogueUI;         // 대사 UI 오브젝트
    public TextMeshProUGUI dialogueText;  // 대사 텍스트
    public Image characterIllustration;   // 캐릭터 이미지
    public AudioSource catSound;          // 고양이 오디오 소스
    [TextArea(1, 10)]
    public string[] dialogues;            // 대사 내용 배열
    public float hideDelay = 5.0f;        // 고양이 오브젝트가 사라질 때까지의 지연 시간
    public Animator catAnimator;          // 고양이 애니메이터 컴포넌트

    private Camera mainCamera;            // 메인 카메라
    private bool isFollowingCat = false;  // 카메라가 고양이를 따라가고 있는지 여부
    private TechStudentController playerControllerScript; // 플레이어 이동 스크립트 참조
    private Rigidbody2D playerRigidbody;  // 플레이어 Rigidbody2D
    private Animator playerAnimator;      // 플레이어 애니메이터
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    private bool isDialogueActive = false; // 대화 진행 여부

    private void Start()
    {
        mainCamera = Camera.main;

        // 대사 UI와 캐릭터 이미지 초기 비활성화
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // 플레이어 관련 컴포넌트 참조
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerControllerScript = player.GetComponent<TechStudentController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.instance.is_1F_Locked == 1 && GameManager.instance.PlayCat_1F == 0)
        {
            StartCoroutine(PlayCatAnimation());
        }
    }

    private IEnumerator PlayCatAnimation()
    {
        DisablePlayerMovement(); // 고양이 이동 시작 시 플레이어 움직임 비활성화

        catObject.SetActive(true); // 고양이 오브젝트 활성화

        if (catAnimator != null)
        {
            catAnimator.SetTrigger("Appear"); // 고양이 애니메이션 시작
        }

        if (catSound != null)
        {
            catSound.Play(); // 오디오 재생
        }

        isFollowingCat = true; // 카메라가 고양이를 따라가도록 설정

        yield return new WaitForSeconds(hideDelay); // 고양이 대기 시간

        isFollowingCat = false; // 고양이 팔로우 중지
        catObject.SetActive(false); // 고양이 오브젝트 비활성화

        // 플레이어 위치로 정확히 이동
        Vector3 playerPosition = new Vector3(
            playerFollowTarget.position.x,
            playerFollowTarget.position.y,
            mainCamera.transform.position.z
        );

        // 카메라를 플레이어 위치로 부드럽게 이동
        yield return StartCoroutine(SmoothCameraTransition(playerPosition));

        StartDialogue(); // 대화 시작
    }


    // 카메라를 부드럽게 이동시키는 코루틴
    private IEnumerator SmoothCameraTransition(Vector3 targetPosition)
    {
        float transitionDuration = 1.0f; // 카메라 이동 시간
        Vector3 startPosition = mainCamera.transform.position;

        // 목표 위치에서 y 값을 7.99로 고정
        Vector3 targetCameraPosition = new Vector3(
            targetPosition.x,
            7.99f, // 고정된 y 값
            startPosition.z // z 값 유지
        );

        // CameraFollow 비활성화
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // 부드럽게 이동
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetCameraPosition, elapsedTime / transitionDuration);
            yield return null;
        }

        // 최종 위치로 설정
        mainCamera.transform.position = targetCameraPosition;

        // CameraFollow 활성화
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }
    }



    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // 대사 UI 활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // 캐릭터 이미지 활성화
        }

        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex]; // 첫 번째 대사 출력
        isDialogueActive = true;
    }

    private void Update()
    {
        // 대화 중 Space 키로 다음 대사 진행
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextDialogue();
        }
    }

    private void ShowNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            // 다음 대사 출력
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // 대화 종료
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // 대사 UI 비활성화
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // 캐릭터 이미지 비활성화
        }

        EnablePlayerMovement(); // 대화 종료 후 플레이어 움직임 활성화

        isDialogueActive = false;

        GameManager.instance.PlayCat_1F = 1; // 상태 업데이트
    }

    private void DisablePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // 이동 속도 제거
            playerRigidbody.angularVelocity = 0f;   // 회전 속도 제거
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle 상태로 전환
        }

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false; // 이동 스크립트 비활성화
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // 이동 스크립트 활성화
        }
    }

    private void LateUpdate()
    {
        if (isFollowingCat && catFollowTarget != null)
        {
            // 카메라가 고양이를 따라가면서 y값을 7.99로 고정
            mainCamera.transform.position = new Vector3(
                catFollowTarget.position.x,
                7.99f,
                mainCamera.transform.position.z
            );
        }
    }
}