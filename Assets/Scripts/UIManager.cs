using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerMotion player = default;
    // バッテリー関連
    [SerializeField] GameObject b_1 = default;
    [SerializeField] GameObject b_2 = default;
    [SerializeField] GameObject b_3 = default;
    [SerializeField] GameObject b_4 = default;
    [SerializeField] GameObject b_5 = default;

    //スコア関連
    private int score = 0;
    [SerializeField] public Text ScoreText = default;

    // エンジン関連
    [SerializeField] GameObject EngineImage = default;

    //タイム関連
    [SerializeField] Text timeText = default;
    private float countdown;
    private int minites;
    private int seconds;
    private int mseconds;

    // ゴミの個数
    [SerializeField] PaperManager pm = default;
    [SerializeField] Text paper1Text = default;
    [SerializeField] Text paper2Text = default;
    [SerializeField] Text paper3Text = default;
    [SerializeField] Text ballText = default;

    // ゲーム開始
    [SerializeField] float waitTime = 1.5f;
    [SerializeField] Text readyGoText = default;
    // [SerializeField]
    // GameObject Audio_Go = default;

    // ゲーム終了
    [SerializeField] Text finishText = default;
    [SerializeField] Text completeText = default;
    private bool isfinishflag = false;

    // SE
    AudioSource audioSource;
    public AudioClip go_se;
    public AudioClip finish_se;
    public AudioClip complete_se;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      audioSource = GetComponent<AudioSource>();

      StartCoroutine(ReadyGo());
    }

    // Update is called once per frame
    void Update()
    {
      // バッテリー関係
      switch(player.PowerLevel){
        case 0:
           b_1.gameObject.SetActive(false);
           b_2.gameObject.SetActive(false);
           b_3.gameObject.SetActive(false);
           b_4.gameObject.SetActive(false);
           b_5.gameObject.SetActive(false);
           break;
        case 1:
           b_1.gameObject.SetActive(true);
           b_2.gameObject.SetActive(false);
           b_3.gameObject.SetActive(false);
           b_4.gameObject.SetActive(false);
           b_5.gameObject.SetActive(false);
           break;
        case 2:
           b_1.gameObject.SetActive(true);
           b_2.gameObject.SetActive(true);
           b_3.gameObject.SetActive(false);
           b_4.gameObject.SetActive(false);
           b_5.gameObject.SetActive(false);
           break;
        case 3:
           b_1.gameObject.SetActive(true);
           b_2.gameObject.SetActive(true);
           b_3.gameObject.SetActive(true);
           b_4.gameObject.SetActive(false);
           b_5.gameObject.SetActive(false);
           break;
        case 4:
           b_1.gameObject.SetActive(true);
           b_2.gameObject.SetActive(true);
           b_3.gameObject.SetActive(true);
           b_4.gameObject.SetActive(true);
           b_5.gameObject.SetActive(false);
           break;
        case 5:
           b_1.gameObject.SetActive(true);
           b_2.gameObject.SetActive(true);
           b_3.gameObject.SetActive(true);
           b_4.gameObject.SetActive(true);
           b_5.gameObject.SetActive(true);
           break;
        default:
           break;
      }

      // スコア表示
      score = PlayerMotion.GetScore();
      ScoreText.text = "Score : " + score.ToString();

      // エンジン残量
      EngineImage.GetComponent<Image>().fillAmount = FillAmountSystem(player.EngineNow, player.EngineLimit);

      // タイム表示
      countdown = TimeManager.GetTime();
      minites = Mathf.FloorToInt(countdown / 60F);
      seconds = Mathf.FloorToInt(countdown - minites * 60);
      mseconds = Mathf.FloorToInt((countdown - minites * 60 - seconds) * 100);
      timeText.text = "Time : " + string.Format("{0:00}:{1:00}:{2:00}", minites, seconds, mseconds);
      if (countdown <= 10f){
        timeText.color = new Color(1.0f, 0f, 0f, 1.0f);
      }
      else {
        timeText.color = new Color(0f,0f,0f,1f);
      }

      //ゴミカウント
      paper1Text.text = pm.paper1_num.ToString();
      paper2Text.text = pm.paper2_num.ToString();
      paper3Text.text = pm.paper3_num.ToString();
      ballText.text = pm.ball_num.ToString();

      //Finish処理
      if (countdown <= 0f && isfinishflag == false){
        StartCoroutine(Finish());
        isfinishflag = true;
      }
      if (score >= 5000 && isfinishflag == false){
        StartCoroutine(Complete());
        isfinishflag = true;
      }
    }

    // 残量の可視化
    float FillAmountSystem (float now, float max) {
       return now / max;
    }

    // ゲーム開始処理
    IEnumerator ReadyGo()
    {
        yield return new WaitForEndOfFrame();

        //プレイヤーを停止させる
        gameManager.game_stop_flg = true;
        gameManager.pause_flg = false;

        yield return new WaitForSeconds(1.0f);

        readyGoText.text = "Ready?";

        yield return new WaitForSeconds(waitTime);

        readyGoText.text = "GO!!";
        // SE
        audioSource.PlayOneShot(go_se);
        // Instantiate(Audio_Go, transform.position, transform.rotation);

        //プレイヤーを移動可能にさせる
        gameManager.game_stop_flg = false;
        gameManager.pause_flg = true;


        yield return new WaitForSeconds(waitTime);

        readyGoText.enabled = false;

    }

    // ゲーム終了処理
    IEnumerator Finish()
    {
      yield return new WaitForEndOfFrame();

      //プレイヤーを停止させる
      gameManager.game_stop_flg = true;
      gameManager.pause_flg = false;

      finishText.text = "Finish!";
      // SE
      audioSource.PlayOneShot(finish_se);

      yield return new WaitForSeconds(3.0f);
      finishText.enabled = false;
      gameManager.MoveToStage(2);

    }

    IEnumerator Complete()
    {
      yield return new WaitForEndOfFrame();

      //プレイヤーを停止させる
      gameManager.game_stop_flg = true;
      gameManager.pause_flg = false;

      completeText.text = "Complete!";
      // SE
      audioSource.PlayOneShot(complete_se);

      yield return new WaitForSeconds(3.0f);
      completeText.enabled = false;
      gameManager.MoveToStage(2);

    }
}
