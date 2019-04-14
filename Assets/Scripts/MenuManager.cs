using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject mainScreenGameObject;
    public GameObject highscoresGameObject;
    public GameObject settingsGameObject;
    public GameObject playerSelectorGameObject;
    public GameObject levelSelectorGameObject;
    public GameObject levelAnimationGameObject;

    public int currentMenu = 0;


    private void Start() {
        initLevels();
        initHighscores();
        playerSelectorGameObject.transform.GetChild(1).GetComponent<Image>().color = Constants.colors[PersistanceDataManager.instance.currentColor];
        playerSelectorGameObject.transform.GetChild(2).GetComponent<Image>().color = Constants.colors[PersistanceDataManager.instance.currentColor];
    }

    #region Button Animation Calls

    public void ToggleMainScreen(int state) {
        mainScreenGameObject.GetComponents<TweenTransforms>()[state].Begin();
        if (state == 1) AudioManager.instance.playMainMusic();
    }
    public void ToggleSettings(int state) {
        settingsGameObject.GetComponents<TweenTransforms>()[state].Begin();
        playEnterBackSound(state);
    }
    public void ToggleHighscores(int state) {
        highscoresGameObject.GetComponents<TweenTransforms>()[state].Begin();
        playEnterBackSound(state);
    }
    public void ToggleLevelSelector(int state) {
        currentMenu = state < 0 ? currentMenu - 1 : currentMenu + 1;
        levelSelectorGameObject.GetComponents<TweenTransforms>()[state].Begin();
        if (state == 1) AudioManager.instance.playSelectorMusic();
    }
    public void TogglePlayerSelector(int state) {
        currentMenu = state < 0 ? currentMenu-1 : currentMenu+1;
        playerSelectorGameObject.GetComponents<TweenTransforms>()[state].Begin();
    }
    public void ToggleAnimationToLevel(int state) {
        levelAnimationGameObject.GetComponents<TweenTransforms>()[state].Begin();
        if (state == 0) {
            StartCoroutine(loadScene(PersistanceDataManager.instance.currentLevel));
            AudioManager.instance.playEnterSound();
        }
    }

    public void playEnterBackSound(int state) {
        if (state == 1 || state == 3)
            AudioManager.instance.playEnterSound();
        else
            AudioManager.instance.playBackSound();
    }

    #endregion

    private void initHighscores() {

        int minHits = 100;
        float averageHits = 0.0f;

        for (int i = 0; i < PersistanceDataManager.instance.levelsShotHighscore.Length; i++) {
            if (PersistanceDataManager.instance.levelsShotHighscore[i] < minHits) { minHits = PersistanceDataManager.instance.levelsShotHighscore[i]; }
            averageHits += (float)PersistanceDataManager.instance.levelsShotHighscore[i];
        }
        averageHits = averageHits / Constants.NUM_LEVELS;

        highscoresGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "00" + " seconds\n\n" +
            minHits.ToString() + " hits\n\n" +
            averageHits.ToString() + " hits";
    }

    private void initLevels() {
        for(int i = 0; i < Constants.NUM_LEVELS; i++) {
            int x = i + 1;
            GameObject go = Instantiate(Resources.Load("LevelButton") as GameObject);
            go.transform.SetParent(levelSelectorGameObject.transform.GetChild(1), false);
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            go.transform.GetComponent<Button>().interactable = i != 11 ? true : false;
            if (PersistanceDataManager.instance.currentLevel - 1 == i) {               
                go.transform.GetComponent<Button>().Select();
            }
            go.transform.GetComponent<Button>().onClick.AddListener(() => SelectLevel(x));
        }
    }

    private void SelectLevel(int numLevel) {
        AudioManager.instance.playBackSound();
        PersistanceDataManager.instance.currentLevel = numLevel;
        levelSelectorGameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "selected level: " + numLevel.ToString();
    }

    public void SelectColor(int id) {
        StartCoroutine(lerpColor(id));
        PersistanceDataManager.instance.currentColor = id;
    }

    public void SelectGameMode(int id) {
        //TODO: Change current sprite
        PersistanceDataManager.instance.currentGameMode = id;
    }

    IEnumerator loadScene(int id_scene) {
        yield return new WaitForSeconds(0.4f); //TODO: Is it really necessary
        SceneManager.LoadSceneAsync(id_scene);
    }

    IEnumerator lerpColor(int id) {

        Color initColor = playerSelectorGameObject.transform.GetChild(1).GetComponent<Image>().color;
        Color finalColor = Constants.colors[id];
        float maxt = 0.4f;
        float t = 0.0f;

        while( t < maxt) {
            playerSelectorGameObject.transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(initColor, finalColor, t / maxt);
            playerSelectorGameObject.transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(initColor, finalColor, t / maxt);
            t += Time.deltaTime;
            yield return null;
        }

    }

}
