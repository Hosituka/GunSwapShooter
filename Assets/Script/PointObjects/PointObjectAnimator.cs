using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//このクラスはpointObjectのアニメーションを担当します。アタッチされるゲームオブジェクトはPointObjectの各具象クラスがアタッチされている対象と同じです。
public class PointObjectAnimator : MonoBehaviour
{

    [Header("#全てにおいて設定必須項目")]
    [SerializeField]List<TMPandMeshRenderer> _tmpAndMeshRendererList;
    [Header("#PointObjectにおいて設定必須項目")]
    [SerializeField] MeshRenderer _lifeTimeGUI_MR;
    [Header("#表示用")]
    public ExplosionPhase CurtExplosionPhase{private set;get;}
    [field:SerializeField]public SpinThenExplodePhase CurtSpinThenExplodePhase{private set;get;} 
    public SpinAndFadeOutPhase CurtSpinAndFadeOutPhase{private set;get;}
    public SpinThenFadeOutPhase CurtSpinThenFadeOutPhase{private set;get;}   
    public FadeOutPhase CurtFadeOutPhase{private set; get;}
    public ShowAnimOfMainPhase CurtShowAnimOfMainPhase{private set; get;}
    public TimeOverAnimPhase CurtTimeOverAnimPhase{private set; get;}
    bool _isPlayingBreakAnim;
    MaterialPropertyBlock _propBlock;
    //#PointObjectが銃撃された時のアニメーション群、PointObjectの具象クラスにより呼ばれる。
    public void PlayExplosion(Vector3 explosionEffectPos,Color explosionEffectColor,float explosionEffectSize)
    {
        StartCoroutine(ExplosionCoroutine());
        IEnumerator ExplosionCoroutine()
        {
            _isPlayingBreakAnim = true;
            ExplosionEffect explosionEffect = StageUI_manager.Current.GenerateExplosionEffect(explosionEffectPos,explosionEffectColor,explosionEffectSize);
            Utility.ChangeEnabledTMPorMeshRenderers(_tmpAndMeshRendererList,false);
            CurtExplosionPhase = ExplosionPhase.Explosion;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.Exploding);
            CurtExplosionPhase = ExplosionPhase.Completed;
            yield return null;
        }
    }
    public enum ExplosionPhase
    { NotPlayed,Explosion,Completed,}

    public void PlaySpinThenExplode(Vector3 explosionEffectPos,Color explosionEffectColor,float explosionEffectSize)
    {
        StartCoroutine(RotateThenExplosionCoroutine());
        IEnumerator RotateThenExplosionCoroutine()
        {
            _isPlayingBreakAnim = true;
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Spin;
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                yield return null;
            }
            ExplosionEffect explosionEffect = StageUI_manager.Current.GenerateExplosionEffect(explosionEffectPos,explosionEffectColor,explosionEffectSize);
            Utility.ChangeEnabledTMPorMeshRenderers(_tmpAndMeshRendererList,false);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Exploding;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.Completed);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Completed;
        }

    }
    public enum SpinThenExplodePhase
    {NotPlayed,Spin,Exploding,Completed,}

    public void PlaySpinAndFadeOut()
    {
        StartCoroutine(PlayRotateAndFadeOutCoroutine());
        IEnumerator PlayRotateAndFadeOutCoroutine()
        {
            _isPlayingBreakAnim = true;
            CurtSpinAndFadeOutPhase = SpinAndFadeOutPhase.SpinAndFadeout;
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                foreach(TMPandMeshRenderer fadeTarget in _tmpAndMeshRendererList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }
            CurtSpinAndFadeOutPhase = SpinAndFadeOutPhase.Completed;
        }
    }
    public enum SpinAndFadeOutPhase
    {NotPlayed,SpinAndFadeout,Completed,}
    public void PlaySpinThenFadeOut()
    {
        StartCoroutine(PlayRotateAndFadeOutCoroutine());
        IEnumerator PlayRotateAndFadeOutCoroutine()
        {
            _isPlayingBreakAnim = true;
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.Spin;
            //ただ回転するだけのアニメーション
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 2f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                yield return null;
            }
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.SpinAndFadeOutPhase;
            //回転とフェードアウトのアニメーション
            startRotation = transform.rotation;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 2f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                foreach(TMPandMeshRenderer fadeTarget in _tmpAndMeshRendererList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.Completed;
        }
    }
    public enum SpinThenFadeOutPhase
    {NotPlayed,Spin,SpinAndFadeOutPhase,Completed,}

    public void PlayFadeOut(float duration)
    {
        StartCoroutine(FadeOutCoroutine());
        IEnumerator FadeOutCoroutine()
        {
            _isPlayingBreakAnim = true;
            CurtFadeOutPhase = FadeOutPhase.Fade;
            //フェードアウトのアニメーション
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / duration)){
                foreach(TMPandMeshRenderer fadeTarget in _tmpAndMeshRendererList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }
            CurtFadeOutPhase = FadeOutPhase.Completed;
        }

    }
    public enum FadeOutPhase
    {NotPlayed,Fade,Completed,}

    //#PointObjectのライフサイクルによるアニメーション、TimeKeeperにより呼ばれる。
    //##PointObjectのメイン部分が有効化された時に呼ばれるアニメーション
    public void PlayActivationTimer(float duration)
    {
        StartCoroutine(ActivationTimer());
        IEnumerator ActivationTimer(){
            for(float playback = 0;playback < duration; playback += Time.deltaTime)
            {
                if(_isPlayingBreakAnim)yield break;
                _propBlock.SetFloat("_outerFrameAnim",playback / duration);
                _lifeTimeGUI_MR.SetPropertyBlock(_propBlock);
                yield return null;
            }
            _lifeTimeGUI_MR.GetPropertyBlock(_propBlock);
        }
    }

    public void PlayShowAnimOfMain(Transform mainTr,float activateAnimDuration)
    {
        StartCoroutine(ExpandCoroutine());
        IEnumerator ExpandCoroutine()
        {
            CurtShowAnimOfMainPhase = ShowAnimOfMainPhase.Expanding;
            float playBackActiveAnimTime = 0;
            while(playBackActiveAnimTime < activateAnimDuration)
            {
                playBackActiveAnimTime += Time.deltaTime;
                mainTr.localScale = Vector3.one * (playBackActiveAnimTime / activateAnimDuration);
                mainTr.localScale = Vector3.ClampMagnitude(mainTr.localScale,Vector3.one.magnitude);
                yield return null;
            }
                CurtShowAnimOfMainPhase = ShowAnimOfMainPhase.Completed;
         }
    
    }
    public enum ShowAnimOfMainPhase
    {NotPlayed,Expanding,Completed,}

    public void PlayDeactivationTimer(float duration)
    {
        StartCoroutine(DeactivationTimer());
        IEnumerator DeactivationTimer(){
            for(float playback = 0;playback < duration; playback += Time.deltaTime)
            {
                if(_isPlayingBreakAnim)yield break;
                _propBlock.SetFloat("_outerFrameAnim",1 - playback / duration);
                _lifeTimeGUI_MR.SetPropertyBlock(_propBlock);
                yield return null;
            }
        }
    }

    //##PointObjectの時間制限を超えて的が撃たれなかったときに呼ばれるアニメーション
    public void PlayTimeOverAnim(float animDuration)
    {
        StartCoroutine(FadeOutCoroutine());
        IEnumerator FadeOutCoroutine()
        {
            CurtTimeOverAnimPhase = TimeOverAnimPhase.Fading;
            float playbackDeSpawnTime = 0;

            while(playbackDeSpawnTime < animDuration)
            {
                playbackDeSpawnTime += Time.deltaTime;
                foreach(TMPandMeshRenderer fadeTarget in _tmpAndMeshRendererList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playbackDeSpawnTime / animDuration);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playbackDeSpawnTime / animDuration);
                }
                yield return null;
            }
            foreach(TMPandMeshRenderer tMPorMeshRenderer in _tmpAndMeshRendererList)
            {
                tMPorMeshRenderer.SetFadeOfMeshRenderers(1);
                tMPorMeshRenderer.SetAlphaOfTextMeshPros(0);
            }
            CurtTimeOverAnimPhase = TimeOverAnimPhase.Completed;
        }
    }
    public enum TimeOverAnimPhase
    {NotPlayed,Fading,Completed,}
    //#その他
    //##各アニメーションをもう一度再生できるようにするためリセットする処理
    public void Reset()
    {
        ResetPhase();
        ResetPropBlock();
        ResetEnabled();
        //#各アニメーションの進捗を表す物のリセット
        void ResetPhase()
        {
            CurtExplosionPhase = ExplosionPhase.NotPlayed;
            CurtSpinAndFadeOutPhase = SpinAndFadeOutPhase.NotPlayed;
            CurtSpinThenExplodePhase = SpinThenExplodePhase.NotPlayed;
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.NotPlayed;
            CurtFadeOutPhase = FadeOutPhase.NotPlayed;
            CurtShowAnimOfMainPhase = ShowAnimOfMainPhase.NotPlayed;
            CurtTimeOverAnimPhase = TimeOverAnimPhase.NotPlayed;
            _isPlayingBreakAnim = false;
        }
        void ResetPropBlock(){
            foreach(TMPandMeshRenderer tMPorMeshRenderer in _tmpAndMeshRendererList)
            {
                tMPorMeshRenderer.SetFadeOfMeshRenderers(0);
                tMPorMeshRenderer.SetAlphaOfTextMeshPros(1);
            }
        }
        void ResetEnabled(){
            Utility.ChangeEnabledTMPorMeshRenderers(_tmpAndMeshRendererList,true);
        }
    }
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        foreach(TMPandMeshRenderer tMPorMeshRenderer in _tmpAndMeshRendererList)
        {
            tMPorMeshRenderer.Initialize();
        }

    }
    public void AddFadeTargetList(List<TMPandMeshRenderer> addFadeTargets)
    {
        _tmpAndMeshRendererList.AddRange(addFadeTargets);
    }
    public List<TMPandMeshRenderer> GetFadeTargetList()
    {
        return _tmpAndMeshRendererList;
    }
}
[Serializable]
public class TMPandMeshRenderer
{
    public List<TextMeshPro> TextMeshProList = new List<TextMeshPro>();
    public List<MeshRenderer> MeshRendererList = new List<MeshRenderer>();
    MaterialPropertyBlock _propBlock;
    public TMPandMeshRenderer(TextMeshPro[] addTextMeshPros,MeshRenderer[] addMeshRenderers)
    {
        TextMeshProList.AddRange(addTextMeshPros);
        MeshRendererList.AddRange(addMeshRenderers);
        _propBlock = new MaterialPropertyBlock();
    }
    public TMPandMeshRenderer(TextMeshPro addTextMeshPro,MeshRenderer[] addMeshRenderers)
    {
        TextMeshProList.Add(addTextMeshPro);
        MeshRendererList.AddRange(addMeshRenderers);
        _propBlock = new MaterialPropertyBlock();
    }
    public void Initialize()
    {
        _propBlock = new MaterialPropertyBlock();
    }

        
    
    public void SetAlphaOfTextMeshPros(float alpha)
    {
        foreach(TextMeshPro textMeshPro in TextMeshProList)
        {
            if(textMeshPro == null)continue;
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
    }
    public void SetFadeOfMeshRenderers(float fadeValue)
    {
        foreach(MeshRenderer meshRenderer in MeshRendererList)
        {
            if(meshRenderer == null)continue;
            meshRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_Fade",fadeValue);
            meshRenderer.SetPropertyBlock(_propBlock);
        }
    }

}
