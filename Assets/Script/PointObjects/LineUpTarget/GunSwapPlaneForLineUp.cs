using UnityEngine;
using System.Collections;

public class GunSwapPlaneForLineUp : PlaneForLineUp<GunSwapPlaneForLineUp>,IHitGunSwapRayHandler
{
    protected override void SubUpdate()
    {

    }
    protected override void OnValidCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BlueBullet") || collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
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
        BreakCoroutine();
    }
    protected override IEnumerator SubBreakCoroutine()
    {
        yield break;
    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {
    }


}
