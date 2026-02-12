using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
  public string name;
  public AudioClip clip;
  [Range(0f, 1f)]
  public float volume = 0.7f;
  [Range(0.5f, 1.5f)]
  // public float pitch = 1.0f;
  // public bool loop = false;

  [HideInInspector]
  public AudioSource source; // The AudioSource is assigned at runtime
}

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
  public Sound[] sounds;
  void Awake()
  {
    // Add an AudioSource for each sound
    foreach (Sound s in sounds)
    {
      s.source = gameObject.AddComponent<AudioSource>();
      s.source.clip = s.clip;
      s.source.volume = s.volume;
      // s.source.pitch = s.pitch;
      // s.source.loop = s.loop;
      s.source.playOnAwake = false;
    }
  }
  public void Play(string name)
  {
    Sound s = System.Array.Find(sounds, sound => sound.name == name);
    if (s == null)
    {
      Debug.LogWarning("Sound: " + name + " not found!");
      return;
    }
    s.source.pitch = Random.Range(0.9f, 1.1f);
    s.source.Play();
  }
}
