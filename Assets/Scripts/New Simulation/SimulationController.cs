using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using UnityEditor.SearchService;

public class SimulationController : MonoBehaviour
{
    //DIT WORDT DE INTERFACE MET DE SIMULATOR, SIMULATOR IS GEWOON EEN AANTAL FUNCTIES, DIT GAAT DEZE FUNCTIES AANROEPEN EN NAAR DE AI STUREN

    public float timeStepSize = 0.05f;
    public float timeBetweenVisualisations = 0.05f;

    public int seed;
    public Visualizer visualiser;
    SimulationAPI.Simulator simulator = new SimulationAPI.Simulator();

    public bool runAtSetSpeed = true;
    public bool paused = false;

    public TextMeshProUGUI tpsCounter, MSPTCounter;

    private float lastframe;

    void Start()
    {
        simulator.seed = seed;
        simulator.TestPopulation();
        Step();
        VisualsUpdater();
        lastframe = Time.time;
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
            simulator.Step(timeStepSize);
            float dt = Time.time - lastframe;
            lastframe = Time.time;
            updateTPSCounter(dt);
        }
        if (runAtSetSpeed)
        {
            Invoke(nameof(Step), timeStepSize);
            return;
        }
        Invoke(nameof(Step), 0);
    }

    void updateTPSCounter(float dt)
    {
        tpsCounter.text = "tps:    " + Mathf.Clamp(Mathf.Round(1 / dt), 0, runAtSetSpeed ? 1 / timeStepSize : 99999).ToString();
        dt = Mathf.Round(dt * 10f) / 10f;
        MSPTCounter.text = "mspt: " + (dt * 1000).ToString();
    }

    void VisualsUpdater()
    {
        Visualize();
        Invoke(nameof(VisualsUpdater), timeBetweenVisualisations);
    }

}
