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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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