using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimulationSpeedController : MonoBehaviour
{
    private float simSpeed = 1;
    public TMP_Text text;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            simSpeed *= 2;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            simSpeed *= 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (simSpeed == 0)
            {
                simSpeed = 1;
            }
            else
            {
                simSpeed = 0;
            }
        }

        Time.timeScale = simSpeed;
        text.SetText((Mathf.Round(simSpeed * 100) / 100).ToString());
    }
}
