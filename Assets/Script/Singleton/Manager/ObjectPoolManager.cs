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
    public ObjectPool<T> GeneratePool<T>(T prefab,Transform parentTr,int defaultCapacityOfPool,int maxSizeOfPool)where T:Component,IPoolable<T>
    {
        ObjectPool<T> objectPool = null;
        objectPool = new ObjectPool<T>
        (   //プールの在庫が足りないときに新しく作る処理
            createFunc: () =>
            {
                T poolTarget =  Instantiate(prefab,parentTr);
                poolTarget.SetOnRelease((e)=>objectPool.Release(e));
                return poolTarget;
            },
            actionOnGet: (temp)=>temp.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (temp)=>temp.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (temp)=>Destroy(temp.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: defaultCapacityOfPool,              // 最初に用意する目安
            maxSize: maxSizeOfPool                       // 最大貯蓄数

        );

        return objectPool;
    }
    public ObjectPool<T> GenerateObjectPool<T>(T prefab,int defaultCapacityOfPool,int maxSizeOfPool)where T:Component,IPoolable<T>
    {
        ObjectPool<T> objectPool = null;
        objectPool = new ObjectPool<T>
        (   //プールの在庫が足りないときに新しく作る処理
            createFunc: () =>
            {
                T poolTarget =  Instantiate(prefab,transform);
                poolTarget.SetOnRelease((e)=>objectPool.Release(e));
                return poolTarget;
            },
            actionOnGet: (temp)=>temp.gameObject.SetActive(true),         // プールから借りる時の処理
            actionOnRelease: (temp)=>temp.gameObject.SetActive(false), // プールに返す時の処理
            actionOnDestroy: (temp)=>Destroy(temp.gameObject), // プールが溢れた時に破棄する処理
            defaultCapacity: defaultCapacityOfPool,              // 最初に用意する目安
            maxSize: maxSizeOfPool                       // 最大貯蓄数

        );

        return objectPool;
    }

    //あるオブジェクトプールに対して、事前に在庫を確保させる処理
    public void PrewarmPool<T>(ObjectPool<T> objectPool, int preInsntantiateAmount)where T: Component
    {
        T[] tempArray = new T[preInsntantiateAmount];
        for(int i = 0;i < preInsntantiateAmount; i++)
        {
            tempArray[i] = objectPool.Get();
        }
        foreach(T temp in tempArray)
        {
            objectPool.Release(temp);
        }
    }

}
public interface IPoolable<T> where T : Component
{
    void SetOnRelease(Action<T> onRelease);
}
