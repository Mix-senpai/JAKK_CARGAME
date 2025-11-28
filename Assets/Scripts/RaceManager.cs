using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour 
{
    public static RaceManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentLapTimeText;
    [SerializeField] private TextMeshProUGUI bestLapTimeText;
    [SerializeField] private TextMeshProUGUI overallRaceTimeText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI checkpointMissedText;
    [SerializeField] private TextMeshProUGUI levelTimerText;

    [Header("Race Settings")]
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private int lastCheckpointIndex = -1;
    [SerializeField] private bool isCircuit = false;
    [SerializeField] private int totalLaps = 1;

    private int currentLap = 0;
    private bool raceStarted = false;
    private bool raceFinished = false;
    private bool ifCheckpointMissed = false;

    [Header("Lap Timer")]
    private float currentLapTime = 0f;
    private float bestLapTime = Mathf.Infinity;
    private float overallRaceTime = 0f;

    [Header("Level Timer")]
    [SerializeField] private float levelTimeLimit = 60f; // how long player has
    private float levelTimer = 0f;
    private bool timerActive = false;

	public GameObject WinPanel;
	public GameObject LosePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (raceStarted) UpdateTimers();
        UpdateUI();
    }

    // -------------------------------------------------------------------
    // CHECKPOINT SYSTEM
    // -------------------------------------------------------------------
    public void CheckpointReached(int checkpointIndex)
    {
        if ((!raceStarted && checkpointIndex != 0) || raceFinished) return;

        if (checkpointIndex == lastCheckpointIndex + 1 || lastCheckpointIndex == checkpoints.Length - 1)
        {
            UpdateCheckpoint(checkpointIndex);
            HideCheckpointMissedText();
        }
        else
        {
            bool validLapFinish = isCircuit && raceStarted && lastCheckpointIndex == checkpoints.Length - 1 && checkpointIndex == 0;

            if (validLapFinish)
            {
                HideCheckpointMissedText();
                UpdateCheckpoint(checkpointIndex);
            }
            else ShowCheckpointMissedText();
        }
    }

    private void UpdateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == 0)
        {
            if (!raceStarted) StartRace();
            else if (isCircuit && lastCheckpointIndex == checkpoints.Length - 1) OnLapFinish();
        }
        else OnLapFinish();

        lastCheckpointIndex = checkpointIndex;
    }

    // -------------------------------------------------------------------
    // RACE LOGIC
    // -------------------------------------------------------------------
    private void OnLapFinish()
    {
        currentLap++;

        if (currentLapTime < bestLapTime) bestLapTime = currentLapTime;
        if (currentLap == totalLaps && timerActive)
        {
            EndRace();
			WinPanel.SetActive(true);
        }
    }

    private void StartRace()
    {
        raceStarted = true;
        raceFinished = false;

        // 🔥 START TIMER
        levelTimer = levelTimeLimit;
        timerActive = true;
    }

    private void EndRace()
    {
        raceStarted = false;
        raceFinished = true;
        timerActive = false;
        Debug.Log("Race Over!");
    }

    private void UpdateTimers()
    {
        currentLapTime += Time.deltaTime;
        overallRaceTime += Time.deltaTime;

        if (timerActive)
        {
            levelTimer -= Time.deltaTime;

            if (levelTimer <= 0)
            {
                levelTimer = 0;
                timerActive = false;
                EndRace(); // TIME UP
                Debug.Log("⛔ TIME'S UP — PLAYER FAILED");

				LosePanel.SetActive(true);
            }
        }
    }

    private void UpdateUI()
    {
        overallRaceTimeText.text = FormatTime(overallRaceTime);
        lapText.text = "Checkpoint: " + currentLap + "/" + totalLaps;

        // 🔥 SHOW LEVEL TIMER ON SCREEN
        if(levelTimerText != null)
            levelTimerText.text = FormatTime(levelTimer);

        UpdateCheckpointMissedText();
    }

    private void UpdateCheckpointMissedText()
    {
        if (ifCheckpointMissed)
        {
            float alpha = Mathf.PingPong(Time.time * 2, 1);
            Color newColor = checkpointMissedText.color;
            newColor.a = alpha;
            checkpointMissedText.color = newColor;
        }
    }

    private void ShowCheckpointMissedText()
    {
        if(!ifCheckpointMissed)
        {
            checkpointMissedText.gameObject.SetActive(true);
            ifCheckpointMissed = true;
        }
    }

    private void HideCheckpointMissedText()
    {
        if(ifCheckpointMissed)
        {
            checkpointMissedText.gameObject.SetActive(false);
            ifCheckpointMissed = false;
        }
    }

    // -------------------------------------------------------------------
    // FORMAT TIME DISPLAY
    // -------------------------------------------------------------------
    private string FormatTime(float time)
    {
        if (float.IsInfinity(time) || time < 0) return "--:--";
        int minutes = (int)time / 60;
        float seconds = time % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
