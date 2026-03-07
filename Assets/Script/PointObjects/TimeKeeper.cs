using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Drawing;
using UnityEngine.Pool;

//ある時刻におけるpointObject群のライフサイクルを管理するクラスである。pointObjectGeneraterとの連携も行っている。
public class TimeKeeper : MonoBehaviour,IPoolable<TimeKeeper>
{
    //またPointObjectGeneratorのインスタンスとの連携も担っています。
    [Header("インスペクター設定用")]
    public List<PointObjects> PointObjectsList;
    [SerializeField,Range(0,1)] float _activateAnimRate = 0.2f;
    [SerializeField]float _deactivateAnimDuration = 0.2f;
    
    [SerializeField] float _perlinNoiseMagni = 1;

    [Header("表示用")]
    //PointObjectGeneraterにより設定される基準となる有効化の遅延時間
    public float BaseActivationDelay;
    public TargetState CurrentTargetState = TargetState.Preparing;
    public TimingState CurrentTaimingState = TimingState.GoodTiming;
    [SerializeField]float _sumLifeTime;
    [SerializeField]float _sumNextActivationDelay;
    [SerializeField] int _nextGeneratableCount;
    //BaseActivationDelayとPointObjectが持つoffsetActivationDelayにより最終的に決まる有効化の遅延時間
    [SerializeField] float _finalActivationDelay;
    Action<TimeKeeper> _onRelease;
    public enum TargetState
    {
        //有効化される前の状態
        Preparing,
        //有効化中の状態
        Activating,
        //有効化完了の状態
        ActivationCompleted,
        //無効化中の状態
        Deactivating
    }
    public void Begin()
    {
        StartCoroutine(ManageLifeCycle());
        IEnumerator ManageLifeCycle()
        {
            //対応するポイントオブジェクトの有効化に必要な情報や次のポイントオブジェクトの生成に有効な情報を取得、またPointObjectにおけるStartでもある。
            AllInitializePointObject();
            //対応するポイントオブジェクトの有効化までの時間可視化する関数を実行
            AllPlayActivationTimer(_finalActivationDelay);
            //ポイントオブジェクトの有効化時の出現アニメーションの所要時間を考慮して待機
            yield return new WaitForSeconds(_finalActivationDelay - _activateAnimRate *_finalActivationDelay);
            CurrentTargetState = TargetState.Activating;
            CurrentTaimingState = TimingState.GoodTiming;
            //ポイントオブジェクトの有効化処理と出現アニメーションの再生
            AllActivateMain(_activateAnimRate * _finalActivationDelay);
            //出現アニメーションの所要時間分待機
            yield return new WaitForSeconds(_activateAnimRate * _finalActivationDelay);
            CurrentTargetState = TargetState.ActivationCompleted;
            CurrentTaimingState = TimingState.PerfectTiming;
            PointObjectsGenerator.Current.RequestGeneration(_sumNextActivationDelay,0,_perlinNoiseMagni,_nextGeneratableCount);
            AllPlayDeactivationTimer(_sumLifeTime);
            yield return new WaitForSeconds(_sumLifeTime * 0.5f);
            CurrentTaimingState = TimingState.GreatTiming;
            yield return new WaitForSeconds(_sumLifeTime * 0.25f);
            CurrentTaimingState = TimingState.GoodTiming;
            yield return new WaitForSeconds(_sumLifeTime * 0.25f);
            CurrentTargetState = TargetState.Deactivating;
            //デスポーンの処理とそれのアニメーションの開始
            AllDeactivatePointObject(_deactivateAnimDuration);
            yield return new WaitForSeconds(_deactivateAnimDuration);
            _onRelease.Invoke(this);
        }
        void AllInitializePointObject(){
            _nextGeneratableCount =PointObjectsList[0].NextGeneratableCount;
            _finalActivationDelay = BaseActivationDelay;
            foreach(PointObjects pointObjects in PointObjectsList){
                pointObjects.TimeKeeper = this;
                PointObjects.InitializeResult initializeResult = pointObjects.Initialize();
                _sumNextActivationDelay += initializeResult.NextBaseActivationDelay;
                _sumLifeTime += initializeResult.LifeTime;
                _finalActivationDelay += initializeResult.OffsetActivationDelay;
                _finalActivationDelay += pointObjects.OffsetActivationDelay;
            }

        }

        void AllPlayActivationTimer(float activationDelay){
            foreach(PointObjects pointObjects in PointObjectsList){
                pointObjects.PlayActivationTimer(activationDelay);
            }
        }
        void AllActivateMain(float activateAnimDuration)
        {
            foreach(PointObjects pointObjects in PointObjectsList){
                pointObjects.ActivateMain(activateAnimDuration);
            }

        }
        
        void AllPlayDeactivationTimer(float sumLifeTime){
            foreach(PointObjects pointObjects in PointObjectsList){
                if(pointObjects == null) continue;
                pointObjects.PlayDeactivationTimer(sumLifeTime);
            }
        }
        void AllDeactivatePointObject(float deactivateAnimDuration){
            foreach(PointObjects pointObjects in PointObjectsList.ToArray()){
                if(pointObjects == null) continue;
                pointObjects.TimeOver(deactivateAnimDuration);
            }
        }
    }
    //PointObjectがTimeKeeperの管理から外れる為の関数メンバ
    public void NoticeUnLink(PointObjects pointObject)
    {
        PointObjectsList.Remove(pointObject);
        //管理外から外れる為　対象のポイントオブジェクトとのリンクを切る。
        pointObject.TimeKeeper = null;
        pointObject.transform.SetParent(ObjectPoolManager.Current.transform);
    }
    public void OnCreate(Action<TimeKeeper> onRelease)
    {
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        _sumNextActivationDelay = 0;
        _sumLifeTime = 0;
        BaseActivationDelay = 0;
        _nextGeneratableCount = 0;
        _finalActivationDelay = 0;
        CurrentTargetState = TargetState.Preparing;
        CurrentTaimingState = TimingState.GoodTiming;
        transform.SetParent(ObjectPoolManager.Current.transform);
    }

}
