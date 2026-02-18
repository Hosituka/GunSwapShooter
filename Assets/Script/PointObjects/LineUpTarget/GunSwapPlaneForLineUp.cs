using UnityEngine;
using System.Collections;

public class GunSwapPlaneForLineUp : PlaneForLineUp,IHitGunSwapRayHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        _dot = Vector3.Dot(transform.right, _lineUpTargetTr.right);
        if (_dot > 0 + 0.001)
        {
            if (_isShow == true) return;
            _isShow = true;
            Utility.ChangeEnabledColliders(Colliders,true);
            Utility.ChangeEnabledTMPorMeshRenderers(FadeTargets,true);
        }
        else
        {
            if (_isShow == false) return;
            _isShow = false;
            Utility.ChangeEnabledColliders(Colliders,false);
            Utility.ChangeEnabledTMPorMeshRenderers(FadeTargets,false);
        }

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
        StartCoroutine(BreakCoroutine());
    }
    protected override IEnumerator BreakCoroutine()
    {
        _lineUpTarget.NoticeDestruction(this);
        Utility.ChangeEnabledColliders(Colliders,false);
        _targetBreakAnimator.PlayFadeOut(FadeTargets,0.05f);
        yield return new WaitWhile(()=> _targetBreakAnimator.CurtFadeOutPhase != BreakAnimator.FadeOutPhase.Completed);
        Destroy(gameObject);
    }


}
