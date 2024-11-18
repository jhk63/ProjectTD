using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float startTime;
    private bool isGameCleared = false;

    [SerializeField] private TMP_Text timerText;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (!isGameCleared)
        {
            float curretTime = Time.time - startTime;

            if (timerText != null)
            {
                timerText.text = string.Format("클리어 시간: {0}초", Mathf.FloorToInt(curretTime));
            }
        }
    }

    public void OnPause()
    {
        // isPause = true;
        Time.timeScale = 0f;
    }

    public void OnResume()
    {
        // isPause = false;
        Time.timeScale = 1.0f;
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(0);
    }

    public void OnEndBGM()
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();

        if (audioManager != null)
        {
            audioManager.OnEndBGM();
        }
    }
}
