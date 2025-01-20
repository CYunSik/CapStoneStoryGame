using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public string targetSceneName; // 이동할 씬 이름
    public Vector3 startPositionInTargetScene; // 이동한 씬에서의 시작 위치
    private bool isTransitioning = false; // 씬 이동 중복 방지용 변수

    private void Update()
    {
        // F 키를 누르고 이동 중이 아닐 때 씬 이동
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning)
        {
            TransitionToTargetScene();
        }
    }

    private void TransitionToTargetScene()
    {
        // 이동 중으로 표시하여 중복 이동 방지
        isTransitioning = true;

        // 이동할 씬의 시작 위치를 저장
        PlayerPrefs.SetFloat("StartX", startPositionInTargetScene.x);
        PlayerPrefs.SetFloat("StartY", startPositionInTargetScene.y);
        PlayerPrefs.SetFloat("StartZ", startPositionInTargetScene.z);

        // 지정된 씬으로 이동
        SceneManager.LoadScene(targetSceneName);
    }
}
