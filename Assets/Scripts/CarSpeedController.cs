using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpeedController : MonoBehaviour
{
    private CarMovement carMovement;
    public float hasteMultiplier;
    private float speed;

    void Start()
    {
        hasteMultiplier = Random.Range(0.75f, 1.25f);
        carMovement = GetComponent<CarMovement>();
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
        {
            if (hit.distance < 5f)
            {
                // speed = 0;
            }
        }
        // carMovement.speedMult;
    }
}
