using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
public class BluePlane : MonoBehaviour
{
    [SerializeField]RedBlueTarget _redBlueTarget;
    [SerializeField]Collider[] _blueColliderList;
    [SerializeField]PointObjectAnimator _pointObjectAnimator;
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if (collision.gameObject.CompareTag("RedBullet"))
        {
            StageManager.Current.AddAccidentalShoot(1);
            StageManager.Current.ResetCombo();
        }
        if(collision.gameObject.CompareTag("BlueBullet"))
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
            BlueBreaking().Forget();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    async UniTaskVoid BlueBreaking()
    {
        Utility.ChangeEnabledColliders(_blueColliderList,false);
        _redBlueTarget.DecrementCurrentPlaneCount();
        await _pointObjectAnimator.PlayExplosion(transform.position,Color.blue,14);
        gameObject.SetActive(false);
    }
    public void Reset()
    {
        _collisionCount = 0;
        Utility.ChangeEnabledColliders(_blueColliderList,true);
        _pointObjectAnimator.Initialize();
        gameObject.SetActive(true);
    }
}
