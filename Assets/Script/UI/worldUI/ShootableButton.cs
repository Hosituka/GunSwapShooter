using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ShootableButton : MonoBehaviour,IAimEnterHandler,IAimExitHandler
{
    [SerializeField] Collider[] _colliderArray;
    [SerializeField] MeshRenderer _shootableButtonMR;
    [SerializeField,Range(0,1)] float _aimBrightness = 0.8f;
    [SerializeField] TextMeshPro _explain3dTMP;
    [SerializeField] TextMeshPro _needShotCount3dTMP;
    [SerializeField] PointObjectAnimator _breakAnimator; 
    [SerializeField]UnityEvent _onShoot;
    [SerializeField] int _needShotCount;
    [SerializeField] float _regenDelay;

    int _maxNeedShotCount;
    Color _startMaterialColor;
    Color _startExplain3dTMP_Color;
    Color _startNeedShotCount3dTMP_Color;

    Coroutine _currentRegenCoroutine;
    void Start()
    {
        //sharedMatrials[1]はpurpleマテリアルでないといけません。
        _startMaterialColor = _shootableButtonMR.sharedMaterials[1].GetColor("_BaseColor");
        _startExplain3dTMP_Color = _explain3dTMP.color;
        _startNeedShotCount3dTMP_Color = _needShotCount3dTMP.color;

        _maxNeedShotCount = _needShotCount;
        _needShotCount3dTMP.gameObject.SetActive(false);
        _propBlock = new MaterialPropertyBlock();
    }
    CancellationTokenSource _cts;
    async UniTaskVoid Regen(float delay,CancellationToken ct)
    {
        await UniTask.WaitForSeconds(delay,cancellationToken:ct);
        while (_needShotCount < _maxNeedShotCount)
        {
            _needShotCount++;
            _needShotCount3dTMP.text = _needShotCount.ToString();
            await UniTask.WaitForSeconds(0.4f,cancellationToken:ct);
        }
        _needShotCount3dTMP.gameObject.SetActive(false);
        _explain3dTMP.gameObject.SetActive(true);
        _currentRegenCoroutine = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int _onAimCount;
    MaterialPropertyBlock _propBlock;
    public void OnAimEnter(KindOfGunRay kindOfGunRay)
    {
        _onAimCount++;
        if(_onAimCount != 1) return;
        _propBlock.SetColor("_BaseColor",_startMaterialColor * _aimBrightness);
        _shootableButtonMR.SetPropertyBlock(_propBlock,1);
        _explain3dTMP.color = _startExplain3dTMP_Color * _aimBrightness; 
        _needShotCount3dTMP.color = _startNeedShotCount3dTMP_Color * _aimBrightness;
    }
    public void OnAimExit(KindOfGunRay kindOfGunRay)
    {
        _onAimCount--;
        if(_onAimCount != 0) return;
        _propBlock.SetColor("_BaseColor",_startMaterialColor);
        _shootableButtonMR.SetPropertyBlock(_propBlock,1);
        _explain3dTMP.color = _startExplain3dTMP_Color; 
        _needShotCount3dTMP.color = _startNeedShotCount3dTMP_Color;
    }
    int _collisionCount;
    void OnCollisionEnter(Collision collision)
    {
        _collisionCount++;
        Debug.Log(_collisionCount);
        if(_collisionCount != 1) return;
        if(_needShotCount == 0) return;
        if (collision.gameObject.CompareTag("BlueBullet") || collision.gameObject.CompareTag("RedBullet"))
        {
            _needShotCount--;
            _needShotCount3dTMP.text = _needShotCount.ToString();
            if(_cts != null)
            { _cts.Cancel();_cts.Dispose();}
            _cts = new CancellationTokenSource();
            Regen(_regenDelay,_cts.Token).Forget();

            if(_needShotCount == _maxNeedShotCount - 1)
            {
                _explain3dTMP.gameObject.SetActive(false);
                _needShotCount3dTMP.gameObject.SetActive(true);
            }

            if(_needShotCount == 0)
            {
                BreakAsync().Forget();
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        _collisionCount--;
    }
    async UniTaskVoid BreakAsync()
    {
        _breakAnimator.PlaySpinThenExplode(transform.position,Color.magenta,14).Forget();
        await UniTask.WaitWhile(()=> _breakAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Exploding);
        Utility.ChangeEnabledColliders(_colliderArray,false);
        await UniTask.WaitWhile(()=> _breakAnimator.CurtSpinThenExplodePhase != PointObjectAnimator.SpinThenExplodePhase.Completed);
        _onShoot.Invoke();
        Destroy(gameObject);
    }

    // Update is called once per frame
}

