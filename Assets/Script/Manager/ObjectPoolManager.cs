using UnityEngine;
using UnityEngine.Pool;
/*
オブジェクトプールしたいゲームオブジェクトを使うときに間に入るという責務を持ちます。
またステージの開始から終わりまでのシングルトンなクラスです。
※ちょっと能力が足らなさ過ぎて、完成していません、めちゃくちゃ拡張性が低いです。使わないでください。ジェネリック関数ってなんだよ。これ使えれば完成しそう
*/
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Current;
    void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField]Bullet _redBullet;
    [SerializeField]Bullet _blueBullet;
    ObjectPool<Bullet> _redBulletPool;
    ObjectPool<Bullet> _blueBulletPool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // プールの設定
        _redBulletPool = new ObjectPool<Bullet>(
            createFunc: ()=>Instantiate(_redBullet),       // 足りない時に新しく作る処理
            actionOnGet: (Bullet bullet)=>bullet.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (Bullet bullet)=>bullet.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (Bullet bullet)=>Destroy(bullet.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: 10,              // 最初に用意する目安
            maxSize: 30                       // 最大貯蓄数
        );       
        // プールの設定
        _blueBulletPool = new ObjectPool<Bullet>(
            createFunc: ()=>Instantiate(_blueBullet),       // 足りない時に新しく作る処理
            actionOnGet: (Bullet bullet)=>bullet.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (Bullet bullet)=>bullet.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (Bullet bullet)=>Destroy(bullet.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: 10,              // 最初に用意する目安
            maxSize: 30                       // 最大貯蓄数
        );       


    }
    public void GetRedBullet(Vector3 position,Quaternion rotation)
    {
        Bullet bullet = _redBulletPool.Get();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetOnRelease((Bullet b)=>_redBulletPool.Release(b));
        //インスペクターでプールされているゲームオブジェクト確認用
        bullet.transform.SetParent(transform);
    }
    public void GetBlueBullet(Vector3 position,Quaternion rotation)
    {
        Bullet bullet = _blueBulletPool.Get();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetOnRelease((Bullet b)=>_blueBulletPool.Release(b));
        //インスペクターでプールされているゲームオブジェクト確認用
        bullet.transform.SetParent(transform);
    }

}
