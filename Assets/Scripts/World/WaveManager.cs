 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


/// Attach to a WaveManager GameObject in your level scene.
/// Configure waves in the Inspector. When all waves are cleared,
/// calls LevelCompletion.CompleteLevel() automatically.

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        [Tooltip("Leave null to use defaultZombiePrefab.")]
        public GameObject zombiePrefab;
        [Tooltip("How many zombies to spawn. 0 = use baseZombieCount + waveIndex * zombieCountIncreasePerWave.")]
        public int zombieCount = 0;
        [Tooltip("Leave empty to use globalSpawnPoints with progressive unlock.")]
        public Transform[] spawnPoints;
        [Tooltip("Seconds between each zombie spawning in this wave.")]
        public float spawnInterval = 1f;
    }

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [Tooltip("Seconds to wait after one wave is cleared before the next begins.")]
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Scaling Defaults")]
    [Tooltip("Prefab used when a Wave's zombiePrefab is null.")]
    [SerializeField] private GameObject defaultZombiePrefab;
    [Tooltip("All spawn points in the level, ordered by distance from the NPC. Wave N unlocks the first N+1 points.")]
    [SerializeField] private Transform[] globalSpawnPoints;
    [Tooltip("Base zombie count when a Wave's zombieCount is 0.")]
    [SerializeField] private int baseZombieCount = 3;
    [Tooltip("Added to baseZombieCount for each subsequent wave.")]
    [SerializeField] private int zombieCountIncreasePerWave = 2;

    [Header("Chrono Shard Drop")]
    [Tooltip("The Chrono Shard ItemData to drop from the last zombie of each wave.")]
    [SerializeField] private ItemData chronoShardItem;

    [Header("UI")]
    [SerializeField] private GameObject waveAnnouncementPanel;
    [SerializeField] private TMP_Text waveAnnouncementText;
    [SerializeField] private GameObject zombiesRemainingPanel;
    [SerializeField] private TMP_Text zombiesRemainingText;

    private LevelCompletion levelCompletion;
    private HUD hud;
    private StoryNPC storyNPC;
    private int currentWave = 0;
    private List<GameObject> activeZombies = new List<GameObject>();
    private bool waveInProgress = false;
    private bool allWavesDone = false;
    private bool wavesStarted = false;
    private bool levelFailed = false;
    private bool shardDeliveredForWave = false;

    private void Start()
    {
        levelCompletion = FindFirstObjectByType<LevelCompletion>();
        hud = FindFirstObjectByType<HUD>();
        storyNPC = FindFirstObjectByType<StoryNPC>();

        if (zombiesRemainingPanel != null)
            zombiesRemainingPanel.SetActive(false);

        if (zombiesRemainingText != null)
            zombiesRemainingText.text = string.Empty;

        // If there is a StoryNPC in the scene, wait for them to finish talking.
        // Otherwise start waves immediately.
        if (storyNPC != null)
        {
            storyNPC.OnStoryFinished += StartWaves;
            storyNPC.OnChronoShardDelivered += HandleChronoShardDelivered;
        }
        else
        {
            hud?.SetDefaultPrompt("Defend the Elder and recover the next Chrono Shard.");
            StartWaves();
        }
    }

    public void StartWaves()
    {
        if (wavesStarted) return;
        wavesStarted = true;
        hud?.SetDefaultPrompt("Defend the Elder and recover the next Chrono Shard.");

        if (zombiesRemainingPanel != null)
            zombiesRemainingPanel.SetActive(true);

        StartCoroutine(RunWaves());
    }

    private void Update()
    {
        if (allWavesDone || levelFailed) return;

        // Clean up destroyed entries
        activeZombies.RemoveAll(z => z == null);

        // Always keep the counter visible and current once waves have started
        if (wavesStarted && zombiesRemainingText != null)
        {
            if (waveInProgress)
                zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Zombies Left: {activeZombies.Count}";

            // Wave cleared when all spawned zombies are dead
            if (waveInProgress && activeZombies.Count == 0)
                waveInProgress = false;
        }
    }

    // Spawn helpers 

    private Transform[] GetAllSpawnPoints()
    {
        if (globalSpawnPoints == null || globalSpawnPoints.Length == 0) return null;
        return globalSpawnPoints;
    }

    /// <summary>Halt waves immediately (called by LevelFailManager).</summary>
    public void StopWaves()
    {
        levelFailed = true;
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if (storyNPC != null)
        {
            storyNPC.OnStoryFinished -= StartWaves;
            storyNPC.OnChronoShardDelivered -= HandleChronoShardDelivered;
        }
    }

    private void HandleChronoShardDelivered(int waveNumber)
    {
        if (waveNumber == currentWave + 1)
            shardDeliveredForWave = true;
    }

    // Wave flow 

    private IEnumerator RunWaves()
    {
        for (currentWave = 0; currentWave < waves.Length; currentWave++)
        {
            if (levelFailed) yield break;

            Wave wave = waves[currentWave];

            // Resolve effective settings for this wave
            int effectiveCount = wave.zombieCount > 0
                ? wave.zombieCount
                : baseZombieCount + currentWave * zombieCountIncreasePerWave;

            GameObject prefab = wave.zombiePrefab != null ? wave.zombiePrefab : defaultZombiePrefab;

            Transform[] spawnPoints = (wave.spawnPoints != null && wave.spawnPoints.Length > 0)
                ? wave.spawnPoints
                : GetAllSpawnPoints();

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning($"[WaveManager] Wave '{wave.waveName}' has no spawn points. Assign globalSpawnPoints.");
                spawnPoints = new Transform[] { transform };
            }

            // Announce wave
            if (zombiesRemainingText != null)
                zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Zombies Left: 0";

            ShowAnnouncement($"{wave.waveName} ({currentWave + 1}/{waves.Length})");
            yield return new WaitForSeconds(2f);
            HideAnnouncement();

            if (levelFailed) yield break;

            // Spawn zombies
            waveInProgress = true;
            activeZombies.Clear();

            if (prefab == null)
            {
                Debug.LogWarning($"[WaveManager] Wave '{wave.waveName}' has no zombie prefab. Assign defaultZombiePrefab.");
            }
            else
            {
                GameObject lastZombieSpawned = null;
                for (int i = 0; i < effectiveCount; i++)
                {
                    if (levelFailed) yield break;

                    Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
                    GameObject zombie = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                    lastZombieSpawned = zombie;

                    activeZombies.Add(zombie);

                    yield return new WaitForSeconds(wave.spawnInterval);
                }

                // Mark the last zombie to drop the Chrono Shard on death
                if (lastZombieSpawned != null && chronoShardItem != null)
                {
                    ChronoShardCarrier carrier = lastZombieSpawned.AddComponent<ChronoShardCarrier>();
                    carrier.Configure(chronoShardItem);
                }
            }

            // Wait until all zombies from this wave are dead
            yield return new WaitUntil(() => !waveInProgress || levelFailed);

            if (levelFailed) yield break;

            bool isLastWave = currentWave >= waves.Length - 1;
            if (storyNPC != null)
            {
                shardDeliveredForWave = false;
                storyNPC.PrepareChronoShardTurnIn(currentWave + 1, isLastWave);

                if (zombiesRemainingText != null)
                    zombiesRemainingText.text = isLastWave
                        ? "Final wave cleared  |  Return the final shard to the Elder"
                        : $"Wave {currentWave + 1}/{waves.Length}  |  Return the recovered shard to the Elder";

                yield return new WaitUntil(() => shardDeliveredForWave || levelFailed);

                if (levelFailed) yield break;
            }

            if (currentWave < waves.Length - 1)
            {
                ShowAnnouncement("Wave Cleared!");

                if (zombiesRemainingText != null)
                    zombiesRemainingText.text = $"Wave {currentWave + 1}/{waves.Length}  |  Cleared";

                yield return new WaitForSeconds(timeBetweenWaves);
                HideAnnouncement();

                if (levelFailed) yield break;
            }
        }

        allWavesDone = true;
        hud?.SetDefaultPrompt("All waves cleared. Speak with the Elder.");

        if (zombiesRemainingText != null)
            zombiesRemainingText.text = "All Waves Cleared!";
        if (waveAnnouncementText != null)
            waveAnnouncementText.text = string.Empty;

        if (levelCompletion != null && !levelCompletion.IsComplete)
            levelCompletion.CompleteLevel();
    }

    private void ShowAnnouncement(string message)
    {
        if (waveAnnouncementText != null)
            waveAnnouncementText.text = message;
        Debug.Log($"[WaveManager] {message}");
    }

    private void HideAnnouncement()
    {
        if (waveAnnouncementText != null)
            waveAnnouncementText.text = string.Empty;
    }
}
