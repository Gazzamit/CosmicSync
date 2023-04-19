using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AudioSource _thrusterHAS, _thrusterMAS, _thrusterLAS, _engineRumbleAS, _laserAS, _explosionAS;

    public void PlayThruster()
    {

        int _index = Random.Range(0, 3);
        Debug.Log("Playong Thruster: " + _index);
        if (_index == 0 && _thrusterHAS.isPlaying == false) _thrusterHAS.Play();
        else if (_index == 1 && _thrusterMAS.isPlaying == false) _thrusterMAS.Play();
        else if (_index == 2 && _thrusterLAS.isPlaying == false) _thrusterLAS.Play();

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
}
