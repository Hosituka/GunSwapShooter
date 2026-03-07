using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using System;

using Random = UnityEngine.Random;
//stageごとに存在するPointObjectGeneraterの親クラスです。
public abstract class PointObjectGenerater : MonoBehaviour
{
    [Header("#親クラスによるパラメータ群")]
    [SerializeField]protected GameManager.Difficult _difficultAsGenerater;
    [Header("##生成に関するパラメータ群")]
    [SerializeField]float _defaultDistanceOfGenerate = 40;
    public int GenerateHalfYawAngle = 40;
    public int GenerateYawStep = 15;

    public int GenerateHalfPitchAngle = 30;
    public int GeneratePitchStep = 20;

    public float PerlinNoiseScale;
    public float PerlinNoiseMagni;
    public float MaxPointObjectCost = 10;
    public float Bpm;


    [Header("##生成候補となるパラメータ群")]
    [SerializeField] PointObjects[] _pointObjects;

    [Header("##設定必須項目")]
    [SerializeField]protected TimeKeeper _timeKeeper;
    [Header("##表示専用")]
    //生成が完了したことを表すフラグ
    [SerializeField]bool _isRequestComplete;
    //生成可能であることを伝えるフラグ、生成失敗時にはfalseとなり、PointObjectの数が減ったらtrueとなる。
    [SerializeField]bool _isGeneratable;
    [SerializeField]float _fourthNote;
    [SerializeField]float _eighthNote;
    [SerializeField]float _sixteenthNote;
    [SerializeField]float _perlinNoiseSeed;
    //一回に生成するPointObjectの数
    [SerializeField] int _generateCount = 1;
    [SerializeField]float _sumPointObjectCost;
    public static PointObjectGenerater Current{get;private set;}
    public (int x, int y) PointObjectMapLength;

    protected ObjectPool<TimeKeeper> _timeKeeperPool;
    protected Transform _objectPoolManagerTr;
    float _currentBaseActivationDelay;
    bool[,] _pointObjectMap;
    //#SearchPointObjectPosでしか使われない奴
    //##訪れた場所にチェック入れる奴
    bool[,] _visited;
    //##探索候補を入れるキュー
    Queue<Vector2Int> _searchCandidates;
    Vector2Int[] _offsets = {Vector2Int.right,Vector2Int.down,Vector2Int.left,Vector2Int.up};
    //#その他
    Transform _playerTr;
    void Start()
    {
        //特殊なシングルトン化する処理、具体的にはインスタンスメンバの難易度と、GameManagerが持つ難易度が一致しているインスタンスだけ残す処理
        if(_difficultAsGenerater != GameManager.Current.CurrentDifficult)
        {
            Destroy(this.gameObject);
            return;
        }
        if (Current != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Current = this;
        //#その他
        _playerTr = Player.Current.transform;
        _objectPoolManagerTr = ObjectPoolManager.Current.transform;
        PointObjectMapLength.x = (GenerateHalfYawAngle / GenerateYawStep * 2) + 1;
        PointObjectMapLength.y = (GenerateHalfPitchAngle / GeneratePitchStep * 2) + 1;
        _pointObjectMap = new bool[PointObjectMapLength.x, PointObjectMapLength.y];
        InitializeSertchPointObjectPos();
        SetNotes();
        //#各objectPoolの設定
        SettingObjectPool();
        //#最初の生成の実行
        StartCoroutine(StayFadeIn());
        IEnumerator StayFadeIn()
        {
            yield return new WaitWhile(()=>GameManager.Current.FadeInComplete == false);
            RequestGeneration(2f,0.5f,1,1);
        }
        //#SertchPointObjectPosにだけでしか使われない奴のインスタンス生成
        void InitializeSertchPointObjectPos()
        {
            _visited = new bool[PointObjectMapLength.x,PointObjectMapLength.y];
            _searchCandidates = new Queue<Vector2Int>();
        }
    }
    //#こいつの子クラスでオブジェクトプールを設定する処理、こいつにより呼ばれる。
    protected abstract void SettingObjectPool();
    public void RequestGeneration(float nextActivationDelay,float delay,float perlinNoiseMagni,int generataCount)
    {
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            yield return new WaitForSeconds(delay);            
            _currentBaseActivationDelay = nextActivationDelay;
            PerlinNoiseMagni = perlinNoiseMagni;
            _perlinNoiseSeed += Random.Range(0f,1f) * PerlinNoiseScale * PerlinNoiseMagni;
            _generateCount = generataCount;
            _isRequestComplete = TryGeneratePointObjects();
            //一発目の生成成功時
            if(_isRequestComplete)
            {
                yield break;
            }//一発目の生成失敗時
            else
            {
                _isGeneratable = false;
                while(_isRequestComplete == false)
                {
                    if(!_isGeneratable)
                    {
                        yield return null;
                        continue;
                    }
                    if(enabled == false) yield break;
                    _isRequestComplete = TryGeneratePointObjects();
                    yield return null;
                }
            }

        }
    }
    public float DistanceOfGenerate{get;protected set;}
    //指定された回数のPointObjectsの生成に挑戦する関数。すべてのPointObjectを生成できなかった時はfalseを返し、全て生成できた成功時はtrueを返す。
    bool TryGeneratePointObjects()
    {
        //#計画フェーズ
        int generatedCount = 0;
        DistanceOfGenerate = _defaultDistanceOfGenerate;
        List<PointObjects> generatablePointObjectsList = new List<PointObjects>();
        while (generatedCount < _generateCount)
        {
            Vector2Int pointObjectPos = SearchPointObjectPos();
            if(pointObjectPos == -Vector2Int.one){Debug.Log(_generateCount + "目で生成可能な場所がありません");
                break;
            }
            PointObjects pointObjects = GetGeneratablePointObjects();
            if(pointObjects == null){Debug.Log(_generateCount + "目で生成予算オーバーです。");
                break;
            }
            //予算と場所の観点から生成可能なpointObjectsとして登録する。
            generatablePointObjectsList.Add(pointObjects);
            //#pointObjectsの座標や回転を定める処理
            //##前方ベクトルにランダムなヨー角を適用
            transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis((pointObjectPos.x - (GenerateHalfYawAngle / GenerateYawStep)) * GenerateYawStep, Vector3.up) * Vector3.forward);
            //##上によって作られたベクトルにランダムなピッチ角を適用
            transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(-(pointObjectPos.y - (GenerateHalfPitchAngle / GeneratePitchStep)) * GeneratePitchStep, transform.right) * transform.forward);
            Vector3 pointObjectPosition = _playerTr.position + transform.forward * DistanceOfGenerate;
            Quaternion pointObjectRotation = Quaternion.LookRotation((_playerTr.position - pointObjectPosition).normalized);
            pointObjects.transform.position = pointObjectPosition;
            pointObjects.transform.rotation = pointObjectRotation;
            pointObjects.PointObjectPos = pointObjectPos;
            generatedCount++;
        }
        //要求された回数分生成出来た時
        if(generatedCount == _generateCount)
        {
            //TimeKeeperの設定を行う処理
            TimeKeeper timeKeeper = _timeKeeperPool.Get();
            timeKeeper.transform.SetParent(transform.root);
            foreach(PointObjects pointObjects in generatablePointObjectsList)
            {
                pointObjects.transform.SetParent(timeKeeper.transform);
                pointObjects.Indicator = StageUI_manager.Current.GenerateIndicatorToTarget(pointObjects.transform);
                pointObjects.TimeKeeper = timeKeeper;

                timeKeeper.PointObjectsList.Add(pointObjects);
                timeKeeper.BaseActivationDelay = _currentBaseActivationDelay;


            }
            timeKeeper.Begin();
            return true;
        }//要求された回数分生成できなかったとき
        else
        {
            Debug.Log("生成失敗しました");
            foreach(PointObjects pointObjects in generatablePointObjectsList)
            {
                pointObjects.Release();
                _pointObjectMap[(int)pointObjects.PointObjectPos.x, (int)pointObjects.PointObjectPos.y] = false;

            }
            return false;
        }
        
        //#空いている生成可能な場所を探し、見つけたら、そこを返す関数。
        Vector2Int SearchPointObjectPos()
        {
            Initialize();
            _perlinNoiseSeed += Random.Range(0f,1f) * PerlinNoiseScale * PerlinNoiseMagni;

            float perlinNoiseInputX = _perlinNoiseSeed  + PointObjectMapLength.x ;
            float perlinNoiseInputY = _perlinNoiseSeed  + PointObjectMapLength.y ;

            Vector2Int startSearchPoint = new Vector2Int((int)(Mathf.PerlinNoise1D(perlinNoiseInputX) * PointObjectMapLength.x), (int)(Mathf.PerlinNoise1D(perlinNoiseInputY) * PointObjectMapLength.y));
            
            // スタート地点が範囲外になる可能性を考慮
            startSearchPoint.x = Mathf.Clamp(startSearchPoint.x, 0, PointObjectMapLength.x - 1);
            startSearchPoint.y = Mathf.Clamp(startSearchPoint.y, 0, PointObjectMapLength.y - 1);

            _visited[startSearchPoint.x,startSearchPoint.y] = true;
            _searchCandidates.Enqueue(startSearchPoint);

            while(_searchCandidates.Count > 0)
            {
                Vector2Int searchPoint = _searchCandidates.Dequeue();
                //探索の基点が空いているかの確認
                if(_pointObjectMap[searchPoint.x,searchPoint.y] == false)
                {
                    _pointObjectMap[searchPoint.x,searchPoint.y] = true;
                    return searchPoint;
                }
                
                //基点が埋まっている場合その四方の「未訪問の」マスを探索候補に追加
                int startOffsetIndex = Random.Range(0,4);
                for(int i = 0;i < _offsets.Length; i++)
                {
                    int offsetIndex = (int)Mathf.Repeat(startOffsetIndex + i,_offsets.Length);
                    Vector2Int neighborPoint = searchPoint + _offsets[offsetIndex];
                    //境界外チェック
                    if(neighborPoint.x >= PointObjectMapLength.x || neighborPoint.y < 0 || neighborPoint.x < 0 || neighborPoint.y >= PointObjectMapLength.y)
                    {
                        //境界外なら次の方向へ
                        continue;
                    }
                    //訪問済みチェック
                    if (_visited[neighborPoint.x, neighborPoint.y])
                    {
                        //訪問済みなら、次の方向へ
                        continue;
                    }
                    
                    //訪問済みとしてマークし、キューに追加
                    _visited[neighborPoint.x, neighborPoint.y] = true;
                    _searchCandidates.Enqueue(neighborPoint);
                }
            }
            return -Vector2Int.one;
            //前回のSertchPointObjectPosの影響を無効化するため各変数の値をリセットする。処理
            void Initialize()
            {
                Array.Clear(_visited,0,_visited.Length);
                _searchCandidates.Clear();
            }
        }
    }
    //#予算と言う観点において生成可能なPointObjectを持つゲームオブジェクトを返す関数。
    PointObjects GetGeneratablePointObjects()
    {
        float pointObjectBudget = MaxPointObjectCost - _sumPointObjectCost;
        int basisPointObjectArrayIndex = Random.Range(0, _pointObjects.Length);
        PointObjects generatablePointObject = null;
        for (int offsetPointObjectsIndex = 0; offsetPointObjectsIndex < _pointObjects.Length; offsetPointObjectsIndex++)
        {
            int generatablePointObjectArrayIndex = (int)Mathf.Repeat(offsetPointObjectsIndex + basisPointObjectArrayIndex, _pointObjects.Length);
            if (_pointObjects[generatablePointObjectArrayIndex].PointObjectCost <= pointObjectBudget)
            {
                generatablePointObject = _pointObjects[generatablePointObjectArrayIndex];
            }

        }
        //予算と言う観点において生成可能なPointObjectが無い時 nullを返す処理
        if(generatablePointObject == null) return null;

        return GetPointObjectsWithDownCast(generatablePointObject);
    }

    //あるPointObjectのインスタンスが与えられて、それが、ある具象クラスに属していたら、それにダウンキャストして返す関数。PointObjectGeneratorにより呼ばれる。
    protected abstract PointObjects GetPointObjectsWithDownCast(PointObjects pointObject);

    
    public void SetNotes()
    {
        _fourthNote = 60 / Bpm;
        _eighthNote = _fourthNote / 2;
        _sixteenthNote = _eighthNote / 2;
        PointObjects.FourthNote = _fourthNote;
        PointObjects.EighthNote = _eighthNote;
        PointObjects.SixteenthNote = _sixteenthNote;        
    }

    public void AddSumPointObjectCost(float pointObjectCost){
        _sumPointObjectCost += pointObjectCost;
    }
    public void RemovePointObject(float subtractPointObjectCost,Vector2 pointObjectPos,float delay){
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            yield return new WaitForSeconds(delay);
            _pointObjectMap[(int)pointObjectPos.x, (int)pointObjectPos.y] = false;
            _sumPointObjectCost -= subtractPointObjectCost;
            _isGeneratable = true;

        }

    }
    

    public Vector2Int WorldPosToPointObjectPos(Vector3 worldPos)
    {
        (float yaw,float pitch) angles = GetYawPitch(worldPos);
        float yawWithOffset = angles.yaw + (GenerateHalfYawAngle / GenerateYawStep * GenerateYawStep);
        float pitchWithOffset = angles.pitch + (GenerateHalfPitchAngle / GeneratePitchStep * GeneratePitchStep);
        int pointObjectMapX = Mathf.RoundToInt(yawWithOffset / GenerateYawStep);
        int pointObjectMapY =  Mathf.RoundToInt(pitchWithOffset / GeneratePitchStep);
        pointObjectMapX = Mathf.Clamp(pointObjectMapX,0,PointObjectMapLength.x - 1);
        pointObjectMapY = Mathf.Clamp(pointObjectMapY,0,PointObjectMapLength.y - 1);
        return new Vector2Int(pointObjectMapX, pointObjectMapY);
    }
    
    public void ChangePointObjectMap(Vector3 worldPos,bool change)
    {
        (int x, int y) targetPointObjectPos;
        targetPointObjectPos.x = WorldPosToPointObjectPos(worldPos).x;
        targetPointObjectPos.y = WorldPosToPointObjectPos(worldPos).y;
        _pointObjectMap[targetPointObjectPos.x, targetPointObjectPos.y] = change;
    }
    
    public (float yaw,float pitch) GetYawPitch(Vector3 worldPos)
    {
        float yaw = Vector3.SignedAngle(Vector3.forward, new Vector3(worldPos.x, 0, worldPos.z), Vector3.up);
        float pitch = Vector3.SignedAngle(new Vector3(worldPos.x, 0, worldPos.z), worldPos, Vector3.Cross(Vector3.up,new Vector3(worldPos.x, 0, worldPos.z)));
        return (yaw,pitch);

    }
}
