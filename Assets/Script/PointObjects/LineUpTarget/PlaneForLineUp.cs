using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
//LineUpTargetが持つ複数の板の抽象クラスです。
public abstract class PlaneForLineUp : MonoBehaviour
{
    public Collider[] Colliders;
    public PointObjectAnimator PointObjectAnimator;
    [SerializeField]protected List<TMPandMeshRenderer> _showAndHideTarget;
    [SerializeField]TextMeshPro _needShotCountTMPro;
    protected bool _isShow = true;
    protected float _dot;
    protected LineUpTarget _lineUpTarget;
    protected Transform _lineUpTargetTr;
    //#LineUpTargetクラスから呼ばれる
    //##具象クラスのupdateに必要な参照を取得する処理
    public void Initialize(LineUpTarget lineUpTarget)
    {
        _lineUpTarget = lineUpTarget;
        _lineUpTargetTr = lineUpTarget.transform;
    }
    //##トランスフォームやゲームオブジェクトの階層を決める処理
    public void SetTransform(Transform axisTr,float pitchRotation)
    {
        //回転軸となるゲームオブジェクトを親として設定し、そこを中心に花びらのような感じで配置
        transform.SetParent(axisTr,false);
        transform.rotation = Quaternion.AngleAxis(pitchRotation, axisTr.up) * axisTr.rotation;

    }
    //##残りの的の個数を示す文字をセットする処理
    public void SetNeedShotCountText(string text)
    {
        _needShotCountTMPro.SetText(text);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    abstract protected IEnumerator BreakCoroutine();

}
