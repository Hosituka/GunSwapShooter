using UnityEngine;
using System.Collections;

public class GunSwapPlaneForLineUp : PlaneForLineUp,IHitGunSwapRayHandler
{
    protected override void OnUpdate()
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
        if(_dot >= 0.95f)
        {StageManager.Current.AddScore(2f,TimingState.PerfectTiming);}
        else if(_dot >= 0.8f)
        {StageManager.Current.AddScore(1.4f,TimingState.GreatTiming);}
        else
        {StageManager.Current.AddScore(0.8f,TimingState.GoodTiming);}
        StartBreakCoroutine();
    }
    protected override IEnumerator BreakCoroutine()
    {
        PointObjectAnimator.PlayFadeOut(0.05f);
        yield return new WaitWhile(()=> PointObjectAnimator.CurtFadeOutPhase != PointObjectAnimator.FadeOutPhase.Completed);
        gameObject.SetActive(false);
    }


}
