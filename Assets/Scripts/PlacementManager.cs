using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementManager : MonoBehaviour
{
    public GameObject pointPrefab;

    public GameObject placementIndicator;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    private bool loopComplete = false;
    private List<upperPlanePoint> upperPlanePointList = new List<upperPlanePoint>();
    private List<Transform> pointList = new List<Transform>();

    public LineRenderer lineRenderer;
    public GameObject planeHandlerPanel;

    private bool isUp = false;
    private bool isDown = false;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;  //Screen Timeout

        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        lineRenderer.positionCount = 1;

        planeHandlerPanel.SetActive(false);
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !loopComplete)
        {
            ARPlacePoint();
        }

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (isUp)  // up button is pressed
        {
            moveUp();
        }
        else if (isDown)  // down button is pressed
        {
            moveDown();
        }
    }

    //update placement indicator according to placement conditions
    void UpdatePlacementIndicator()
    {
        if (!loopComplete)
        {
            if (placementPoseIsValid)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, PlacementPose.position);
            }
            else
            {
                placementIndicator.SetActive(false);
                if (pointList.Count == 0)
                {
                    lineRenderer.SetPosition(0, Vector3.zero);
                }
                else
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, pointList[pointList.Count - 1].position);
                }
            }
        }
    }

    //detect plane and update placement position
    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }


    // on click on screen place point on the position of placement indicator and add the point in line renderer
    void ARPlacePoint()
    {
        if (pointList.Count > 0)
        {
            Debug.Log(" distance  : " + Vector3.Distance(PlacementPose.position, pointList[pointList.Count - 1].position));
            Debug.Log(" distance first point : " + Vector3.Distance(PlacementPose.position, pointList[0].position));
            if (Vector3.Distance(PlacementPose.position, pointList[0].position) <= 0.02f)  //check for the loop if distance of first point and last point less then 0.02 then it completes loop
            {
                loopComplete = true;
                placementIndicator.SetActive(false);
                lineRenderer.positionCount = lineRenderer.positionCount - 1;
                addPointsAfterLoop();
                planeHandlerPanel.SetActive(true);
                return;
            }
        }


        //instantiate point on the placement indicator position and add posint to line renderer
        GameObject pointGB = Instantiate(pointPrefab, PlacementPose.position, PlacementPose.rotation);
        pointList.Add(pointGB.transform);
        lineRenderer.positionCount = pointList.Count + 1;
        lineRenderer.SetPosition(pointList.Count - 1, pointList[pointList.Count - 1].position);
    }


    // on close loop place points for upper plane structure
    void addPointsAfterLoop()
    {
        int initialPointCount = pointList.Count;

        GameObject pointGB = Instantiate(pointPrefab, pointList[0].position, PlacementPose.rotation);
        pointList.Add(pointGB.transform);
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPosition(pointList.Count - 1, pointList[0].position);

        Vector3 pos = pointList[0].position;
        pos.y += 0.1f;
        pointGB = Instantiate(pointPrefab, pos, PlacementPose.rotation);
        pointList.Add(pointGB.transform);
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPosition(pointList.Count - 1, pointList[pointList.Count - 1].position);
        upperPlanePointList.Add(new upperPlanePoint());
        upperPlanePointList[upperPlanePointList.Count - 1].index = pointList.Count - 1;
        upperPlanePointList[upperPlanePointList.Count - 1].point = pointList[pointList.Count - 1];

        for (int i = 1; i < initialPointCount; i++)
        {
            pos = pointList[i].position;
            pos.y += 0.1f;
            pointGB = Instantiate(pointPrefab, pos, PlacementPose.rotation);
            pointList.Add(pointGB.transform);
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPosition(pointList.Count - 1, pointList[pointList.Count - 1].position);
            upperPlanePointList.Add(new upperPlanePoint());
            upperPlanePointList[upperPlanePointList.Count - 1].index = pointList.Count - 1;
            upperPlanePointList[upperPlanePointList.Count - 1].point = pointList[pointList.Count - 1];

            pointGB = Instantiate(pointPrefab, pointList[i].position, PlacementPose.rotation);
            pointList.Add(pointGB.transform);
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPosition(pointList.Count - 1, pointList[i].position);

            pos = pointList[i].position;
            pos.y += 0.1f;
            pointGB = Instantiate(pointPrefab, pos, PlacementPose.rotation);
            pointList.Add(pointGB.transform);
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPosition(pointList.Count - 1, pointList[pointList.Count - 1].position);
            upperPlanePointList.Add(new upperPlanePoint());
            upperPlanePointList[upperPlanePointList.Count - 1].index = pointList.Count - 1;
            upperPlanePointList[upperPlanePointList.Count - 1].point = pointList[pointList.Count - 1];
        }


        pos = pointList[0].position;
        pos.y += 0.1f;
        pointGB = Instantiate(pointPrefab, pos, PlacementPose.rotation);
        pointList.Add(pointGB.transform);
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPosition(pointList.Count - 1, pointList[pointList.Count - 1].position);
        upperPlanePointList.Add(new upperPlanePoint());
        upperPlanePointList[upperPlanePointList.Count - 1].index = pointList.Count - 1;
        upperPlanePointList[upperPlanePointList.Count - 1].point = pointList[pointList.Count - 1];

        Debug.Log("uper point list count : " + upperPlanePointList.Count);
    }


    // on up and down button (pointer down event)
    public void onPointerDownArrowBtn(bool isUpArrow)
    {
        if (isUpArrow)
        {
            isUp = true;
            isDown = false;
        }
        else
        {
            isUp = false;
            isDown = true;
        }
    }

    // on up and down button ( pointer up event)
    public void onPointerUpArrowBtn()
    {
        isUp = false;
        isDown = false;
    }


    // on up button is pressed move upper plane to upword direction by 0.01 unit
    void moveUp()
    {
        for(int i = 0; i < upperPlanePointList.Count; i++)
        {
            Vector3 pos = upperPlanePointList[i].point.position;
            pos.y += 0.01f;
            upperPlanePointList[i].point.position = pos;
            lineRenderer.SetPosition(upperPlanePointList[i].index, pos);
        }
    }


    // on down button is pressed move upper plane to downword direction by 0.01 unit
    void moveDown()
    {
        for (int i = 0; i < upperPlanePointList.Count; i++)
        {
            Vector3 pos = upperPlanePointList[i].point.position;
            pos.y -= 0.01f;
            upperPlanePointList[i].point.position = pos;
            lineRenderer.SetPosition(upperPlanePointList[i].index, pos);
        }
    }

    public void onCloseBtnClick()
    {
        Application.Quit();
    }
}

public class upperPlanePoint
{
    public int index;
    public Transform point;
}
