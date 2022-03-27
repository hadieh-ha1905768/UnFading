using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyLightSc : MonoBehaviour
{
    new Light light;
    void Start()
    {
        light = GetComponent<Light>();

    }

    public void lightChange() {   
        light.intensity = Mathf.Lerp(0.5f, 1.1f, Mathf.PingPong(Time.time, 1));
    }

}
