using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Linq;


public class ResponseData
{
    public int action;
}

public class SimulationController : MonoBehaviour
{
    public static SimulationController instance;
    //DIT WORDT DE INTERFACE MET DE SIMULATOR, SIMULATOR IS GEWOON EEN AANTAL FUNCTIES, DIT GAAT DEZE FUNCTIES AANROEPEN EN NAAR DE AI STUREN
    public int stepCount;
    public float timeStepSize = 0.05f;
    public float timeBetweenVisualisations = 0.05f;

    public int seed;
    public Visualizer visualiser;
    SimulationAPI.Simulator simulator;

    public bool runAtSetSpeed = true;
    [HideInInspector] public bool paused = false;
    public bool doVisualize = true;

    public bool showNLAverage = false;

    public TextMeshProUGUI tpsCounter, MSPTCounter;

    private float lastframe;

    // private QLearnAgent qAgent;
    // private int[] networkNeuronCounts = { 4, 6, 4 }; // [3*(4*carLimit), ~, cycles]
    public bool isAIControlled = true;
    private int maxIterations = 250;

    private List<int> currentActions; // [trafficLight, action]
    private List<List<int>> previousActions = new List<List<int>> { new List<int>(), new List<int>() }; // [traficLight, ...allActions]
    private string SERVER_URL0 = "http://localhost:8000/";
    private string SERVER_URL1 = "http://localhost:8001/";
    private List<float> Scores = new List<float>();

    void Start()
    {
        if (instance != null)
        {
            throw new Exception("SimulationController already exists!");
        }
        instance = this;

        ResetSimulation(seed);

        Step();
        VisualsUpdater();
        lastframe = Time.time;
    }

    IEnumerator Upload(string state, int action, float reward, bool done, string url)
    {
        string data =
            "{ \"state\": " + state
            + ", \"action\": " + action
            + ", \"reward\": " + reward
            + ", \"done\": " + (done ? 1 : 0) + " }";

        using (UnityWebRequest www = UnityWebRequest.Post(url, data, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!" + www.downloadHandler.text);
            }
        }
    }

    private IEnumerator SaveLoadRequest(string name, string operation, string url)
    {
        string data = "{ \"name\": \"" + name + "\" }";
        using (UnityWebRequest www = UnityWebRequest.Post(url + operation, data, "application/json"))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error while saving/loading: " + www.error);
            }
            else
            {
                // Debug.Log("Successfully " + ((operation == "save") ? "saved " : "loaded ") + name);
            }
        }
    }

    IEnumerator GetRequest(string url, int intersectionIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                    // Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    this.previousActions[intersectionIndex].Add(this.currentActions[intersectionIndex]);

                    List<int> prevActions = this.previousActions[intersectionIndex];
                    if (prevActions.Count > 9)
                    {
                        for (int i = prevActions.Count - 1; i >= prevActions.Count - 8; i--)
                        {
                            if (prevActions[i] == responseData.action)
                            {
                                if (i <= (prevActions.Count - 4))
                                {
                                    Simulator.instance.scoreAddend[intersectionIndex] -= 0.5f;
                                }
                                else
                                {
                                    Simulator.instance.scoreAddend[intersectionIndex] += 0.05f;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    // if(this.currentActions[intersectionIndex] == responseData.action) {
                    //     Simulator.instance.scoreAddend[intersectionIndex] += 0.18f;
                    // }


                    this.currentActions[intersectionIndex] = responseData.action;
                    break;
            }
        }

    }

    static void simulator_print(object sender, WriteEventArgs e)
    {
        print(e.msg);
    }

    void Visualize()
    {
        visualiser.SetVisuals(simulator.GetGraphicSceneState());
        visualiser.timeBetweenVisualisations = timeBetweenVisualisations;
        updateStepCounter();
    }

    void Step()
    {
        if (!paused)
        {
            this.stepCount++;
            if (isAIControlled && this.stepCount % 20 == 0 && this.stepCount > 6 * 20)
            {
                bool done = ((this.stepCount) % (this.maxIterations * 20)) == 0;
                for (int i = 0; i < 2; i++)
                {
                    string url = i == 1 ? this.SERVER_URL1 : this.SERVER_URL0;
                    StartCoroutine(GetRequest(url, i));
                    if (this.currentActions[i] >= 0)
                    {
                        simulator.intersections[i].ChangeSignalFase(this.currentActions[i]);
                    }

                    List<int> lastActions = GetLastActions(i, 8);
                    List<float> state = simulator.GetState(i, 16, lastActions);

                    float reward = simulator.intersections[i].CalculateReward();

                    string jsonState = "[" + string.Join(", ", state) + "]";
                    StartCoroutine(Upload(jsonState, this.currentActions[i], reward, done, url));
                }
                if (done)
                {
                    print("NEXT GENERATION INCOMMING!");
                    ResetSimulation(this.seed + 1);
                }
                // List<float> debugValues = qAgent.Step(simulator);
            }
            if (!isAIControlled && this.stepCount == 5000 && showNLAverage)
            {

                float currentScore = (simulator.intersections[0].CalculateReward() + simulator.intersections[1].CalculateReward()) / 2;
                Scores.Add(currentScore);
                print("current Score: " + currentScore.ToString() + " Average: " + Scores.Average() + " Min: " + Scores.Min() + " Max: " + Scores.Max());
                ResetSimulation(seed + 1);
            }
            simulator.Step(this.timeStepSize, isAIControlled);

        }
        if (runAtSetSpeed)
        {
            Invoke(nameof(Step), this.timeStepSize);
        }
        else
        {
            Invoke(nameof(Step), 0);
        }
    }

    void updateStepCounter()
    {
        tpsCounter.text = "step:\n" + this.stepCount / 1000 + "k";
    }

    void VisualsUpdater()
    {
        if (doVisualize)
        {
            Visualize();
        }
        Invoke(nameof(VisualsUpdater), timeBetweenVisualisations);
    }

    public void ResetSimulation(int newSeed)
    {
        this.stepCount = 0;
        this.currentActions = new List<int> { -2, -2 };

        for (int i = visualiser.transform.childCount - 1; i >= 0; i--)  // clear children of visualizer;
        {
            Destroy(visualiser.transform.GetChild(i).gameObject);
        }
        visualiser.idToCars = null;
        visualiser.state1 = null;
        visualiser.state2 = null;

        CancelInvoke();
        // print("Simulation Reset with seed: " + newSeed);
        seed = newSeed;
        Simulator.instance = null; //remove old instance
        simulator = new Simulator(seed);
        simulator.write += simulator_print;



        Step();
        visualiser.SetSeed(seed);
        VisualsUpdater();
    }

    public List<int> GetLastActions(int intersectionIndex, int numberOfActions)
    {
        List<int> lastActions = new List<int>();
        int paddingCount = Math.Max(0, numberOfActions - this.previousActions[intersectionIndex].Count);

        for (int i = 0; i < paddingCount; i++)
        {
            lastActions.Add(0);
        }
        for (int i = Math.Max(0, this.previousActions[intersectionIndex].Count - numberOfActions); i < this.previousActions[intersectionIndex].Count; i++)
        {
            lastActions.Add(this.previousActions[intersectionIndex][i]);
        }
        return lastActions;
    }

    public void SaveNN(string name)
    {
        StartCoroutine(SaveLoadRequest(name + "0", "save", SERVER_URL0));
        StartCoroutine(SaveLoadRequest(name + "1", "save", SERVER_URL1));
        Debug.Log(name);
    }

    public void LoadNN(string name, int seed = 0)
    {
        Debug.Log(name);
        StartCoroutine(SaveLoadRequest(name + "0", "load", SERVER_URL0));
        StartCoroutine(SaveLoadRequest(name + "1", "load", SERVER_URL1));
        ResetSimulation(seed);
    }
}

