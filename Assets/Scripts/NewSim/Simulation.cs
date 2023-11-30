using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator;

public class Simulation : MonoBehaviour
{
    //DIT WORDT DE INTERFACE MET DE SIMULATOR, SIMULATOR IS GEWOON EEN AANTAL FUNCTIES, DIT GAAT DEZE FUNCTIES AANROEPEN EN NAAR DE AI STUREN

    public float timeStepSize = 0.05f;
    public float timeBetweenVisualisations = 0.05f;

    public int seed;
    public Visualizer visualiser;
    Simulator.Simulator simulator = new Simulator.Simulator();

    public bool runAtSetSpeed = true;
    public bool paused = false;

    void Start()
    {
        simulator.seed = seed;
        simulator.TestPopulation();
        Visualize();
        Step();
        VisualsUpdater();
    }

    void Visualize()
    {
        visualiser.SetVisuals(simulator.GetGraphicSceneState());
    }

    void Step()
    {
        if (!paused)
            simulator.Step(timeStepSize);
        if (runAtSetSpeed)
        {
            Invoke(nameof(Step), timeStepSize);
            return;
        }
        Invoke(nameof(Step), 0);

    }

    void VisualsUpdater()
    {
        Visualize();
        Invoke(nameof(VisualsUpdater), timeBetweenVisualisations);
    }

}
