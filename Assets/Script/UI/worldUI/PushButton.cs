using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Audio;

public class PushButton : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
{
    
    [SerializeField] Collider[] _colliders;
    [SerializeField] UnityEvent _unityEvent;
    [SerializeField] MeshRenderer _buttonMR;
    [SerializeField] Transform _buttonTr;
    [SerializeField] Color _downColor;
    [SerializeField] Vector3 _offset = new Vector3(0, -0.35f, 0);
    Color _startColor;
    Vector3 _startPosition;
    MaterialPropertyBlock _propBlock;
    static event Action<GameObject> _allCheckColliders;
    static int _downCount;
    //ほかのボタンにより、この自作コンポーネントにアタッチされたゲームオブジェクトが有効化された時、リセットする処理
    void OnEnable()
    {
        _allCheckColliders += CheckColliders;
        _startColor = _buttonMR.sharedMaterial.GetColor("_BaseColor");
        _startPosition = _buttonTr.localPosition;
        _propBlock = new MaterialPropertyBlock();
        SetUpState();
    }
    void OnDisable() {
        _allCheckColliders -= CheckColliders;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        SetDownState();
        _downCount++;
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(_downCount > 0)
        {
            SetDownState();
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(_downCount > 0)
        {
            SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
            SetUpState();
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        _downCount --;
        _allCheckColliders.Invoke(pointerEventData.pointerCurrentRaycast.gameObject);
    }
    private void SetDownState()
    {
        _propBlock.SetColor("_BaseColor",_downColor);
        _buttonMR.SetPropertyBlock(_propBlock);
        _buttonTr.localPosition = _startPosition + _offset;
    }

    private void SetUpState()
    {
        _propBlock.SetColor("_BaseColor",_startColor);
        _buttonMR.SetPropertyBlock(_propBlock);
        _buttonTr.localPosition = _startPosition;
    }
    void CheckColliders(GameObject _pointerCurrentRaycastObj)
    {
        foreach(Collider collider in _colliders)
        {
            if(collider.gameObject == _pointerCurrentRaycastObj)
            {
                SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
                _unityEvent.Invoke();
                SetUpState();
                _pointerCurrentRaycastObj = null;
                _downCount = 0;
            }
        }
    }

}
