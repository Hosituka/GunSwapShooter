using System;
using System.Collections;
using System.IO.Compression;
using UnityEngine;
//StageUI_managerクラスによりプールされている∧的の破壊を表すエフェクトの振る舞いが書かれているクラス
public class ExplosionEffect : MonoBehaviour,IPoolable<ExplosionEffect>
{
    public enum AnimPhase
    {
        NotPlayed,
        PlayingExplosion,
        Completed,
    }

    public float FadeInTime;
    public float ClippingTime;
    public Color BaseColor;
    public float Intensity;
    [Header("表示用")]public AnimPhase CurrentAnimPhase{private set;get;}

    public MeshRenderer MeshRenderer;
    MaterialPropertyBlock _propBlock;
    Action<ExplosionEffect> _onRelease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play(Vector3 pos,Color color,float size)
    {
        Initialize(pos, color,size);
        StartCoroutine(AnimCoroutine());
        IEnumerator AnimCoroutine()
        {
            float playFadeInTime = 0;
            float playClippingTime = 0;
            CurrentAnimPhase = AnimPhase.PlayingExplosion;
            _propBlock.SetColor("_BaseColor",BaseColor * Intensity);
            _propBlock.SetFloat("_clipping",0);
            while(playFadeInTime < FadeInTime)
            {
                _propBlock.SetFloat("_fadeIn",playFadeInTime / FadeInTime);
                MeshRenderer.SetPropertyBlock(_propBlock);
                playFadeInTime += Time.deltaTime;
                yield return null;
            }
            _propBlock.SetFloat("_fadeIn",1);
            MeshRenderer.SetPropertyBlock(_propBlock);
            yield return null;
            while(playClippingTime < ClippingTime)
            {
                _propBlock.SetFloat("_clipping",playClippingTime / ClippingTime);
                MeshRenderer.SetPropertyBlock(_propBlock);
                playClippingTime += Time.deltaTime;
                yield return null;
            }
            CurrentAnimPhase = AnimPhase.Completed;
            _onRelease?.Invoke(this);
        }
        void Initialize(Vector3 pos,Color color,float size)
        {
            _propBlock = new MaterialPropertyBlock();
            CurrentAnimPhase = AnimPhase.NotPlayed;

            transform.position = pos;
            transform.rotation = Quaternion.LookRotation(pos);
            transform.localScale = new Vector3(size,size,1);
            BaseColor = color;

        }
    }
    public void SetOnRelease(Action<ExplosionEffect> onRelease)
    {
        _onRelease = onRelease;
    }
    // Update is called once per frame
}
