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
/// 描述：地图坐标转换测试
/// <para>创建时间：</para>
/// </summary>
public class MapCooditionTest : MonoBehaviour
{

    public GameObject MapElements;

    public int row = 2;
    private int lastRow = 0;

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
    }

	// Use this for initialization
	void Start ()
	{
	    MapCamera.MapMainCamera.transform.position = CoordinationConvert.OrginMapPoint - Vector3.forward * 100;
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

    private void mergeMap()
    {
        if (lastRow == this.row) return;

        if(root!= null) GameObject.Destroy(root);

        root = new GameObject("root");
        root.transform.localScale = Vector3.one;
        root.transform.position = Vector3.zero;

        int mapCount = 10;

        lastRow = row;
//
//        //新的坐标系M,N
//        int halfWidth = (int)MapCreater.TileRange.x / 2; // M点在像素坐标系X轴的偏移
//        int halfHeight = (int)MapCreater.TileRange.y / 2;  // 像素坐标系Y轴的偏移

        for (int i = 0; i < mapCount; i++)
        {
            GameObject gObj = GameObject.Instantiate(MapElements) as GameObject;
            gObj.transform.parent = root.transform;
            gObj.name += i;

            TiledMap tiled = gObj.GetComponent<TiledMap>();
            float mapWidth = tiled.GetMapWidthInPixelsScaled();
            float mapHeight = tiled.GetMapHeightInPixelsScaled();

            float halfWidth = mapWidth / 2;
            float halfHeight = mapHeight / 2;

//            int indexX = i%row;
//            int indexY = i/row + i % row;
            // i % 2 == 0 ? i / 2 + 0.5 : i - 1 
            //            gObj.transform.position = new Vector3(-offsetWidth * (indexX / 2 +  indexX  % 2  * 0.5f) , 
            //                                                -offsetHeight * (i / row + (i  % row) * 0.5f));

            int column = i/row;
            float tiledX = halfWidth*column - halfWidth*(i % row);
            float tiledY = -halfHeight*column - halfHeight*(i % row);  
            gObj.transform.position = new Vector3(tiledX , tiledY);
            gObj.transform.localScale = Vector3.one;
        }
    }

	// Update is called once per frame
	void Update () {
//        this.mergeMap();
	}
}
