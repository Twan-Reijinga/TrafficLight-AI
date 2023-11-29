using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator;

public class Simulation : MonoBehaviour
{
    //DIT WORDT DE INTERFACE MET DE SIMULATOR, SIMULATOR IS GEWOON EEN AANTAL FUNCTIES, DIT GAAT DEZE FUNCTIES AANROEPEN EN NAAR DE AI STUREN

    public float timeStepSize = 0.05f;
    public float timeBetweenSteps = 0.05f;

    public int seed;
    public Visualizer visualiser;
    Simulator.Simulator simulator = new Simulator.Simulator();

    void Start()
    {
        simulator.seed = seed;
        simulator.TestPopulation();
        Visualize();

        Step();
    }

    void Visualize()
    {
        visualiser.SetVisuals(simulator.GetGraphicSceneState());
    }

    void Step()
    {
        simulator.Step(timeStepSize);
        Invoke(nameof(Step), timeBetweenSteps);
        Visualize();
    }

}
