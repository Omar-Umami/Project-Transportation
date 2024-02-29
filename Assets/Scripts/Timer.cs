using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    private float timeLeft = 30f;
    private bool timerRunning = false;

    public static event Action timerEnd;
    // ReSharper disable Unity.PerformanceAnalysis
    public void StartTimer(float value)
    {
        timeLeft = value;
        timerRunning = true;
        CancelInvoke(nameof(UpdateTimer));
        InvokeRepeating(nameof(UpdateTimer), 0f, 1f);
    }
    private void UpdateTimer()
    {
        if (!timerRunning) return;
        timeLeft -= 1f; 

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            timerRunning = false;
            Debug.Log("Timer has ended!");
            CancelInvoke(nameof(UpdateTimer));
            timerEnd?.Invoke();
        }

        UpdateTimerDisplay();
    }
    

    private void UpdateTimerDisplay()
    {
        var seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = seconds.ToString("D2");
    }
}