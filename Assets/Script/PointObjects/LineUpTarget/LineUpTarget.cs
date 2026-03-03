using UnityEngine.Pool;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;
public class LineUpTarget : PointObject<LineUpTarget>
{
    [Header("#LineUpTargetの設定必須項目")]
    [SerializeField]PlanesForLineUp[] _planesForLineUpForGeneration;
    [SerializeField]int _maxPlaneCount = 4;
    [SerializeField]int _minPlaneCount = 4;
    [SerializeField] Transform _axisTr;
    [Header("#LineUpTargetがPlaneForLineUpを生成する際のコピー元")]
    [SerializeField]RedPlaneForLineUp _redPlaneForLineUp;
    [SerializeField]BluePlaneForLineUp _bluePlaneForLineUp;
    [SerializeField]GunSwapPlaneForLineUp _gunSwapPlaneForLineUp;

    [Header("表示用")]
    //次の的が来るまでに必要な時間
    [SerializeField] float _nextShowPlaneInterval;
    [SerializeField] GameObject[] _setPlaneArray;
    [SerializeField] int _planeCount;
    [SerializeField] List<PlanesForLineUp> _planesForLineUpList; 
    float _pitchRotationStep;
    //一回転するまでに必要な時間
    float _rotationInterval;
    ObjectPool<RedPlaneForLineUp> _redPlaneForLineUpPool;
    ObjectPool<BluePlaneForLineUp>_bluePlaneForLineUpPool;
    ObjectPool<GunSwapPlaneForLineUp>_gunSwapPlaneForLineUpPool;

    public override InitializeResult Initialize()
    {
        _planeCount = Random.Range(_minPlaneCount, _maxPlaneCount + 1);
        PlanesForLineUp planeForLineUp;
        _planesForLineUpList = new List<PlanesForLineUp>();

        _pitchRotationStep = 360f / _planeCount;
        for (int generatedCount = 0; generatedCount < _planeCount; generatedCount++)
        {
            planeForLineUp = GetPlanesForLineUpWithDownCast(_planesForLineUpForGeneration[Random.Range(0, _planesForLineUpForGeneration.Length)]);
            _planesForLineUpList.Add(planeForLineUp);
            planeForLineUp.Initialize(this,_axisTr,generatedCount * _pitchRotationStep);
            planeForLineUp.SetNeedShotCountText(_planeCount.ToString());
        }


        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
            _nextShowPlaneInterval = FourthNote;
            SetRotationInterval();
            return new InitializeResult(
                        _nextShowPlaneInterval * _planeCount + FourthNote,
                        _nextShowPlaneInterval * _planeCount * 2,
                        FourthNote
                    );
            case GameManager.Difficult.normal:
            _nextShowPlaneInterval = FourthNote;
            SetRotationInterval();
            return new InitializeResult(
                        _nextShowPlaneInterval * _planeCount + FourthNote,
                        _nextShowPlaneInterval * _planeCount * 2,
                        FourthNote
                    );
            case GameManager.Difficult.hard:
            _nextShowPlaneInterval = FourthNote + SixteenthNote;
            SetRotationInterval();
            return new InitializeResult(
                        _nextShowPlaneInterval * _planeCount + FourthNote,
                        _nextShowPlaneInterval * _planeCount * 2,
                        FourthNote
                    );
            default:
                Debug.LogError("未対応の難易度が選択されています。");
            return new InitializeResult();
             
        }
        PlanesForLineUp GetPlanesForLineUpWithDownCast(PlanesForLineUp planesForLineUp)
        {
            switch (planesForLineUp)
            {
                case BluePlaneForLineUp:
                    return _bluePlaneForLineUpPool.Get();
                case RedPlaneForLineUp:
                    return _redPlaneForLineUpPool.Get();

                case GunSwapPlaneForLineUp:
                    return _gunSwapPlaneForLineUpPool.Get();

            }
            Debug.LogError("どの具象クラスにも属さないPlaneForLineUpのインスタンスが渡されました");
            return null;
        }
        void SetRotationInterval()
        {
            _rotationInterval = _nextShowPlaneInterval * _planeCount;
        }

    }
    protected override IEnumerator SubTimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(_planeCount);
        foreach(PlanesForLineUp planesForLineUp in _planesForLineUpList)
        {
            planesForLineUp.TimeOver(animDuration);
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeKeeper == null)return;
        if(TimeKeeper.CurrentTargetState == TimeKeeper.TargetState.Activating) return;
        _axisTr.rotation = Quaternion.AngleAxis(1 / _rotationInterval * Time.deltaTime * 360, transform.up) * _axisTr.rotation;
    }
    //PLanesForLineUpが銃撃されたことをLineUpTargetに伝える処理
    public void NoticeDestruction(PlanesForLineUp planesForLineUp)
    {
        _planeCount--;
        foreach (PlanesForLineUp planeForLineUp in _planesForLineUpList)
        {
            planeForLineUp.SetNeedShotCountText(_planeCount.ToString());
        }
        UnLinkPlanesForLineUp(planesForLineUp);
        if (_planeCount == 0)
        {BreakCoroutine();}
    }
    //PlanesForLineUpがLineUpTargetの管理から外れるための関数メンバ  
    void UnLinkPlanesForLineUp(PlanesForLineUp planesForLineUp)
    {
        _planesForLineUpList.Remove(planesForLineUp);
        planesForLineUp.LineUpTarget = null;
        planesForLineUp.transform.SetParent(ObjectPoolManager.Current.transform);
    }
    protected override IEnumerator SubBreakCoroutine()
    {
        _pointObjectAnimator.PlaySpinThenExplode(transform.position,Color.yellow,18);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        _onRelease.Invoke(this);
    }
    protected override void SubOnCreate()
    {
        _bluePlaneForLineUpPool = ObjectPoolManager.Current.GetObjectPool<BluePlaneForLineUp>(_bluePlaneForLineUp,_maxPlaneCount,_maxPlaneCount);
        _redPlaneForLineUpPool = ObjectPoolManager.Current.GetObjectPool<RedPlaneForLineUp>(_redPlaneForLineUp,_maxPlaneCount,_maxPlaneCount);
        _gunSwapPlaneForLineUpPool = ObjectPoolManager.Current.GetObjectPool<GunSwapPlaneForLineUp>(_gunSwapPlaneForLineUp,_maxPlaneCount,_maxPlaneCount);
    }
    protected override void SubOnRelease()
    {
        _axisTr.localRotation = Quaternion.identity;
        foreach(PlanesForLineUp planesForLineUp in _planesForLineUpList.ToList())
        {
            UnLinkPlanesForLineUp(planesForLineUp);
        }
    }

}
