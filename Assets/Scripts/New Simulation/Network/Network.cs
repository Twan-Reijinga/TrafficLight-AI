using System;

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

class QLearnAgent {
    Network network;
    private int NumberOfInputs;
    private int NumberOfOutputs;
    private float epsilon;
    public QLearnAgent(int[] neuronCounts, float startExploreChance) {
        Network network = new Network(neuronCounts);
        NumberOfInputs = neuronCounts[0];
        NumberOfOutputs = neuronCounts[neuronCounts.Length - 1];
        epsilon = startExploreChance;
    }
    
    public void SelectAction(float[] states) {
        Random rand = new Random();
        float randomChanceValue = (float) rand.NextDouble();
        if(randomChanceValue < epsilon) {
            rand.Next(0, NumberOfOutputs);
            // explore
            Console.WriteLine("explore mode");

        } else {
            // exploit
            Console.WriteLine("exploit mode");
            Network.FeedForward(network, states);
        }
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