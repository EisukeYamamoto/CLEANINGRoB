/*
上空からプレイヤーを撮影するカメラのスプリクト
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player = default;

    // Start is called before the first frame update
    void Start()
    {
      this.transform.position = new Vector3(player.transform.position.x, 10f, player.transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      this.transform.position = new Vector3(player.transform.position.x, 10f, player.transform.position.z);
    }
}
