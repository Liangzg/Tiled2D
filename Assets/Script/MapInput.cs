/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;


public class MapInput : MonoBehaviour
{
    private Vector2 beginPos;
    private Vector2 offset = Vector2.zero;
    private Vector2 offsetZero = Vector2.zero;
    private Vector3 lastMousePos;
    private float beginTime;
    private float endTime = float.MaxValue;
    private float touchTime = 0.2f;

    public bool build;
    public Vector2 buildTileRange;
    private List<LandTiled> landTileds = new List<LandTiled>();

    private Camera mapCamera;
    
    public void Start()
    {
        mapCamera = MapCamera.MapMainCamera;
    }


    public void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount <= 0) return;

	    Touch touch = Input.GetTouch(0);

	    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
        {
            beginTime = Time.time;
            beginPos = mapCamera.ScreenToWorldPoint(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            offset = (Vector2)mapCamera.ScreenToWorldPoint(touch.position) - beginPos;
        }else if (touch.phase == TouchPhase.Ended)
        {
            offset = offsetZero;
            lastMousePos = touch.position;
            endTime = Time.time;
            MapCamera.Instance.CallFinish();
        }
#else


        if (Input.GetMouseButtonDown(0))
        {
            beginPos = mapCamera.ScreenToWorldPoint(Input.mousePosition + MapCamera.Instance.depath);
            beginTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTime = Time.time;
            offset = offsetZero;
            beginPos = offsetZero;
            lastMousePos = Input.mousePosition;
            MapCamera.Instance.CallFinish();
        }
        else if (beginPos != offsetZero)
        {
            Vector3 curMouseSceenPos = Input.mousePosition;
            float distance = Vector3.Distance(lastMousePos, curMouseSceenPos);
            if (distance > 5f)
            {
                lastMousePos = curMouseSceenPos;
                Vector3 curMousePos = mapCamera.ScreenToWorldPoint(lastMousePos + MapCamera.Instance.depath);
                offset = (Vector2)curMousePos - beginPos;
            }
        }
#endif
        //移动移动
        if (offset != offsetZero)
        {
            //移动相反的方向
            MapCamera.Instance.Move(-offset);
            //	        cacheTrans.position += (Vector3)offset;
            offset = offsetZero;

           
        }

        //点击操作
        if (Math.Abs(endTime - beginTime) < touchTime)
        {
            endTime = 0;
            LandTiled landTiled = MapData.Instance.FindLandTiled(lastMousePos);
            if (landTiled != null) landTiled.SetTiledState(landTiled.CurrentState == ELandStatus.Select ?
                                                           ELandStatus.None : ELandStatus.Select);
        }


        if (build)
        {
            Vector2 mapPoint = CoordinationConvert.ConvertTiledIndex(Input.mousePosition);

        }
    }



}
