using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//このクラスはpointObjectのアニメーションを担当します。アタッチされるゲームオブジェクトはPointObjectの各具象クラスがアタッチされている対象と同じです。
public class PointObjectAnimator : MonoBehaviour
{

    [Header("#設定必須項目")]
    [SerializeField]List<TMPandMeshRenderer> _tmpAndMeshRendererList;
    [Header("#表示用")]
    public ExplosionPhase CurtExplosionPhase{private set;get;}
    public SpinThenExplodePhase CurtSpinThenExplodePhase{private set;get;} 
    public SpinAndFadeOutPhase CurtSpinAndFadeOutPhase{private set;get;}
    public SpinThenFadeOutPhase CurtSpinThenFadeOutPhase{private set;get;}   
    public FadeOutPhase CurtFadeOutPhase{private set; get;}
    //#PointObjectが銃撃された時のアニメーション群、PointObjectの具象クラスにより呼ばれる。
    public void PlayExplosion(Vector3 explosionEffectPos,Color explosionEffectColor,float explosionEffectSize)
    {
        StartCoroutine(ExplosionCoroutine());
        IEnumerator ExplosionCoroutine()
        {
            ExplosionEffect explosionEffect = StageUI_manager.Current.GenerateExplosionEffect(explosionEffectPos,explosionEffectColor,explosionEffectSize);
            Utility.ChangeEnabledTMPorMeshRenderers(_tmpAndMeshRendererList,false);
            CurtExplosionPhase = ExplosionPhase.Explosion;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.PlayingExplosion);
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
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Spin;
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                yield return null;
            }
            ExplosionEffect explosionEffect = StageUI_manager.Current.GenerateExplosionEffect(explosionEffectPos,explosionEffectColor,explosionEffectSize);
            Utility.ChangeEnabledTMPorMeshRenderers(_tmpAndMeshRendererList,false);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Explosion;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.Completed);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Completed;
        }

    }
    public enum SpinThenExplodePhase
    {NotPlayed,Spin,Explosion,Completed,}

    public void PlaySpinAndFadeOut()
    {
        StartCoroutine(PlayRotateAndFadeOutCoroutine());
        IEnumerator PlayRotateAndFadeOutCoroutine()
        {
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
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.Spin;
            //ただ回転するだけのアニメーション
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                yield return null;
            }
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.SpinAndFadeOutPhase;
            //回転とフェードアウトのアニメーション
            startRotation = transform.rotation;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
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

    //#PointObjectのライフサイクルによるアニメーションTimeKeeperにより呼ばれる。
    //##PointObjectのメイン部分が有効化された時に呼ばれるアニメーション
    public void PlayShowAnimOfMain(Transform mainTr,float activateAnimDuration)
    {
        StartCoroutine(ExpandCoroutine());
        IEnumerator ExpandCoroutine()
        {
            float playBackActiveAnimTime = 0;
            while(playBackActiveAnimTime < activateAnimDuration)
            {
                playBackActiveAnimTime += Time.deltaTime;
                mainTr.localScale = Vector3.one * (playBackActiveAnimTime / activateAnimDuration);
                mainTr.localScale = Vector3.ClampMagnitude(mainTr.localScale,Vector3.one.magnitude);
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
            Destroy(gameObject);
        }

    }
    //#その他
    //##このイベント関数は現状TMPandMeshrendererの初期化(materialPropertyblockのインスタンス生成)を行っている。
    void Start()
    {
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
        _propBlock.Clear();
        foreach(MeshRenderer meshRenderer in MeshRendererList)
        {
            if(meshRenderer == null)continue;
            _propBlock.SetFloat("_Fade",fadeValue);
            for(int propertyBlockIndex = 0; propertyBlockIndex < meshRenderer.sharedMaterials.Length; propertyBlockIndex++)
            {
                meshRenderer.SetPropertyBlock(_propBlock,propertyBlockIndex);
            }
        }
    }

}
