using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager instance; //Singleton
    public enum GameMode { ShotBeat, ShotFree, TimeBeat, TimeFree };

    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject victoryPanel;
    public GameObject animationPanel;
 
    private int currentLevel;
    private GameMode currentGameMode;
    private Color currentPlayerColor;
    private int currentHighscore;

    private float timePassed = 0.0f;
    private int shotsTaken = 0;
    private bool isPaused = false;

    private TextMeshProUGUI smallText;
    private TextMeshProUGUI smallText2;
    private TextMeshProUGUI largeText;

    void Awake() {

        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

    }

    private void Start() {

        //Receive config info
        currentLevel = PersistanceDataManager.instance.currentLevel;
        currentGameMode = GameMode.ShotBeat;
        currentPlayerColor = Constants.colors[PersistanceDataManager.instance.currentColor];
        currentHighscore = PersistanceDataManager.instance.levelsShotHighscore[currentLevel-1];

        //Get References
        smallText = hudPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        smallText2 = hudPanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        largeText = hudPanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        smallText.text = "Level " + (currentLevel+1).ToString();
        smallText2.text = currentHighscore.ToString() + " shots";
        largeText.text = "0";

        //Init animations and music
        animationPanel.GetComponents<TweenTransforms>()[0].Begin();
        AudioManager.instance.playLevelMusic();

    }

    void Update() {

        if (!isPaused && (currentGameMode.Equals(GameMode.TimeBeat) || currentGameMode.Equals(GameMode.TimeFree))) {
            UpdateTimer();
        }

    }

    private void UpdateTimer() {
        timePassed += Time.deltaTime;
        string minutes = Mathf.Floor(timePassed / 60).ToString("00");
        string seconds = (timePassed % 60).ToString("00");
        smallText.text = string.Format("{0}:{1}", minutes, seconds);
    }

    public void RegisterShot() {
        shotsTaken++;
        largeText.text = shotsTaken.ToString();
    }

    #region Button Events

    public void TogglePausePanel(int state) {
        pausePanel.transform.GetChild(0).gameObject.GetComponents<TweenAlpha>()[state].Begin();
        pausePanel.transform.GetChild(1).gameObject.GetComponents<TweenTransforms>()[state].Begin();
        pausePanel.GetComponent<Canvas>().sortingOrder = state == 1 ? 6 : 2;
        isPaused = state == 1 ? true : false;
        AudioManager.instance.togglePauseEffect(isPaused);
        if (isPaused) {
            AudioManager.instance.playPauseSound();
        } else {
            AudioManager.instance.playResumeSound();
        }
        BallController.instance.SetBallEnabled(!isPaused);

    }

    public void PlayGameOverAnimation() {

        victoryPanel.transform.GetChild(0).GetComponent<TweenTransforms>().Begin();
        victoryPanel.transform.GetChild(1).GetComponent<TweenTransforms>().Begin();
        if (shotsTaken > currentHighscore) {
            victoryPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "GAME OVER";
        } else if (PersistanceDataManager.instance.UpdateLevelShotHighscore(currentLevel, shotsTaken)) {
            victoryPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "new highscore ( " + shotsTaken.ToString() + " )";
        } else {
            victoryPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "shots taken ( " + shotsTaken.ToString() + " )";
        }
        victoryPanel.transform.GetChild(2).GetComponent<TweenTransforms>().Begin();

        StartCoroutine(LoadNewLevel());
    }

    public void RestartLevel() {
        AudioManager.instance.playResetSound();
        StartCoroutine(LoadSceneAfterSeconds());
    }

    public void ExitToMainMenu() {
        AudioManager.instance.playMainMusic();
        SceneManager.LoadSceneAsync(0);
    }

    public void toggleMusic() {
        AudioManager.instance.toggleMusicVolume();
    }

    public void toggleFx() {
        AudioManager.instance.toggleFxVolume();
    }


    #endregion

    IEnumerator LoadNewLevel() {

        yield return new WaitForSeconds(2.0f); 
        victoryPanel.transform.GetChild(0).GetComponent<TweenTransforms>().Begin();
        victoryPanel.transform.GetChild(1).GetComponent<TweenTransforms>().Begin();
        victoryPanel.transform.GetChild(2).GetComponent<TweenTransforms>().Begin();
        yield return new WaitForSeconds(1.0f); 
        animationPanel.GetComponents<TweenTransforms>()[0].Begin();
        yield return new WaitForSeconds(0.4f); 

        Scene scene = SceneManager.GetActiveScene();
        if(PersistanceDataManager.instance.currentLevel+1 <= Constants.NUM_LEVELS) {
            PersistanceDataManager.instance.currentLevel++;
            SceneManager.LoadSceneAsync(PersistanceDataManager.instance.currentLevel);
        } else {
            AudioManager.instance.playMainMusic();
            SceneManager.LoadSceneAsync(0);
        }


    }

    IEnumerator LoadSceneAfterSeconds() {

        yield return new WaitForSeconds(0.4f); 
        animationPanel.GetComponents<TweenTransforms>()[0].Begin();
        yield return new WaitForSeconds(0.4f); 

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(scene.name);

    }

}