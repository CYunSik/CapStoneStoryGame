using System.Diagnostics;
using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    public Transform player; // ĳ������ Transform
    public Vector3 targetPosition; // �̵��� ��ǥ ��ġ

    private void Update()
    {
        // F Ű�� ������ �� ĳ���͸� �̵�
        if (Input.GetKeyDown(KeyCode.F))
        {
            MovePlayerToTargetPosition();
        }
    }

    private void MovePlayerToTargetPosition()
    {
        // ĳ���͸� ������ ��ǥ ��ġ�� �̵�
        player.position = targetPosition;

    }
}
