using UnityEngine;
using TMPro;
using System.Collections;
using System;
//スコアの増加量を表すテキストのアニメーションを表す責務を持つ∧StageUI_managerによりオブジェクトプールされている。
public class AddScoreText : MonoBehaviour
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
        StartCoroutine(FadeOut());
        IEnumerator  FadeOut()
        {
            _textMeshProUGUI.SetText("{0:1}＋",_addScore);
            Color vertexColor = _textMeshProUGUI.color;
            for (float animTime = 1; animTime >= 0; animTime -= Time.deltaTime * (1/_animDuration))
            {
                vertexColor.a = animTime;
                _textMeshProUGUI.color = vertexColor;
                _rectTransform.position += Vector3.up * Time.deltaTime * (1/_animDuration) * _moveUpDistance;              
                yield return null;
            }
            vertexColor.a = 0;
            _textMeshProUGUI.color = vertexColor;
            Reset();
            _onRelease.Invoke(this);
        }
        void Initialize(float addScore)
        {
            _addScore = addScore;  
        }
        void Reset()
        {
            _rectTransform.position -= Vector3.up * _moveUpDistance;                
        }
    }
    public void SetOnRelease(Action<AddScoreText> onRelease)
    {
        _onRelease = onRelease;
    }
}
