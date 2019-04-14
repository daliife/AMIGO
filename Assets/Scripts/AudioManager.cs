using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/*
    TRACKLIST

    mainscreen - Ambient sound

    selection menu - Funky One / Funk Gameloop (en bucle)

    levels - Funky chunk / Your Call (random pick)

    scoreboard - There it is

*/

public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    public AudioClip[] musicClips;
    public AudioClip[] fxClips;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup fxGroup;

    public AudioMixerSnapshot snapShotMusicPaused, snapShotMusicNormal, snapShotMusicMuted;
    public AudioMixerSnapshot snapShotFxNormal, snapShotFxMuted;

    private AudioSource [] musicSource;
    private AudioSource fxSource;

    private bool musicLoops = true;
    private int currMusicId = 0;

    private void Awake() {
       
        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Start() {

        musicSource = gameObject.transform.GetChild(0).GetComponents<AudioSource>();
        musicSource[0].outputAudioMixerGroup = musicSource[1].outputAudioMixerGroup = musicGroup;
        musicSource[0].loop = musicSource[1].loop = musicLoops;
        fxSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();
        fxSource.outputAudioMixerGroup = fxGroup;

    }

    #region Snapshot Presets

    public void togglePauseEffect(bool isPause) {
        if (isPause) {
            snapShotMusicPaused.TransitionTo(0.5f);
        } else {
            snapShotMusicNormal.TransitionTo(0.5f);
        }
    }

    public void toggleFxVolume() {
        if (PersistanceDataManager.instance.isFxOn == 1) {
            snapShotFxMuted.TransitionTo(0.5f);
            PersistanceDataManager.instance.isFxOn = 0;
        } else {
            snapShotFxNormal.TransitionTo(0.5f);
            PersistanceDataManager.instance.isFxOn = 1;
        }
    }

    public void toggleMusicVolume() {
        if (PersistanceDataManager.instance.isMusicOn == 1) {
            snapShotMusicMuted.TransitionTo(0.5f);
            PersistanceDataManager.instance.isMusicOn = 0;
        } else {
            snapShotMusicNormal.TransitionTo(0.5f);
            PersistanceDataManager.instance.isMusicOn = 1;
        }
    }

    public void initSnapshotStates() {

        if (PersistanceDataManager.instance.isMusicOn == 1)
            snapShotMusicNormal.TransitionTo(0f);
        else
            snapShotMusicMuted.TransitionTo(0f);
        

        if (PersistanceDataManager.instance.isFxOn == 1) 
            snapShotFxNormal.TransitionTo(0f);
        else
            snapShotFxMuted.TransitionTo(0f);

    }

    #endregion

    #region Crossfade Presets

    private void Crossfade(AudioClip newTrack, float fadeTime = 1.0f) {

        if(musicSource[currMusicId].volume > 0.0f) {

            int tempCurrId = currMusicId == 0 ? 1 : 0;

            AudioSource newAudioSource = musicSource[tempCurrId];
            newAudioSource.volume = 1.0f;
            newAudioSource.clip = newTrack;
            newAudioSource.Play();
            instance.StartCoroutine(AcutallyCrossfade(newAudioSource, fadeTime));

        }

    }

    IEnumerator AcutallyCrossfade(AudioSource newSource, float fadeTime) {

        float t = 0.0f;
        while(t < fadeTime){
            newSource.volume = Mathf.Lerp(0.0f, 1.0f, t / fadeTime);
            musicSource[currMusicId].volume = 1.0f - newSource.volume;
            t += Time.deltaTime;
            yield return null;
        }

        musicSource[currMusicId].Stop();
        newSource.volume = 1.0f;
        currMusicId = currMusicId == 0 ? 1 : 0;

    }

    #endregion

    public void playLevelMusic() {
        int tempId = Random.Range(2, 5);
        if (PersistanceDataManager.instance.isFxOn == 1) Crossfade(musicClips[tempId], 1.0f);
    }
    public void playSelectorMusic() {
        if (PersistanceDataManager.instance.isFxOn == 1) Crossfade(musicClips[1], 1.0f);
    }
    public void playMainMusic() {
        if (PersistanceDataManager.instance.isFxOn == 1) Crossfade(musicClips[0], 1.0f);
    }

    #region Oneshots Fx Presets
    
    //TODO: Ugly copypaste, modularize and improve asap.

    public void playFxSound(AudioClip clipToPlay) {
        fxSource.PlayOneShot(clipToPlay);
    }
    public void playClickSound() {
        fxSource.PlayOneShot(fxClips[0]);
    }
    public void playEnterSound() {
        fxSource.PlayOneShot(fxClips[1]);
    }
    public void playBackSound() {
        fxSource.PlayOneShot(fxClips[2]);
    }
    public void playShotSound() {
        fxSource.PlayOneShot(fxClips[3]);
    }
    public void playHoleSound() {
        fxSource.PlayOneShot(fxClips[4]);
    }
    public void playPauseSound() {
        fxSource.PlayOneShot(fxClips[5]);
    }
    public void playResumeSound() {
        fxSource.PlayOneShot(fxClips[6]);
    }
    public void playResetSound() {
        fxSource.PlayOneShot(fxClips[7]);
    }

    public void playToggleShotSound(bool isAscending) {
        if(isAscending)
            fxSource.PlayOneShot(fxClips[8]);
        else
            fxSource.PlayOneShot(fxClips[9]);
    }

    public void playCancelShot() {
        fxSource.PlayOneShot(fxClips[10]);
    }

    #endregion

}