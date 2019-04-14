using UnityEngine;

public class Constants : MonoBehaviour {

    //Game Info
    public static int NUM_LEVELS = 2;
    public static string[] gameModes = { "ShotBeat", "ShotFree", "TimeBeat", "TimeBeat" };
    //public enum GameMode { ShotBeat, ShotFree, TimeBeat, TimeFree };

    //String Tags
    public static string CURRENT_LEVEL_TAG = "currentLevel";
    public static string CURRENT_GAME_MODE_TAG = "currentGameMode";
    public static string CURRENT_COLOR_TAG = "currentColor";
    public static string IS_MUSIC_ON_TAG = "isMusicOn";
    public static string IS_FX_ON_TAG = "isFxOn";
    public static string LEVELS_STATE_TAG = "levelsState_";
    public static string LEVELS_SHOT_HIGHSCORE_TAG = "levelsShotHighscore_";

    //Colors & design
    public static Color[] colors = { new Color(0, 0, 1, 1),
                                     new Color(1, 0, 0, 1),
                                     new Color(1, 1, 0, 1),
                                     new Color(0, 1, 0, 1)};



}