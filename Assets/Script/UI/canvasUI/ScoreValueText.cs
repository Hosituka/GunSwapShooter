using UnityEngine;
using System.Collections;
using TMPro;
public class ScoreValueText : MonoBehaviour
{
    [SerializeField]RectTransform _scoreValueTextRectTr;
    [SerializeField]TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]float _addScaleTime;
    [SerializeField]float _subtractScaleTime;
    [SerializeField]float _addScale;
    Coroutine _currentCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshProUGUI.text = "0";
    }
    public void UpdateText(float score)
    {
        _textMeshProUGUI.SetText("{0:1}",score);
        if(_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine); 
        }
        _currentCoroutine = StartCoroutine(UpdateAnimCoroutine());
        IEnumerator UpdateAnimCoroutine()
        {
            //増加するアニメーション
            for(float playback = 0;playback <= 1;playback += Time.deltaTime * (1 / _addScaleTime))
            {
                _scoreValueTextRectTr.localScale = Vector3.one + new Vector3(playback * _addScale,playback * _addScale,0);
                yield return null;
            }
            _scoreValueTextRectTr.localScale = Vector3.one + new Vector3(_addScale,_addScale,0);

            //減少するアニメーション
            for(float playback = 0;playback <= 1;playback += Time.deltaTime * (1 / _subtractScaleTime))
            {
                _scoreValueTextRectTr.localScale = Vector3.one + new Vector3((1 - playback) * _addScale,(1 - playback) * _addScale,0);
                yield return null;
            }
            _scoreValueTextRectTr.localScale = Vector3.one;
            _currentCoroutine = null;
        }
    }

}
