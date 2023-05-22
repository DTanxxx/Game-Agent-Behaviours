using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class WorldInterface : MonoBehaviour
{
    public GameObject[] allResources;
    public NavMeshSurface surface;
    public GameObject hospital;
    
    private GameObject newResourcePrefab;
    private GameObject focusObj;
    private ResourceData focusObjData;
    private Vector3 goalPos;
    private Vector3 clickOffset = Vector3.zero;
    private bool offsetCalc = false;
    private bool deleteResource = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // check if mouse cursor is over an UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // pressing down, spawn a resource
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }

            // reset click offset
            clickOffset = Vector3.zero;
            offsetCalc = false;

            Resource resource = hit.transform.gameObject.GetComponent<Resource>();

            if (resource != null)
            {
                // mouse selected a resource
                focusObj = hit.transform.gameObject;
                focusObjData = resource.info;
            }
            else if (newResourcePrefab != null)
            {
                goalPos = hit.point;
                focusObj = Instantiate(newResourcePrefab, goalPos, newResourcePrefab.transform.rotation);
                focusObjData = focusObj.GetComponent<Resource>().info;
            }

            if (focusObj != null)
            {
                // disable resource's collider to prevent repetitive movement
                focusObj.GetComponent<Collider>().enabled = false;
            }
        }
        else if (focusObj && Input.GetMouseButtonUp(0))
        {
            if (deleteResource)
            {
                // delete the resource when mouse is released
                GameWorld.Instance.GetQueue(focusObjData.resourceQueue).RemoveResource(focusObj);
                GameWorld.Instance.GetWorld().ModifyState(focusObjData.resourceState, -1);
                Destroy(focusObj);
            }
            else
            {
                // mouse button released, place resource
                focusObj.transform.parent = hospital.transform;

                // add this resource to game world
                GameWorld.Instance.GetQueue(focusObjData.resourceQueue).AddResource(focusObj);
                GameWorld.Instance.GetWorld().ModifyState(focusObjData.resourceState, 1);

                // enable resource's collider to allow future mouse selection
                focusObj.GetComponent<Collider>().enabled = true;
            }
            
            surface.BuildNavMesh();
            focusObj = null;
        }
        else if (focusObj && Input.GetMouseButton(0))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Floor");
            // mouse button held down, drag resource
            RaycastHit hitMove;
            Ray rayMove = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(rayMove, out hitMove, Mathf.Infinity, layerMask))
            {
                return;
            }
            if (!offsetCalc)
            {
                // haven't calculated the mouse click offset
                clickOffset = hitMove.point - focusObj.transform.position;
                offsetCalc = true;
            }
            goalPos = hitMove.point - clickOffset;
            focusObj.transform.position = goalPos;
        }

        // rotate the selected resource
        if (focusObj && (Input.GetKeyDown(KeyCode.Less) || Input.GetKeyDown(KeyCode.Comma)))
        {
            focusObj.transform.Rotate(0, 90, 0);
        }
        else if (focusObj && (Input.GetKeyDown(KeyCode.Greater) || Input.GetKeyDown(KeyCode.Period)))
        {
            focusObj.transform.Rotate(0, -90, 0);
        }
    }

    public void MouseOnHoverTrash()
    {
        deleteResource = true;
    }

    public void MouseOutHoverTrash()
    {
        deleteResource = false;
    }

    public void ActivateToilet()
    {
        newResourcePrefab = allResources[0];
    }

    public void ActivateCubicle()
    {
        newResourcePrefab = allResources[1];
    }
}
