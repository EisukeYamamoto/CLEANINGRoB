/*
バッテリーの出現位置を調整するスクリプト
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour
{
    [SerializeField] GameObject Battery_Prefab = default;
    // [SerializeField] PlayerMotion player = default;
    [SerializeField] Transform Field = default;
    [SerializeField] int Battery_limit = 5;

    // Start is called before the first frame update
    void Start()
    {
      for(int i = 1; i <= Battery_limit; i++){
        GameObject battery = Instantiate(Battery_Prefab);
        switch(i){
          case 1:
             battery.transform.position = new Vector3(10f, 0.9f, 10f);
             break;
          case 2:
             battery.transform.position = new Vector3(-4f, 0.9f, -16f);
             break;
          case 3:
             battery.transform.position = new Vector3(-10f, 2.5f, -10f);
             break;
          case 4:
             battery.transform.position = new Vector3(-18f, 3.4f, 19f);
             break;
          case 5:
             battery.transform.position = new Vector3(24f, 3.2f, 2.5f);
             break;
          default:
             break;
        }
      }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
