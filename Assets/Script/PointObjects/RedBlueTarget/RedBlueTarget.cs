using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

public class RedBlueTarget : PointObject<RedBlueTarget>
{
    [Header("RedBlueTargetの設定用プロパティ")]
    [SerializeField]RedPlane _redPlane;
    [SerializeField]BluePlane _bluePlane;
    //PointObjectGenerator1or2側が左右の順で赤青の的か否かの判断するための奴
    public bool IsLeftBluePlane;
    //レッドプレーン＋ブループレンの個数
    int _currentPlaneCount = 2;

    protected override InitializeResult SubInitialize()
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
    protected override async UniTaskVoid SubTimeOver(float duration)
    {
        StageManager.Current.AddOverlookCount(_currentPlaneCount);
    }
    protected override async UniTaskVoid SubBreakAsync()
    {
        await _pointObjectAnimator.PlaySpinThenFadeOut();
        _onRelease.Invoke(this);
    }


    protected override void OnValidCollisionEnter(Collision collision)
    {
        
    }

    public void DecrementCurrentPlaneCount()
    {
        _currentPlaneCount--;
        if(_currentPlaneCount == 0)
        {BreakAsync();}

    }
    protected override void SubOnCreate()
    {
    }
    protected override void SubOnRelease()
    {
        _redPlane.Reset();
        _bluePlane.Reset();
        _currentPlaneCount = 2;
    }
}
