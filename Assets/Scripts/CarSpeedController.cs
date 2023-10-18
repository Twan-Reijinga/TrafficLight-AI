using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarSpeedController : MonoBehaviour
{
    private CarMovement carMovement;
    public float hasteMultiplier = 1f;
    private float speed = 1f;
    public LayerMask ignoreOnRayCast;

    void Start()
    {
        hasteMultiplier = 1f;//Random.Range(0.5f, 1.5f);
        // print(hasteMultiplier);
        carMovement = GetComponent<CarMovement>();
    }

    void Update()
    {
        RaycastHit hitR;
        RaycastHit hitL;

        bool rayR = Physics.Raycast(transform.position + transform.right * 0.5f, transform.forward, out hitR, 5f, ~(ignoreOnRayCast));
        bool rayL = Physics.Raycast(transform.position - transform.right * 0.5f, transform.forward, out hitL, 5f, ~(ignoreOnRayCast));

        if (Selection.activeGameObject == transform.gameObject)
        {
            if (rayR)
            {
                print("R: " + hitR.transform.name);
            }
            if (rayL)
            {
                print("L: " + hitL.transform.name);
            }
        }

        Debug.DrawRay(transform.position + transform.right * 0.5f, transform.forward * 5f, Color.green);
        Debug.DrawRay(transform.position - transform.right * 0.5f, transform.forward * 5f, Color.green);


        if (rayR || rayL)
        {
            RaycastHit hit;
            if (rayR)
            {
                if (rayL)
                {
                    hit = hitL;
                    if (hitR.distance < hitL.distance)
                    {
                        hit = hitR;
                    }
                }
                else
                {
                    hit = hitR;
                }
            }
            else
            {
                hit = hitL;
            }

            // if (hit.transform.gameObject.GetComponent<SpeedMeter>().isCar || carMovement.action == 'f')
            // {
            //     if (hit.distance < 2.5f)
            //     {
            //         speed = 0;
            //     }
            //     else if (hit.transform.gameObject.GetComponent<SpeedMeter>() != null)
            //     {
            //         float thisSpeed = GetComponent<SpeedMeter>().velocity.magnitude;
            //         if (thisSpeed != 0)
            //         {
            //             if (hit.transform.gameObject.GetComponent<SpeedMeter>().velocity.magnitude == 0)
            //             {
            //                 if (Vector3.Dot(transform.forward, hit.transform.forward) < 0)
            //                 {
            //                     speed = Mathf.Max(Mathf.Min(hit.transform.gameObject.GetComponent<SpeedMeter>().velocity.magnitude / thisSpeed, 1.0f), 0.5f);
            //                 }

            //             }
            //         }
            //         else
            //         {
            //             speed = 0;
            //         }
            //     }
            // }
            float thisSpeed = GetComponent<SpeedMeter>().velocity.magnitude;
            GetComponent<SpeedMeter>().isCar = true;
            speed = 1;
            if (hit.transform.gameObject.GetComponent<SpeedMeter>().isCar)
            {
                speed = Mathf.Max(Mathf.Min(hit.transform.gameObject.GetComponent<SpeedMeter>().velocity.magnitude / thisSpeed, 1.0f), 0.5f);

                if (hit.distance < 2.5f)
                {
                    speed = 0;
                }

            }
            else if (carMovement.action == 'f')
            {
                GetComponent<SpeedMeter>().isCar = false;
                speed = Mathf.Max(Mathf.Min(hit.transform.gameObject.GetComponent<SpeedMeter>().velocity.magnitude / thisSpeed, 1.0f), 0.5f);

                if (hit.distance < 2.5f)
                {
                    speed = 0;
                }
            }
        }
        else
        {
            speed = 1;
        }
        carMovement.speedMult = speed * hasteMultiplier;
    }
}
