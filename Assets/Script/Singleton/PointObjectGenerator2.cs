using UnityEngine;
using UnityEngine.Pool;
//stage2に対応するPointObjectGeneraterのクラス
public class PointObjectGenerator2 : PointObjectGenerater
{
    [Header("#具象クラスによるパラメータ群")]
    [Header("##設定必須項目")]
    [Header("###次の的の生成個数が1なインスタンスを入れる箱(prefab経由)")]
    [SerializeField]BlueTarget _blueTarget1;
    [SerializeField]RedTarget _redTarget1;
    [SerializeField]RedBlueTarget _redBlueTarget1;
    [SerializeField]RedBlueTarget _blueRedTarget1;
    [SerializeField]ButtonMashingTarget _buttonMashingTarget1;
    //####ステージ2固有の奴
    [SerializeField]GunSwapTarget _gunSwapTarget1;
    [SerializeField]MoveBlueTarget _moveBlueTarget1;
    [SerializeField]MoveRedTarget _moveRedTarget1;
    [SerializeField]LineUpTarget _lineUpTarget1;
    [Header("###次の的の生成個数が2なインスタンスを入れる箱(prefab経由)")]
    [SerializeField]BlueTarget _blueTarget2;
    [SerializeField]RedTarget _redTarget2;
    //####ステージ2固有の奴
    [SerializeField]GunSwapTarget _gunSwapTarget2;
    //#次の的の生成個数が1なPointObjectの具象クラスのプール
    ObjectPool<BlueTarget> _blueTargetPool1;
    ObjectPool<RedTarget> _redTargetPool1;
    ObjectPool<RedBlueTarget> _redBlueTargetPool1;
    ObjectPool<RedBlueTarget> _blueRedTargetPool1;
    ObjectPool<ButtonMashingTarget>_buttonMashingTargetPool1;
    //##ステージ2固有の奴
    ObjectPool<GunSwapTarget> _gunSwapTargetPool1;
    ObjectPool<MoveBlueTarget>_moveBlueTargetPool1;
    ObjectPool<MoveRedTarget>_moveRedTargetPool1;
    ObjectPool<LineUpTarget>_lineUpTargetPool1;
    //#次の的の生成個数が2なPointObjectの具象クラスのプール

    ObjectPool<BlueTarget> _blueTargetPool2;
    ObjectPool<RedTarget> _redTargetPool2;
    //##ステージ2固有の奴
    ObjectPool<GunSwapTarget> _gunSwapTargetPool2;


    protected override void SettingObjectPool()
    {
        _timeKeeperPool = ObjectPoolManager.Current.GetObjectPool<TimeKeeper>(_timeKeeper,7,15);
        switch (_difficultAsGenerater)
        {
            case GameManager.Difficult.easy:
                _redTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedTarget>(_redTarget1,10,15);
                _blueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<BlueTarget>(_blueTarget1,10,15);
                _redBlueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_redBlueTarget1,5,10);
                _blueRedTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_blueRedTarget1,5,10);
                _buttonMashingTargetPool1 = ObjectPoolManager.Current.GetObjectPool<ButtonMashingTarget>(_buttonMashingTarget1,5,10);

                _gunSwapTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<GunSwapTarget>(_gunSwapTarget1,10,15);
                _moveRedTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveRedTarget>(_moveRedTarget1,5,10);
                _moveBlueTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveBlueTarget>(_moveBlueTarget1,5,10);
                _lineUpTargetPool1 = ObjectPoolManager.Current.GetObjectPool<LineUpTarget>(_lineUpTarget1,3,5);
            break;
            case GameManager.Difficult.normal:
                _redTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedTarget>(_redTarget1,10,15);
                _blueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<BlueTarget>(_blueTarget1,10,15);
                _redBlueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_redBlueTarget1,5,10);
                _blueRedTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_blueRedTarget1,5,10);
                _buttonMashingTargetPool1 = ObjectPoolManager.Current.GetObjectPool<ButtonMashingTarget>(_buttonMashingTarget1,5,10);

                _gunSwapTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<GunSwapTarget>(_gunSwapTarget1,10,15);
                _moveRedTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveRedTarget>(_moveRedTarget1,5,10);
                _moveBlueTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveBlueTarget>(_moveBlueTarget1,5,10);
                _lineUpTargetPool1 = ObjectPoolManager.Current.GetObjectPool<LineUpTarget>(_lineUpTarget1,3,5);
            break;
            case GameManager.Difficult.hard:
                _redTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedTarget>(_redTarget1,10,15);
                _blueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<BlueTarget>(_blueTarget1,10,15);
                _redBlueTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_redBlueTarget1,5,10);
                _blueRedTargetPool1 = ObjectPoolManager.Current.GetObjectPool<RedBlueTarget>(_blueRedTarget1,5,10);
                _buttonMashingTargetPool1 = ObjectPoolManager.Current.GetObjectPool<ButtonMashingTarget>(_buttonMashingTarget1,5,10);

                _gunSwapTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<GunSwapTarget>(_gunSwapTarget1,10,15);
                _moveRedTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveRedTarget>(_moveRedTarget1,5,10);
                _moveBlueTargetPool1 =  ObjectPoolManager.Current.GetObjectPool<MoveBlueTarget>(_moveBlueTarget1,5,10);
                _lineUpTargetPool1 = ObjectPoolManager.Current.GetObjectPool<LineUpTarget>(_lineUpTarget1,3,5);

                _redTargetPool2 =  ObjectPoolManager.Current.GetObjectPool<RedTarget>(_redTarget2,5,10);
                _blueTargetPool2 =  ObjectPoolManager.Current.GetObjectPool<BlueTarget>(_blueTarget2,5,10);
                _gunSwapTargetPool2 =  ObjectPoolManager.Current.GetObjectPool<GunSwapTarget>(_gunSwapTarget2,5,10);
            break;
        }
    }
    protected override PointObjects GetPointObjectsWithDownCast(PointObjects pointObject)
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
                if(pointObject.NextGeneratableCount == 9)
                {return _blueTargetPool1.Get();}
                else if(pointObject.NextGeneratableCount == 2)
                {return _blueTargetPool2.Get();}
                break;
            case RedBlueTarget redBlueTarget:
                if (redBlueTarget.IsLeftBluePlane)
                {return _blueRedTargetPool1.Get();}
                if (!redBlueTarget.IsLeftBluePlane)
                {return _redBlueTargetPool1.Get();}
                break;
            case ButtonMashingTarget:
                return _buttonMashingTargetPool1.Get();
            case GunSwapTarget:
                if(pointObject.NextGeneratableCount == 1)
                {return _gunSwapTargetPool1.Get();}
                else if(pointObject.NextGeneratableCount == 2)
                {return _gunSwapTargetPool2.Get();}
                break;
            case MoveBlueTarget:
                DistanceOfGenerate -= 3;
                return _moveBlueTargetPool1.Get();
            case MoveRedTarget:
                DistanceOfGenerate -= 3;
                return _moveRedTargetPool1.Get();
            case LineUpTarget:
                return _lineUpTargetPool1.Get();

        }
        Debug.LogError("ステージ2では登場する事が想定されていない、PointObjectの具象クラスに属する物が渡されました。");
        return null;
    }

}

