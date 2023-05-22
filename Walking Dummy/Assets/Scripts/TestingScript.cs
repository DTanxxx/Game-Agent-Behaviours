using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [SerializeField] private NavGraphNode node = null;
    [SerializeField] private NavGraphEdge edge = null;

    private void Start()
    {
        /*var node1 = Instantiate(node, Vector3.zero, Quaternion.identity);
        var node2 = Instantiate(node, new Vector3(1, 1, 2), Quaternion.identity);
        var quat = new Quaternion();
        var fracx = (node2.transform.position.z - node1.transform.position.z) / (node2.transform.position.x - node1.transform.position.x);
        var fracz = (node2.transform.position.y - node1.transform.position.y) / (node2.transform.position.z - node1.transform.position.z);
        
        Vector3 angles = new Vector3(Mathf.Rad2Deg * Mathf.Atan(fracx), 0, -Mathf.Rad2Deg * Mathf.Atan(fracz));
        quat.eulerAngles = angles;
        Instantiate(edge, (node1.transform.position + node2.transform.position) / 2, quat);*/
    }
}
