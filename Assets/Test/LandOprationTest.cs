/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 土地操作测试
/// </summary>
public class LandOprationTest : MonoBehaviour
{
    private GameObject root;

    private MapCreater creater;
    
    //    private int[] testWorldMap = new[]
    //    {
    //        1,2,1,2,1,2,1,2,1,2,
    //        1,2,1,1,1,2,1,2,1,2,
    //        2,2,1,2,1,2,1,1,1,2,
    //        1,2,1,2,1,2,1,2,1,2,
    //        1,1,1,2,1,2,1,2,1,2,
    //        1,2,2,2,1,2,1,2,1,2,
    //        1,2,1,1,1,2,1,2,1,2,
    //        1,2,1,2,1,2,2,2,1,2,
    //        1,1,1,2,1,2,1,2,1,2,
    //        1,2,1,2,1,2,1,2,2,1
    //    };

    private int[] testWorldMap = new[]
    {
        1,2,1,
        1,1,2,
        2,2,1
    };

    void Awake()
    {
        creater = MapCreater.Instance;
        creater.LoadDel = LoadAssetMap;

        //初始化世界地图索引
        MapData.Instance.Initilize(testWorldMap);

        //        MapDebug.DebugLandTiled = true;
        //        MapDebug.DebugLandTiled = true;

        MapDebug.DebugSceneTiled = true;
    }

    // Use this for initialization
    void Start()
    {
        Vector3 position = CoordinationConvert.OrginMapPoint - Vector3.forward * 200;
        position += new Vector3(SceneTiled.halfSceneWidth , -SceneTiled.halfSceneHeight);

        MapCamera.MapMainCamera.transform.position = position;

        //mergeMap();
        creater.MergeMap(MapData.Instance.AllSceneDatas);
    }



    private void LoadAssetMap(string map, Action<GameObject> callback)
    {
        UnityEngine.Object resObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(map);

        if (resObj == null)
        {
            Debug.LogWarning("cant find :" + map);
            return;
        }
        GameObject gObj = Instantiate(resObj) as GameObject;

        callback.Invoke(gObj);
    }
}