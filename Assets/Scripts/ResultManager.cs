using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
  //スコア関連
  [SerializeField] Text ScoreText = default;
  private int score = 0;

  //タイム関連
  [SerializeField] Text timeText = default;
  private float countdown;
  private int minites;
  private int seconds;
  private int mseconds;

  //ボーナス関連
  [SerializeField] Text BonusText = default;
  private int bonus = 0;

  //トータル関連
  [SerializeField] Text TotalText = default;
  public static int total = 0;

  // SE
  AudioSource audioSource;
  public AudioClip score_se;
  public AudioClip total_se;

  GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      audioSource = GetComponent<AudioSource>();
      StartCoroutine(Result());
    }

    // Update is called once per frame
    void Update()
    {

    }


    // リザルト処理
    IEnumerator Result()
    {
        yield return new WaitForEndOfFrame();

        //プレイヤーを停止させる
        gameManager.game_stop_flg = true;
        gameManager.pause_flg = false;

        yield return new WaitForSeconds(1.0f);

        score = PlayerMotion.GetScore();
        audioSource.PlayOneShot(score_se);
        ScoreText.text = score.ToString();

        yield return new WaitForSeconds(1.0f);

        countdown = TimeManager.GetTime();
        minites = Mathf.FloorToInt(countdown / 60F);
        seconds = Mathf.FloorToInt(countdown - minites * 60);
        mseconds = Mathf.FloorToInt((countdown - minites * 60 - seconds) * 100);
        audioSource.PlayOneShot(score_se);
        if (countdown == 0f){
          timeText.text = "Over";
        }
        else{
          timeText.text = string.Format("{0:00}:{1:00}:{2:00}", minites, seconds, mseconds);
        }
        bonus = Mathf.FloorToInt(countdown * 10);
        BonusText.text = "+" + bonus.ToString();
        // Instantiate(Audio_Go, transform.position, transform.rotation);

        yield return new WaitForSeconds(1.5f);

        total = score + bonus;
        TotalText.text = total.ToString();
        audioSource.PlayOneShot(total_se);

        yield return new WaitForSeconds(1.5f);
        // ランキング表示
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(total);

        yield return new WaitUntil(()=>naichilab.RankingSceneManager.GetRanking() == true);
        gameManager.GameOver();

    }
}
