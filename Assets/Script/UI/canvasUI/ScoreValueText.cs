using UnityEngine;
using System.Collections;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
public class ScoreValueText : MonoBehaviour
{
    [SerializeField]RectTransform _scoreValueTextRectTr;
    [SerializeField]TextMeshProUGUI _textMeshProUGUI;
    [SerializeField]float _addScaleTime;
    [SerializeField]float _subtractScaleTime;
    [SerializeField]float _addScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshProUGUI.text = "0";
    }
    CancellationTokenSource _cts;
    public void UpdateText(float score)
    {
        if(_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose(); 
        }
        _textMeshProUGUI.SetText("{0:1}",score);
        _cts = new CancellationTokenSource();
        UpdateAnimCoroutine(_cts.Token).Forget();
        async UniTaskVoid UpdateAnimCoroutine(CancellationToken cts)
        {
            //増加するアニメーション
            for(float playback = 0;playback <= 1;playback += Time.deltaTime * (1 / _addScaleTime))
            {
                _scoreValueTextRectTr.localScale = Vector3.one + new Vector3(playback * _addScale,playback * _addScale,0);
                await UniTask.Yield(PlayerLoopTiming.Update,cts);
            }
            _scoreValueTextRectTr.localScale = Vector3.one + new Vector3(_addScale,_addScale,0);

            //減少するアニメーション
            for(float playback = 0;playback <= 1;playback += Time.deltaTime * (1 / _subtractScaleTime))
            {
                _scoreValueTextRectTr.localScale = Vector3.one + new Vector3((1 - playback) * _addScale,(1 - playback) * _addScale,0);
                await UniTask.Yield(PlayerLoopTiming.Update,cts);
            }
            _scoreValueTextRectTr.localScale = Vector3.one;
            _cts = null;
        }
    }

}
