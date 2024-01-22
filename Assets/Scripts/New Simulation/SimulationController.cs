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

    // private QLearnAgent qAgent;
    // private int[] networkNeuronCounts = { 4, 6, 4 }; // [3*(4*carLimit), ~, cycles]
    [SerializeField] private bool isAIControlled = true;
    private int maxIterations = 100;

    private int[] currentActions;
    private string SERVER_URL0 = "http://localhost:8000/";
    private string SERVER_URL1 = "http://localhost:8001/";

    void Start()
    {
        // isAIControlled = true; // TO DO: isAIControlled given as parameter of Start() //
        // if (isAIControlled)
        // {
        //     qAgent = new QLearnAgent(networkNeuronCounts, maxIterations);
        // }
        Reset();
        visualiser.SetSeed(seed);

        Step();
        VisualsUpdater();
        lastframe = Time.time;
    }

    void Reset()
    {
        this.stepCount = 0;
        this.currentActions = new int[2] { -2, -2};

        simulator = new Simulator(seed);
        simulator.write += simulator_print;
        simulator.TestPopulation();
        this.seed += 1;
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
                Debug.Log("Form upload complete!" + www.downloadHandler.text);
            }
        }
    }

    IEnumerator GetRequest(string url, int actionIndex)
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
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.currentActions[actionIndex] = responseData.action;
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
                bool done = (this.stepCount / 20) % this.maxIterations == 0;
                for(int i = 0; i < 2; i++) {
                    string url = i == 1 ? this.SERVER_URL1 : this.SERVER_URL0;
                    StartCoroutine(GetRequest(url, i));
                    if (this.currentActions[i] >= 0)
                    {
                        simulator.intersections[i].ChangeSignalFase(this.currentActions[i]);
                    }

                    List<float> state = simulator.GetState(i, 16);
                    float reward = simulator.intersections[i].CalculateReward();

                    string jsonState = "[" + string.Join(", ", state) + "]";
                    StartCoroutine(Upload(jsonState, this.currentActions[i], reward, done, url));
                }
                if(done) {
                    // Reset();
                }
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
