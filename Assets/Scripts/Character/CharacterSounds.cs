using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioSource noise_audio_source;
    public AudioClip noise_sound;

    public AudioSource moving_audio_source;
    public AudioClip[] carpet_sounds;

    public AudioSource lantern_audio_source;
    public AudioClip lantern_click_sound;

    bool moving_sound_stop_requested = false;
    public float min_pitch = 0.5f;
    public float max_pitch = 1.0f;

    public AudioSource heartbeat_audio_source;
    public AudioClip heartbeat_sound;

    private CharacterMovement character_movement;

    private void Start()
    {
        // Get player behavior
        character_movement = GetComponent<CharacterMovement>();

        // Set noises
        noise_audio_source.clip = noise_sound;
        lantern_audio_source.clip = lantern_click_sound;
        heartbeat_audio_source.clip = heartbeat_sound;
        LoadWalkingSound();


        // Adjust sounds
        lantern_audio_source.pitch = 0.75f;


        // Play sounds
        heartbeat_audio_source.loop = true;
        heartbeat_audio_source.Play();
        noise_audio_source.Play();

    }

    private void Update()
    {

        // Let the move sound finish when stop moving so it doesnt cut abruptly the sound
        if (moving_sound_stop_requested && !moving_audio_source.isPlaying) { 
            moving_sound_stop_requested = false;
            moving_audio_source.Stop();
        }

        float tension = character_movement.GetTension();
        float t;
        if (tension <= 40)
        {
            heartbeat_audio_source.volume = 0.0f;
            heartbeat_audio_source.pitch = 1.0f;
            moving_audio_source.volume = 1.0f;
            lantern_audio_source.volume = 1.0f;
        }
        else if (tension >= 60)
        {
            heartbeat_audio_source.volume = tension * 0.01f;
            heartbeat_audio_source.pitch = 1.0f + heartbeat_audio_source.volume * 0.5f;
            moving_audio_source.volume = 1.0f - heartbeat_audio_source.volume * 2.0f;
            if (moving_audio_source.volume < 0.1f) moving_audio_source.volume = 0.1f;
            lantern_audio_source.volume = 1.0f - heartbeat_audio_source.volume * 2.0f;
            if (lantern_audio_source.volume < 0.1f) lantern_audio_source.volume = 0.1f;
        }
        else
        {
            t = (tension - 40.0f) / (60.0f - 40.0f); // Normalize tension between 30 and 50 to a 0-1 scale
            float initialHeartbeatVolume = 0.0f;
            float targetHeartbeatVolume = tension * 0.01f;
            float initialHeartbeatPitch = 1.0f;
            float targetHeartbeatPitch = 1.0f + targetHeartbeatVolume * 0.5f;
            float initialMovingVolume = 1.0f;
            float targetMovingVolume = 1.0f - targetHeartbeatVolume * 2.0f;

            heartbeat_audio_source.volume = Mathf.Lerp(initialHeartbeatVolume, targetHeartbeatVolume, t);
            heartbeat_audio_source.pitch = Mathf.Lerp(initialHeartbeatPitch, targetHeartbeatPitch, t);
            moving_audio_source.volume = Mathf.Lerp(initialMovingVolume, targetMovingVolume, t);
            lantern_audio_source.volume = Mathf.Lerp(initialMovingVolume, targetMovingVolume, t);
        }
    }

    public void PlayMovingSound()
    {
        moving_audio_source.clip = carpet_sounds[Random.Range(0, carpet_sounds.Length)];
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
        moving_audio_source.clip = carpet_sounds[Random.Range(0, carpet_sounds.Length)];
        moving_audio_source.volume = 1.0f;
        min_pitch = 0.94f;
        max_pitch = 0.97f;
    }
    public void LoadStealthSound()
    {
        moving_audio_source.clip = carpet_sounds[Random.Range(0, carpet_sounds.Length)];
        moving_audio_source.volume = 0.5f;
        min_pitch = 0.49f;
        max_pitch = 0.52f;
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
