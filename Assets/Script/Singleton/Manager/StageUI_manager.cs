using TMPro;
using UnityEngine;
using Audio;
using UnityEngine.Pool;
using System;
public class StageUI_manager : MonoBehaviour
{
    public static StageUI_manager Current;
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
    [Header("#通常のUIに関するメンバ")]
    [SerializeField]AR_BackGround _aR_BackGround;
    [SerializeField] Indicator _indicatorToTarget;
    [SerializeField] Transform _indicatorsUI_Tr;
    [SerializeField] TextMeshProUGUI _timeTextTM;
    [SerializeField] ScoreValueText _scoreValueText;
    [SerializeField] TextMeshProUGUI _accidentalShootTextTM;
    [SerializeField] AddScoreText _addScoreText;
    [SerializeField] TextMeshProUGUI _comboTextTM;
    [SerializeField] ScoreMultiplierText _scoreMultiplierText;
    [SerializeField] ExplosionEffect _explosionEffect;
    [SerializeField] Transform _standardUI_Tr;
    [SerializeField,Header("##判定を表すUIに関するメンバ")]
    JudgeText _goodOfJudgeText;
    [SerializeField]JudgeText _greatOfJudgeText;
    [SerializeField]JudgeText _perfectOfJudgeText;


    [SerializeField,Header("#その他")]
    GameClearUI _gameClearUI;
    



    void Start()
    {
        //_explosionEffectPoolの作成と設定
        _explosionEffectPool = ObjectPoolManager.Current.GetObjectPool<ExplosionEffect>(_explosionEffect,10,10);
        //_addScoreTextPoolの作成と設定
        _addScoreTextPool = ObjectPoolManager.Current.GetObjectPool<AddScoreText>(_addScoreText,_standardUI_Tr,10,10);
        //_indicatorToTargetPoolの作成と設定
        _indicatorToTargetPool = ObjectPoolManager.Current.GetObjectPool<Indicator>(_indicatorToTarget,_indicatorsUI_Tr,10,10);
        //_scoreMultiplierTextPoolの作成と設定
        _scoreMultiplierTextPool = ObjectPoolManager.Current.GetObjectPool<ScoreMultiplierText>(_scoreMultiplierText,3,3);
        //各種JudgeTextPoolの作成と設定
        _goodTextPool = ObjectPoolManager.Current.GetObjectPool<JudgeText>(_goodOfJudgeText,_standardUI_Tr,15,15);
        _greatTextPool = ObjectPoolManager.Current.GetObjectPool<JudgeText>(_greatOfJudgeText,_standardUI_Tr,15,15);
        _perfectTextPool = ObjectPoolManager.Current.GetObjectPool<JudgeText>(_perfectOfJudgeText,_standardUI_Tr,15,15);
        //その他
        _aR_BackGround.StartShowWebCam();
        GameManager.Current.StartFadeIn();
        _gameClearUI.Hide();
    }

    public void UpdateScoreText(float score)
    {
        _scoreValueText.UpdateText(score);
    }
    public void UpdateAccidentalShootText(float miss)
    {
        _accidentalShootTextTM.SetText("{0:0}：誤射",miss);
    }
    public void UpdateTimeText(float time)
    {
        _timeTextTM.SetText("時間：{0:1}",time);
    }
    public void UpdateComboText(int combo)
    {
        _comboTextTM.SetText("{0:0}",combo);
    }
    ObjectPool<ScoreMultiplierText> _scoreMultiplierTextPool;
    public void GenerateScoreMultiplierText(int scoreMultiplier)
    {
        ScoreMultiplierText scoreMultiplierText = _scoreMultiplierTextPool.Get();
        scoreMultiplierText.Play(scoreMultiplier);
    }
    ObjectPool<AddScoreText> _addScoreTextPool;
    public void GenerateAddScoreText(float addScore)
    {
        AddScoreText addScoreText = _addScoreTextPool.Get();
        addScoreText.Play(addScore);
    }
    ObjectPool<ExplosionEffect> _explosionEffectPool;
    public ExplosionEffect GenerateExplosionEffect(Vector3 pos,Color color,float size)
    {
        ExplosionEffect explosionEffect = _explosionEffectPool.Get();
        explosionEffect.Play(pos,color ,size);
        return explosionEffect;
    }
    ObjectPool<JudgeText> _goodTextPool;
    ObjectPool<JudgeText> _greatTextPool;
    ObjectPool<JudgeText> _perfectTextPool;
    public void GenerateJudgeTexts(TimingState timing)
    {
        switch (timing)
        {
            case TimingState.GoodTiming:
            {
                JudgeText judgeText = _goodTextPool.Get();
                judgeText.Play();
                break;
            }
            case TimingState.GreatTiming:
            {
                JudgeText judgeText = _greatTextPool.Get();
                judgeText.Play();
                break;
            }
            case TimingState.PerfectTiming:
            {
                JudgeText judgeText = _perfectTextPool.Get();
                judgeText.Play();
                break;
            }
        }
    }
    public void ShowGameClearUI(double score, double accidentalShoot,int perfectCount,int greatCount,int goodCount,int overlookCount)
    {
        _gameClearUI.Show(score,accidentalShoot,perfectCount,greatCount,goodCount,overlookCount);
    }
    ObjectPool<Indicator> _indicatorToTargetPool;
    public Indicator GenerateIndicatorToTarget(Transform targetTr)
    {
        Indicator indicator = _indicatorToTargetPool.Get();
        indicator.Initialize(targetTr);
        return indicator;
    }
    public void RestartButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        GameManager.Current.ReloadCurrentScene();
    }
    public void GoBackTitleButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        GameManager.Current.LoadTitle();
    }
    public void LoadStageButton(string stageName)
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        GameManager.Current.LoadScene(stageName);
    }
    public void ResetRotationButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        GameManager.Current.ResetRotation();
    }

    public void ShowSettingsButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        Time.timeScale = 0;
    }
    public void CloseSettingsButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        Time.timeScale = 1;
    }
    public void StartFullScreenButton()
    {
        Screen.fullScreen = true;
        _aR_BackGround.ReCaluculateScale();
    }
    public void ExitFullScreenButton()
    {
        Screen.fullScreen = false;
        _aR_BackGround.ReCaluculateScale();
    }

}
    public enum TimingState
    {
        //パーフェクト判定となる期間
        PerfectTiming,
        //グレート判定となる期間
        GreatTiming,
        //グッド判定となる期間
        GoodTiming,

    }
