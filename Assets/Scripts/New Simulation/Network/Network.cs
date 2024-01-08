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

    public static void BackPropagate(Network network, List<float> state, int action, float targetValue) {
        // TODO: backpropagate //
        return;
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

            if(layer.isOutputLayer) {
                outputs.Add(weightedSum - layer.biases[i]);
            } else {
                outputs.Add(Signoid(weightedSum - layer.biases[i]));
            }
        }
        return outputs;
    }
    
    public static void BackPropagate(Network network, List<float> state, int action, float targetValue) {
        // TODO: backpropagate //
        return;
    }

    private static float Signoid(float x) {
        return 1.0f/(1.0f + MathF.Exp(-x)); 
    }

    public string GetInfo() {
        return "input: " + inputCount + " outputCount: " + outputCount;
    }
};