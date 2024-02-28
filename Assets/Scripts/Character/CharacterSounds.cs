using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioSource noise_audio_source;
    public AudioClip noise_sound;

    public AudioSource moving_audio_source;
    public AudioClip[] walking_sounds;
    public AudioClip running_sound;
    public AudioClip stealth_sound;

    public AudioSource lantern_audio_source;
    public AudioClip lantern_click_sound;

    bool moving_sound_stop_requested = false;
    public float min_pitch = 0.5f;
    public float max_pitch = 1.0f;

    private CharacterMovement character_movement;

    private void Start()
    {
        // Get player behavior
        character_movement = GetComponent<CharacterMovement>();

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
        moving_audio_source.clip = walking_sounds[Random.Range(0, walking_sounds.Length)];
        moving_audio_source.pitch = Random.Range(min_pitch, max_pitch);
        moving_audio_source.Play();
        StartCoroutine(DetectSoundFinish(moving_audio_source));
    }

    public void StopMovingSound()
    {
        moving_sound_stop_requested = true;
        moving_audio_source.loop = false;
    }

    public void LoadWalkingSound() {
        moving_audio_source.volume = 0.75f;
        min_pitch = 0.74f;
        max_pitch = 0.78f;
    }
    public void LoadRunningSound()
    {
        moving_audio_source.clip = running_sound;
        moving_audio_source.volume = 1.0f;
        min_pitch = 0.94f;
        max_pitch = 0.97f;
    }
    public void LoadStealthSound()
    {
        moving_audio_source.clip = stealth_sound;
        moving_audio_source.volume = 0.5f;
        min_pitch = 0.39f;
        max_pitch = 0.42f;
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


    private IEnumerator DetectSoundFinish(AudioSource audio_source)
    {
        // Wait while the sound is still playing
        yield return new WaitWhile(() => audio_source.isPlaying);
        OnSoundFinished();
    }

    private void OnSoundFinished()
    {
        character_movement.walk_sound_trigger = false;
    }
}
