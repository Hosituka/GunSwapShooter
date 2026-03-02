using System;
using UnityEngine;
using UnityEngine.Pool;
/*
オブジェクトプールしたいゲームオブジェクトを使うときに間に入るという責務を持ちます。
またステージの開始から終わりまでのシングルトンなクラスです。
※ちょっと能力が足らなさ過ぎて、完成していません、めちゃくちゃ拡張性が低いです。使わないでください。ジェネリック関数ってなんだよ。これ使えれば完成しそう
*/
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Current{get; private set;}
    bool _isPrewarming;
    void Awake()
    {
        if (Current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Current = this;
    }
    public ObjectPool<T> GetObjectPool<T>(T prefab,Transform parentTr,int defaultCapacityOfPool,int maxSizeOfPool)where T:Component,IPoolable<T>
    {
        ObjectPool<T> objectPool = null;
        objectPool = new ObjectPool<T>
        (   //プールの在庫が足りないときに新しく作る処理
            createFunc: () =>
            {
                T poolTarget =  Instantiate(prefab,parentTr);
                poolTarget.OnCreate((e)=>objectPool.Release(e));
                return poolTarget;
            },
            // プールから借りる時の処理
            actionOnGet: (temp)=>temp.gameObject.SetActive(true),         
            // プールに返す時の処理
            actionOnRelease: (temp)=>
            {
                temp.gameObject.SetActive(false);
                if(_isPrewarming) return;
                temp.OnRelease();
            }, 
            // プールが溢れた時に破棄する処理
            actionOnDestroy: (temp)=>Destroy(temp.gameObject), 
             // 最初に用意する目安
            defaultCapacity: defaultCapacityOfPool, 
            // 最大貯蓄数            
            maxSize: maxSizeOfPool                       

        );
        PrewarmPool<T>(objectPool,defaultCapacityOfPool);
        return objectPool;
    }
    public ObjectPool<T> GetObjectPool<T>(T prefabComponent,int defaultCapacityOfPool,int maxSizeOfPool)where T:Component,IPoolable<T>
    {
        ObjectPool<T> objectPool = null;
        objectPool = new ObjectPool<T>
        (   //プールの在庫が足りないときに新しく作る処理
            createFunc: () =>
            {
                T poolTarget =  Instantiate(prefabComponent,transform);
                poolTarget.OnCreate((e)=>objectPool.Release(e));
                return poolTarget;
            },
            actionOnGet: (temp)=>temp.gameObject.SetActive(true),         // プールから借りる時の処理
            // プールに返す時の処理
            actionOnRelease: (temp)=>
            {                
                if(!_isPrewarming)
                {temp.OnRelease();}

                temp.gameObject.SetActive(false);

            }, 
            actionOnDestroy: (temp)=>Destroy(temp.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: defaultCapacityOfPool,              // 最初に用意する目安
            maxSize: maxSizeOfPool                       // 最大貯蓄数

        );
        PrewarmPool<T>(objectPool,defaultCapacityOfPool);
        return objectPool;

    }
    //あるオブジェクトプールに対して、事前に在庫を確保させる処理
    void PrewarmPool<T2>(ObjectPool<T2> objectPool, int preInsntantiateAmount)where T2: Component,IPoolable<T2>
    {
        _isPrewarming = true;
        T2[] tempArray = new T2[preInsntantiateAmount];
        for(int i = 0;i < preInsntantiateAmount; i++)
        {
            tempArray[i] = objectPool.Get();
        }
        foreach(T2 temp in tempArray)
        {
            objectPool.Release(temp);
        }
        _isPrewarming = false;
    }

}

public interface IPoolable<T> where T : Component
{
    //ObjectPoolの在庫を追加するために走る処理
    void OnCreate(Action<T> onRelease);
    //IPoolableを実装している物が、ObjectPoolに返すときに走る処理
    void OnRelease()
    {
        
    }
}
