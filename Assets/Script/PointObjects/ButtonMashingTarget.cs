using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
public class ButtonMashingTarget : PointObject
{
    [Header("ButtonMashingTargetの設定用プロパティ")]
    public int Hp;
    public int MaxHp = 7;
    public int MinHp = 4;
    [SerializeField] TextMeshPro _needShotCountText;
    [SerializeField]bool _isDestruction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override InitializeResult Initialize()
    {
        Hp = Random.Range(MinHp, MaxHp);
        _needShotCountText.SetText(Hp.ToString());
        switch (GameManager.Current.CurrentDifficult)
        {
            case GameManager.Difficult.easy:
            return new InitializeResult(
                        SixteenthNote * Hp + EighthNote,
                        SixteenthNote * Hp * 4,
                        0
                    );
            case GameManager.Difficult.normal:
            return new InitializeResult(
                        SixteenthNote * Hp + EighthNote,
                        SixteenthNote * Hp * 4,
                        0
                    );
            case GameManager.Difficult.hard:
            return new InitializeResult(
                        SixteenthNote * Hp + EighthNote,
                        SixteenthNote * Hp * 4,
                        0
                    );
            default:
                Debug.LogError("未対応の難易度が選択されています。");
            return new InitializeResult();
             
        }
    }
    public override void TimeOver(float animDuration)
    {
        StageManager.Current.AddOverlookCount(Hp);

        Utility.ChangeEnabledColliders(ColliderList,false);
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        _targetIndicator.Destroy();
        _targetPointObjectAnimator.PlayTimeOverAnim(animDuration);

    }

    // Update is called once per frame
    void Update()
    {

    }
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        if(_collisionCount != 1) return;
        if(_isDestruction == true) return;
        if(Hp <= 0) return;
        if (collision.gameObject.CompareTag("BlueBullet") || collision.gameObject.CompareTag("RedBullet"))
        {
            Hp--;
            _needShotCountText.text = Hp.ToString();
            StageManager.Current.AddCombo();
            switch (TargetTimeKeeper.CurrentTaimingState)
            {
                case TimingState.GoodTiming:
                StageManager.Current.AddScore(0.4f,TimingState.GoodTiming);
                break;
                case TimingState.GreatTiming:
                StageManager.Current.AddScore(0.7f,TimingState.GreatTiming);
                break;
                case TimingState.PerfectTiming:
                StageManager.Current.AddScore(1,TimingState.PerfectTiming);
                break;
            }

            if(Hp == 0)
            {
                _isDestruction = true;
                StartCoroutine(BreakCoroutine());
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    protected override IEnumerator BreakCoroutine()
    {
        PointObjectGenerater.Current.SubtractSumPointObjectCost(PointObjectCost);
        PointObjectGenerater.Current.RemovePointObjectPos(PointObjectPos,2);
        TargetTimeKeeper.NoticeDestruction(this);
        _targetIndicator.Destroy();
        
        _targetPointObjectAnimator.PlaySpinThenExplode(transform.position,Color.magenta,17);
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Explosion);
        Utility.ChangeEnabledColliders(ColliderList,false);
        yield return new WaitWhile(()=> _targetPointObjectAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        Destroy(gameObject);
    }

}
