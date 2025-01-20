using UnityEngine;

public class TechStudentController : MonoBehaviour
{
    public float speed = 5f;  // 기본 속도 5
    public AudioClip walkingSound;  // 걷는 소리 클립 (인스펙터에서 설정)
    public float walkingSoundInterval = 0.4f; // 걷는 소리 간격

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private AudioSource audioSource;  // 걷는 소리를 재생할 AudioSource
    private float walkingSoundTimer; // 걷는 소리 재생 간격을 위한 타이머

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Z축 회전을 고정하여 캐릭터가 넘어지지 않도록 처리
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // AudioSource 초기화
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = walkingSound;
        audioSource.loop = false; // 발소리는 반복 재생되지 않도록 설정
        audioSource.playOnAwake = false; // 자동 재생 비활성화
    }

    void Update()
    {
        // 이동 입력을 받음
        moveInput.x = Input.GetAxis("Horizontal");

        // 이동 속도 적용
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);

        // 걷는 소리 재생 간격 관리
        walkingSoundTimer += Time.deltaTime;

        // 왼쪽으로 이동할 때
        if (moveInput.x < 0)
        {
            animator.SetBool("isMovingLeft", true);
            animator.SetBool("isMovingRight", false);

            PlayWalkingSound();
        }
        // 오른쪽으로 이동할 때
        else if (moveInput.x > 0)
        {
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", true);

            PlayWalkingSound();
        }
        // 가만히 있을 때
        else
        {
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", false);

            // Idle 애니메이션 강제 전환
            animator.Play("Idle");

            // 걷는 소리 정지
            StopWalkingSound();
        }
    }

    /// <summary>
    /// 걷는 소리를 재생합니다.
    /// </summary>
    private void PlayWalkingSound()
    {
        if (!audioSource.isPlaying && walkingSoundTimer >= walkingSoundInterval)
        {
            audioSource.Play();
            walkingSoundTimer = 0f; // 타이머 초기화
        }
    }

    /// <summary>
    /// 걷는 소리를 정지합니다.
    /// </summary>
    private void StopWalkingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
