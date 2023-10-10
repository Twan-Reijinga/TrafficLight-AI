using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficLightsControllerNL : MonoBehaviour
{
    private List<List<string>> distances;

    private List<DualTrafficLightsHolder> Lights;

    private List<TrafficLight> GreenLights = new List<TrafficLight>();
    public float maxClearingTime = 15f;
    private float clearingtime = 0f;
    public float carSpeed = 13f;

    public enum State
    {
        WaitingForCar,
        LightsAreGreen,
        ClearingDelay
    }

    [SerializeField]
    private TrafficLight nextToTurnGreen;
    private TrafficLight lastPinged;

    public float greenTimer = 5f;

    [SerializeField]
    private float innerGreenTimer;

    public State state = State.WaitingForCar;


    public TrafficLight test;
    public TrafficLight test2;


    void Start()
    {
        distances = ParseCSV(Resources.Load<TextAsset>("distances").text);
        Lights = GetComponentsInChildren<DualTrafficLightsHolder>().ToList<DualTrafficLightsHolder>();
        if (Lights[0].gameObject.name != "0")
        {
            print("there might be an error with the traffic lights!!!");
        }
    }



    void Update()
    {
        // if (test != null && test2 != null)
        // {
        //     print(GetDist(test, test2));
        //     test = null;
        //     test2 = null;
        // }
        // return;
        foreach (DualTrafficLightsHolder holder in Lights)
        {
            RaycastHit hit;
            if (Physics.Raycast(holder.left.transform.position + Vector3.down * 4, holder.left.transform.right, out hit, 10f))
            {
                if (nextToTurnGreen == null && !holder.left.isGreen)
                {
                    nextToTurnGreen = holder.left;
                }
                lastPinged = holder.left;
            }
            else if (Physics.Raycast(holder.forward.transform.position + Vector3.down * 4, holder.forward.transform.right, out hit, 10f))
            {
                if (nextToTurnGreen == null && !holder.forward.isGreen)
                {
                    nextToTurnGreen = holder.forward;
                }
                lastPinged = holder.forward;
            }
            Debug.DrawRay(holder.left.transform.position + Vector3.down * 4, holder.left.transform.right * 10, Color.green);
            Debug.DrawRay(holder.forward.transform.position + Vector3.down * 4, holder.forward.transform.right * 10, Color.green);
        }

        switch (state)
        {
            case State.WaitingForCar:
                {
                    WaitingForCar();
                    break;
                }
            case State.LightsAreGreen:
                {
                    LightsAreGreen();
                    break;
                }
            case State.ClearingDelay:
                {
                    ClearingDelay();
                    break;
                }

        }
    }

    void WaitingForCar()
    {
        if (nextToTurnGreen != null)
        {
            innerGreenTimer = greenTimer;

            nextToTurnGreen.isGreen = true;
            UpdateGreenLightsList();

            nextToTurnGreen = null;
            state = State.LightsAreGreen;
        }
    }

    void LightsAreGreen()
    {
        if (lastPinged != null && GreenLights.Count > 0)
        {
            bool canBeGreen = true;
            foreach (TrafficLight light in GreenLights)
            {
                float tmp = GetDist(lastPinged, light);

                print(tmp);

                if (tmp != -1)
                {
                    canBeGreen = false;
                }
            }

            if (canBeGreen)
            {
                lastPinged.isGreen = true;
                UpdateGreenLightsList();
            }
            lastPinged = null;
        }

        innerGreenTimer -= Time.deltaTime;
        if (innerGreenTimer <= 0)
        {
            innerGreenTimer = 0;
            if (nextToTurnGreen != null && GreenLights.Count != 0)
            {
                clearingtime = 0;
                state = State.ClearingDelay;
                foreach (TrafficLight light in GreenLights)
                {
                    float t_exit = GetDist(light, nextToTurnGreen, true) / carSpeed;
                    float t_entry = GetDist(nextToTurnGreen, light, false) / carSpeed;
                    clearingtime = Mathf.Max(clearingtime, Mathf.Max(t_exit - t_entry, 0.0f));
                }
            }
            else
            {
                state = State.WaitingForCar;
            }
            TurnOffAllLights();
        }
    }

    void ClearingDelay()
    {
        clearingtime -= Time.deltaTime;
        if (clearingtime <= 0)
        {
            clearingtime = 0;
            state = State.WaitingForCar;
        }
    }

    void TurnOffAllLights(bool green = false)
    {
        foreach (DualTrafficLightsHolder holder in Lights)
        {
            holder.forward.isGreen = green;
            holder.left.isGreen = green;
        }
        UpdateGreenLightsList();
    }

    bool CheckForCompatibleTrafficLights(TrafficLight tl)
    {
        return true;
    }

    List<List<string>> ParseCSV(string csvText)
    {
        List<List<string>> rows = new List<List<string>>();

        // Split CSV text into rows
        string[] lines = csvText.Split('\n');

        for (int i = 1; i < lines.Length; i++) // Start from index 1 to skip the first row
        {
            // Split each row into columns
            List<string> columns = lines[i].Split(',').Select(s => s.Trim()).ToList();
            rows.Add(columns.Skip(1).ToList()); // Skip the first column
        }

        return rows;
    }

    void UpdateGreenLightsList()
    {
        GreenLights.Clear();
        foreach (DualTrafficLightsHolder holder in Lights)
        {
            if (holder.forward.isGreen)
            {
                GreenLights.Add(holder.forward);
                if (holder.forward == nextToTurnGreen)
                {
                    nextToTurnGreen = null;
                }
            }
            if (holder.left.isGreen)
            {
                GreenLights.Add(holder.left);
                if (holder.left == nextToTurnGreen)
                {
                    nextToTurnGreen = null;
                }
            }
        }
    }

    int GetLightYPosInTable(TrafficLight reference, TrafficLight other)
    {
        int result = 0;
        DualTrafficLightsHolder refParent = reference.transform.parent.GetComponent<DualTrafficLightsHolder>();
        DualTrafficLightsHolder otherParent = other.transform.parent.GetComponent<DualTrafficLightsHolder>();

        int name = int.Parse(refParent.name);
        int otherName = int.Parse(otherParent.name);

        if (otherName < name) otherName += 4;
        result = (otherName - name) * 4;

        if (other == otherParent.left) result += 2;

        return result;
    }

    int GetLightXPosInTable(TrafficLight light)
    {
        if (light.transform.parent.GetComponent<DualTrafficLightsHolder>().forward == light)
        {
            return 0;
        }
        return 1;
    }

    float GetDist(TrafficLight reference, TrafficLight other, bool getOutTime = false)
    {
        int x = GetLightXPosInTable(reference);
        int y = GetLightYPosInTable(reference, other);
        print("x = " + x.ToString() + " y =" + y.ToString());
        if (getOutTime == true)
        {
            y += 1;
        }

        if (distances[y][x] == "inf")
        {
            return -1;
        }
        return float.Parse(distances[y][x]);
    }
}
