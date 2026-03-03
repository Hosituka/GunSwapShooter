using UnityEngine;
using System.Collections;
public class BluePlaneForLineUp : PlaneForLineUp<BluePlaneForLineUp>
{
    
    protected override void SubUpdate()
    {
    }
    [SerializeField]int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if (collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        else if(collision.gameObject.CompareTag("BlueBullet"))
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
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
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
        _collisionCount = 0;
    }

}
