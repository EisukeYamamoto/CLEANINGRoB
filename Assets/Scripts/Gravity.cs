using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
  public PlayerMotion player;
  public float forceFactor = 0.1f;
  private float x, y, z;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.gameObject.GetComponent<PlayerMotion>();
        x = player.PowerLevel * 3f/5f;
        y = player.PowerLevel * 3f/5f;
        z = player.PowerLevel * 3f/5f;
    }

    void Update() {
      this.transform.localScale = new Vector3(x, y, z);
      x = player.PowerLevel * 3f/5f;
      y = player.PowerLevel * 3f/5f;
      z = player.PowerLevel * 3f/5f;
    }

  Vector3 GetForce(Vector3 pos) {
    float dist = Vector3.Distance(transform.root.position, pos);
    Vector3 dir = (transform.root.position - pos).normalized;
    return dir / (dist * dist) * forceFactor;
  }

  void OnTriggerStay(Collider other){
      if (other.gameObject.tag != "Field"
       && other.gameObject.tag != "Player"
       && other.gameObject.tag != "Battery"
       && other.gameObject.tag != "Engine"
       && other.gameObject.tag != "Battery_son"
       && other.gameObject.tag != "Ball"
       && other.gameObject.tag != "Warp"){
        // Debug.Log(other.gameObject.tag);
        var rb = other.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(GetForce(rb.transform.position));
        // objectBodies.Add(rb);
    }
  }
}
