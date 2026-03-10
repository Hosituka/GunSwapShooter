using System.Collections;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class RedTarget : PointObject<RedTarget>
{
    [Header("RedTargetの設定用プロパティ")]
    [SerializeField]bool _isDestruction;
    protected override InitializeResult SubInitialize()
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
    protected override async UniTaskVoid SubTimeOver(float duration)
    {
        StageManager.Current.AddOverlookCount(1);
    }
    protected override void OnValidCollisionEnter(Collision collision)
    {
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
            switch (TimeKeeper.CurrentTaimingState)
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
            BreakAsync();
        }
    }
    protected override async UniTaskVoid SubBreakAsync()
    {
        Utility.ChangeEnabledColliders(ColliderList,false); 
        await _pointObjectAnimator.PlaySpinThenExplode(transform.position,Color.red,12);
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
