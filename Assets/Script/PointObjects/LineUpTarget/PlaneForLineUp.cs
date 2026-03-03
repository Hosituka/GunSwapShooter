using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
//ジェネリックなクラスであるPlaneForLineUpの型引数を明示せずに保持したい場合に引用するクラス。
public abstract class PlanesForLineUp : MonoBehaviour
{
    public LineUpTarget LineUpTarget;
    public Collider[] Colliders;
    //#LineUpTargetクラスから呼ばれる
    //##初期化処理
    public abstract void Initialize(LineUpTarget lineUpTarget,Transform axisTr,float pitchRotation); 
    //##残りの的の個数を示す文字をセットする処理
    public abstract void SetNeedShotCountText(string text);
    //##LineUpTargetの時間切れによるフェードアウトと同期するために呼ばれる処理
    public abstract void TimeOver(float duration);
    //#Unityにより呼ばれる処理
    protected abstract void Update();
    //#具象クラス側により呼ばれる処理
    protected abstract void BreakCoroutine();

}
/*・LineUpTargetが持つ複数の板の抽象クラスです。
  ・またこれはジェネリッククラスなため、型変数を意識せずに保持したいのであれば、PlanesForLineUPを引用してください。*/
public abstract class PlaneForLineUp<T> : PlanesForLineUp,IPoolable<T>where T:PlaneForLineUp<T>
{
    protected float _dot;
    [SerializeField]bool _isShow = false;
    [SerializeField]Transform _lineUpTargetTr;
    [SerializeField]List<TMPandMeshRenderer> _showAndHideTarget;
    [SerializeField]TextMeshPro _needShotCountTMPro;
    [SerializeField]PointObjectAnimator _pointObjectAnimator;
    Action<T> _onRelease;
    public override void Initialize(LineUpTarget lineUpTarget,Transform axisTr,float pitchRotation)
    {
        LineUpTarget = lineUpTarget;
        _lineUpTargetTr = lineUpTarget.transform;
        //回転軸となるゲームオブジェクトを親として設定し、そこを中心に花びらのような感じで配置
        transform.SetParent(axisTr,false);
        transform.rotation = Quaternion.AngleAxis(pitchRotation, axisTr.up) * axisTr.rotation;
        transform.localPosition = new Vector3(0,0,0);

    }
    public override void SetNeedShotCountText(string text)
    {
        _needShotCountTMPro.SetText(text);
    }
    public override void TimeOver(float duration)
    {
        StartCoroutine(BaseTimeOver());
        IEnumerator BaseTimeOver()
        {
            _pointObjectAnimator.PlayTimeOverAnim(duration);
            yield return new WaitWhile(()=> _pointObjectAnimator.CurtTimeOverAnimPhase != PointObjectAnimator.TimeOverAnimPhase.Completed);
            _onRelease.Invoke((T)this);
        }
    }
    protected override void Update()
    {
        BaseUpdate();
        SubUpdate();
        void BaseUpdate()
        {
            _dot = Vector3.Dot(transform.right, _lineUpTargetTr.right);
            if (_dot > 0 + 0.001)
            {
                if (_isShow == true) return;
                _isShow = true;
                Utility.ChangeEnabledColliders(Colliders,true);
                Utility.ChangeEnabledTMPorMeshRenderers(_showAndHideTarget,true);
            }
            else
            {
                if (_isShow == false) return;
                _isShow = false;
                Utility.ChangeEnabledColliders(Colliders,false);
                Utility.ChangeEnabledTMPorMeshRenderers(_showAndHideTarget,false);
            }
        }
    }
    protected abstract void SubUpdate();

    protected override void BreakCoroutine()
    {
        StartCoroutine(BaseBreakCoroutine());
        StartCoroutine(SubBreakCoroutine());
        IEnumerator BaseBreakCoroutine()
        {
            LineUpTarget.NoticeDestruction(this);
            Utility.ChangeEnabledColliders(Colliders,false);
            _pointObjectAnimator.PlayFadeOut(0.05f);
            yield return new WaitWhile(()=> _pointObjectAnimator.CurtFadeOutPhase != PointObjectAnimator.FadeOutPhase.Completed);
            _onRelease.Invoke((T)this);

        }
    }
    abstract protected IEnumerator SubBreakCoroutine();



    public void OnCreate(Action<T> onRelease)
    {
        SubOnCreate();
        BaseOnCreate();
        void BaseOnCreate()
        {
            _onRelease = onRelease;
        }
    }
    protected abstract void SubOnCreate();
    public void OnRelease()
    {
        SubOnRelease();
        BaseOnRelease();
        void BaseOnRelease()
        {
            Utility.ChangeEnabledColliders(Colliders,true);
            _pointObjectAnimator.Reset();
            _isShow = true;
        }
    }
    protected abstract void SubOnRelease();
}
