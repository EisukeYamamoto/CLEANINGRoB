using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
  public static float countdown = 300f;

  private int minites;
  private int seconds;
  private int mseconds;
  GameManager gamemanager;
    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        countdown = 300f;
    }

    // Update is called once per frame
    void Update()
    {
      if (gamemanager.game_stop_flg == false){
        countdown -= Time.deltaTime;

        if(countdown <= 0.0f){
          countdown = 0.0f;
          // gamemanager.GameOver();
        }
      }
    }

    public static float GetTime(){
      return countdown;
    }
}
