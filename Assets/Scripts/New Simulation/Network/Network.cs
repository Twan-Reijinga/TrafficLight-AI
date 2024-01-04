using System;
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

    public bool Add(float[] state, int action, float reward, float[] nextState) {
        if(replayIndex > capacity) return false;
        states[replayIndex] = state;
        actions[replayIndex] = action;
        rewards[replayIndex] = reward;
        nextStates[replayIndex] = nextState;
        replayIndex++;
        return true;
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
    private int currentStep;
    public QLearnAgent(int[] neuronCounts, int maxIterations) {
        network = new Network(neuronCounts);
        NumberOfInputs = neuronCounts[0];
        NumberOfOutputs = neuronCounts[neuronCounts.Length - 1];
        
        this.maxIterations = maxIterations;
        experienceReplay = new ExperienceReplay(maxIterations);
        currentStep = 0;
    }

    public void Step(SimulationAPI.Simulator simulator) {
        int[] traficLightNumbers = {5, 6, 7, 8};
        float[] queueLenghtsBefore = simulator.GetQueueLenghts(traficLightNumbers);
        float[] traficLightStateBefore = simulator.GetTraficLightState(traficLightNumbers);

        // TODO: get prevStates somehow...
        float[] prevState = {5.0f, 13.0f, 14.0f};

        float[] state = CalcState(prevState, queueLenghtsBefore, traficLightStateBefore);
        int action = SelectAction(state);
        simulator.ChangeSignalFase(action);
        float[] queueLenghtsAfter = simulator.GetQueueLenghts(traficLightNumbers);
        float[] traficLightStateAfter = simulator.GetTraficLightState(traficLightNumbers);
        float[] nextState = CalcState(state, queueLenghtsAfter, traficLightStateAfter);
        float reward = CalcReward(state, nextState);

        // TODO: implement done variable for exp.replay //
        experienceReplay.Add(state, action, reward, nextState);


    }
    
    public int SelectAction(float[] state) {
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
            float[] actionSpace = Network.FeedForward(network, state);
            action = Array.IndexOf(actionSpace, actionSpace.Max());

        }
        return action;
    }

    private float[] CalcState(float[] prevState, float[] queueLenghts, float[] traficLightState) {
        // TODO: state = current + 2 previous; add last 2 states to nextState //
        return queueLenghts;
    }

    private float CalcReward(float[] queueLenghtsBefore, float[] queueLenghtsAfter) {
        // TODO: calc reward form improvement in state (-1, 1) //
        return 1.0f;
    }
}

class Network {
    private Layer[] layers;
    public Network(int[] neuronCounts) {
        layers = new Layer[neuronCounts.Length -1];
        for(int i = 0; i < neuronCounts.Length - 1;i++) {
            Layer layer = new Layer(neuronCounts[i], neuronCounts[i + 1]);
            Layer.Randomize(layer);
            layers[i] = layer;
        }
    }

    public static float[] FeedForward(Network network, float[] inputs) {
        float[] outputs = Layer.FeedForward(network.layers[0], inputs);
        for(int i = 1; i < network.layers.Length; i++) {
            outputs = Layer.FeedForward(network.layers[i], outputs);
        }
        return outputs;
    }
};

class Layer {
    int inputCount;
    int outputCount;
    float[] biases;
    float[][] weights;
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

    public static float[] FeedForward(Layer layer, float[] inputs) {
        if(inputs.Length != layer.inputCount) {
            throw new Exception("layer inputCount (" + layer.inputCount + ") does not match given inputs (" + inputs.Length + ")");
        } 
        float[] outputs = new float[layer.outputCount];
        for(int i = 0; i < layer.outputCount; i++) {
            float weightedSum = 0;
            for(int j = 0; j < layer.inputCount; j++) {
                weightedSum += inputs[j] * layer.weights[i][j];
            }
            outputs[i] = Signoid(weightedSum - layer.biases[i]);
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