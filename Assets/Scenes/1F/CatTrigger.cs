using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro ���

public class CatTrigger : MonoBehaviour
{
    public GameObject catObject;          // ����� ������Ʈ
    public Transform catFollowTarget;     // ī�޶� ���� ����� ��ġ
    public Transform playerFollowTarget;  // ī�޶� ���� �÷��̾� ��ġ
    public GameObject dialogueUI;         // ��� UI ������Ʈ
    public TextMeshProUGUI dialogueText;  // ��� �ؽ�Ʈ
    public Image characterIllustration;   // ĳ���� �̹���
    public AudioSource catSound;          // ����� ����� �ҽ�
    [TextArea(1, 10)]
    public string[] dialogues;            // ��� ���� �迭
    public float hideDelay = 5.0f;        // ����� ������Ʈ�� ����� �������� ���� �ð�
    public Animator catAnimator;          // ����� �ִϸ����� ������Ʈ

    private Camera mainCamera;            // ���� ī�޶�
    private bool isFollowingCat = false;  // ī�޶� ����̸� ���󰡰� �ִ��� ����
    private TechStudentController playerControllerScript; // �÷��̾� �̵� ��ũ��Ʈ ����
    private Rigidbody2D playerRigidbody;  // �÷��̾� Rigidbody2D
    private Animator playerAnimator;      // �÷��̾� �ִϸ�����
    private int currentDialogueIndex = 0; // ���� ��ȭ �ε���
    private bool isDialogueActive = false; // ��ȭ ���� ����

    private void Start()
    {
        mainCamera = Camera.main;

        // ��� UI�� ĳ���� �̹��� �ʱ� ��Ȱ��ȭ
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false);
        }

        // �÷��̾� ���� ������Ʈ ����
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
        DisablePlayerMovement(); // ����� �̵� ���� �� �÷��̾� ������ ��Ȱ��ȭ

        catObject.SetActive(true); // ����� ������Ʈ Ȱ��ȭ

        if (catAnimator != null)
        {
            catAnimator.SetTrigger("Appear"); // ����� �ִϸ��̼� ����
        }

        if (catSound != null)
        {
            catSound.Play(); // ����� ���
        }

        isFollowingCat = true; // ī�޶� ����̸� ���󰡵��� ����

        yield return new WaitForSeconds(hideDelay); // ����� ��� �ð�

        isFollowingCat = false; // ����� �ȷο� ����
        catObject.SetActive(false); // ����� ������Ʈ ��Ȱ��ȭ

        // �÷��̾� ��ġ�� ��Ȯ�� �̵�
        Vector3 playerPosition = new Vector3(
            playerFollowTarget.position.x,
            playerFollowTarget.position.y,
            mainCamera.transform.position.z
        );

        // ī�޶� �÷��̾� ��ġ�� �ε巴�� �̵�
        yield return StartCoroutine(SmoothCameraTransition(playerPosition));

        StartDialogue(); // ��ȭ ����
    }


    // ī�޶� �ε巴�� �̵���Ű�� �ڷ�ƾ
    private IEnumerator SmoothCameraTransition(Vector3 targetPosition)
    {
        float transitionDuration = 1.0f; // ī�޶� �̵� �ð�
        Vector3 startPosition = mainCamera.transform.position;

        // ��ǥ ��ġ���� y ���� 7.99�� ����
        Vector3 targetCameraPosition = new Vector3(
            targetPosition.x,
            7.99f, // ������ y ��
            startPosition.z // z �� ����
        );

        // CameraFollow ��Ȱ��ȭ
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // �ε巴�� �̵�
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetCameraPosition, elapsedTime / transitionDuration);
            yield return null;
        }

        // ���� ��ġ�� ����
        mainCamera.transform.position = targetCameraPosition;

        // CameraFollow Ȱ��ȭ
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }
    }



    private void StartDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // ��� UI Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(true); // ĳ���� �̹��� Ȱ��ȭ
        }

        currentDialogueIndex = 0;
        dialogueText.text = dialogues[currentDialogueIndex]; // ù ��° ��� ���
        isDialogueActive = true;
    }

    private void Update()
    {
        // ��ȭ �� Space Ű�� ���� ��� ����
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
            // ���� ��� ���
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            EndDialogue(); // ��ȭ ����
        }
    }

    private void EndDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // ��� UI ��Ȱ��ȭ
        }

        if (characterIllustration != null)
        {
            characterIllustration.gameObject.SetActive(false); // ĳ���� �̹��� ��Ȱ��ȭ
        }

        EnablePlayerMovement(); // ��ȭ ���� �� �÷��̾� ������ Ȱ��ȭ

        isDialogueActive = false;

        GameManager.instance.PlayCat_1F = 1; // ���� ������Ʈ
    }

    private void DisablePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // �̵� �ӵ� ����
            playerRigidbody.angularVelocity = 0f;   // ȸ�� �ӵ� ����
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isMovingLeft", false);
            playerAnimator.SetBool("isMovingRight", false);
            playerAnimator.Play("Idle"); // Idle ���·� ��ȯ
        }

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false; // �̵� ��ũ��Ʈ ��Ȱ��ȭ
        }
    }

    private void EnablePlayerMovement()
    {
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true; // �̵� ��ũ��Ʈ Ȱ��ȭ
        }
    }

    private void LateUpdate()
    {
        if (isFollowingCat && catFollowTarget != null)
        {
            // ī�޶� ����̸� ���󰡸鼭 y���� 7.99�� ����
            mainCamera.transform.position = new Vector3(
                catFollowTarget.position.x,
                7.99f,
                mainCamera.transform.position.z
            );
        }
    }
}