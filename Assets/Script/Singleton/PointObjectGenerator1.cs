using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Pool;
//stage1に対応するPointObjectGeneraterのクラス
public class PointObjectGenerator1 : PointObjectGenerater
{
    [Header("#具象クラスによるパラメータ群")]
    [Header("##設定必須項目")]
    [Header("###次の的の生成個数が1なインスタンスを入れる箱(prefab経由)")]
    [SerializeField]BlueTarget _blueTarget1;
    [SerializeField]RedTarget _redTarget1;
    [SerializeField]RedBlueTarget _redBlueTarget;
    [SerializeField]RedBlueTarget _blueRedTarget;
    [SerializeField]ButtonMashingTarget _buttonMashingTarget;
    [Header("###次の的の生成個数が2なインスタンスを入れる箱(prefab経由)")]
    [SerializeField]BlueTarget _blueTarget2;
    [SerializeField]RedTarget _redTarget2;

    //#次の的の生成個数が1なPointObjectの具象クラスのプール
    ObjectPool<BlueTarget> _blueTargetPool1;
    ObjectPool<RedTarget> _redTargetPool1;
    ObjectPool<RedBlueTarget> _redBlueTargetPool;
    ObjectPool<RedBlueTarget> _blueRedTargetPool;
    ObjectPool<ButtonMashingTarget>_buttonMashingTargetPool;
    //#次の的の生成個数が2なPointObjectの具象クラスのプール

    ObjectPool<BlueTarget> _blueTargetPool2;
    ObjectPool<RedTarget> _redTargetPool2;


    protected override void SettingObjectPool()
    {
        Debug.Log("Test");
        _timeKeeperPool = ObjectPoolManager.Current.GetObjectPool<TimeKeeper>(_timeKeeper,_objectPoolManagerTr,7,15);
        switch (_difficultAsGenerater)
        {
            case GameManager.Difficult.normal:
                _redTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedTarget>(_redTarget1,_objectPoolManagerTr,10,15);

                _blueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<BlueTarget>(_blueTarget1,_objectPoolManagerTr,10,15);

                _redBlueTargetPool = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_redBlueTarget,_objectPoolManagerTr,5,10);

                _blueRedTargetPool = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_blueRedTarget,_objectPoolManagerTr,5,10);

                _buttonMashingTargetPool = ObjectPoolManager.Current.GetObjectPool<ButtonMashingTarget>(_buttonMashingTarget,_objectPoolManagerTr,5,10);


            break;
        }
    }
    protected override PointObject GetPointObjectWithDownCast(PointObject pointObject)
    {
        switch (pointObject)
        {
            case RedTarget:
                if(pointObject.NextGeneratableCount == 1)
                {return _redTargetPool1.Get();}
                else if(pointObject.NextGeneratableCount == 2)
                {return _redTargetPool2.Get();}
                break;
            case BlueTarget:
                if(pointObject.NextGeneratableCount == 1)
                {return _blueTargetPool1.Get();}
                else if(pointObject.NextGeneratableCount == 2)
                {return _blueTargetPool2.Get();}
                break;
            case RedBlueTarget redBlueTarget:
                if (redBlueTarget.IsLeftBluePlane)
                {return _blueRedTargetPool.Get();}
                if (!redBlueTarget.IsLeftBluePlane)
                {return _redBlueTargetPool.Get();}
                break;
            case ButtonMashingTarget:
                return _buttonMashingTargetPool.Get();
        }
        Debug.LogError("ステージ1では登場する事が想定されていない、PointObjectの具象クラスに属する物が渡されました。");
        return null;
    }
}
