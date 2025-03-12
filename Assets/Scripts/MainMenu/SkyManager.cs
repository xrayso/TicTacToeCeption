using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour{

    [SerializeField] float speed;

    [SerializeField] Material nightSkyBox, daySkyBox;


    void Start(){

        System.DateTime currentTime = System.DateTime.Now;
        System.DateTime targetTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 19, 0, 0);

        if (currentTime > targetTime){
            RenderSettings.skybox = nightSkyBox;
        }else{
            RenderSettings.skybox = daySkyBox;
        }
    }

}
