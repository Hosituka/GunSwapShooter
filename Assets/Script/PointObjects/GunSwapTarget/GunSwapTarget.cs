using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwapTarget : PointObject,IHitGunSwapRayHandler
{
    [Header("GunSwapTargetの設定用プロパティ")]
    [SerializeField]int temp;
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

    // Update is called once per frame
    void Update()
    {

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
    protected override IEnumerator BreakCoroutine()
    {
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        TargetTimeKeeper.NoticeDestruction(this);
        _targetIndicator.Destroy();

        Utility.ChangeEnabledColliders(ColliderList,false);
        _targetPointObjectAnimator.PlayExplosion(transform.position,Color.gray,12);
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtExplosionPhase != PointObjectAnimator.ExplosionPhase.Completed);
        Destroy(gameObject);
    }

}
