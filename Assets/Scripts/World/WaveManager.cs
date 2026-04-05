using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Attach to a WaveManager GameObject in your level scene.
/// Configure waves in the Inspector. When all waves are cleared,
/// calls LevelCompletion.CompleteLevel() automatically.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject zombiePrefab;
        public int zombieCount = 5;
        public Transform[] spawnPoints;
        [Tooltip("Seconds between each zombie spawning in this wave.")]
        public float spawnInterval = 1f;
    }

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [Tooltip("Seconds to wait after one wave is cleared before the next begins.")]
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("UI")]
    [SerializeField] private GameObject waveAnnouncementPanel;
    [SerializeField] private TMP_Text waveAnnouncementText;
    [SerializeField] private GameObject zombiesRemainingPanel;
    [SerializeField] private TMP_Text zombiesRemainingText;

    private LevelCompletion levelCompletion;
    private HUD hud;
    private int currentWave = 0;
    private List<GameObject> activeZombies = new List<GameObject>();
    private bool waveInProgress = false;
    private bool allWavesDone = false;
    private bool wavesStarted = false;

    private void Start()
    {
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
        hud = FindFirstObjectByType<HUD>();

        if (waveAnnouncementPanel == null && waveAnnouncementText != null && waveAnnouncementText.transform.parent != null)
            waveAnnouncementPanel = waveAnnouncementText.transform.parent.gameObject;

        if (zombiesRemainingPanel == null && zombiesRemainingText != null && zombiesRemainingText.transform.parent != null)
            zombiesRemainingPanel = zombiesRemainingText.transform.parent.gameObject;

        if (waveAnnouncementPanel != null)
            waveAnnouncementPanel.SetActive(false);
        else if (waveAnnouncementText != null)
            waveAnnouncementText.gameObject.SetActive(false);

        if (zombiesRemainingText != null)
        {
            zombiesRemainingText.text = string.Empty;

            if (zombiesRemainingPanel != null)
                zombiesRemainingPanel.SetActive(false);
            else
                zombiesRemainingText.gameObject.SetActive(false);
        }

        // If there is a StoryNPC in the scene, wait for them to finish talking.
        // Otherwise start waves immediately.
        StoryNPC storyNPC = FindFirstObjectByType<StoryNPC>();
        if (storyNPC != null)
            storyNPC.OnStoryFinished += StartWaves;
        else
        {
            hud?.SetDefaultPrompt("Protect the Elder");
            StartWaves();
        }
    }

    public void StartWaves()
    {
        if (wavesStarted) return;
        wavesStarted = true;
        hud?.SetDefaultPrompt("Protect the Elder");

        if (zombiesRemainingPanel != null)
            zombiesRemainingPanel.SetActive(true);
        else if (zombiesRemainingText != null)
            zombiesRemainingText.gameObject.SetActive(true);

        StartCoroutine(RunWaves());
    }

    private void Update()
    {
        if (allWavesDone || !waveInProgress) return;

        // Clean up destroyed entries
        activeZombies.RemoveAll(z => z == null);

        if (zombiesRemainingText != null)
            zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Zombies Left: {activeZombies.Count}";

        // Wave cleared when all spawned zombies are dead
        if (activeZombies.Count == 0)
        {
            waveInProgress = false;
        }
    }

    private IEnumerator RunWaves()
    {
        for (currentWave = 0; currentWave < waves.Length; currentWave++)
        {
            Wave wave = waves[currentWave];

            // Announce wave
            if (zombiesRemainingText != null)
                zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Zombies Left: 0";

            ShowAnnouncement($"{wave.waveName} ({currentWave + 1}/{waves.Length})");
            yield return new WaitForSeconds(2f);
            HideAnnouncement();

            // Spawn zombies
            waveInProgress = true;
            activeZombies.Clear();

            for (int i = 0; i < wave.zombieCount; i++)
            {
                if (wave.spawnPoints == null || wave.spawnPoints.Length == 0)
                {
                    Debug.LogWarning($"[WaveManager] Wave '{wave.waveName}' has no spawn points assigned.");
                    break;
                }

                Transform spawnPoint = wave.spawnPoints[i % wave.spawnPoints.Length];
                GameObject zombie = Instantiate(wave.zombiePrefab, spawnPoint.position, spawnPoint.rotation);
                activeZombies.Add(zombie);

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            // Wait until all zombies from this wave are dead
            yield return new WaitUntil(() => !waveInProgress);

            if (currentWave < waves.Length - 1)
            {
                ShowAnnouncement("Wave Cleared!");

                if (zombiesRemainingText != null)
                    zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Cleared";

                yield return new WaitForSeconds(timeBetweenWaves);
                HideAnnouncement();
            }
        }

        allWavesDone = true;
        hud?.SetDefaultPrompt("All waves cleared. Return to the boat.");

        if (zombiesRemainingPanel != null)
            zombiesRemainingPanel.SetActive(false);
        else if (zombiesRemainingText != null)
            zombiesRemainingText.gameObject.SetActive(false);

        ShowAnnouncement("All Waves Cleared!");
        yield return new WaitForSeconds(2f);
        HideAnnouncement();

        levelCompletion?.CompleteLevel();
    }

    private void ShowAnnouncement(string message)
    {
        if (waveAnnouncementText != null)
        {
            if (waveAnnouncementPanel != null)
                waveAnnouncementPanel.SetActive(true);
            else
                waveAnnouncementText.gameObject.SetActive(true);

            waveAnnouncementText.text = message;
        }
        Debug.Log($"[WaveManager] {message}");
    }

    private void HideAnnouncement()
    {
        if (waveAnnouncementPanel != null)
            waveAnnouncementPanel.SetActive(false);
        else if (waveAnnouncementText != null)
            waveAnnouncementText.gameObject.SetActive(false);
    }
}
