using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ここは全ての的に共通する振る舞いや状態を書く抽象クラスです。
public abstract class PointObject : MonoBehaviour
{
    [Header("PointObjectの設定用プロパティ")]
    public List<Collider> ColliderList;
    public float PointObjectCost;
    [SerializeField]protected GameObject _mainObj;
    //次のpointobjectを生成する時、同時に生成される個数
    public int NextGeneratableCount = 1;
    //このフィールドを持つインスタンスの有効化に必要な時間のoffset
    public float OffsetActivationDelay{get;protected set;}
    [SerializeField]protected PointObjectAnimator _pointObjectAnimator;
    [Header("表示用")]
    public Vector2 PointObjectPos;
    public Vector2 DebugPointObjectPos;
    public Vector2 PointObjectPosAsCenter;
    public Vector2 NormalizePointObjectPosAsCenter;
    public TimeKeeper TimeKeeper;
    public Indicator Indicator{private get; set;}

    //　インスペクターから見れん奴ら
    /// 音符
    public static float FourthNote;
    public static float EighthNote;
    public static float SixteenthNote;

    
    public struct InitializeResult
    {
        public float NextBaseActivationDelay;
        public float LifeTime;
        public float OffsetActivationDelay;
        public InitializeResult(float nextBaseActivationDelay,float lifeTime,float offsetActivationDelay)
        {
            NextBaseActivationDelay = nextBaseActivationDelay;
            LifeTime = lifeTime;
            OffsetActivationDelay = offsetActivationDelay;
        }
    }
    public abstract InitializeResult Initialize();
    //PointObjectのメインとなる部分を有効化する関数。
    public void ActivateMain(float activateAnimDuration)
    {
        StartCoroutine(BaseActivateMain());
        IEnumerator BaseActivateMain()
        {
            _mainObj.SetActive(true);
            this.enabled = true;
            _pointObjectAnimator.PlayShowAnimOfMain(_mainObj.transform,activateAnimDuration);
            PointObjectPosAsCenter = new Vector2(PointObjectPos.x - PointObjectGenerater.Current.PointObjectMapLength.x / 2, PointObjectPos.y - PointObjectGenerater.Current.PointObjectMapLength.y / 2);
            NormalizePointObjectPosAsCenter = new Vector2(PointObjectPosAsCenter.x / (PointObjectGenerater.Current.PointObjectMapLength.x / 2), PointObjectPosAsCenter.y / (PointObjectGenerater.Current.PointObjectMapLength.y / 2));
            yield return new WaitWhile(() =>_pointObjectAnimator.CurtShowAnimOfMainPhase != PointObjectAnimator.ShowAnimOfMainPhase.Completed);
            PointObjectGenerater.Current.AddSumPointObjectCost(PointObjectCost);
        }
    }

    //時間制限内に射撃されなかったとき、TimeKeeperにより呼ばれる
    public void PlayTimeOver(float animDuration)
    {
        StartCoroutine(BaseTimeOver());
        StartCoroutine(TimeOver(animDuration));

        IEnumerator BaseTimeOver()
        {
            Utility.ChangeEnabledColliders(ColliderList,false);
            PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
            PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
            Indicator.Destroy();
            yield break;
        }
        
    }

    protected abstract IEnumerator TimeOver(float animDuration);
    protected void StartBreakCoroutine()
    {
        BaseBreakProcess();
        StartCoroutine(BreakCoroutine());

        void BaseBreakProcess()
        {
            PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
            PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
            TimeKeeper.NoticeUnLink(this);
            Indicator.Destroy();
        }
    }
    abstract protected IEnumerator BreakCoroutine();
    //OnCreateはOnReleaseと同じく、ObjectPoolにより呼ばれる為、テンプレートメソッドパターンが出来ない、共通処理はここに書き、それをOnCreateから呼ぶ形とする。
    protected void BaseOnCreate()
    {
        _mainObj.SetActive(false); 
    }
    public void BaseOnRelease()
    {
        this.enabled = false;
        _mainObj.SetActive(false);
        _pointObjectAnimator.Reset();
        Utility.ChangeEnabledColliders(ColliderList, true);
    }


    public void PlayDeactivationTimer(float duration)
    {
        _pointObjectAnimator.PlayDeactivationTimer(duration);
        Indicator.PlayDeActivationTimer(duration);
    }
    public void PlayActivationTimer(float duration)
    {
        _pointObjectAnimator.PlayActivationTimer(duration);
        Indicator.PlayActivationTimer(duration);
    }


}
