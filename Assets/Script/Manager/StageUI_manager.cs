using TMPro;
using UnityEngine;
using Audio;
using UnityEngine.Pool;
using System.Collections.Generic;
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
    [SerializeField] GameObject _indicatorsUIObj;
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
        //_explosionEffectPoolの設定
        _explosionEffectPool = new ObjectPool<ExplosionEffect>(
            createFunc: ()=>
            {  // 足りない時に新しく作る処理
                ExplosionEffect poolTarget =  Instantiate(_explosionEffect);
                poolTarget.SetOnRelease((e)=>_explosionEffectPool.Release(e));
                return poolTarget;
            },     
            actionOnGet: (explosionEffect)=>explosionEffect.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (explosionEffect)=>explosionEffect.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (explosionEffect)=>Destroy(explosionEffect.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: 10,              // 最初に用意する目安
            maxSize: 10                       // 最大貯蓄数
        );       
        //_addScoreTextPoolの設定
        _addScoreTextPool = new ObjectPool<AddScoreText>(
            createFunc: () =>
            {
                AddScoreText poolTarget = Instantiate(_addScoreText,_standardUI_Tr);
                poolTarget.SetOnRelease((a)=>_addScoreTextPool.Release(a));
                return poolTarget;
            },
            actionOnGet:(addScoreText)=>addScoreText.gameObject.SetActive(true),
            actionOnRelease:(addScoreText)=>addScoreText.gameObject.SetActive(false),
            actionOnDestroy:(addScoreText)=>Destroy(addScoreText.gameObject),
            defaultCapacity: 10,
            maxSize:10
        );
        //_indicatorToTargetPoolの設定
        _indicatorToTargetPool = new ObjectPool<Indicator>(
            createFunc: () =>
            {
                Indicator poolTarget = Instantiate(_indicatorToTarget,_indicatorsUIObj.transform);
                poolTarget.SetOnRelease((i)=>_indicatorToTargetPool.Release(i));
                return poolTarget;
            },
            actionOnGet:(i)=>i.gameObject.SetActive(true),
            actionOnRelease:(i)=>i.gameObject.SetActive(false),
            actionOnDestroy:(i)=>Destroy(i.gameObject),
            defaultCapacity: 3,
            maxSize:10
        );
        //_scoreMultiplierTextPoolの設定
        _scoreMultiplierTextPool = new ObjectPool<ScoreMultiplierText>(
            createFunc: () =>
            {
                ScoreMultiplierText poolTarget = Instantiate(_scoreMultiplierText);
                poolTarget.SetOnRelease((i)=>_scoreMultiplierTextPool.Release(i));
                return poolTarget;
            },
            actionOnGet:(i)=>i.gameObject.SetActive(true),
            actionOnRelease:(i)=>i.gameObject.SetActive(false),
            actionOnDestroy:(i)=>Destroy(i.gameObject),
            defaultCapacity: 3,
            maxSize:3
        );
        _goodTextPool = new ObjectPool<JudgeText>(
            createFunc: () =>
            {
                JudgeText poolTarget = Instantiate(_goodOfJudgeText,_standardUI_Tr);
                poolTarget.SetOnRelease((i)=>_goodTextPool.Release(i));
                return poolTarget;
            },
            actionOnGet:(j)=>j.gameObject.SetActive(true),
            actionOnRelease:(j)=>j.gameObject.SetActive(false),
            actionOnDestroy:(j)=>Destroy(j.gameObject),
            defaultCapacity: 10,
            maxSize:10

        );
        _greatTextPool = new ObjectPool<JudgeText>(
            createFunc: () =>
            {
                JudgeText poolTarget = Instantiate(_greatOfJudgeText,_standardUI_Tr);
                poolTarget.SetOnRelease((i)=>_greatTextPool.Release(i));
                return poolTarget;
            },
            actionOnGet:(j)=>j.gameObject.SetActive(true),
            actionOnRelease:(j)=>j.gameObject.SetActive(false),
            actionOnDestroy:(j)=>Destroy(j.gameObject),
            defaultCapacity: 10,
            maxSize:10

        );
        _perfectTextPool = new ObjectPool<JudgeText>(
            createFunc: () =>
            {
                JudgeText poolTarget = Instantiate(_perfectOfJudgeText,_standardUI_Tr);
                poolTarget.SetOnRelease((i)=>_perfectTextPool.Release(i));
                return poolTarget;
            },
            actionOnGet:(j)=>j.gameObject.SetActive(true),
            actionOnRelease:(j)=>j.gameObject.SetActive(false),
            actionOnDestroy:(j)=>Destroy(j.gameObject),
            defaultCapacity: 10,
            maxSize:10

        );
        //事前にプールを満たす処理；
        ObjectPoolManager.Current.PrewarmPool<ExplosionEffect>(_explosionEffectPool,5);
        ObjectPoolManager.Current.PrewarmPool<AddScoreText>(_addScoreTextPool,10);
        ObjectPoolManager.Current.PrewarmPool<Indicator>(_indicatorToTargetPool,10);
        ObjectPoolManager.Current.PrewarmPool<ScoreMultiplierText>(_scoreMultiplierTextPool,3);
        ObjectPoolManager.Current.PrewarmPool<JudgeText>(_goodTextPool,10);
        ObjectPoolManager.Current.PrewarmPool<JudgeText>(_greatTextPool,10);
        ObjectPoolManager.Current.PrewarmPool<JudgeText>(_perfectTextPool,10);

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
