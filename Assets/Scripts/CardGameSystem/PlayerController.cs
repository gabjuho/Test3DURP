using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject gameSettingPanel;

    [SerializeField]
    private GameObject combiningCardPanel;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (combiningCardPanel.activeSelf == true)
            {
                CombiningCardManager.Inst.ExitCombiningCardMode();
                return;
            }
            
            if (gameSettingPanel.activeSelf == true)
            {
                gameSettingPanel.SetActive(false);
            }
            else
            {
                gameSettingPanel.SetActive(true);
            }

        }
    }
}
