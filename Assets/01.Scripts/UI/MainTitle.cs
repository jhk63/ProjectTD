using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTitle : MonoBehaviour
{
    [SerializeField] private GameObject TitlePanel;
    [SerializeField] private GameObject OptionPanel;

    [SerializeField] private TitleMove1 mover;

    private void Start()
    {
        TitlePanel.SetActive(true);
        OptionPanel.SetActive(false);
    }

    public void OnClick_Start()
    {
        TitlePanel.SetActive(false);
        mover.isStart = true;
    }

    public void OnClick_Option()
    {
        OptionPanel.SetActive(true);
    }

    public void OnClick_Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
