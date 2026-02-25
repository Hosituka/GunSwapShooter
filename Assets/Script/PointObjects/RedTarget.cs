using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTarget : PointObject
{
    [Header("RedTargetの設定用プロパティ")]
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
    public override void TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(1);

        Utility.ChangeEnabledColliders(ColliderList,false);
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        _targetIndicator.Destroy();
        _targetPointObjectAnimator.PlayTimeOverAnim(animDuration);
    }
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if(_isDestruction == true) return;
        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        else if(collision.gameObject.CompareTag("RedBullet"))
        {
            _isDestruction = true;
            StageManager.Current.AddCombo();
            switch (TargetTimeKeeper.CurrentTaimingState)
            {
                case TimingState.GoodTiming:
                StageManager.Current.AddScore(0.4f,TimingState.GoodTiming);
                break;
                case TimingState.GreatTiming:
                StageManager.Current.AddScore(0.7f,TimingState.GreatTiming);
                break;
                case TimingState.PerfectTiming:
                StageManager.Current.AddScore(1f,TimingState.PerfectTiming);
                break;
            }
            StartCoroutine(BreakCoroutine());
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    protected override IEnumerator BreakCoroutine()
    {
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        TargetTimeKeeper.NoticeDestruction(this);
        _targetIndicator.Destroy();

        _targetPointObjectAnimator.PlaySpinThenExplode(transform.position,Color.red,12);
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Explosion);
        Utility.ChangeEnabledColliders(ColliderList,false); 
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        Destroy(gameObject);
    }
}
