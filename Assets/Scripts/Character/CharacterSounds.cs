using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioSource noise_audio_source;
    public AudioClip noise_sound;

    public AudioSource moving_audio_source;
    public AudioClip walking_sound;
    public AudioClip running_sound;
    public AudioClip stealth_sound;

    public AudioSource lantern_audio_source;
    public AudioClip lantern_click_sound;

    bool moving_sound_stop_requested = false;


    private void Start()
    {
        // Set noises
        noise_audio_source.clip = noise_sound;
        lantern_audio_source.clip = lantern_click_sound;
        LoadWalkingSound();


        // Adjust sounds
        lantern_audio_source.pitch = 0.75f;


        // Play sounds
        noise_audio_source.Play();

    }

    private void Update()
    {

        // Let the move sound finish when stop moving so it doesnt cut abruptly the sound
        if (moving_sound_stop_requested && !moving_audio_source.isPlaying) { 
            moving_sound_stop_requested = false;
            moving_audio_source.Stop();
        }

    }

    public void PlayMovingSound()
    {
        moving_audio_source.loop = true;
        moving_audio_source.Play();
    }

    public void StopMovingSound()
    {
        moving_sound_stop_requested = true;
        moving_audio_source.loop = false;
    }

    public void LoadWalkingSound() {
        moving_audio_source.clip = walking_sound;
        moving_audio_source.volume = 0.75f;
        moving_audio_source.pitch = 0.75f;
    }
    public void LoadRunningSound()
    {
        moving_audio_source.clip = running_sound;
        moving_audio_source.volume = 1.0f;
        moving_audio_source.pitch = 1.0f;
    }
    public void LoadStealthSound()
    {
        moving_audio_source.clip = stealth_sound;
        moving_audio_source.volume = 0.5f;
        moving_audio_source.pitch = 0.65f;
    }

    public void PlayLanternActivateSound()
    {
        lantern_audio_source.Stop();
        lantern_audio_source.Play();
    }

    public void PlayLanternDeactivateSound()
    {
        lantern_audio_source.Stop();
        lantern_audio_source.Play();
    }
}
