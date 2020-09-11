using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryRotation : MonoBehaviour
{
    public float rotateSpeed = 1f;
    private float rotate = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      this.transform.rotation = Quaternion.Euler(30f, rotate * rotateSpeed, 0f);
      rotate += 1f;
      if (rotate > 360f){
        rotate -= 360f;
      }
    }
}
