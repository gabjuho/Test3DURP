using UnityEngine;
using UnityEngine.UI;

public class TurningEndButton : MonoBehaviour
{
    private Button turningEndButton;

    private void Awake()
    {
        turningEndButton = GetComponent<Button>();
    }

    //TurningEndButton 클릭 시 해당 버튼이 비활성화되는 함수
    public void OnClickTurningEndButton()
    {
        turningEndButton.interactable = false;
    }
}
