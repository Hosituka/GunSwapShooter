using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//PointObjectの具象クラスの位置を表示する責務を持つ∧StageUI_managerによりオブジェクトプールされています。
public class Indicator : MonoBehaviour,IPoolable<Indicator>
{
    [SerializeField]Transform _indicatorsUI_Tr;
    [SerializeField]Camera _camera;
    [SerializeField]GameObject _mainObj;
    [SerializeField]RectTransform _arrowRectTr;
    [SerializeField]Image _innerArrowImage;
    [SerializeField]Image _ArrowOutlineImage;
    [SerializeField]RectTransform _rectTr;
    [SerializeField]Transform _targetTr;

    Vector3 _targetViewportPos;
    Vector3 _targetScreenPos;

    Vector3 _targetViewportPosAsCenter;


    Vector2 _clampedTargetScreenPos;

    bool _isInCamera;
    Action<Indicator> _onRelease{get;set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(Transform targetTr)
    {
        _targetTr = targetTr;
        transform.SetParent(_indicatorsUI_Tr);
        transform.SetSiblingIndex(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(_targetTr == null) return;
        _targetViewportPos = _camera.WorldToViewportPoint(_targetTr.position);
        _targetScreenPos = _camera.WorldToScreenPoint(_targetTr.position);
        _clampedTargetScreenPos = new Vector2(Mathf.Clamp(_targetScreenPos.x, 0, Screen.width), Mathf.Clamp(_targetScreenPos.y, 0, Screen.height));
        _targetViewportPosAsCenter = new Vector2(Mathf.Clamp01(_targetViewportPos.x), Mathf.Clamp01(_targetViewportPos.y));
        _targetViewportPosAsCenter = ((_targetViewportPosAsCenter - new Vector3(0.5f, 0.5f)) * 2).normalized;



        //インディケーターの位置や向きと描画の処理
        _isInCamera = _targetViewportPos.x >= 0 & _targetViewportPos.x <= 1 & _targetViewportPos.y >= 0 & _targetViewportPos.y <= 1;
        if (_targetViewportPos.z >= 0)///カメラの前側
        {
            if (_isInCamera)
            {
                //Debug.Log("視野内です");
                _mainObj.SetActive(false);
            }
            else
            {
                //Debug.Log("視野外です");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(Mathf.Clamp01(_targetViewportPos.x),Mathf.Clamp01(_targetViewportPos.y));
                _rectTr.position = _clampedTargetScreenPos;
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, _targetViewportPosAsCenter);
            }

        }
        else///カメラの後ろ側
        {
            if (_isInCamera)
            {
                //Debug.Log("カメラの真後ろ側です");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(1 - (_targetViewportPosAsCenter.x + 1)/2,1 - (_targetViewportPosAsCenter.y + 1)/2);
                //_rectTr.position = new Vector2(Screen.width,Screen.height) - _clampedTargetScreenPos;
                _rectTr.position = new Vector2(Screen.width/2,Screen.height/2) - new Vector2(_targetViewportPosAsCenter.x * Screen.width / 2,_targetViewportPosAsCenter.y * Screen.height / 2);
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, -_targetViewportPosAsCenter);
            }
            else
            {
                //Debug.Log("カメラの後ろ側面です。");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(1 - Mathf.Clamp01(_targetViewportPos.x),1 - Mathf.Clamp01(_targetViewportPos.y));
                _rectTr.position = new Vector2(Screen.width,Screen.height) - _clampedTargetScreenPos;
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, -_targetViewportPosAsCenter);

            }
        }
    }
    public void PlayActivationTimer(float duration)
    {
        StartCoroutine(ActivationTimer());
        IEnumerator ActivationTimer(){
            Color _innerArrowColor = Color.black + new Color(0.04f,0.04f,0.04f);
            _ArrowOutlineImage.color = Color.white;
            for(float playback = 0;playback < duration; playback += Time.deltaTime)
            {
                _innerArrowColor.a = playback / duration;
                _innerArrowImage.color = _innerArrowColor;
                yield return null;
            }
            _innerArrowColor.a = 1;
            _innerArrowImage.color = _innerArrowColor;
        }

    }

    public void PlayDeActivationTimer(float duration)
    {
        StartCoroutine(DeActivationTimer());
        IEnumerator DeActivationTimer(){
            Color _innerArrowColor = Color.white;
            _ArrowOutlineImage.color = Color.black;
            for(float playback = 0;playback < duration; playback += Time.deltaTime)
            {
                _innerArrowColor.a = 1 - playback / duration;
                _innerArrowImage.color = _innerArrowColor;
                yield return null;
            }
            _innerArrowColor.a = 0;
            _innerArrowImage.color = _innerArrowColor;
        }

    }
    
    public void Destroy()
    {
        _onRelease.Invoke(this);
    }
    public void OnCreate(Action<Indicator> onRelease)
    {
        _onRelease = onRelease;
    }

    public void OnRelease()
    {
    }


}
