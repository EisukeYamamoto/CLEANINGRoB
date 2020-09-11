using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

  // [SerializeField] Transform Field = default;
  private float Field_x;
  private float Field_z;
  Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
      Field_x = 50f - 3f;
      Field_z = 50f - 3f;
      rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < 0.01f){
          Debug.Log("Droped");
          this.transform.position = new Vector3(0f, 5f, 0f);
          rb.velocity = Vector3.zero;
          rb.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionStay(Collision Collision){
      string layerName = LayerMask.LayerToName(Collision.gameObject.layer);

      if (layerName == "SofaBase"){
        Debug.Log("Base");
        this.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                 0.8f,
                                                 Random.value * Field_z - Field_z/2f);
      }
    }
}
