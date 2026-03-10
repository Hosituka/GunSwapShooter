using System;
using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
//StageUI_managerクラスによりプールされている∧的の破壊を表すエフェクトの振る舞いが書かれているクラス
public class ExplosionEffect : MonoBehaviour,IPoolable<ExplosionEffect>
{
    public enum AnimPhase
    {NotPlayed,Exploding,Completed,}

    public float FadeInTime;
    public float ClippingTime;
    public Color BaseColor;
    public float Intensity;
    [Header("表示用")]public AnimPhase CurrentAnimPhase{private set;get;}

    public MeshRenderer MeshRenderer;
    MaterialPropertyBlock _propBlock;
    Action<ExplosionEffect> _onRelease;
    public async UniTask Explosion(Vector3 pos,Color color,float size)
    {
        Initialize(pos, color,size);
        float playFadeInTime = 0;
        float playClippingTime = 0;
        CurrentAnimPhase = AnimPhase.Exploding;
        _propBlock.SetColor("_BaseColor",BaseColor * Intensity);
        _propBlock.SetFloat("_clipping",0);
        while(playFadeInTime < FadeInTime)
        {
            _propBlock.SetFloat("_fadeIn",playFadeInTime / FadeInTime);
            MeshRenderer.SetPropertyBlock(_propBlock);
            playFadeInTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update,destroyCancellationToken);
        }
        _propBlock.SetFloat("_fadeIn",1);
        MeshRenderer.SetPropertyBlock(_propBlock);
        await UniTask.Yield(PlayerLoopTiming.Update,destroyCancellationToken);
        while(playClippingTime < ClippingTime)
        {
            _propBlock.SetFloat("_clipping",playClippingTime / ClippingTime);
            MeshRenderer.SetPropertyBlock(_propBlock);
            playClippingTime += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update,destroyCancellationToken);
        }
        _onRelease.Invoke(this);
        CurrentAnimPhase = AnimPhase.Completed;

        void Initialize(Vector3 pos,Color color,float size)
        {
            CurrentAnimPhase = AnimPhase.NotPlayed; 
            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(pos);
            transform.localScale = new Vector3(size,size,1);
            BaseColor = color;

        }
    }
    public void OnCreate(Action<ExplosionEffect> onRelease)
    {
        _onRelease = onRelease;
        _propBlock = new MaterialPropertyBlock();
    }
}
