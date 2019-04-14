using UnityEngine;

public class PersistanceDataManager : MonoBehaviour {

    public static PersistanceDataManager instance;

    public int currentLevel;
    public int currentGameMode;
    public int currentColor;
    public int isMusicOn;
    public int isFxOn;

    public int[] levelsState;
    public int[] levelsShotHighscore;

    void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    void Start() {

        if (PlayerPrefs.HasKey("currentLevel")) {
            //Debug.Log("Playerprefs existsting.");
            GetPlayerPrefs();
        } else {
            //Debug.Log("New playerprefs existsting.");
            InitPlayerPrefs();
        }

        AudioManager.instance.initSnapshotStates();

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) DeleteData();
    }

    public void InitPlayerPrefs() {

        //Setting initial state
        currentLevel = 1;
        currentGameMode = 0;
        currentColor = 0;
        isMusicOn = 1;
        isFxOn = 1;
        levelsState = new int[Constants.NUM_LEVELS];
        levelsShotHighscore = new int[Constants.NUM_LEVELS];
        for (int i = 0; i < Constants.NUM_LEVELS; i++) {
            levelsState[i] = 0;
            levelsShotHighscore[i] = 10;
        }
        levelsState[0] = 1;

    }

    public void GetPlayerPrefs() {

        currentLevel = PlayerPrefs.GetInt(Constants.CURRENT_LEVEL_TAG);
        currentGameMode = PlayerPrefs.GetInt(Constants.CURRENT_GAME_MODE_TAG);
        currentColor = PlayerPrefs.GetInt(Constants.CURRENT_COLOR_TAG);
        isMusicOn = PlayerPrefs.GetInt(Constants.IS_MUSIC_ON_TAG);
        isFxOn = PlayerPrefs.GetInt(Constants.IS_FX_ON_TAG);

        levelsState = new int[Constants.NUM_LEVELS];
        levelsShotHighscore = new int[Constants.NUM_LEVELS];
        for (int i=0; i < Constants.NUM_LEVELS; i++) {
            levelsState[i] = PlayerPrefs.GetInt(Constants.LEVELS_STATE_TAG + i.ToString());
            levelsShotHighscore[i] = PlayerPrefs.GetInt(Constants.LEVELS_SHOT_HIGHSCORE_TAG + i.ToString());
        }

    }

    public bool UpdateLevelShotHighscore(int num_level, int num_shots) {
        if (num_shots < levelsShotHighscore[num_level - 1]) { 
            levelsShotHighscore[num_level-1] = num_shots;
            //PlayerPrefs.SetInt(Constants.LEVELS_SHOT_HIGHSCORE_TAG + num_level.ToString(), num_shots);            
            return true;
        }
        return false;
    }

    public void SaveAll() {

        PlayerPrefs.SetInt(Constants.CURRENT_LEVEL_TAG, currentLevel);
        PlayerPrefs.SetInt(Constants.CURRENT_GAME_MODE_TAG, currentGameMode);
        PlayerPrefs.SetInt(Constants.CURRENT_COLOR_TAG, currentColor);
        PlayerPrefs.SetInt(Constants.IS_MUSIC_ON_TAG, isMusicOn);
        PlayerPrefs.SetInt(Constants.IS_FX_ON_TAG, isFxOn);

        for (int i = 0; i < Constants.NUM_LEVELS; i++) {
            PlayerPrefs.SetInt(Constants.LEVELS_STATE_TAG + i.ToString(), levelsState[i]);
            PlayerPrefs.SetInt(Constants.LEVELS_SHOT_HIGHSCORE_TAG + i.ToString(), levelsShotHighscore[i]);
        }

        PlayerPrefs.Save();

    }

    public void DeleteData() {
        PlayerPrefs.DeleteAll();
        Debug.Log("Deleted playerprefs");
    }

    private void OnApplicationQuit() {
        SaveAll();
    }

}