using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageEnv : MonoBehaviour
{
    public List<GameObject> Envis;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    public void DisableAllExcept(GameObject chosen_one)
    {
        foreach (GameObject g_o in Envis)
        {
            if(g_o != chosen_one)
            {
                g_o.GetComponent<ChoseAmbient>().DisableChilds();
            }
            else
            {
                g_o.GetComponent<ChoseAmbient>().EnableChilds();
            }
        }
    }
}
