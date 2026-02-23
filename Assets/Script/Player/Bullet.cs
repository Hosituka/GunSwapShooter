using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
//PlayerGunが生成する銃弾です。　オブジェクトプールにより管理されています。
public class Bullet : MonoBehaviour,IPoolable<Bullet>
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] TrailRenderer _trailRenderer;
    [SerializeField] Rigidbody _rb;
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] float _speed = 5000;
    [SerializeField] float _maxWidth = 0.5f;
    [SerializeField] float _minWidth = 0.3f;
    [SerializeField] float _maxAlpha = 0.5f;
    [SerializeField] float _minAlpha = 0.3f;

    Action<Bullet> _onRelease;
    public void SetOnRelease(Action<Bullet> onRelease)
    {
        _onRelease = onRelease;
    }
    public void Initialize()
    {
        _rb.linearVelocity = Vector3.zero;
        _meshRenderer.enabled = true;
        _rb.isKinematic = false;
        SetTrail();
        void SetTrail()
        {
            _trailRenderer.enabled = false;
            _trailRenderer.Clear();
            _trailRenderer.enabled = true;
            _trailRenderer.widthMultiplier = Random.Range(_minWidth,_maxWidth);
            _trailRenderer.colorGradient.alphaKeys[0].alpha = Random.Range(_minAlpha,_maxAlpha);
        }
    }
    void FixedUpdate()
    {
        _rb.linearVelocity = transform.forward * _speed;
    }
    void Update()
    {
        if(transform.position.magnitude >= 500)
        {
            _onRelease?.Invoke(this);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Explosion();
    }
    void Explosion()
    {
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            _explosion.Play();
            _meshRenderer.enabled = false;
            _rb.isKinematic = true;
            yield return new WaitWhile(() => _explosion.isPlaying == true);
            _onRelease?.Invoke(this);
        }

    }
}
