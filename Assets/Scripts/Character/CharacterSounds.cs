using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    public AudioSource noise_audio_source;
    public AudioClip noise_sound;

    public AudioSource footstep_audio_source;
    public AudioClip[] footstep_sounds;

    private void Start()
    {
        // Play stereo noise sound
        noise_audio_source.clip = noise_sound;
        noise_audio_source.Play();

        // Adjust sounds
        footstep_audio_source.pitch = 0.75f;
    }
    public void PlayFootstepSound()
    {
        if (footstep_sounds.Length > 0)
        {
            footstep_audio_source.clip = footstep_sounds[Random.Range(0, footstep_sounds.Length)];
            footstep_audio_source.Play();
        }
    }

    public void StopFootstepSound()
    {
        footstep_audio_source.Stop();
    }
}
