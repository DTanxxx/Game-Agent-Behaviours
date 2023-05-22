using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties : MonoBehaviour
{
    public Vector3 GetNormal(Vector3 overshoot, float randomAngleVariation, float perpendicularityToleranceValue)
    {
        // check overshoot's direction relative to this wall's local space
        Vector3 localOvershoot = transform.InverseTransformDirection(overshoot);
        if (transform.localScale.x > transform.localScale.z)
        {
            Vector3 forwardNormal = Vector3.forward;
            if (localOvershoot.z > 0)
            {
                forwardNormal *= -1;
            }

            // rotate this normal according to wall's rotation in y axis
            Vector3 rotatedNormal = transform.rotation * forwardNormal;

            if (Mathf.Abs(localOvershoot.x) < perpendicularityToleranceValue)
            {
                // straight overshoot, add some variation to the rotatedNormal
                if (localOvershoot.x < 0)
                {
                    // to the left, add some counterclockwise rotation
                    rotatedNormal = Quaternion.AngleAxis(randomAngleVariation, Vector3.up) * rotatedNormal;
                    if (localOvershoot.z > 0)
                    {
                        // flipped
                        rotatedNormal = Quaternion.AngleAxis(-randomAngleVariation, Vector3.up) * rotatedNormal;
                    }
                }
                else
                {
                    // to the right, add some clockwise rotation
                    rotatedNormal = Quaternion.AngleAxis(-randomAngleVariation, Vector3.up) * rotatedNormal;
                    if (localOvershoot.z > 0)
                    {
                        rotatedNormal = Quaternion.AngleAxis(randomAngleVariation, Vector3.up) * rotatedNormal;
                    }
                }
            }
            return rotatedNormal;
        }
        else
        {
            Vector3 rightNormal = Vector3.right;
            if (localOvershoot.x > 0)
            {
                rightNormal *= -1;
            }

            Vector3 rotatedNormal = transform.rotation * rightNormal;
            
            if (Mathf.Abs(localOvershoot.z) < perpendicularityToleranceValue)
            {
                if (localOvershoot.z < 0)
                {
                    // to the bottom, add some counterclockwise rotation
                    rotatedNormal = Quaternion.AngleAxis(randomAngleVariation, Vector3.up) * rotatedNormal;
                    if (localOvershoot.x < 0)
                    {
                        // to the left, add some counterclockwise rotation
                        rotatedNormal = Quaternion.AngleAxis(-randomAngleVariation, Vector3.up) * rotatedNormal;
                    }
                }
                else
                {
                    // to the up, add some clockwise rotation
                    rotatedNormal = Quaternion.AngleAxis(-randomAngleVariation, Vector3.up) * rotatedNormal;
                    if (localOvershoot.x < 0)
                    {
                        // to the left, add some counterclockwise rotation
                        rotatedNormal = Quaternion.AngleAxis(randomAngleVariation, Vector3.up) * rotatedNormal;
                    }
                }
            }
            return rotatedNormal;
        }
    }
}
