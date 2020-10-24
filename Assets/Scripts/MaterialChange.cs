/*
マテリアルの色を変更するスクリプト 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour
{
    public Material[] _material;
    [SerializeField] PlayerMotion player = default;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (player.isWarpFlag == true){
        this.GetComponent<Renderer>().material = _material[0];
      }
      else{
        this.GetComponent<Renderer>().material = _material[1];
      }
    }
}
