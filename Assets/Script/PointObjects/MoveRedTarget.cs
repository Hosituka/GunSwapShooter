using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
public class MoveRedTarget : PointObject,IPoolable<MoveRedTarget>
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
    Action<MoveRedTarget> _onRelease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
        _playerTr = Player.Current.GetComponent<Transform>();
        _isVerticalToRotate = Random.Range(0, 2) == 1;
        _distanceForGenerate = PointObjectGenerater.Current.DistanceForGenerate;
        _generateYawStep = PointObjectGenerater.Current.GenerateYawStep;
        _generatePitchStep = PointObjectGenerater.Current.GeneratePitchStep;
        _startGenerateYaw = PointObjectGenerater.Current.GetYawPitch(transform.position).yaw;
        _startGeneratePitch = PointObjectGenerater.Current.GetYawPitch(transform.position).pitch;
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
    protected override IEnumerator TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(1);
        _pointObjectAnimator.PlayTimeOverAnim(animDuration);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtTimeOverAnimPhase != PointObjectAnimator.TimeOverAnimPhase.Completed);
        _onRelease.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeKeeper == null)return;
        if(TimeKeeper.CurrentTargetState != TimeKeeper.TargetState.ActivationCompleted) return;

        _pingPongTime += Time.deltaTime;
        if (_isVerticalToRotate)//ローカル座標系のy軸方向にRotateAroundさせる
        {
            _remapPingPong = Mathf.PingPong(_pingPongTime / _intervalForMove, 1) * 2 - 1; 
            transform.position = Quaternion.AngleAxis(_startGenerateYaw + _remapPingPong * _generateYawStep, Vector3.up) * Vector3.forward;
            transform.position = Quaternion.AngleAxis(_startGeneratePitch,Vector3.Cross(Vector3.up,transform.position)) * transform.position;

        }
        else//ローカル座標系のx軸方向にRotateAroundさせる。
        {
            _remapPingPong = Mathf.PingPong(_pingPongTime / _intervalForMove, 1) * 2 - 1; 
            transform.position = Quaternion.AngleAxis(_startGenerateYaw, Vector3.up) * Vector3.forward;
            transform.position = Quaternion.AngleAxis(_startGeneratePitch + _remapPingPong * _generatePitchStep, Vector3.Cross(Vector3.up,new Vector3(transform.position.x,0,transform.position.z))) * transform.position;
        }
        transform.position *= _distanceForGenerate - 3;
        _directionToPlayer = _playerTr.position - transform.position;
        transform.rotation = Quaternion.LookRotation(_directionToPlayer);

    }
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
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
            StartBreakCoroutine();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    protected override IEnumerator BreakCoroutine()
    {
        Utility.ChangeEnabledColliders(ColliderList,false);
        _pointObjectAnimator.PlaySpinAndFadeOut();
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinAndFadeOutPhase != PointObjectAnimator.SpinAndFadeOutPhase.Completed);
        _onRelease.Invoke(this);
    }
    public void OnCreate(Action<MoveRedTarget> onRelease)
    {
        BaseOnCreate();
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
       BaseOnRelease(); 
    }

}
