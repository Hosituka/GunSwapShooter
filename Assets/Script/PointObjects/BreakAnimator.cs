using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//このクラスはpointObjectの破壊表現を担当します。アタッチされるゲームオブジェクトはPointObjectの各具象クラスがアタッチされている対象と同じです。
public class BreakAnimator : MonoBehaviour
{
    public enum ExplosionPhase
    {
        NotPlayed,
        Explosion,
        Completed,
    }
    public enum SpinThenExplodePhase
    {
        NotPlayed,
        Spin,
        Explosion,
        Completed,
    }

    public enum SpinAndFadeOutPhase
    {
        NotPlayed,
        SpinAndFadeout,
        Completed,
    }
    public enum SpinThenFadeOutPhase
    {
        NotPlayed,
        Spin,
        SpinAndFadeOutPhase,
        Completed,
    }
    public enum FadeOutPhase
    {
        NotPlayed,
        Fade,
        Completed,
    }
    [Header("表示用")]
    public ExplosionPhase CurtExplosionPhase{private set;get;}
    public SpinThenExplodePhase CurtSpinThenExplodePhase{private set;get;} 
    public SpinAndFadeOutPhase CurtSpinAndFadeOutPhase{private set;get;}
    public SpinThenFadeOutPhase CurtSpinThenFadeOutPhase{private set;get;}   
    public FadeOutPhase CurtFadeOutPhase{private set; get;}
    public void PlayExplosion(Vector3 explosionEffectPos,Color explosionEffectColor,float explosionEffectSize,List<TMPandMeshRenderer> disableTMPandMeshRenderers)
    {
        StartCoroutine(ExplosionCoroutine());
        IEnumerator ExplosionCoroutine()
        {
            ExplosionEffect explosionEffect = StageUI_manager.Current.GenerateExplosionEffect(explosionEffectPos,explosionEffectColor,explosionEffectSize);
            Utility.ChangeEnabledTMPorMeshRenderers(disableTMPandMeshRenderers,false);
            CurtExplosionPhase = ExplosionPhase.Explosion;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.PlayingExplosion);
            CurtExplosionPhase = ExplosionPhase.Completed;
            yield return null;
        }
    }
    public void PlaySpinThenExplode(Vector3 explosionEffectPos,Color explosionEffectColor,float explosionEffectSize,List<TMPandMeshRenderer> disableTMPandMeshRenderers)
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
            Utility.ChangeEnabledTMPorMeshRenderers(disableTMPandMeshRenderers,false);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Explosion;
            yield return new WaitWhile(() => explosionEffect.CurrentAnimPhase != ExplosionEffect.AnimPhase.Completed);
            CurtSpinThenExplodePhase = SpinThenExplodePhase.Completed;
        }

    }
    public void PlaySpinAndFadeOut(List<TMPandMeshRenderer> fadeTargetList)
    {
        StartCoroutine(PlayRotateAndFadeOutCoroutine());
        IEnumerator PlayRotateAndFadeOutCoroutine()
        {
            CurtSpinAndFadeOutPhase = SpinAndFadeOutPhase.SpinAndFadeout;
            Quaternion startRotation = transform.rotation;
            Vector3 angleAxis = transform.right;
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / 0.5f)){
                transform.rotation = Quaternion.AngleAxis(360 * playback,angleAxis) * startRotation;
                foreach(TMPandMeshRenderer fadeTarget in fadeTargetList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }
            CurtSpinAndFadeOutPhase = SpinAndFadeOutPhase.Completed;
        }
    }
    public void PlaySpinThenFadeOut(List<TMPandMeshRenderer> fadeTargetList)
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
                foreach(TMPandMeshRenderer fadeTarget in fadeTargetList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }
            CurtSpinThenFadeOutPhase = SpinThenFadeOutPhase.Completed;
        }
    }
    public void PlayFadeOut(List<TMPandMeshRenderer> fadeTargetList,float duration)
    {
        StartCoroutine(FadeOutCoroutine());
        IEnumerator FadeOutCoroutine()
        {
            //フェードアウトのアニメーション
            for(float playback = 0;playback < 1; playback += Time.deltaTime * (1 / duration)){
                foreach(TMPandMeshRenderer fadeTarget in fadeTargetList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playback);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playback);
                }
                yield return null;
            }

        }

    }

}