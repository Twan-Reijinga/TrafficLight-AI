using System;

class Test{
    static void Main() {
        int[] neuronCounts = {8, 2, 4};
        Network network = new Network(neuronCounts);
        float[] inputs = {0.1f, 0.2f, 0.4f, 0.8f, 1.7f};
        Network.feedForward(network, inputs);
    }
}

class Network {
Layer[] layers;
    public Network(int[] neuronCounts) {
        layers = new Layer[neuronCounts.Length -1];
        for(int i = 0; i < neuronCounts.Length - 1;i++) {
            Layer layer = new Layer(neuronCounts[i], neuronCounts[i + 1]);
            Layer.randomize(layer);
            layers[i] = layer;
            Console.WriteLine(layer.getInfo());
        }
        Console.WriteLine("length: " + layers.Length);
    }

    public static float[] feedForward(Network network, float[] inputs) {
        float[] outputs = Layer.feedForward(network.layers[0], inputs);
        for(int i = 0; i < network.layers.Length; i++) {
            outputs = Layer.feedForward(network.layers[i], outputs);
        }
        return outputs;
    }
};

class Layer {
    int[] inputNodes;
    int[] outputNodes;
    float[] biases;
    float[][] weights;
    public Layer(int inputCount, int outputCount) {
        inputNodes = new int[inputCount];
        outputNodes = new int[outputCount];
    }

    public static void randomize(Layer layer) {
        Random rand = new Random();
        layer.biases = new float[layer.outputNodes.Length];
        for(int i = 0; i < layer.outputNodes.Length; i++) {
            layer.biases[i] = (float) rand.NextDouble() * 2 - 1;
        }

        layer.weights = new float[layer.outputNodes.Length][];
        for (int i = 0; i < layer.outputNodes.Length; i++) {
            for (int j = 0; j < layer.inputNodes.Length; j++) {
                layer.weights[i] = new float[layer.inputNodes.Length];
                layer.weights[i][j] = (float) rand.NextDouble() * 2 - 1;
                Console.WriteLine(layer.weights[i][j]);
            }
        }
        return;
    }

    public static float[] feedForward(Layer layer, float[] inputs) {
        for(int i = 0; i < inputs.Length; i++) {
            inputs[i] += 1.0f;
            Console.WriteLine("input: " + inputs[i]);
        }
        return inputs;
    }

    public string getInfo() {
        return "biases: " + biases.Length + " outputCount: " + outputNodes.Length;
    }
};

// using System;
// class HelloWorld {
//   static void Main() {
//     Console.WriteLine("Hello World!");
//   }
// }