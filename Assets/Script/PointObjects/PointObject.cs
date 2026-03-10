using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

//ジェネリックなクラスであるPointObjectの型変数を意識せずにフィールドとして持ちたいときに使うPointObjectの親クラスです。
public abstract class PointObjects : MonoBehaviour
{
    [Header("PointObjectの設定用プロパティ")]
    public List<Collider> ColliderList;
    public float PointObjectCost;
    //次のpointobjectを生成する時、同時に生成される個数
    public int NextGeneratableCount = 1;
    //このフィールドを持つインスタンスの有効化に必要な時間のoffset
    public float OffsetActivationDelay{get;protected set;}
    [Header("表示用")]
    public Vector2Int PointObjectPos;
    public Vector2 DebugPointObjectPos;
    public Vector2 PointObjectPosAsCenter;
    public Vector2 NormalizePointObjectPosAsCenter;
    public TimeKeeper TimeKeeper;
    public Indicator Indicator{get; set;}

    //#　インスペクターから見れん奴ら
    //## 音符
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
    public abstract void ActivateMain(float activateAnimDuration);
    public abstract void TimeOver(float animDuration);

    public abstract void PlayDeactivationTimer(float duration);
    public abstract void PlayActivationTimer(float duration);

    protected abstract void BreakAsync();
    public abstract void Release();

}
/*ここは全ての的に共通する振る舞いや状態を書く抽象クラスです。ジェネリックなフィールドを扱うために、これはジェネリッククラスです。
型変数が明示しない形で、これを保持したければ、PointObjectsでフィールドを定義してください*/
public abstract class PointObject<T> :PointObjects,IPoolable<T> where T: PointObject<T>
{
    [SerializeField]protected PointObjectAnimator _pointObjectAnimator;
    [SerializeField]GameObject _mainObj;
    //##オブジェクトプールに戻るためのアクション
    protected Action<T> _onRelease;
    public override InitializeResult Initialize()
    {
        BaseInitialize();
        return SubInitialize();
        void BaseInitialize()
        {
            _pointObjectAnimator.Initialize();
        }
    }
    protected abstract InitializeResult SubInitialize();
    public override void ActivateMain(float activateAnimDuration)
    {
        BaseActivateMain().Forget();
        async UniTaskVoid BaseActivateMain()
        {
            _mainObj.SetActive(true);
            this.enabled = true;
            PointObjectPosAsCenter = new Vector2(PointObjectPos.x - PointObjectsGenerator.Current.PointObjectMapLength.x / 2, PointObjectPos.y - PointObjectsGenerator.Current.PointObjectMapLength.y / 2);
            NormalizePointObjectPosAsCenter = new Vector2(PointObjectPosAsCenter.x / (PointObjectsGenerator.Current.PointObjectMapLength.x / 2), PointObjectPosAsCenter.y / (PointObjectsGenerator.Current.PointObjectMapLength.y / 2));
            await _pointObjectAnimator.PlayShowAnimOfMain(_mainObj.transform,activateAnimDuration);
            PointObjectsGenerator.Current.AddSumPointObjectCost(PointObjectCost);
        }
    }
    protected override void BreakAsync()
    {
        BaseBreakProcess();
        SubBreakAsync().Forget();

        void BaseBreakProcess()
        {
            PointObjectsGenerator.Current.RemovePointObject(PointObjectCost,PointObjectPos,2);
            TimeKeeper.NoticeUnLink(this);
            Indicator.Destroy();
        }
    }
    protected abstract UniTaskVoid SubBreakAsync();


    //時間制限内に射撃されなかったとき、TimeKeeperにより呼ばれる
    public override void TimeOver(float animDuration)
    {
        SubTimeOver(animDuration).Forget();
        BaseTimeOver(animDuration).Forget();

        async UniTaskVoid BaseTimeOver(float animDuration)
        {
            Utility.ChangeEnabledColliders(ColliderList,false);
            PointObjectsGenerator.Current.RemovePointObject(PointObjectCost,PointObjectPos,2);
            TimeKeeper.NoticeUnLink(this);
            Indicator.Destroy();
            await _pointObjectAnimator.PlayTimeOverAnim(animDuration);
            _onRelease.Invoke((T)this);
        }
        
    }

    protected abstract UniTaskVoid SubTimeOver(float animDuration);
    public override void PlayDeactivationTimer(float duration)
    {
        _pointObjectAnimator.PlayDeactivationTimer(duration).Forget();
        Indicator.PlayDeActivationTimer(duration).Forget();
    }
    public override void PlayActivationTimer(float duration)
    {
        _pointObjectAnimator.PlayActivationTimer(duration).Forget();
        Indicator.PlayActivationTimer(duration).Forget();
    }
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
    public void OnCreate(Action<T> onRelease)
    {        
        SubOnCreate();
        BaseOnCreate();
        void BaseOnCreate()
        {
            _pointObjectAnimator.Initialize();
            _onRelease = onRelease;
            _mainObj.SetActive(false); 
        }
    }

    protected abstract void SubOnCreate();
    public  void OnRelease()
    {
        SubOnRelease();
        BaseOnRelease();
        void BaseOnRelease()
        {
            this.enabled = false;
            _mainObj.SetActive(false);
            Utility.ChangeEnabledColliders(ColliderList, true);
            _lastCollisionGameObject = null;
            _lastProcessedFrame = -1;
        }

    }
    protected abstract void SubOnRelease();
    public override void Release()
    {
        _onRelease.Invoke((T)this);
    }





}
