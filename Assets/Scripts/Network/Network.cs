using System;

class Test{
    static void Main() {
        int[] neuronCounts = {8, 2, 4};
        Network network = new Network(neuronCounts);
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
            Console.WriteLine(layer.biases[i]);
        }

        layer.weights = new float[inputNodes.Length][outputNodes.Length];
        for (int i = 0; i < layer.inputNodes.Length; i++) {
            for (int j = 0; j < layer.inputNodes.Length; j++) {
                layer.weights[i][j] = (float) rand.NextDouble() * 2 - 1;
            }
        }
        return;
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