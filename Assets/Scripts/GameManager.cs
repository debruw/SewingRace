using System.Collections;
using System.Collections.Generic;
using TapticPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }

            return instance;
        }
    }

    public PlayerControl playerControl;
    public ObiRopeManager obiRopeManager;

    public int currentLevel = 1;
    int MaxLevelNumber = 10;
    public bool isGameStarted, isGameOver, isScalingRope;

    #region UI Elements
    public GameObject WinPanel, LosePanel, InGamePanel;
    public Button VibrationButton, TapToStartButton;
    public Sprite on, off;
    public Text LevelText;
    public GameObject PlayText, ContinueText;
    public Slider RoadSlider;
    public GameObject Tuto1, Tuto2;
    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 60;

        //if (!PlayerPrefs.HasKey("VIBRATION"))
        //{
        //    PlayerPrefs.SetInt("VIBRATION", 1);
        //    VibrationButton.GetComponent<Image>().sprite = on;
        //}
        //else
        //{
        //    if (PlayerPrefs.GetInt("VIBRATION") == 1)
        //    {
        //        VibrationButton.GetComponent<Image>().sprite = on;
        //    }
        //    else
        //    {
        //        VibrationButton.GetComponent<Image>().sprite = off;
        //    }
        //}

        PlayerPrefs.SetInt("FromMenu", 1);
        if (PlayerPrefs.GetInt("FromMenu") == 1)
        {
            ContinueText.SetActive(false);
            PlayerPrefs.SetInt("FromMenu", 0);
        }
        else
        {
            PlayText.SetActive(false);
        }
        currentLevel = PlayerPrefs.GetInt("LevelId");
        LevelText.text = currentLevel.ToString();
    }

    private void OnEnable()
    {
        instance = this;
    }

    public IEnumerator WaitAndGameWin()
    {
        Debug.Log("Win");
        playerControl.fc.ActivateEffect();
        isGameOver = true;
        SoundManager.Instance.StopAllSounds();
        SoundManager.Instance.playSound(SoundManager.GameSounds.Win);

        yield return new WaitForSeconds(1f);

        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Light);

        currentLevel++;
        PlayerPrefs.SetInt("LevelId", currentLevel);
        WinPanel.SetActive(true);
    }

    public IEnumerator WaitAndGameLose()
    {
        Debug.Log("Lose");
        isGameOver = true;
        SoundManager.Instance.playSound(SoundManager.GameSounds.Lose);

        yield return new WaitForSeconds(1f);

        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Medium);

        LosePanel.SetActive(true);
    }

    public void TapToNextButtonClick()
    {
        if (currentLevel > MaxLevelNumber)
        {
            int rand = Random.Range(1, MaxLevelNumber);
            if (rand == PlayerPrefs.GetInt("LastRandomLevel"))
            {
                rand = Random.Range(1, MaxLevelNumber);
            }
            else
            {
                PlayerPrefs.SetInt("LastRandomLevel", rand);
            }
            SceneManager.LoadScene("Level" + rand);
        }
        else
        {
            SceneManager.LoadScene("Level" + currentLevel);
        }
    }

    public void TapToTryAgainButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TapToStartButtonClick()
    {
        isGameStarted = true;
        TapToStartButton.gameObject.SetActive(false);
        if (currentLevel == 1)
        {
            Tuto1.SetActive(true);
        }
    }

    public void VibrateButtonClick()
    {
        if (PlayerPrefs.GetInt("VIBRATION").Equals(1))
        {//Vibration is on
            PlayerPrefs.SetInt("VIBRATION", 0);
            VibrationButton.GetComponent<Image>().sprite = off;
        }
        else
        {//Vibration is off
            PlayerPrefs.SetInt("VIBRATION", 1);
            VibrationButton.GetComponent<Image>().sprite = on;
        }

        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Light);
    }
}
