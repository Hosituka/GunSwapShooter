using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Cysharp.Threading.Tasks;
//スコアの増加量を表すテキストのアニメーションを表す責務を持つ∧StageUI_managerによりオブジェクトプールされている。
public class AddScoreText : MonoBehaviour,IPoolable<AddScoreText>
{
    [SerializeField]
    float _animDuration = 0.5f;
    [SerializeField]TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]RectTransform _rectTransform;
    [SerializeField]float _moveUpDistance = 20;
    [Header("表示用"),SerializeField]float _addScore;
    Action<AddScoreText> _onRelease;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play(float addScore)
    {
        Initialize(addScore);
        FadeOut().Forget();
        async UniTaskVoid FadeOut()
        {
            _textMeshProUGUI.SetText("{0:1}＋",_addScore);
            Color vertexColor = _textMeshProUGUI.color;
            for (float animTime = 1; animTime >= 0; animTime -= Time.deltaTime * (1/_animDuration))
            {
                vertexColor.a = animTime;
                _textMeshProUGUI.color = vertexColor;
                _rectTransform.position += Vector3.up * Time.deltaTime * (1/_animDuration) * _moveUpDistance;              
                await UniTask.Yield(PlayerLoopTiming.Update,destroyCancellationToken);
            }
            vertexColor.a = 0;
            _textMeshProUGUI.color = vertexColor;
            _onRelease.Invoke(this);
        }
        void Initialize(float addScore)
        {
            _addScore = addScore;  
        }
    }
    public void OnCreate(Action<AddScoreText> onRelease)
    {
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        _rectTransform.position -= Vector3.up * _moveUpDistance;                
    }
}
