using UnityEngine;
using System.Collections;

public class RedBlueTarget : PointObject
{
    [Header("RedBlueTargetの設定用プロパティ")]
    [SerializeField]GameObject _redPlane;
    [SerializeField]GameObject _bluePlane;
    //レッドプレーン＋ブループレンの個数
    int _currentPlaneCount = 2;

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
    public override void TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(_currentPlaneCount);

        Utility.ChangeEnabledColliders(ColliderList,false);
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        _targetIndicator.Destroy();
        _targetPointObjectAnimator.PlayTimeOverAnim(animDuration);


    }

    // Update is called once per frame
    public void DecrementCurrentPlaneCount()
    {
        _currentPlaneCount--;
        if(_currentPlaneCount == 0)
        {StartCoroutine(BreakCoroutine());}

    }
    protected override IEnumerator BreakCoroutine()
    {
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        TargetTimeKeeper.NoticeDestruction(this);
        _targetIndicator.Destroy();

        _targetPointObjectAnimator.PlaySpinThenFadeOut();
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtSpinThenFadeOutPhase != PointObjectAnimator.SpinThenFadeOutPhase.Completed);
        Destroy(gameObject);
    }


}
