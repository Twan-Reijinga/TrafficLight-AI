using System;
using System.Collections.Generic;
using System.Linq;

// class Test{
//     static void Main() {
        // int[] neuronCounts = {5, 6, 4};
        // Network network = new Network(neuronCounts);
        // float[] states = {0.1f, 0.2f, 0.4f, 0.8f, 1.7f};
        // Network.feedForward(network, inputs);
        // QLearnAgent agent = new QLearnAgent(network, 0.5f);
        // agent.SelectAction(states, neuronCounts[neuronCounts.Length - 1]);
//     }
// }

class ExperienceReplay {
    private int replayIndex;
    private int capacity;

    private float[][] states;
    private int[] actions;
    private float[] rewards;
    private float[][] nextStates;

    public ExperienceReplay(int capacity) {
        replayIndex = 0;
        this.capacity = capacity;
        states = new float[capacity][];
        actions = new int[capacity];
        rewards = new float[capacity];
        nextStates = new float[capacity][];
    }

    // TODO: state and nextState in List<float> form //
    public bool Add(float[] state, int action, float reward, float[] nextState) {
        if(replayIndex > capacity) return false;
        states[replayIndex] = state;
        actions[replayIndex] = action;
        rewards[replayIndex] = reward;
        nextStates[replayIndex] = nextState;
        replayIndex++;
        return true;
    }

    public bool IsEmpty() {
        return replayIndex == 0;
    }

    public (List<float>, int, float, List<float>) GetSample() {
        Random rand = new Random();
        int randomIndex = rand.Next();

        List<float> state = states[randomIndex].ToList();
        int action = actions[randomIndex];
        float reward = rewards[randomIndex];
        List<float> nextState = nextStates[randomIndex].ToList();


        return (state, action, reward, nextState);        
    }
}

class QLearnAgent {
    Network network;
    ExperienceReplay experienceReplay;
    private int maxIterations;
    private int NumberOfInputs;
    private int NumberOfOutputs;

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
        experienceReplay.Add(state.ToArray(), action, reward, nextState.ToArray());

        // Temp!
        return state;
    }

    public void Learn() {
        // TODO: use experience to learn from experience //
        (List<float> state, int action, float reward, List<float> nextState) = experienceReplay.GetSample();
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

class Network {
    private Layer[] layers;
    public Network(int[] neuronCounts) {
        layers = new Layer[neuronCounts.Length -1];
        for(int i = 0; i < neuronCounts.Length - 1;i++) {
            Layer layer = new Layer(neuronCounts[i], neuronCounts[i + 1]);
            Layer.Randomize(layer);
            if(i == neuronCounts.Length - 2) {
                layer.isOutputLayer = true;
            }
            layers[i] = layer;
        }
    }

    public static List<float> FeedForward(Network network, List<float> inputs) {
        List<float> outputs = Layer.FeedForward(network.layers[0], inputs);
        for(int i = 1; i < network.layers.Length; i++) {
            outputs = Layer.FeedForward(network.layers[i], outputs);
        }
        return outputs;
    }
};

class Layer {
    private int inputCount;
    private int outputCount;
    private float[] biases;
    private float[][] weights;
    public bool isOutputLayer = false;

    public Layer(int numberOfInputs, int numberOfOutputs) {
        inputCount = numberOfInputs;
        outputCount = numberOfOutputs;
    }

    public static void Randomize(Layer layer) {
        Random rand = new Random();
        layer.biases = new float[layer.outputCount];
        for(int i = 0; i < layer.outputCount; i++) {
            layer.biases[i] = (float) rand.NextDouble();
        }

        layer.weights = new float[layer.outputCount][];
        for (int i = 0; i < layer.outputCount; i++) {
            layer.weights[i] = new float[layer.inputCount];
            for (int j = 0; j < layer.inputCount; j++) {
                layer.weights[i][j] = (float) rand.NextDouble() * 2 - 1;
            }
        }
        return;
    }

    public static List<float> FeedForward(Layer layer, List<float> inputs) {
        if(inputs.Count != layer.inputCount) {
            throw new Exception("layer inputCount (" + layer.inputCount + ") does not match given inputs (" + inputs.Count + ")");
        } 
        // List<float> outputs = new float[layer.outputCount];
        List<float> outputs = new List<float>();
        for(int i = 0; i < layer.outputCount; i++) {
            float weightedSum = 0;
            for(int j = 0; j < layer.inputCount; j++) {
                weightedSum += inputs[j] * layer.weights[i][j];
            }

            if(layer.isOutputLayer){
                outputs.Add(weightedSum - layer.biases[i]);
            } else {
                outputs.Add(Signoid(weightedSum - layer.biases[i]));
            }
        }
        return outputs;
    }

    private static float Signoid(float x) {
        return 1.0f/(1.0f + MathF.Exp(-x)); 
    }

    public string GetInfo() {
        return "input: " + inputCount + " outputCount: " + outputCount;
    }
};