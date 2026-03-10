using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
//ジェネリックなクラスであるPlaneForLineUpの型引数を明示せずに保持したい場合に引用するクラス。
public abstract class PlanesForLineUp : MonoBehaviour
{
    public LineUpTarget LineUpTarget;
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
    protected abstract void Break();

}
/*・LineUpTargetが持つ複数の板の抽象クラスです。
  ・またこれはジェネリッククラスなため、型変数を意識せずに保持したいのであれば、PlanesForLineUPを引用してください。*/
public abstract class PlaneForLineUp<T> : PlanesForLineUp,IPoolable<T>where T:PlaneForLineUp<T>
{
    protected float _dot;
    [SerializeField]bool _isShow = false;
    [SerializeField]Transform _lineUpTargetTr;
    [SerializeField]Collider[] _colliders;
    [SerializeField]List<TMPandMeshRenderer> _showAndHideTarget;
    [SerializeField]TextMeshPro _needShotCountTMPro;
    [SerializeField]PointObjectAnimator _pointObjectAnimator;
    Action<T> _onRelease;
    public override void Initialize(LineUpTarget lineUpTarget,Transform axisTr,float pitchRotation)
    {
        _pointObjectAnimator.Initialize();
        LineUpTarget = lineUpTarget;
        _lineUpTargetTr = lineUpTarget.transform;
        //#回転軸となるゲームオブジェクトを親として設定し、そこを中心に花びらのような感じで配置
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
        BaseTimeOver().Forget();
        async UniTaskVoid BaseTimeOver()
        {
            Utility.ChangeEnabledColliders(_colliders,false);
            await _pointObjectAnimator.PlayTimeOverAnim(duration);
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
                Utility.ChangeEnabledColliders(_colliders,true);
                Utility.ChangeEnabledTMPorMeshRenderers(_showAndHideTarget,true);
            }
            else
            {
                if (_isShow == false) return;
                _isShow = false;
                Utility.ChangeEnabledColliders(_colliders,false);
                Utility.ChangeEnabledTMPorMeshRenderers(_showAndHideTarget,false);
            }
        }
    }
    protected abstract void SubUpdate();

    int _lastProcessedFrame = -1;
    GameObject _currentCollisionGameObject;
    GameObject _lastCollisionGameObject;
    void OnCollisionEnter(Collision collision)
    {
        _currentCollisionGameObject = Utility.GetCollisionGameObject(collision);
        if(_lastProcessedFrame == Time.frameCount && _currentCollisionGameObject == _lastCollisionGameObject) return;
        _lastProcessedFrame = Time.frameCount;
        _lastCollisionGameObject = _currentCollisionGameObject;
        OnValidCollisionEnter(collision);
    }
    //#複合コライダーによる複数のOnCollisionEnter発火を疑似的に一回だけ呼ばれてるように見せる奴、
    protected abstract void OnValidCollisionEnter(Collision collision);

    protected override void Break()
    {
        BaseBreakAsync().Forget();
        SubBreakAsync().Forget();
        async UniTaskVoid BaseBreakAsync()
        {
            LineUpTarget.NoticeDestruction(this);
            Utility.ChangeEnabledColliders(_colliders,false);
            await _pointObjectAnimator.PlayFadeOut(0.05f);
            _onRelease.Invoke((T)this);

        }
    }
    abstract protected UniTaskVoid SubBreakAsync();



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
            Utility.ChangeEnabledColliders(_colliders,true);
            _isShow = true;
        }
    }
    protected abstract void SubOnRelease();
}
