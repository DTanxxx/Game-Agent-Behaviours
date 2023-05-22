using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    Camera cam;
    public float speed = 10.0f;
    public float rotationSpeed = 20.0f;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cam.gameObject.transform.LookAt(transform.position);
    }

    private void Update()
    {
        // horizontal and vertical camera movement
        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(0, 0, translationZ);
        transform.Translate(translationX, 0, 0);

        // camera rotation around y axis
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        // camera zoom
        if (Input.GetKey(KeyCode.R) && cam.gameObject.transform.position.y > 5)
        {
            cam.gameObject.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.F) && cam.gameObject.transform.position.y < 45)
        {
            cam.gameObject.transform.Translate(0, 0, -speed * Time.deltaTime);
        }

        float angle = Vector3.Angle(cam.gameObject.transform.forward, Vector3.up);

        // camera tilt
        if (Input.GetKey(KeyCode.T) && angle < 175f)
        {
            cam.gameObject.transform.Translate(Vector3.up);
            cam.gameObject.transform.LookAt(transform.position);
        }
        if (Input.GetKey(KeyCode.G) && angle > 95f)
        {
            cam.gameObject.transform.Translate(Vector3.down);
            cam.gameObject.transform.LookAt(transform.position);
        }
    }
}
