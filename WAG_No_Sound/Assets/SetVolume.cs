using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
    public AudioMixer audio_mixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterVolume(float slider_value)
    {
        audio_mixer.SetFloat("Master_Volume", Mathf.Log10(slider_value)*20);
    }

    public void SetMusicVolume(float slider_value)
    {
        audio_mixer.SetFloat("Music_Volume", Mathf.Log10(slider_value) * 20);
    }
}
