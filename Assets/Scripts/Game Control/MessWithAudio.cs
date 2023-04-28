using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessWithAudio : MonoBehaviour
{
    public AudioSource _audioSource;
    public GameObject _beatControllerObject;

    void Start()
    {
        if (GameManagerDDOL._doWelcome == true && DialogueManager._instance.DEV_BYPASS_INTRO == false)
        {
            //_audioSource = _audioSourceObj.GetComponent<AudioSource>();
            StartCoroutine(PitchChangeAudio());
        }
    }

    IEnumerator PitchChangeAudio()
    {
        Debug.Log("BC - Messing with audio");
        yield return new WaitForSeconds(11);
        float _currentPitch = 1f;
        while (WelcomeDialogue._resetAudioWhenRaiReboots == false)
        {
            float _newPitch = Random.Range(0.5f, 0.8f);
            Debug.Log("BC - New Pitch: " + _newPitch);
            float _durationOfChange = Random.Range(2f, 3f);
            float _ratio = 0.0f;

            while (_ratio < 0.98f)
            {
                _ratio += Time.deltaTime / _durationOfChange;
                _audioSource.pitch = Mathf.Lerp(_currentPitch, _newPitch, _ratio);
                yield return null;
            }

            _currentPitch = _newPitch;
        }

        //quickly reset audio pitch
        Debug.Log("BC - Reset Audio");
        float _elapsedTime = 0.0f;
        float _resetDuration = 0.3f;

        while (_elapsedTime < _resetDuration)
        {
            float _ratio = _elapsedTime / _resetDuration;
            _audioSource.pitch = Mathf.Lerp(_currentPitch, 1.0f, _ratio);
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        //better safe than sorry!
        _audioSource.pitch = 1.0f;

        //beatontroller will reactivate it
        _audioSource.Stop();
    }
}
