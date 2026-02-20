using TMPro;
using UnityEngine;
using Audio;
using UnityEngine.Pool;
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
    [SerializeField] GameObject _indicatorToTarget;
    [SerializeField] GameObject _indicatorsUIObj;
    [SerializeField] TextMeshProUGUI _timeTextTM;
    [SerializeField] ScoreValueText _scoreValueText;
    [SerializeField] TextMeshProUGUI _accidentalShootTextTM;
    [SerializeField] GameObject _addScoreTextObj;
    [SerializeField] TextMeshProUGUI _comboTextTM;
    [SerializeField] GameObject _scoreMultiplierTextObj;
    [SerializeField] GameObject _explosionEffectObj;
    [SerializeField] Transform _standardUI_Tr;
    [SerializeField,Header("##判定を表すUIに関するメンバ")]
    GameObject _goodTextPrefab;
    [SerializeField]GameObject _greatTextPrefab;
    [SerializeField]GameObject _perfectTextPrefab;


    [SerializeField,Header("#その他")]
    GameClearUI _gameClearUI;



    void Start()
    {
        //ExplosionEffectPoolの設定
        _explosionEffectPool = new ObjectPool<ExplosionEffect>(
            createFunc: ()=>Instantiate(_explosionEffectObj).GetComponent<ExplosionEffect>(),       // 足りない時に新しく作る処理
            actionOnGet: (explosionEffect)=>explosionEffect.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (explosionEffect)=>explosionEffect.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (explosionEffect)=>Destroy(explosionEffect.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: 3,              // 最初に用意する目安
            maxSize: 10                       // 最大貯蓄数
        );       
        
        //その他
        _aR_BackGround.StartShowWebCam();
        GameManager.Current.StartFadeIn();
        _gameClearUI.Hide();
        _indicatorToTarget.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

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
        _comboTextTM.text = combo.ToString();
    }
    public void GenerateScoreMultiplierText(int __scoreMultiplier)
    {
        Debug.Log("スコア倍率増加");
        Vector3 playerPos = Player.Current.transform.position;
        GameObject gameObject = Instantiate(_scoreMultiplierTextObj);
        gameObject.transform.position = Player.Current.transform.forward * 30;
        gameObject.transform.rotation = Quaternion.LookRotation(playerPos - gameObject.transform.position);
        gameObject.GetComponent<ScoreMultiplierText1>().ScoreMultiplier = __scoreMultiplier;
    }
    public void GenerateAddScoreText(float addScore)
    {
        GameObject gameObject = Instantiate(_addScoreTextObj, _standardUI_Tr);
        gameObject.GetComponent<AddScoreText>().AddScore = addScore;
    }
    ObjectPool<ExplosionEffect> _explosionEffectPool;
    public ExplosionEffect GenerateExplosionEffect(Vector3 pos,Color color,float size)
    {
        GameObject gameObject = Instantiate(_explosionEffectObj,pos,Quaternion.LookRotation(pos));
        gameObject.transform.localScale = new Vector3(size,size,1);
        ExplosionEffect explosionEffect = gameObject.GetComponent<ExplosionEffect>();
        explosionEffect.BaseColor = color;
        return explosionEffect;
    }
    public void GenerateJudgeTexts(TimingState timing)
    {
        switch (timing)
        {
            case TimingState.GoodTiming:
            Instantiate(_goodTextPrefab,_standardUI_Tr);
            break;
            case TimingState.GreatTiming:
            Instantiate(_greatTextPrefab,_standardUI_Tr);
            break;
            case TimingState.PerfectTiming:
            Instantiate(_perfectTextPrefab,_standardUI_Tr);
            break;
        }
    }
    public void ShowGameClearUI(double score, double accidentalShoot,int perfectCount,int greatCount,int goodCount,int overlookCount)
    {
        _gameClearUI.Show(score,accidentalShoot,perfectCount,greatCount,goodCount,overlookCount);
    }
    public Indicator2 GenerateIndicatorToTarget(Transform targetTr)
    {
        GameObject indicator2Obj = Instantiate(_indicatorToTarget);
        Indicator2 indicator2 = indicator2Obj.GetComponent<Indicator2>();
        indicator2.TargetTr = targetTr;
        indicator2Obj.transform.SetParent(_indicatorsUIObj.transform);
        indicator2Obj.transform.SetSiblingIndex(0);
        indicator2Obj.SetActive(true);
        return indicator2;
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
