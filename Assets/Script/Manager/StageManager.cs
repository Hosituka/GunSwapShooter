using UnityEngine;
using unityroom.Api;
using Audio;
public class StageManager : MonoBehaviour
{
    public static StageManager Current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public enum GameState
    {
        playing,
        end,
    }
    [Header("ステージに関するパラメータ")]
    public GameState GamePhase;
    public int ScoreBoardNumber = 1;
    public float Time;
    


    float _score;
    int _perfectCount;
    int _greatCount;
    int _goodCount;
    [SerializeField]int _overlookCount;
    int _scoreMultiplier;
    float _accidentalShoot;
    int _combo;

    void Start()
    {
        StageUI_manager.Current.UpdateAccidentalShootText(0);
        StageUI_manager.Current.UpdateComboText(0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GamePhase)
        {
            case GameState.playing:
                Time -= UnityEngine.Time.deltaTime;
                if (Time < 0)
                {
                    Time = 0;
                    GamePhase = GameState.end;
                    StageUI_manager.Current.ShowGameClearUI(_score,_accidentalShoot,_perfectCount,_greatCount,_goodCount,_overlookCount);
                    UnityroomApiClient.Instance.SendScore(ScoreBoardNumber, _score - _accidentalShoot, ScoreboardWriteMode.HighScoreDesc);
                    PointObjectGenerater2.CurrentPointObjectGenerater2.enabled = false;
                }
                StageUI_manager.Current.UpdateTimeText(Time);
                break;
            case GameState.end:
                break;
        }
    }

    public void AddScore(float addScore,TimingState judge)
    {
        if (GamePhase == GameState.end) return;
        _scoreMultiplier = 1 + (_combo / 10);
        addScore =  addScore * _scoreMultiplier;
        _score += addScore;

        if (_combo % 10 == 0)
        {
            StageUI_manager.Current.GenerateScoreMultiplierText(_scoreMultiplier);
            SoundManager.Current.PlayOneShot2D_SE(OneShot.comboConnect,0.5f);
        }
        switch (judge)
        {
            case TimingState.PerfectTiming:
                _perfectCount += 1;
            break;
            case TimingState.GreatTiming:
                _greatCount += 1;
            break;
            case TimingState.GoodTiming:
                _goodCount += 1;
            break;
        }
        StageUI_manager.Current.GenerateAddScoreText(addScore);
        StageUI_manager.Current.UpdateScoreText(_score);
        StageUI_manager.Current.GenerateJudgeTexts(judge);
    }
    public void AddAccidentalShoot(float add)
    {
        if (GamePhase == GameState.end) return;
        _accidentalShoot += add;
        StageUI_manager.Current.UpdateAccidentalShootText(_accidentalShoot);
    }
    public void ResetCombo()
    {
        if (GamePhase == GameState.end) return;
        _combo = 0;
        StageUI_manager.Current.UpdateComboText(_combo);
        _scoreMultiplier = 1 + (_combo / 10);
    }
    public void AddCombo()
    {
        if (GamePhase == GameState.end) return;
        _combo += 1;
        StageUI_manager.Current.UpdateComboText(_combo);
    }
    public void AddOverlookCount(int add)
    {
        if(GamePhase == GameState.end) return;
        _overlookCount += add;
    }
}
