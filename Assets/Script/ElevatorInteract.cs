using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteract : MonoBehaviour
{
    public GameObject elevatorUI; // "��� ����" UI ������Ʈ
    public AudioSource warningSound; // ����� ����� �ҽ�
    private bool isPlayerInRange = false; // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
    private bool uiActive = false; // UI�� Ȱ��ȭ�Ǿ����� Ȯ��

    private void Start()
    {
        if (elevatorUI != null)
        {
            elevatorUI.SetActive(false); // ���� �� UI ��Ȱ��ȭ
        }

        if (warningSound != null)
        {
            warningSound.Stop(); // ���� �� ����� ����
        }
    }

    private void Update()
    {
        // ���� �ȿ� ���� ���� "F" Ű �Է��� üũ
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // GameManager�� has_Interacted_Elevator ���� 1�� ����
            GameManager.instance.has_Interacted_Elevator = 1;

            // UI ���� ��ȯ �� ����� ���
            ToggleElevatorUI();
        }

        // �÷��̾ ���� ������ ������ �� UI ��Ȱ��ȭ
        if (!isPlayerInRange && uiActive)
        {
            elevatorUI.SetActive(false);
            uiActive = false;
        }
    }

    private void ToggleElevatorUI()
    {
        if (elevatorUI != null)
        {
            uiActive = !uiActive;
            elevatorUI.SetActive(uiActive); // UI ���� ��ȯ

            // UI�� Ȱ��ȭ�� ���� ����� ���
            if (uiActive && warningSound != null)
            {
                warningSound.Play();
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
