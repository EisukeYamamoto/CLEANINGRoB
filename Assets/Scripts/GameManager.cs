/*
ゲーム全体の処理を行うスクリプト
ステージ遷移やゲームオーバー処理を行っている
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
  [System.NonSerialized]
  public int currentStageNum = 0; //現在のステージ番号（0始まり）

  [SerializeField]
  GameObject fadeCanvasPrefab = default;
  [SerializeField]
  GameObject gameOverCanvasPrefab = default;
  [SerializeField]
  GameObject PauseCanvasPrefab = default;
  [SerializeField]
  float fadeWaitTime = 1.0f; //フェード時の待ち時間

  GameObject fadeCanvasClone;
  FadeManager fadeCanvas;
  GameObject gameOverCanvasClone;
  GameObject gameClearCanvasClone;
  GameObject PauseCanvasClone;
  PlayerMotion playermotion;
  Rigidbody playerRigidbody;

  // SE
  AudioSource audioSource;
  public AudioClip start_se;
  public AudioClip positive_se;
  public AudioClip negative_se;
  public AudioClip pause_se;

  Button[] buttons;

  public bool game_stop_flg = false;
  public bool pause_flg;


  //最初の処理
  void Start ()
  {
    //シーンを切り替えてもこのゲームオブジェクトを削除しないようにする
    DontDestroyOnLoad(gameObject);
    game_stop_flg = false;
    pause_flg = false;

    audioSource = GetComponent<AudioSource>();

    //デリゲートの登録
    SceneManager.sceneLoaded += OnSceneLoaded;

  }

  //シーンのロード時に実行（最初は実行されない）
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      //改めて取得
      LoadComponents();
    }

    //コンポーネントの取得
    void LoadComponents()
    {
      //タイトル画面じゃないなら取得
      if(SceneManager.GetActiveScene().name == "MainScene")
      {
        playermotion = GameObject.Find("Player").GetComponent<PlayerMotion>();
        playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();

        game_stop_flg = true;
        pause_flg = false;
      }
    }

  //毎フレームの処理
  void Update ()
  {
    if(Input.GetKeyDown(KeyCode.P)){
      if(pause_flg == true){
        Pause();
      }
    }
  }

  //次のステージに進む処理
  public void NextStage()
  {
    currentStageNum += 1;

    // SE
    audioSource.PlayOneShot(start_se);

    //コルーチンを実行
    StartCoroutine(WaitForLoadScene(currentStageNum));
  }

  //任意のステージに移動する処理
  public void MoveToStage(int stageNum)
  {
    //コルーチンを実行
    StartCoroutine(WaitForLoadScene(stageNum));
  }

  //シーンの読み込みと待機を行うコルーチン
  IEnumerator WaitForLoadScene(int stageNum)
  {
    //フェードオブジェクトを生成
    fadeCanvasClone = Instantiate(fadeCanvasPrefab);

    //コンポーネントを取得
    fadeCanvas = fadeCanvasClone.GetComponent<FadeManager>();

    //フェードインさせる
    fadeCanvas.fadeIn = true;

    yield return new WaitForSeconds(fadeWaitTime);

    //シーンを非同期で読込し、読み込まれるまで待機する
    yield return SceneManager.LoadSceneAsync(stageNum);

    // //フェードアウトさせる
    fadeCanvas.fadeOut = true;

  }

  //ゲームオーバー処理
  public void GameOver()
  {
    //キャラやカメラの移動を停止させる
    game_stop_flg = true;
    pause_flg = false;

    //ゲームオーバー画面表示
    gameOverCanvasClone = Instantiate(gameOverCanvasPrefab);

    // ボタンを取得
    buttons = gameOverCanvasClone.GetComponentsInChildren<Button>();

    // ボタンにイベント設定
    buttons[0].onClick.AddListener(Retry);
    buttons[1].onClick.AddListener(Return);

  }

  //ゲームオーバー処理
  public void Pause()
  {
    //キャラやカメラの移動を停止させる
    playerRigidbody.isKinematic = true;
    game_stop_flg = true;
    pause_flg = false;

    //ゲームオーバー画面表示
    PauseCanvasClone = Instantiate(PauseCanvasPrefab);

    // SE
    audioSource.PlayOneShot(pause_se);

    //ボタンを取得
    buttons = PauseCanvasClone.GetComponentsInChildren<Button>();

    //ボタンにイベント設定
    buttons[0].onClick.AddListener(Retry_Pause);
    buttons[1].onClick.AddListener(Restart);
    buttons[2].onClick.AddListener(Return_Pause);

  }

  //リトライ
    public void Retry()
    {
      Destroy(gameOverCanvasClone);

      // SE
      audioSource.PlayOneShot(positive_se);

      MoveToStage(1);
    }

    //最初のシーンに戻る
    public void Return()
    {
      Destroy(gameOverCanvasClone);

      // SE
      audioSource.PlayOneShot(negative_se);

      MoveToStage(0);
    }

    // リスタート
    public void Restart()
    {
      Destroy(PauseCanvasClone);
      // SE
      audioSource.PlayOneShot(positive_se);
      MoveToStage(1);
    }

    //リトライ
    public void Retry_Pause()
    {
      Destroy(PauseCanvasClone);

      // SE
      audioSource.PlayOneShot(positive_se);
      playerRigidbody.isKinematic = false;
      game_stop_flg = false;
      pause_flg = true;
    }

    //最初のシーンに戻る
    public void Return_Pause()
    {
      Destroy(PauseCanvasClone);

      // SE
      audioSource.PlayOneShot(negative_se);

      MoveToStage(0);
    }

    //ゲーム終了
    public void ExitGame()
    {
      // SE
      audioSource.PlayOneShot(negative_se);
      Application.Quit();
    }
}
