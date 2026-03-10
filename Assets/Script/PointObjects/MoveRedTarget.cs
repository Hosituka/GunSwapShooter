using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
public class MoveRedTarget : PointObject<MoveRedTarget>
{
    [Header("MoveRedTargetの設定用プロパティ")]

    [SerializeField]float _intervalForMove = 1;

    Transform _playerTr;
    bool _isVerticalToRotate;
    Vector3 _directionToPlayer;
    float _distanceForGenerate;
    float _remapPingPong;
    int _generateYawStep;
    int _generatePitchStep;
    float _startGenerateYaw;
    float _startGeneratePitch;
    float _pingPongTime;
    protected override InitializeResult SubInitialize()
    {
        _playerTr = Player.Current.GetComponent<Transform>();
        _isVerticalToRotate = Random.Range(0, 2) == 1;
        _distanceForGenerate = PointObjectsGenerator.Current.DistanceOfGenerate;
        _generateYawStep = PointObjectsGenerator.Current.GenerateYawStep;
        _generatePitchStep = PointObjectsGenerator.Current.GeneratePitchStep;
        _startGenerateYaw = PointObjectsGenerator.Current.GetYawPitch(transform.position).yaw;
        _startGeneratePitch = PointObjectsGenerator.Current.GetYawPitch(transform.position).pitch;
        if (_isVerticalToRotate)
        {
            _pingPongTime = 1 * _intervalForMove / 2; 
        }
        else
        {
            _pingPongTime = 1 * _intervalForMove / 2;
        }
        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            case GameManager.Difficult.normal:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            case GameManager.Difficult.hard:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            default:
                Debug.LogError("未対応の難易度が選択されています。");
            return new InitializeResult();
             
        }

    }
    protected override async UniTaskVoid SubTimeOver(float duration)
    {
        StageManager.Current.AddOverlookCount(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeKeeper == null) return;
        if(TimeKeeper.CurrentTargetState != TimeKeeper.TargetState.ActivationCompleted) return;

        _pingPongTime += Time.deltaTime;
        if (_isVerticalToRotate)//ローカル座標系のy軸方向にRotateAroundさせる
        {
            _remapPingPong = Mathf.PingPong(_pingPongTime / _intervalForMove, 1) - 0.5f; 
            transform.position = Quaternion.AngleAxis(_startGenerateYaw + _remapPingPong * _generateYawStep, Vector3.up) * Vector3.forward;
            transform.position = Quaternion.AngleAxis(_startGeneratePitch,Vector3.Cross(Vector3.up,transform.position)) * transform.position;

        }
        else//ローカル座標系のx軸方向にRotateAroundさせる。
        {
            _remapPingPong = Mathf.PingPong(_pingPongTime / _intervalForMove, 1) - 0.5f; 
            transform.position = Quaternion.AngleAxis(_startGenerateYaw, Vector3.up) * Vector3.forward;
            transform.position = Quaternion.AngleAxis(_startGeneratePitch + _remapPingPong * _generatePitchStep, Vector3.Cross(Vector3.up,new Vector3(transform.position.x,0,transform.position.z))) * transform.position;
        }
        transform.position *= _distanceForGenerate;
        _directionToPlayer = _playerTr.position - transform.position;
        transform.rotation = Quaternion.LookRotation(_directionToPlayer);

    }
    protected override void OnValidCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        else if(collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddCombo();
            switch (TimeKeeper.CurrentTaimingState)
            {
                case TimingState.GoodTiming:
                StageManager.Current.AddScore(0.5f,TimingState.GoodTiming);
                break;
                case TimingState.GreatTiming:
                StageManager.Current.AddScore(1,TimingState.GreatTiming);
                break;
                case TimingState.PerfectTiming:
                StageManager.Current.AddScore(1.5f,TimingState.PerfectTiming);
                break;
            }
            BreakAsync();
        }
    }
    protected override async UniTaskVoid SubBreakAsync()
    {
        Utility.ChangeEnabledColliders(ColliderList,false);
        await _pointObjectAnimator.PlaySpinAndFadeOut();
        _onRelease.Invoke(this);
    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {
    }

}
