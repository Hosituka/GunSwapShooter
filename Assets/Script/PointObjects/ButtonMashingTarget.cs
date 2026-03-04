using UnityEngine;
using System.Collections;
using TMPro;
using System;
using Random = UnityEngine.Random;
public class ButtonMashingTarget : PointObject<ButtonMashingTarget>
{
    [Header("ButtonMashingTargetの設定用プロパティ")]
    [SerializeField] int _maxHp = 7;
    [SerializeField] int _minHp = 4;
    [SerializeField] int _hp;
    [SerializeField] TextMeshPro _needShotCountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
        _hp = Random.Range(_minHp, _maxHp);
        _needShotCountText.SetText(_hp.ToString());
        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
            return new InitializeResult(
                        SixteenthNote * _hp + EighthNote,
                        SixteenthNote * _hp * 4,
                        0
                    );
            case GameManager.Difficult.normal:
            return new InitializeResult(
                        SixteenthNote * _hp + EighthNote,
                        SixteenthNote * _hp * 4,
                        0
                    );
            case GameManager.Difficult.hard:
            return new InitializeResult(
                        SixteenthNote * _hp + EighthNote,
                        SixteenthNote * _hp * 4,
                        0
                    );
            default:
                Debug.LogError("未対応の難易度が選択されています。");
            return new InitializeResult();
             
        }
    }
    protected override IEnumerator SubTimeOver(float duration)
    {
        StageManager.Current.AddOverlookCount(_hp);
        yield break;
    }
    protected override void OnValidCollisionEnter(Collision collision)
    {
        Debug.Log("test2");
        if(_hp <= 0) return;
        if (collision.gameObject.CompareTag("BlueBullet") || collision.gameObject.CompareTag("RedBullet"))
        {
            _hp--;
            _needShotCountText.SetText(_hp.ToString());
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

            if(_hp == 0)
            {
                BreakCoroutine();
            }
        }
    }
    protected override IEnumerator SubBreakCoroutine()
    {        
        _pointObjectAnimator.PlaySpinThenExplode(transform.position,Color.magenta,17);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Exploding);
        Utility.ChangeEnabledColliders(ColliderList,false);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        Debug.Log("test");
        _onRelease.Invoke(this);
    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {  
        _hp = 0;
    }

}
