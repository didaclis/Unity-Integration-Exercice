using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseAmbient : MonoBehaviour
{
    public GameObject day_music;
    public GameObject night_music;
    public GameObject day_music_real;
    public GameObject night_music_real;
    public GameObject sun;
    bool change = false;
    // Update is called once per frame
    void ManageAudio()
    {
        if (day_music == day_music_real)
            return;
        if (sun.GetComponent<DayNightCycle>().timeOfDay >= 21.1f && sun.GetComponent<DayNightCycle>().timeOfDay <= 21.2f && !change)
        {
            day_music.SetActive(false);
            night_music.SetActive(true);
            night_music.GetComponent<AudioSource>().Play();
            day_music_real.SetActive(false);
            night_music_real.SetActive(true);
            night_music_real.GetComponent<AudioSource>().Play();
            change = true;
        }
        else if (sun.GetComponent<DayNightCycle>().timeOfDay >= 6.1f && sun.GetComponent<DayNightCycle>().timeOfDay <= 6.2f && change)
        {
            night_music.SetActive(false);
            day_music.SetActive(true);
            day_music.GetComponent<AudioSource>().Play();
            night_music_real.SetActive(false);
            day_music_real.SetActive(true);
            day_music_real.GetComponent<AudioSource>().Play();
            change = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.GetComponent<ManageEnv>().DisableAllExcept(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ManageAudio();
        }
    }
    public void EnableChilds()
    {
        if (sun.GetComponent<DayNightCycle>().timeOfDay >= 6.1f && sun.GetComponent<DayNightCycle>().timeOfDay <= 21.2f)
        {
            night_music.SetActive(false);
            day_music.SetActive(true);
            day_music.GetComponent<AudioSource>().Play();
            night_music_real.SetActive(false);
            day_music_real.SetActive(true);
            day_music_real.GetComponent<AudioSource>().Play();
        }      
        else
        {
            day_music.SetActive(false);
            night_music.SetActive(true);
            night_music.GetComponent<AudioSource>().Play();
            day_music_real.SetActive(false);
            night_music_real.SetActive(true);
            night_music_real.GetComponent<AudioSource>().Play();
        }
    }
    public void DisableChilds()
    {
        day_music.SetActive(false);
        night_music.SetActive(false);
        day_music_real.SetActive(false);
        night_music_real.SetActive(false);
    }
}
