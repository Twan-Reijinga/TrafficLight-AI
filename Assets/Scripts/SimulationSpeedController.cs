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
            Time.timeScale *= 2;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale *= 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = simSpeed;
            }
            else
            {
                simSpeed = Time.timeScale;
                Time.timeScale = 0;
            }
        }

        text.SetText((Mathf.Round(Time.timeScale * 100) / 100).ToString());
    }
}
