using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AudioSource _thrusterHAS, _thrusterMAS, _thrusterLAS, _engineRumbleAS, _laserAS, _explosionAS, _welcomeAS;

    public AudioClip[] _audioClip;

    //[SerializeField] private AudioManager _audioManager; //Links to script - can run functions
    //[serializeField] private AudioSource _audioSource; // can do _audioSource.pitch etc after 
    //    public AudioSource _audioSource; public GameObject _audioSourceObj;
    //    _audioSource = _audioSourceObj.GetComponent<AudioSource>();

    private float _lastPlayTime = 0f; //thruster cooldown to avoid audio phasing on playing back twice (both rings have same hit point)
    private float _cooldownDuration = 0.1f; // 100 milliseconds cooldown

    public void PlayThruster()
    {
        if (Time.time < _lastPlayTime + _cooldownDuration)
        {
            return; // Skip playing the sound if we're in the cooldown period
        }
        
        int _index = Random.Range(0, 3);
        Debug.Log("Playing Thruster: " + _index);
        if (_index == 0) _thrusterHAS.PlayOneShot(_audioClip[_index]);
        else if (_index == 1) _thrusterMAS.PlayOneShot(_audioClip[_index]);
        else if (_index == 2) _thrusterLAS.PlayOneShot(_audioClip[_index]);
        
        _lastPlayTime = Time.time; // Update the last play time
    }

    public void PlayEngineRumble(float _volume)
    {
        _engineRumbleAS.Play();
        _engineRumbleAS.volume = _volume;
    }

    public void AdjustEngineRumbleVolume(float _volume)
    {
        _engineRumbleAS.volume = _volume;
    }

    public void PlayLaser()
    {
        _laserAS.Play();
    }

    public void PlayExplosion()
    {
        _explosionAS.Play();
    }

    public void PlayWelcome()
    {
        _welcomeAS.Play();
    }
}
