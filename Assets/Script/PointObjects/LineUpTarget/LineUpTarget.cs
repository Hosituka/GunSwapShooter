using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
public class LineUpTarget : PointObject,IPoolable<LineUpTarget>
{
    [Header("LineUpTargetの設定必須項目")]
    public GameObject[] PlaneForLineUpPrefab;
    public int MaxPlaneCount = 7;
    public int MinPlaneCount = 5;
    [SerializeField] Transform _axisTr;

    [Header("表示用")]
    //次の的が来るまでに必要な時間
    [SerializeField] float _nextShowPlaneInterval;
    [SerializeField] GameObject[] _setPlaneArray;
    [SerializeField] int _planeCount;
    [SerializeField] List<PlaneForLineUp> _planeForLineUpList; 
    float _pitchRotationStep;
    //一回転するまでに必要な時間
    float _rotationInterval;
    Action<LineUpTarget> _onRelease;
    TextMeshPro[] _needShotCountTexts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
        _planeCount = Random.Range(MinPlaneCount, MaxPlaneCount + 1);
        PlaneForLineUp instancePlaneForLineUp;
        _setPlaneArray = new GameObject[_planeCount];
        _planeForLineUpList = new List<PlaneForLineUp>();
        _needShotCountTexts = new TextMeshPro[_planeCount];

        _pitchRotationStep = 360f / _planeCount;
        for (int generatedCount = 0; generatedCount < _planeCount; generatedCount++)
        {
            GameObject generatePlanePrefab = PlaneForLineUpPrefab[Random.Range(0, PlaneForLineUpPrefab.Length)];
            GameObject generatedPlaneObj = Instantiate(generatePlanePrefab, generatePlanePrefab.transform.position, _axisTr.rotation);
            instancePlaneForLineUp = generatedPlaneObj.GetComponent<PlaneForLineUp>();
            _planeForLineUpList.Add(instancePlaneForLineUp);
            //PlaneForLineUpのゲームオブジェクトのコライダーとTMPorMeshRendererを取得。
            ColliderList.AddRange(instancePlaneForLineUp.Colliders);
            _pointObjectAnimator.AddFadeTargetList(instancePlaneForLineUp.PointObjectAnimator.GetFadeTargetList());
            instancePlaneForLineUp.Initialize(this);
            instancePlaneForLineUp.SetTransform(_axisTr,generatedCount * _pitchRotationStep);
            instancePlaneForLineUp.SetNeedShotCountText(_planeCount.ToString());
            _setPlaneArray[generatedCount] = generatedPlaneObj;
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
    }
    public int hoge()
    {
        int y = Random.Range(0,2);
        if(y == 1)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
    protected override IEnumerator TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(_planeCount);
        _pointObjectAnimator.PlayTimeOverAnim(animDuration);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtTimeOverAnimPhase != PointObjectAnimator.TimeOverAnimPhase.Completed);
        _onRelease.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeKeeper == null)return;
        if(TimeKeeper.CurrentTargetState == TimeKeeper.TargetState.Activating) return;
        _axisTr.rotation = Quaternion.AngleAxis(1 / _rotationInterval * Time.deltaTime * 360, transform.up) * _axisTr.rotation;
    }
    void SetRotationInterval()
    {
        _rotationInterval = _nextShowPlaneInterval * _planeCount;
    }

    //PlaneForLineUpがLineUpTargetの管理から外れるための関数メンバ
    public void NoticeDestruction(PlaneForLineUp removePlaneForLineUp)
    {
        _planeCount--;
        foreach (PlaneForLineUp planeForLineUp in _planeForLineUpList)
        {
            planeForLineUp.SetNeedShotCountText(_planeCount.ToString());
        }
        _planeForLineUpList.Remove(removePlaneForLineUp);
        if (_planeCount == 0)
        {StartBreakCoroutine();}
    }
    protected override IEnumerator BreakCoroutine()
    {
        _pointObjectAnimator.PlaySpinThenExplode(transform.position,Color.yellow,18);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        _onRelease.Invoke(this);
    }
    public void OnCreate(Action<LineUpTarget> onRelease)
    {
        BaseOnCreate();
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        BaseOnRelease();
    }

}
