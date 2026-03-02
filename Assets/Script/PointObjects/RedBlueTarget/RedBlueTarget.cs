using UnityEngine;
using System;
using System.Collections;

public class RedBlueTarget : PointObject,IPoolable<RedBlueTarget>
{
    [Header("RedBlueTargetの設定用プロパティ")]
    [SerializeField]GameObject _redPlaneObj;
    [SerializeField]GameObject _bluePlaneObj;
    //PointObjectGenerator1or2側が左右の順で赤青の的か否かの判断するための奴
    public bool IsLeftBluePlane;
    //レッドプレーン＋ブループレンの個数
    int _currentPlaneCount = 2;
    Action<RedBlueTarget> _onRelease;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            case GameManager.Difficult.normal:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            case GameManager.Difficult.hard:
            return new InitializeResult(
                        FourthNote,
                        FourthNote * 5,
                        0
                    );
            default:
                Debug.LogError("未対応の難易度が選択されています。");
            return new InitializeResult();
             
        }
    }
    protected override IEnumerator TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(_currentPlaneCount);
        _pointObjectAnimator.PlayTimeOverAnim(animDuration);
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtTimeOverAnimPhase != PointObjectAnimator.TimeOverAnimPhase.Completed);
        _onRelease.Invoke(this);
    }

    public void DecrementCurrentPlaneCount()
    {
        _currentPlaneCount--;
        if(_currentPlaneCount == 0)
        {StartBreakCoroutine();}

    }
    protected override IEnumerator BreakCoroutine()
    {
        _pointObjectAnimator.PlaySpinThenFadeOut();
        yield return new WaitWhile(()=> _pointObjectAnimator.CurtSpinThenFadeOutPhase != PointObjectAnimator.SpinThenFadeOutPhase.Completed);
        _onRelease.Invoke(this);
    }

    public void OnCreate(Action<RedBlueTarget> onRelease)
    {
        BaseOnCreate();
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        BaseOnRelease();
        _redPlaneObj.SetActive(true);
        _bluePlaneObj.SetActive(true);
        _currentPlaneCount = 2;
    }
}
