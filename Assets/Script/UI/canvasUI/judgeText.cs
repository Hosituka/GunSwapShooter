using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class JudgeText : MonoBehaviour,IPoolable<JudgeText>
{
    [SerializeField]TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]RectTransform _rectTransform;
    [SerializeField]float _addScaleTime = 0.5f; 
    [SerializeField]float _moveUpDistance = 20;
    Action<JudgeText> _onRelease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play()
    {
        FadeOut().Forget();
        async UniTaskVoid  FadeOut()
        {
            Color vertexColor = _textMeshProUGUI.color;
            for (float a = 1; a >= 0; a -= Time.deltaTime * (1 / _addScaleTime))
            {
                vertexColor.a = a;
                _textMeshProUGUI.color = vertexColor;
                _rectTransform.position += Vector3.up * Time.deltaTime * (1 / _addScaleTime) * _moveUpDistance;              
                await UniTask.Yield(PlayerLoopTiming.Update,destroyCancellationToken);
            }
            vertexColor.a = 0;
            _textMeshProUGUI.color = vertexColor;
            _onRelease.Invoke(this);
        }
    }
    public void OnCreate(Action<JudgeText> onRelease)
    {
        _onRelease = onRelease;
    }
    public void OnRelease()
    {
        _rectTransform.position -= Vector3.up * _moveUpDistance;                
    }
    // Update is called once per frame
}
