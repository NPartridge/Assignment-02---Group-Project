using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip menuSong;
    [SerializeField] AudioClip[] gameMusic;
    [SerializeField] AudioClip gameOver;

    [SerializeField] float volumeDecrement = 0.05f;

    private int currentSceneIndex;
    private bool hasSceneChanged = false;

    private AudioSource audioSource;

    private void Start()
    {
        // Record the current scene index when we start the game
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Get a reference to the AudioSource
        audioSource = GetComponent<AudioSource>();
        // Load Menu Music
        LoadSong(menuSong, true);
        // Play Menu Music
        audioSource.Play();
    }

    private void Update()
    {
        // Check if the current scene index we have recorded matches the active scene index
        // If not match, make the current scene index the active scene index and display a message to say scene has changed
        hasSceneChanged = RecordSceneChange(currentSceneIndex, SceneManager.GetActiveScene().buildIndex);

        // If the scene has changed we can do stuff
        if (hasSceneChanged)
        {
            // Check to see which scene we are in and load appropriate music
            if (currentSceneIndex == 0)
            {
                LoadSong(menuSong, true);
                audioSource.Play();
            }

            // if we enter the game scene, lower the menu music volume, and the stop the audio player.
            if (currentSceneIndex == 1)
            {
                StartCoroutine(LowerVolume(audioSource));
            }
        }

        // if we are in the game scene, check if the music has stopped and get the next song
        if (currentSceneIndex == 1 && audioSource.isPlaying == false)
        {
            LoadSong(GetNextSong(audioSource.clip, gameMusic), false);
            audioSource.Play();
            Debug.Log(audioSource.clip.name);
        }

        // For Debugging
        if (Input.GetKey(KeyCode.Space))
        {
            audioSource.Stop();
        }

    }

    private bool RecordSceneChange(int currentScene, int activeScene)
    {
        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            return true;
        }
        return false;
    }


    private void LoadSong(AudioClip songTrack, bool isLooping, float songVolume = 0.3f)
    {
        // Set the audio clip to be the menu music
        audioSource.clip = songTrack;
        // Set the audio source loop behavior and volume
        audioSource.loop = isLooping;
        audioSource.volume = songVolume;

    }

    // Choose a random song from the song List
    private AudioClip GetNextSong(AudioClip currentSong, AudioClip[] songList)
    {
        AudioClip nextSong;
        do
        {
            int nextSongNumber = Random.Range(0, songList.Length);
            nextSong = songList[nextSongNumber];
        }
        while (nextSong == currentSong);

        return nextSong;
    }

    IEnumerator LowerVolume(AudioSource audio)
    {
        while(audio.volume != 0)
        {
            audio.volume -= volumeDecrement * Time.deltaTime;
            yield return null;
        }

        // NB audio.Stop() initially caused game to hitch. Selecting "Preload Audio Data" in the import settings for each clip seemed to fix this. 
        // Reference: https://www.reddit.com/r/Unity3D/comments/188retb/game_stutters_when_switching_to_a_new_audio_clip/
        audio.Stop();
    }

    // Call this method from the GameOverManager Script
    public void PlayGameOverMusic()
    {
        LoadSong(gameOver, false);
        audioSource.Play();
    }
}
