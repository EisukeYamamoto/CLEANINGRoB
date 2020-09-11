using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerMotion : MonoBehaviour
{
  //十字キーのみで操作(上下矢印キー＝前後，左右矢印キー＝回転)
  //CharacterControllerが必要

  public float speed = 6.0F;          //歩行速度
  public float jumpSpeed = 8.0F;      //ジャンプ力
  public float gravity = 20.0F;       //重力の大きさ
  public float rotateSpeed = 3.0F;    //回転速度
  public static int score;

  private Rigidbody _rigidBody;
  private Vector3 moveDirection = Vector3.zero;
  private float h,v;

  ///    ジャンプ入力フラグ
  ///    ジャンプ入力が一度でもあったらON、着地したらOFF
  private bool _jumpInput = false;

  ///    ジャンプ処理中フラグ
  ///    ジャンプ処理が開始されたらON、着地したらOFF
  private bool _isJumping = false;


  ///    接地してから何フレーム経過したか
  ///    接地してない間は常にゼロとする
  private int _isGround = 0;

  ///    接地してない間、何フレーム経過したか
  ///    接地している間は常にゼロとする
  private int _notGround = 0;

  ///    このフレーム数分接地していたらor接地していなかったら
  ///    状態が変わったと認識する（ジャンプ開始したor着地した）
  ///    接地してからキャラの状態が安定するまでに数フレーム用するため、
  ///    キャラが安定する前に再ジャンプ入力を受け付けてしまうとバグる（ジャンプ出来なくなる）
  ///    筆者PCでは 3 で安定するが、安全をとって今回は 5 とした
  private const int _isGroundStateChange = 5;

  ///    プレイヤーと地面の間の距離
  ///    IsGround()が呼ばれるたびに更新される
  [SerializeField] private float _groundDistance = 0f;
  // [SerializeField] private float _floorDistance = 0f;

  ///    _groundDistanceがこの値以下の場合接地していると判定する
  private const float _groundDistanceLimit = 0.2f;

  ///    判定元の原点が地面に極端に近いとrayがヒットしない場合があるので、
  ///    オフセットを設けて確実にヒットするようにする
  private Vector3 _raycastOffset  = new Vector3(0f, 0.05f, 0f);

  ///    プレイヤーキャラから下向きに地面判定のrayを飛ばす時の上限距離
  ///    ゲーム中でプレイヤーキャラと地面が最も離れると考えられる場面の距離に、
  ///    マージンを多少付けた値にしておくのが良
  ///    Mathf.Infinityを指定すれば無制限も可能だが重くなる可能性があるかも？
  private const float _raycastSearchDistance = 100f;

  // パワーアップレベル
  public int PowerLevel = 0;
  private float PowerUpWait = 1f;
  private float BatteryGetTime = 0f;
  private bool isBatteryGetFlag = false;

  // エンジン
  public bool isEngineGetFlag = false;
  public float HobaringForce = 1f;
  public float EngineLimit = 110f;
  public float EngineNow = 0f;
  private float EngineCost = 1f;
  private float EngineRecovery = 1f;

  // ワープゾーン
  [SerializeField] Transform Warp1 = default;
  [SerializeField] Transform Warp2 = default;
  public bool isWarpFlag = true;
  private float warptime = 0f;
  private float warplimit = 40f;

  [SerializeField] PaperManager pm = default;

  //SE
  AudioSource audioSource;
  public AudioClip paper_se;
  public AudioClip battery_se;
  public AudioClip Engine_se;
  public AudioClip Warp_se;

  GameManager gamemanager;

  // Use this for initialization
  void Start () {
    _rigidBody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
    score = 0;
    PowerLevel = 0;
    isEngineGetFlag = false;
    EngineNow = EngineLimit;

    gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
  }//Start()

  // Update is called once per frame
  void Update () {
    if (gamemanager.game_stop_flg == false){
      h = Input.GetAxis ("Horizontal");    //左右矢印キーの値(-1.0~1.0)
      v = Input.GetAxis ("Vertical");      //上下矢印キーの値(-1.0~1.0)

      CheckGroundDistance(() => {
        _jumpInput = false;
        _isJumping = false;
        });

        // 既にジャンプ入力が行われていたら、ジャンプ入力チェックを飛ばす
        if (isEngineGetFlag == false){
          if (_jumpInput || JumpInput()) _jumpInput = true;
        }
        else{
          if( JumpInput() ) _jumpInput = true;
          else _jumpInput = false;
        }

        if (isBatteryGetFlag){
          BatteryGetTime += 0.1f;
          if (BatteryGetTime >= PowerUpWait){
            isBatteryGetFlag = false;
            BatteryGetTime = 0f;
          }
        }

        if (isEngineGetFlag == false){
          EngineNow = 0f;
        }

        if (isWarpFlag == false){
          warptime += 0.1f;
          if (warptime >= warplimit){
            isWarpFlag = true;
            warptime = 0f;
          }
        }
        // Debug.Log(_jumpInput);
        // Debug.Log(EngineNow);
      }
  }//Update()

  void FixedUpdate() {
    if (gamemanager.game_stop_flg == false){
      transform.Rotate (new Vector3 (0, rotateSpeed * h, 0));
      moveDirection = speed * v * gameObject.transform.forward;

      if (_jumpInput) {
        if (isEngineGetFlag == false){
          if (!_isJumping) {
            _isJumping = true;
            DoJump();
          }
        }
        else{
          if (EngineNow > 0){
            _isJumping = true;
            DoHobaring();
          }
        }
      }
      transform.position += moveDirection * Time.deltaTime;
    }
  }

  ///    ジャンプ入力チェック
  private bool JumpInput()
  {
         // ジャンプ最速入力のテスト用にGetButton
         //if (Input.GetButton("Jump")) return true;    // ジャンプキー押しっぱなしで連続ジャンプ
         //if (Input.GetButtonDown("Jump")) return true;    // ジャンプキーが押された時だけジャンプにする時はこっち
         if (isEngineGetFlag == false){
           if (Input.GetKeyDown(KeyCode.Space)) {
             //Debug.Log("jump");
             return true;
           }
           return false;
         }
         else{
           if (Input.GetKey(KeyCode.Space)) {
             //Debug.Log("jump");
             if(EngineNow > 0){
               EngineNow -= EngineCost;
             }
             else{
               EngineNow = 0;
             }
             return true;
           }
           return false;
         }
  }

  ///    ジャンプのための上方向への加圧
  private void DoJump()
  {
         _rigidBody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
  }

  ///    ホバリングのための上方向への加圧
  private void DoHobaring()
  {
         _rigidBody.AddForce(Vector3.up * HobaringForce, ForceMode.Impulse);
  }

  ///    接地判定
  private void CheckGroundDistance(UnityAction landingAction = null, UnityAction takeOffAction = null)
  {
         RaycastHit hit;
         var layerMask = LayerMask.GetMask("Ground");

         // プレイヤーの位置から下向きにRaycast
         // レイヤーマスクでGroundを設定しているので、
         // 地面のGameObjectにGroundのレイヤーを設定しておけば、
         // Groundのレイヤーを持つGameObjectで一番近いものが一つだけヒットする
         var isGroundHit = Physics.Raycast(
                           transform.position + _raycastOffset,
                           transform.TransformDirection(Vector3.down),
                           out hit,
                           _raycastSearchDistance,
                           layerMask
                           );

         if (isGroundHit) {
              _groundDistance = hit.distance;
         } else {
              // ヒットしなかった場合はキャラの下方に地面が存在しないものとして扱う
              _groundDistance = float.MaxValue;
         }
         // Debug.Log(_groundDistance);

         // 地面とキャラの距離は環境によって様々で
         // 完全にゼロにはならない時もあるため、
         // ジャンプしていない時の値に多少のマージンをのせた
         // 一定値以下を接地と判定する
         // 通常あり得ないと思われるが、オーバーフローされると再度アクションが実行されてしまうので、越えたところで止める
         if (_groundDistance < _groundDistanceLimit) {
              if (_isGround <= _isGroundStateChange) {
                   _isGround += 1;
                   _notGround = 0;
              }
              if (EngineNow < EngineLimit){
                EngineNow += EngineRecovery;
              }
              else{
                EngineNow = EngineLimit;
              }

         } else {
              if (_notGround <= _isGroundStateChange) {
                   _isGround = 0;
                   _notGround += 1;

              }
         }

         // 接地後またはジャンプ後、特定フレーム分状態の変化が無ければ、
         // 状態が安定したものとして接地処理またはジャンプ処理を行う
         if (_isGroundStateChange == _isGround && _notGround == 0) {
              if (landingAction != null){
                   landingAction();

                   //Debug.Log("landing");
              }
         } else {
              if (_isGroundStateChange == _notGround && _isGround == 0) {
                   if (takeOffAction != null){
                        takeOffAction();
                        //Debug.Log("takeOFF");
                   }
              }
         }
    }

    void OnCollisionEnter(Collision collision) {
      if (gamemanager.game_stop_flg == false){
        // Debug.Log(collision.gameObject.tag);
        switch(collision.gameObject.tag){
          case "Battery":
             if (isBatteryGetFlag == false){
               isBatteryGetFlag = true;
               Destroy(collision.gameObject);
               audioSource.PlayOneShot(battery_se);
               if (PowerLevel < 5){
                 PowerLevel += 1;
               }
             }
             break;
          case "Engine":
             isEngineGetFlag = true;
             EngineNow = EngineLimit;
             Destroy(collision.gameObject);
             audioSource.PlayOneShot(Engine_se);
             break;
          case "Paper_1":
             score += 10;
             Destroy(collision.gameObject);
             audioSource.PlayOneShot(paper_se);
             pm.paper1_num -= 1;
             break;
          case "Paper_2":
             score += 20;
             Destroy(collision.gameObject);
             audioSource.PlayOneShot(paper_se);
             pm.paper2_num -= 1;
             break;
          case "Paper_3":
             score += 30;
             Destroy(collision.gameObject);
             audioSource.PlayOneShot(paper_se);
             pm.paper3_num -= 1;
             break;
          case "Ball":
             score += 100;
             Destroy(collision.gameObject);
             audioSource.PlayOneShot(paper_se);
             pm.ball_num -= 1;
             break;
          case "Warp":
             if (collision.gameObject.name == "Warp1"){
               if (isWarpFlag){
                 audioSource.PlayOneShot(Warp_se);
                 this.transform.position = Warp2.transform.position;
                 this.transform.rotation = Quaternion.Euler(0, 180, 0);
                 //_rigidBody.AddForce(this.transform.forward * 10f);
                 isWarpFlag = false;
               }
             }
             if (collision.gameObject.name == "Warp2"){
               if (isWarpFlag){
                 audioSource.PlayOneShot(Warp_se);
                 this.transform.position = Warp1.transform.position;
                 this.transform.rotation = Quaternion.Euler(0, -90f, 0);
                 //_rigidBody.AddForce(this.transform.forward * 10f);
                 isWarpFlag = false;
               }
             }
             break;
          default :
             break;
        }
      }
    }

    public static int GetScore(){
      return score;
    }
}
