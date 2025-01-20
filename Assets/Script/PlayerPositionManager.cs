using System.Diagnostics;
using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    public Transform player; // 캐릭터의 Transform
    public Vector3 targetPosition; // 이동할 목표 위치

    private void Update()
    {
        // F 키를 눌렀을 때 캐릭터를 이동
        if (Input.GetKeyDown(KeyCode.F))
        {
            MovePlayerToTargetPosition();
        }
    }

    private void MovePlayerToTargetPosition()
    {
        // 캐릭터를 지정된 목표 위치로 이동
        player.position = targetPosition;

    }
}
