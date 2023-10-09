using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingUI : MonoBehaviour
{
    public void OnClickResumeButton()
    {
        gameObject.SetActive(false);
    }

    public void OnClickRetryButton()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickExitButton()
    {
        DOTween.Clear(true);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(); // 어플리케이션 종료
#endif
    }
}
