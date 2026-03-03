using System;
using System.Collections;
using UnityEngine;

public class BlueTarget : PointObject<BlueTarget>
{
    [Header("BlueTargetの設定用プロパティ")]
    [SerializeField]bool _isDestruction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
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
    protected override IEnumerator SubTimeOver(float duration)
    {
        StageManager.Current.AddOverlookCount(1);
        yield break;
    }

    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if(_isDestruction == true) return;
        if (collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        else if(collision.gameObject.CompareTag("BlueBullet"))
        {
            _isDestruction = true;
            StageManager.Current.AddCombo();
            switch (TimeKeeper.CurrentTaimingState)
            {
                case TimingState.GoodTiming:
                StageManager.Current.AddScore(0.4f,TimingState.GoodTiming);
                break;
                case TimingState.GreatTiming:
                StageManager.Current.AddScore(0.7f,TimingState.GreatTiming);
                break;
                case TimingState.PerfectTiming:
                StageManager.Current.AddScore(1,TimingState.PerfectTiming);
                break;
            }
            BreakCoroutine();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    protected override IEnumerator SubBreakCoroutine()
    {
        _pointObjectAnimator.PlaySpinThenExplode(transform.position,Color.blue,12);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Exploding);
        Utility.ChangeEnabledColliders(ColliderList,false); 
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        _onRelease.Invoke(this);
    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {
        _isDestruction = false;
    }
}
