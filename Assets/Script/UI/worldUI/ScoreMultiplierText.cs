using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;
/*10コンボ感覚で発生する、スコア増加率増加を表すテキストのアニメーションを責務として持つ∧
StageUI_managerによりオブジェクトプールされています。*/
public class ScoreMultiplierText : MonoBehaviour,IPoolable<ScoreMultiplierText>
{
    [SerializeField]float ShowAnimDuration = 0.3f; 
    [SerializeField]float HideAnimDuration = 0.2f;

    [SerializeField]float _distance = 30;
    [SerializeField]TextMeshPro _textMeshPro;
    [SerializeField]MeshRenderer _TMP_meshRenderer;
    Action<ScoreMultiplierText> _onRelease;
    MaterialPropertyBlock _propBlock;

    public void Play(float scoreMultiplier)
    {
        Vector3 directionForMove;
        Initialize(scoreMultiplier);
        Anim().Forget();
        async UniTaskVoid  Anim()
        {
            for (float i = 0; i <= 1; i += Time.deltaTime * (1 / ShowAnimDuration))
            {
                transform.position = directionForMove *_distance * i;
                await UniTask.Yield(PlayerLoopTiming.Update,this.GetCancellationTokenOnDestroy());
            }
            Color outlineColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_OutlineColor");
            Color glowColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_GlowColor");
            //これはTMP_SDF.shaderによるマテリアルのインスペクター上ではThicknessとして書かれている。
            float startOutlineWidth = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_OutlineWidth");
            float startGlowOuter = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_GlowOuter");
            
            float outlineWidth = 0; float glowOuter = 0;
            for (float i = 1; i >= 0; i -= Time.deltaTime * (1 / HideAnimDuration))
            {
                outlineColor.a = i;
                glowColor.a = i;
                outlineWidth = startOutlineWidth * i;
                glowOuter = startGlowOuter * i;
                _propBlock.SetColor("_OutlineColor",outlineColor);
                _propBlock.SetColor("_GlowColor",glowColor);
                _propBlock.SetFloat("_OutlineWidth",outlineWidth);
                _propBlock.SetFloat("_GlowOuter",glowOuter);
                _TMP_meshRenderer.SetPropertyBlock(_propBlock);
                await UniTask.Yield(PlayerLoopTiming.Update,this.GetCancellationTokenOnDestroy());
            }
            _onRelease.Invoke(this);
        }
        void Initialize(float scoreMultiplier)
        {
            _textMeshPro.text = "x" + scoreMultiplier.ToString();
            directionForMove = Player.Current.transform.forward;
            transform.rotation = Quaternion.LookRotation(-directionForMove);
        }
    }
    public void OnCreate(Action<ScoreMultiplierText> onRelease)
    {
        _onRelease = onRelease;
        _propBlock = new MaterialPropertyBlock();
    }
    public void OnRelease()
    {
        //見た目の初期化
        Color startOutlineColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_OutlineColor");
        Color startGlowColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_GlowColor");
        //これはTMP_SDF.shaderによるマテリアルのインスペクター上ではThicknessとして書かれている。
        float startOutlineWidth = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_OutlineWidth");
        float startGlowOuter = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_GlowOuter");

        _propBlock.SetColor("_OutlineColor",startOutlineColor);
        _propBlock.SetColor("_GlowColor",startGlowColor);
        _propBlock.SetFloat("_OutlineWidth",startOutlineWidth);
        _propBlock.SetFloat("_GlowOuter",startGlowOuter);
        _TMP_meshRenderer.SetPropertyBlock(_propBlock);

    }
}
