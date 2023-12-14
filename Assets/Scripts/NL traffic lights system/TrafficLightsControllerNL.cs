using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficLightsControllerNL : MonoBehaviour
{
    private List<List<string>> distances;
    public LayerMask whatIsCar;

    private List<DualTrafficLightsHolder> Lights;

    private List<TrafficLight> GreenLights = new List<TrafficLight>();
    private List<TrafficLight> WaitingLights = new List<TrafficLight>();
    public float maxClearingTime = 15f;
    private float clearingtime = 0f;
    public float carSpeed = 8f;

    public enum State
    {
        WaitingForCar,
        LightsAreGreen,
        ClearingDelay
    }

    // [SerializeField]
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
            if (Physics.Raycast(holder.left.transform.position + Vector3.down * 4.5f - holder.left.transform.right * 2, holder.left.transform.right, out hit, 10f, whatIsCar))
            {
                if (!holder.left.isGreen)
                {
                    WaitingLights.Add(holder.left);
                }
                lastPinged = holder.left;
            }
            else if (Physics.Raycast(holder.forward.transform.position + Vector3.down * 4.5f - holder.forward.transform.right * 2, holder.forward.transform.right, out hit, 10f, whatIsCar))
            {
                if (!holder.forward.isGreen)
                {
                    WaitingLights.Add(holder.forward);
                }
                lastPinged = holder.forward;
            }
            // Debug.DrawRay(holder.left.transform.position + Vector3.down * 4.5f - holder.left.transform.right * 2, holder.left.transform.right * 10, Color.green);
            // Debug.DrawRay(holder.forward.transform.position + Vector3.down * 4.5f - holder.forward.transform.right * 2, holder.forward.transform.right * 10, Color.green);
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
        if (WaitingLights.Count != 0)
        {
            TrafficLight next = WaitingLights[Random.Range(0, WaitingLights.Count - 1)];
            innerGreenTimer = greenTimer;

            next.isGreen = true;
            UpdateGreenLightsList();
            UpdateWaitingLightsList();

            state = State.LightsAreGreen;
        }
    }

    void LightsAreGreen()
    {
        if (WaitingLights.Count != 0 && GreenLights.Count > 0)
        {
            foreach (TrafficLight lp in WaitingLights)
            {
                bool canBeGreen = true;
                foreach (TrafficLight light in GreenLights)
                {
                    float tmp = GetDist(lp, light);

                    // print(tmp);

                    if (tmp != -1)
                    {
                        canBeGreen = false;
                    }
                }

                if (canBeGreen)
                {
                    lp.isGreen = true;
                    UpdateGreenLightsList();
                }
            }
            UpdateWaitingLightsList();
        }
        innerGreenTimer -= Time.deltaTime;
        if (innerGreenTimer <= 0)
        {
            innerGreenTimer = 0;
            if (WaitingLights.Count != 0 && GreenLights.Count != 0)
            {
                clearingtime = 0;
                state = State.ClearingDelay;
                int lightIndex = Random.Range(0, WaitingLights.Count - 1);
                TrafficLight nextLight = WaitingLights[lightIndex];
                foreach (TrafficLight light in GreenLights)
                {
                    float t_exit = GetDist(nextLight, light, true) / carSpeed;
                    float t_entry = GetDist(light, nextLight, false) / carSpeed;

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
        UpdateWaitingLightsList();
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
            if (WaitingLights.Count != 0)
            {
                TrafficLight next = WaitingLights[Random.Range(0, WaitingLights.Count - 1)];
                if (holder.forward.isGreen)
                {
                    GreenLights.Add(holder.forward);
                }
                if (holder.left.isGreen)
                {
                    GreenLights.Add(holder.left);
                }
            }
        }
    }

    void UpdateWaitingLightsList()
    {
        List<int> ints = new List<int>();
        for (int i = 0; i < WaitingLights.Count; i++)
        {
            if (WaitingLights[i].isGreen)
            {
                ints.Add(i);
            }
        }

        if (ints.Count != 0)
        {
            for (int i = ints.Count - 1; i >= 0; i--)
            {
                WaitingLights.RemoveAt(ints[i]);
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
        // print("x = " + x.ToString() + " y =" + y.ToString());
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
