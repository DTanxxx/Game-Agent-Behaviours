using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GAgentVisual : MonoBehaviour
{
    public GameAgent thisAgent;

    // Start is called before the first frame update
    void Start()
    {
        thisAgent = this.GetComponent<GameAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
