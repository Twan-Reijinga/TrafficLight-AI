using System;
using System.Collections.Generic;
using System.Linq;

class QLearnAgent {
    Network network;
    ExperienceReplay experienceReplay;
    private int maxIterations;
    private int NumberOfInputs;
    private int NumberOfOutputs;

    private const float DISCOUNT_FACTOR = 0.9f;
    private const float EPSILON_START = 0.9f;
    private const float EPSILON_END = 0.05f;
    private const float EPSILON_DECAY = 1000; // decay using e^(-steps/decay)

    private const int MAX_QUEUE_LENGTH = 24;
    private int currentStep;

    public QLearnAgent(int[] neuronCounts, int maxIterations) {
        network = new Network(neuronCounts);
        NumberOfInputs = neuronCounts[0];
        NumberOfOutputs = neuronCounts[neuronCounts.Length - 1];
        
        this.maxIterations = maxIterations;
        experienceReplay = new ExperienceReplay(maxIterations);
        currentStep = 0;
    }

    public List<float> Step(SimulationAPI.Simulator simulator) {
        int crossingNumber = 1;
        int[] queueLenghtsBefore = simulator.GetQueueLenghts(crossingNumber);
        List<float> trafficLightStateBefore = simulator.GetTrafficLightState(crossingNumber);

        // TODO: get prevStates somehow...
        float[] testValues = {5.0f, 13.0f, 14.0f};
        List<float> prevState = new List<float>(testValues);

        List<float> state = CalcState(prevState, queueLenghtsBefore, trafficLightStateBefore);
        int action = SelectAction(state);
        simulator.ChangeSignalFase(action);
        int[] queueLenghtsAfter = simulator.GetQueueLenghts(crossingNumber);
        List<float> trafficLightStateAfter = simulator.GetTrafficLightState(crossingNumber);
        List<float> nextState = CalcState(state, queueLenghtsAfter, trafficLightStateAfter);
        float reward = CalcReward(state, nextState);

        // TODO: implement done variable for exp.replay //
        experienceReplay.Add(state, action, reward, nextState);

        // Temp!
        return state;
    }

    public void Learn() {
        // TODO: use experience to learn from experience //
        (List<float> state, int action, float reward, List<float> nextState) = experienceReplay.GetSample(32);
        float nextStateEvaluation = Network.FeedForward(network, nextState).Max();
        float targetValue = reward + DISCOUNT_FACTOR * nextStateEvaluation;
        // network.BackPropagate(state, action, targetValue);

        return;
    }
    
    public int SelectAction(List<float> state) {
        int action;
        Random rand = new Random();
        float randomChanceValue = (float) rand.NextDouble();
        float epsilon = EPSILON_END + (EPSILON_START - EPSILON_END) * MathF.Exp(-currentStep/EPSILON_DECAY);
        currentStep++;
        if(randomChanceValue < epsilon) {
            action = rand.Next(0, NumberOfOutputs);
            // explore
            Console.WriteLine("explore mode");

        } else {
            // exploit
            Console.WriteLine("exploit mode");
            List<float> actionSpace = Network.FeedForward(network, state);
            action = actionSpace.IndexOf(actionSpace.Max());

        }
        return action;
    }

    static private List<float> CalcState(List<float> prevState, int[] queueLenghts, List<float> trafficLightState) {
        // TODO: state = current + 2 previous; add last 2 states to nextState //'
        List<float> processedQueueLengths = processQueueLenghts(queueLenghts);

        List<float> state = new List<float>(processedQueueLengths.Count + trafficLightState.Count);
        state.AddRange(processedQueueLengths);
        state.AddRange(trafficLightState);

        return state;
    }
    
    static private List<float> processQueueLenghts(int[] queueLenghts) {
        List<float> processedQueueLengths = new List<float>();

        for(int i = 0; i < queueLenghts.Length; i++) {
            for(int j = 0; j < queueLenghts[i]; j++) {
                processedQueueLengths.Add(1.0f);
            } 

            for(int j = queueLenghts[i]; j < MAX_QUEUE_LENGTH; j++) {
                processedQueueLengths.Add(0.0f);
            } 
        }
        return processedQueueLengths;
    }

    static private float CalcReward(List<float> queueLenghtsBefore, List<float> queueLenghtsAfter) {
        // TODO: calc reward form improvement in state (-1, 1) //

        float reward = queueLenghtsAfter.Sum();
        return reward;
    }
}
