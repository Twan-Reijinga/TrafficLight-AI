using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using UnityEngine.Networking;


public class ResponseData
{
    public int action;
}

public class SimulationController : MonoBehaviour
{
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

    private float lastframe;

    private QLearnAgent qAgent;
    [SerializeField] private bool isAIControlled = true;
    private int[] networkNeuronCounts = { 4, 6, 4 }; // [3*(4*carLimit), ~, cycles]
    private int maxIterations = 100;

    private int currentAction;
    private string SERVER_URL = "http://localhost:8000/";

    void Start()
    {
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
        lastframe = Time.time;
        visualiser.SetSeed(seed);
    }

    IEnumerator Upload(string state, int action, float reward, bool done)
    {
        string data =
            "{ \"state\": " + state
            + ", \"action\": " + action
            + ", \"reward\": " + reward
            + ", \"done\": " + (done ? 1 : 0) + " }";

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
                bool done = false;
                if((this.stepCount / 20) % this.maxIterations == 0) {
                    done = true;
                }
                StartCoroutine(GetRequest());
                if (this.currentAction >= 0)
                {
                    simulator.intersections[0].ChangeSignalFase(this.currentAction);
                }

                List<float> state = simulator.GetState(0, 16);
                float reward = simulator.intersections[0].CalculateReward();

                string jsonState = "[" + string.Join(", ", state) + "]";
                StartCoroutine(Upload(jsonState, this.currentAction, reward, done));
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

}
