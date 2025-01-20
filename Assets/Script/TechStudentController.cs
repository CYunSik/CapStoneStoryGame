using UnityEngine;

public class TechStudentController : MonoBehaviour
{
    public float speed = 5f;  // �⺻ �ӵ� 5
    public AudioClip walkingSound;  // �ȴ� �Ҹ� Ŭ�� (�ν����Ϳ��� ����)
    public float walkingSoundInterval = 0.4f; // �ȴ� �Ҹ� ����

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private AudioSource audioSource;  // �ȴ� �Ҹ��� ����� AudioSource
    private float walkingSoundTimer; // �ȴ� �Ҹ� ��� ������ ���� Ÿ�̸�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Z�� ȸ���� �����Ͽ� ĳ���Ͱ� �Ѿ����� �ʵ��� ó��
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // AudioSource �ʱ�ȭ
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = walkingSound;
        audioSource.loop = false; // �߼Ҹ��� �ݺ� ������� �ʵ��� ����
        audioSource.playOnAwake = false; // �ڵ� ��� ��Ȱ��ȭ
    }

    void Update()
    {
        // �̵� �Է��� ����
        moveInput.x = Input.GetAxis("Horizontal");

        // �̵� �ӵ� ����
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);

        // �ȴ� �Ҹ� ��� ���� ����
        walkingSoundTimer += Time.deltaTime;

        // �������� �̵��� ��
        if (moveInput.x < 0)
        {
            animator.SetBool("isMovingLeft", true);
            animator.SetBool("isMovingRight", false);

            PlayWalkingSound();
        }
        // ���������� �̵��� ��
        else if (moveInput.x > 0)
        {
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", true);

            PlayWalkingSound();
        }
        // ������ ���� ��
        else
        {
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", false);

            // Idle �ִϸ��̼� ���� ��ȯ
            animator.Play("Idle");

            // �ȴ� �Ҹ� ����
            StopWalkingSound();
        }
    }

    /// <summary>
    /// �ȴ� �Ҹ��� ����մϴ�.
    /// </summary>
    private void PlayWalkingSound()
    {
        if (!audioSource.isPlaying && walkingSoundTimer >= walkingSoundInterval)
        {
            audioSource.Play();
            walkingSoundTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }

    /// <summary>
    /// �ȴ� �Ҹ��� �����մϴ�.
    /// </summary>
    private void StopWalkingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
