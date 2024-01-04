using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using UnityEditor.SearchService;
using System;

public class SimulationController : MonoBehaviour
{
    //DIT WORDT DE INTERFACE MET DE SIMULATOR, SIMULATOR IS GEWOON EEN AANTAL FUNCTIES, DIT GAAT DEZE FUNCTIES AANROEPEN EN NAAR DE AI STUREN

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
    private bool isAIControlled;
    private int[] networkNeuronCounts = {3, 6, 4}; // [3*(4*carLimit), ~, cycles]
    private int maxIterations;

    void Start()
    {
        maxIterations = 50_000; // TO DO: maxIterations given as parameter of Start()  //
        isAIControlled = true; // TO DO: isAIControlled given as parameter of Start() //
        if(isAIControlled) 
        {
            qAgent = new QLearnAgent(networkNeuronCounts, maxIterations);
        }
        simulator = new Simulator(seed);
        simulator.write += simulator_print;
        simulator.TestPopulation();
        Step();
        VisualsUpdater();
        lastframe = Time.time;
    }

    static void simulator_print(object sender, WriteEventArgs e)
    {
        print(e.msg);
    }

    void Visualize()
    {
        visualiser.SetVisuals(simulator.GetGraphicSceneState());
        visualiser.timeBetweenVisualisations = timeBetweenVisualisations;
    }

    void Step()
    {
        if (!paused)
        {
            if(isAIControlled)
            {
                // NOTE: qAgent stepSize could be bigger than simulator stap in future //
                qAgent.Step(simulator);
            }
            simulator.Step(timeStepSize);
            float dt = Time.time - lastframe;
            lastframe = Time.time;
            updateTPSCounter(dt);
        }
        if (runAtSetSpeed)
        {
            Invoke(nameof(Step), timeStepSize);
        } else 
        {
            Invoke(nameof(Step), 0);
        }
    }

    void updateTPSCounter(float dt)
    {
        tpsCounter.text = "tps:  " + Mathf.Clamp(Mathf.Round(1 / dt), 0, runAtSetSpeed ? 1 / timeStepSize : 99999).ToString();
        dt = Mathf.Round(dt * 10f) / 10f;
        MSPTCounter.text = "mspt: " + (dt * 1000).ToString();
    }

    void VisualsUpdater()
    {
        Visualize();
        Invoke(nameof(VisualsUpdater), timeBetweenVisualisations);
    }

}
