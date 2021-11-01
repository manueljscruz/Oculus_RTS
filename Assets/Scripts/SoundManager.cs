using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    [SerializeField]
    private AudioClip[] combatSounds;

    [SerializeField]
    private AudioClip[] chirpSounds;

    [SerializeField]
    private GameObject[] chirpLocations;

    private float chirpTimer;

    private int randomClip;

    // Start is called before the first frame update
    void Awake()
    {
        chirpLocations = GameObject.FindGameObjectsWithTag("ChirpLocations");
        chirpTimer = 3.5f;

        DontDestroyOnLoad(this.gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }

        PlaySound("MainTheme");
        PlaySound("Wind");
    }

    private void Update()
    {
        chirpTimer -= Time.deltaTime;

        if(chirpTimer <= 0)
        {
            chirpTimer = 3.5f;
            ChangeClip("Birds");
            PlaySound("Birds", chirpLocations[UnityEngine.Random.Range(0, chirpLocations.Length)].transform.position);
        }
    }

    public void PlaySound(string name, Vector3? pos = null)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("The sound with this name: " + name + " :was not found");
            return;
        }

        if (pos != null)
        {
            AudioSource.PlayClipAtPoint(s.source.clip, (Vector3)pos);
        }

        else
        {
            s.source.Play();
        }
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("The sound with this name: " + name + " :was not found");
            return;
        }

        s.source.Pause();
    }

    public void ChangePitch(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("The sound with this name: " + name + " :was not found");
            return;
        }

        s.source.pitch = UnityEngine.Random.Range(0f, 3f);
    }

    public void ChangeClip(string name)
    {
        if (name == "UnitsFight")
        {
            randomClip = UnityEngine.Random.Range(0, combatSounds.Length);
            Sound s = Array.Find(sounds, Sound => Sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("The sound with this name: " + name + " :was not found");
                return;
            }

            s.source.clip = combatSounds[randomClip];
        }

        if (name == "Birds")
        {
            randomClip = UnityEngine.Random.Range(0, chirpSounds.Length);
            Sound s = Array.Find(sounds, Sound => Sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("The sound with this name: " + name + " :was not found");
                return;
            }

            s.source.clip = chirpSounds[randomClip];
        }
    }
}