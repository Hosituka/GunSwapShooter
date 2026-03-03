using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedPlane : MonoBehaviour
{
    [SerializeField]RedBlueTarget _redBlueTarget;

    [SerializeField]Collider[] _redColliderList;
    [SerializeField]PointObjectAnimator _pointObjectAnimator;
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if (collision.gameObject.CompareTag("BlueBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        if(collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddCombo();
            switch (_redBlueTarget.TimeKeeper.CurrentTaimingState)
            {
                case TimingState.GoodTiming:
                StageManager.Current.AddScore(0.5f,TimingState.GoodTiming);
                break;
                case TimingState.GreatTiming:
                StageManager.Current.AddScore(1,TimingState.GreatTiming);
                break;
                case TimingState.PerfectTiming:
                StageManager.Current.AddScore(1.5f,TimingState.PerfectTiming);
                break;
            }
            RedBreaking();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    void RedBreaking()
    {
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            Utility.ChangeEnabledColliders(_redColliderList,false);
            _redBlueTarget.DecrementCurrentPlaneCount();

            _pointObjectAnimator.PlayExplosion(transform.position,Color.red,14);
            yield return new WaitWhile(()=> _pointObjectAnimator.CurtExplosionPhase != PointObjectAnimator.ExplosionPhase.Completed);
            gameObject.SetActive(false);
        }
    }
    public void Reset()
    {
        _collisionCount = 0;
        Utility.ChangeEnabledColliders(_redColliderList,true);
        _pointObjectAnimator.Reset();
        gameObject.SetActive(true);
    }

}
