using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

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
        {StageManager.Current.AddScore(3f,TimingState.PerfectTiming);}
        else if(_dot >= 0.8f)
        {StageManager.Current.AddScore(2.1f,TimingState.GreatTiming);}
        else
        {StageManager.Current.AddScore(1.2f,TimingState.GoodTiming);}
        Break();
    }
    protected override async UniTaskVoid SubBreakAsync()
    {
    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {
    }


}
