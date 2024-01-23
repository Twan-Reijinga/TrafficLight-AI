using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using UnityEngine.Networking;
using System;


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
    public bool paused = false;

    public TextMeshProUGUI tpsCounter, MSPTCounter;

    private QLearnAgent qAgent;
    [SerializeField] private bool isAIControlled = true;
    private int[] networkNeuronCounts = { 4, 6, 4 }; // [3*(4*carLimit), ~, cycles]
    private int maxIterations = 5000;

    private int currentAction;
    private string SERVER_URL = "http://localhost:8001/";

    void Start()
    {
        if (instance != null)
        {
            throw new Exception("SimulationController already exists!");
        }
        instance = this;

        currentAction = -2;
        // isAIControlled = true; // TO DO: isAIControlled given as parameter of Start() //
        if (isAIControlled)
        {
            qAgent = new QLearnAgent(networkNeuronCounts, maxIterations);
        }
        simulator = new Simulator(seed);
        simulator.write += simulator_print;
        simulator.TestPopulation();
        Step();
        VisualsUpdater();
        visualiser.SetSeed(seed);
    }

    IEnumerator Upload(string state, int action, int reward, int nextState, int done)
    {
        string data =
            "{ \"state\": " + state
            + ", \"action\": " + action
            + ", \"reward\": " + reward
            + " , \"nextState\": " + nextState
            + ", \"done\": " + done + " }";

        using (UnityWebRequest www = UnityWebRequest.Post(this.SERVER_URL, data, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!" + www.downloadHandler.text);
            }
        }
    }

    IEnumerator GetRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.SERVER_URL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = this.SERVER_URL.Split('/');
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
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.currentAction = responseData.action;
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
        this.stepCount++;
        if (!paused)
        {
            if (isAIControlled && this.stepCount % 20 == 0)
            {
                int done = this.stepCount / this.maxIterations;
                StartCoroutine(GetRequest());
                if (this.currentAction >= 0)
                {
                    simulator.intersections[0].ChangeSignalFase(this.currentAction);
                }

                List<float> state = simulator.GetState(0, 16);

                string jsonState = "[" + string.Join(", ", state) + "]";
                StartCoroutine(Upload(jsonState, this.currentAction, 3, 3, done));
                // List<float> debugValues = qAgent.Step(simulator);
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
        Visualize();
        Invoke(nameof(VisualsUpdater), timeBetweenVisualisations);
    }

    public void ResetSimulation(int newSeed)
    {
        for (int i = visualiser.transform.childCount - 1; i >= 0; i--)  // clear children of visualizer;
        {
            Destroy(visualiser.transform.GetChild(i).gameObject);
        }
        visualiser.idToCars = null;
        visualiser.state1 = null;
        visualiser.state2 = null;

        CancelInvoke();
        print("Simulation Reset with seed: " + newSeed);
        seed = newSeed;
        Simulator.instance = null; //remove old instance
        simulator = new Simulator(seed);
        simulator.write += simulator_print;



        Step();
        visualiser.SetSeed(seed);
        VisualsUpdater();
    }
}
