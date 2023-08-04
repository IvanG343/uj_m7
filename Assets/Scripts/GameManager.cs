using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public ImageTimer harvestTimer;
    public ImageTimer eatingTimer;

    public Image villagerTimerIcon;
    public Image warriorTimerIcon;
    public Image raidTimerIcon;
    public Image GameOverLayerColor;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    public Text raidTimerText;
    public Text nextRaidText;
    public Text gameOverTitleText;
    public Text gameOverText;
    public Text roundCounterText;
    public Text wheatIncomeText;
    public Text wheatOutcomeText;

    public Text totalWavesSurvived;
    public Text totalWheatText;
    public Text totalWarriorsText;
    public Text totalEnemiesText;

    private bool audioMute;

    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    public Button villagerButton;
    public Button warriorButton;
    public Button muteSoundButton;

    public Text villagerCountText;
    public Text warriorCountText;
    public Text wheatCountText;

    public int villagersCount;
    public int warriorCount;
    public int wheatCount;

    public int wheatPerVillager;
    public int wheatPerWarrior;
    public int villagerCost;
    public int warriorCost;

    public float villagerCreateTime;
    public float warriorCreateTime;
    public float raidMaxTime;

    public int nextRaid;
    public int raidIncrease;
    public int raidWaves;
    public int raidsForWin;
    private bool isZeroRound;

    private float villagerTimer = -2;
    private float warriorTimer = -2;
    private float raidTimer;

    public int totalWheat;
    public int totalWarriors;
    public int totalEnemiesDefeated;


    void Start() {

        UpdateText();
        isZeroRound = true;
        raidTimer = raidMaxTime *3;
        raidWaves = 0;
        Time.timeScale = 1;

    }

    void Update() {

        if(isZeroRound) {
            raidTimer -= Time.deltaTime;
            raidTimerIcon.fillAmount = raidTimer / (raidMaxTime * 3);
            raidTimerText.text = Mathf.Round(raidTimer).ToString();
            if (raidTimer <= 0) {
                raidTimer = raidMaxTime;
                raidWaves++;
                isZeroRound = false;
                nextRaid += raidIncrease;
            }
        }

        if(raidWaves >= 1) {
            raidTimer -= Time.deltaTime;
            raidTimerIcon.fillAmount = raidTimer / raidMaxTime;
            raidTimerText.text = Mathf.Round(raidTimer).ToString();
            if (raidTimer <= 0) {
                raidTimer = raidMaxTime;
                warriorCount -= nextRaid;
                totalEnemiesDefeated += nextRaid;
                if (raidWaves < 7) {
                    if (raidWaves % 2 == 0) {
                        nextRaid += raidIncrease;
                    }
                } else {
                    nextRaid += raidIncrease * 2;
                }
                raidTimerIcon.GetComponent<AudioSource>().Play();
                raidWaves++;
            }
        }

        if (harvestTimer.tick) {
            wheatCount += villagersCount * wheatPerVillager;
            totalWheat += villagersCount * wheatPerVillager;
            harvestTimer.timerImg.GetComponent<AudioSource>().Play();
        }

        if (eatingTimer.tick) {
            wheatCount -= warriorCount * wheatPerWarrior;
            eatingTimer.timerImg.GetComponent<AudioSource>().Play();
        }

        if (wheatCount >= villagerCost && villagerTimer == -2) {
            villagerButton.interactable = true;
        } else {
            villagerButton.interactable = false;
        }

        if (wheatCount >= warriorCost && warriorTimer == -2) {
            warriorButton.interactable = true;
        } else {
            warriorButton.interactable = false;
        }

        if (villagerTimer > 0) {
            villagerTimer -= Time.deltaTime;
            villagerTimerIcon.fillAmount = 1 - (villagerTimer / villagerCreateTime);
        } else if (villagerTimer > -1) {
            villagerTimerIcon.fillAmount = 1;
            villagerButton.interactable = true;
            villagersCount++;
            villagerTimerIcon.GetComponent<AudioSource>().Play();
            villagerTimer = -2;
        }

        if (warriorTimer > 0) {
            warriorTimer -= Time.deltaTime;
            warriorTimerIcon.fillAmount = 1 - (warriorTimer / warriorCreateTime);
        } else if (warriorTimer > -1) {
            warriorTimerIcon.fillAmount = 1;
            warriorButton.interactable = true;
            warriorCount++;
            totalWarriors++;
            warriorTimerIcon.GetComponent<AudioSource>().Play();
            warriorTimer = -2;
        }

        if (warriorCount < 0) {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
            GameOverLayerColor.color = new Color32(142, 44, 44, 168);
            gameOverTitleText.text = "Defeat";
            gameOverText.text = "The village has been destroyed by barbarians";
            UpdateStats();
        }

        if (wheatCount < 0) {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
            GameOverLayerColor.color = new Color32(142, 44, 44, 168);
            gameOverTitleText.text = "Defeat";
            gameOverText.text = "The warriors abandoned your village and the villagers starved to death.";
            UpdateStats();
        }

        if (raidWaves == raidsForWin + 1 && warriorCount > -1) {
            Time.timeScale = 0;
            gameOverScreen.SetActive(true);
            GameOverLayerColor.color = new Color32(99, 207, 42, 168);
            gameOverTitleText.text = "Victory";
            gameOverText.text = "You have saved the village";
            UpdateStats();
        }

        UpdateText();
    }

    public void UpdateText() {
        villagerCountText.text = villagersCount.ToString();
        warriorCountText.text = warriorCount.ToString();
        wheatCountText.text = wheatCount.ToString();
        nextRaidText.text = $"x{nextRaid.ToString()}";
        roundCounterText.text = raidWaves.ToString();
        wheatIncomeText.text = (villagersCount * wheatPerVillager).ToString();
        wheatOutcomeText.text = (warriorCount * wheatPerWarrior).ToString();
    }

    public void UpdateStats() {
        if(raidWaves == 0) {
            totalWavesSurvived.text = raidWaves.ToString();
        } else {
            totalWavesSurvived.text = (raidWaves - 1).ToString();
        }
        totalWheatText.text = totalWheat.ToString();
        totalWarriorsText.text = totalWarriors.ToString();
        totalEnemiesText.text = (totalEnemiesDefeated - nextRaid).ToString();
    }

    public void AddVillager() {
        wheatCount -= villagerCost;
        villagerTimer = villagerCreateTime;
        villagerButton.interactable = false;
    }

    public void AddWarrior() {
        wheatCount -= warriorCost;
        warriorTimer = warriorCreateTime;
        warriorButton.interactable = false;
    }

    public void ClickOnPauseButton() {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void ClickOnResumeButton() {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    public void ClickOnMuteSound() {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        
        if (!audioMute) {
            foreach (var audioSource in audioSources) {
                audioSource.mute = true;
            }
            audioMute = true;
            muteSoundButton.GetComponent<Image>().sprite = soundOffSprite;
        } else {
            foreach (var audioSource in audioSources) {
                audioSource.mute = false;
            }
            audioMute = false;
            muteSoundButton.GetComponent<Image>().sprite = soundOnSprite;
        }
    }

    public void ReloadScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ClickOnSpeedControlButton() {
        if(Time.timeScale == 1) {
            Time.timeScale = 3;
        }
        else {
             Time.timeScale = 1;
        }
    }
}
