using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseAmbient : MonoBehaviour
{
    public GameObject day_music;
    public GameObject night_music;

    public GameObject sun;

    // Update is called once per frame
    void Update()
    {
        if(sun.GetComponent<DayNightCycle>().timeOfDay >= 21.1f && sun.GetComponent<DayNightCycle>().timeOfDay <= 21.2f)
        {
            day_music.SetActive(false);
            night_music.SetActive(true);
            night_music.GetComponent<AudioSource>().Play();
        }
        else if (sun.GetComponent<DayNightCycle>().timeOfDay >= 6.1f && sun.GetComponent<DayNightCycle>().timeOfDay <= 6.2f)
        {
            night_music.SetActive(false);
            day_music.SetActive(true);
            day_music.GetComponent<AudioSource>().Play();
        }
    }
}
