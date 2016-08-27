/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using Tiled2Unity;
using UnityEditor;

/// <summary>
/// 描述：动态地图测试
/// <para>创建时间：</para>
/// </summary>
public class DynimacMapTest : MonoBehaviour {

    public GameObject MapElements;

    public int row = 2;
    private int lastRow = 0;

    private GameObject root;

    private MapCreater creater;


    private int[] testWorldMap = new[]
    {
        1,2,1,2,1,2,1,2,1,2,
        1,2,1,1,1,2,1,2,1,2,
        2,2,1,2,1,2,1,1,1,2,
        1,2,1,2,1,2,1,2,1,2,
        1,1,1,2,1,2,1,2,1,2,
        1,2,2,2,1,2,1,2,1,2,
        1,2,1,1,1,2,1,2,1,2,
        1,2,1,2,1,2,2,2,1,2,
        1,1,1,2,1,2,1,2,1,2,
        1,2,1,2,1,2,1,2,2,1
    };

//    private int[] testWorldMap = new[]
//    {
//            1,2,1,2,2,1,
//            1,1,2,1,2,1,
//            2,2,1,2,1,1,
//            1,2,1,2,1,2,
//            2,1,1,2,2,1,
//            1,1,2,1,2,2,
//        };

    void Awake()
    {
        creater = MapCreater.Instance;
        creater.LoadDel = LoadAssetMap;

        //初始化世界地图索引
        MapData.Instance.Initilize(testWorldMap);

        //MapDebug.DebugSceneTiled = true;
//        MapDebug.DebugLandTiled = true;
//        MapDebug.IsMapPoint = true;
    }

    // Use this for initialization
    void Start()
    {
        MapCamera.Instance.MoveChangeDel = creater.CameraChange;

        Vector3 orginCamPos = CoordinationConvert.OrginMapPoint - Vector3.forward * 200;
        orginCamPos += new Vector3(0 , -MapCreater.MapColumn * SceneTiled.MaxSceneHeight * 0.5f );
        MapCamera.MapMainCamera.gameObject.transform.position = orginCamPos;

        creater.CameraChange();
        //mergeMap();
        //creater.MergeMap(MapData.Instance.AllSceneDatas);
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

    // Update is called once per frame
    void Update()
    {
        //        this.mergeMap();
    }
}
