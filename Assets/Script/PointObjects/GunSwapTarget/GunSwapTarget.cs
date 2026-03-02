using System;
using System.Collections;
using UnityEngine;

public class GunSwapTarget : PointObject,IHitGunSwapRayHandler,IPoolable<GunSwapTarget>
{
    [Header("GunSwapTargetの設定用プロパティ")]
    [SerializeField]int temp;
    Action<GunSwapTarget> _onRelease;
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
    protected override IEnumerator TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(1);
        _pointObjectAnimator.PlayTimeOverAnim(animDuration);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtTimeOverAnimPhase != PointObjectAnimator.TimeOverAnimPhase.Completed);
        _onRelease.Invoke(this);

    }
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if (collision.gameObject.CompareTag("BlueBullet") || collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }

    public void OnHitGunSwapRay()
    {
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
        StartBreakCoroutine();
    }
    protected override IEnumerator BreakCoroutine()
    {
        Utility.ChangeEnabledColliders(ColliderList,false);
        _pointObjectAnimator.PlayExplosion(transform.position,Color.gray,12);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtExplosionPhase != PointObjectAnimator.ExplosionPhase.Completed);
        _onRelease.Invoke(this);
    }
    public void OnCreate(Action<GunSwapTarget> onRelease)
    {
        BaseOnCreate();
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        BaseOnRelease();
    }

}
