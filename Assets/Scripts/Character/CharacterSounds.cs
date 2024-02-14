using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioSource noise_audio_source;
    public AudioClip noise_sound;

    public AudioSource footstep_audio_source;
    public AudioClip walk_footstep_sound;

    public AudioSource lantern_audio_source;
    public AudioClip lantern_click_sound;

    private void Start()
    {
        // Set noises
        noise_audio_source.clip = noise_sound;
        lantern_audio_source.clip = lantern_click_sound;


        // Adjust sounds
        footstep_audio_source.pitch = 0.75f;
        lantern_audio_source.pitch = 0.75f;


        // Play sounds
        noise_audio_source.Play();

    }
    public void PlayFootstepSound()
    {
        footstep_audio_source.clip = walk_footstep_sound;
        footstep_audio_source.Play();
    }

    public void StopFootstepSound()
    {
        footstep_audio_source.Stop();
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
