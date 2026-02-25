using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
//ここは全ての的に共通する振る舞いや状態を書く抽象クラスです。
public abstract class PointObject : MonoBehaviour
{
    [Header("PointObjectの設定用プロパティ")]
    public List<Collider> ColliderList;
    public GameObject MainObj;
    public float PointObjectCost;
    //次のpointobjectを生成する時、同時に生成される個数
    public int NextGeneratableCount = 1;
    //このフィールドを持つインスタンスの有効化に必要な時間のoffset
    public float OffsetActivationDelay{get;protected set;}
    public MeshRenderer LifeTimeGUI_MR;
    [SerializeField]protected PointObjectAnimator _targetPointObjectAnimator;
    [Header("表示用")]
    public Vector2 PointObjectPos;
    public Vector2 DebugPointObjectPos;
    public Vector2 PointObjectPosAsCenter;
    public Vector2 NormalizePointObjectPosAsCenter;
    public TimeKeeper TargetTimeKeeper;
    protected Indicator _targetIndicator;

    //　インスペクターから見れん奴ら
    /// 音符
    public static float FourthNote;
    public static float EighthNote;
    public static float SixteenthNote;


    
    public struct InitializeResult
    {
        public float NextBaseActivationDelay;
        public float LifeTime;
        public float OffsetActivationDelay;
        public InitializeResult(float nextBaseActivationDelay,float lifeTime,float offsetActivationDelay)
        {
            NextBaseActivationDelay = nextBaseActivationDelay;
            LifeTime = lifeTime;
            OffsetActivationDelay = offsetActivationDelay;
        }
    }

    public abstract InitializeResult Initialize();
    //時間制限内に射撃されなかったとき、TimeKeeperにより呼ばれる
    public abstract void TimeOver(float animDuration);

    
    
    public void PlaySubtractLifeTimeGUI(float duration)
    {
        StartCoroutine(SubtractLifeTimeGUI());
        IEnumerator SubtractLifeTimeGUI(){
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            for(float playback = 0;playback < duration; playback += Time.deltaTime)
            {
                if(TargetTimeKeeper == null) yield break;
                propBlock.SetFloat("_outerFrameAnim",1 - playback / duration);
                _targetIndicator.SetLifeTimeAnim(1 - playback / duration,Color.white,Color.black);
                LifeTimeGUI_MR.SetPropertyBlock(propBlock);
                yield return null;
            }
        }
    }
    public void PlayAddLifeTimeGUI(float duration)
    {
        StartCoroutine(AddLifeTimeGUI());
        IEnumerator AddLifeTimeGUI(){
            MainObj.SetActive(false);
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            for(float playback = 0;playback < duration;playback += Time.deltaTime)
            {
                if(TargetTimeKeeper == null) yield break;
                propBlock.SetFloat("_outerFrameAnim",playback / duration);
                //shadergraphで作ったshader側の都合で、Color.blackを第二引数に設定する時は、微小な量を足してください。
                _targetIndicator.SetLifeTimeAnim(playback / duration,Color.black + new Color(0.04f,0.04f,0.04f),Color.white);
                LifeTimeGUI_MR.SetPropertyBlock(propBlock);
                yield return null;
            }
        }
    }
    //PointObjectのメインとなる部分を有効化する関数。
    public void ActivateMain(float activateAnimDuration)
    {
        MainObj.SetActive(true);
        this.enabled = true;
        _targetPointObjectAnimator.PlayShowAnimOfMain(MainObj.transform,activateAnimDuration);

        PointObjectPosAsCenter = new Vector2(PointObjectPos.x - PointObjectGenerater.Current.PointObjectMapLength.x / 2, PointObjectPos.y - PointObjectGenerater.Current.PointObjectMapLength.y / 2);
        NormalizePointObjectPosAsCenter = new Vector2(PointObjectPosAsCenter.x / (PointObjectGenerater.Current.PointObjectMapLength.x / 2), PointObjectPosAsCenter.y / (PointObjectGenerater.Current.PointObjectMapLength.y / 2));
        //DebugPointObjectPos = PointObjectGenerater2.pointObjectGenerater2.pointObjectWorldPosToPointObjectPos(transform.position);
    }

    public void SetIndicator(Indicator indicator)
    {
        _targetIndicator = indicator;
    }
    abstract protected IEnumerator BreakCoroutine();
}
