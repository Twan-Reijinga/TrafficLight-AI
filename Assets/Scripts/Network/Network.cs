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
        layers = new Layer[neuronCounts.Length];
        for(int i = 0; i < neuronCounts.Length - 1;i++) {
            Layer layer = new Layer(neuronCounts[i], neuronCounts[i + 1]);
            layers[i] = layer;
            Console.WriteLine(layer.getInfo());
        }
    }
};

class Layer {
    int[] inputNodes;
    int[] outputNodes;
    public Layer(int inputCount, int outputCount) {
        inputNodes = new int[inputCount];
        outputNodes = new int[outputCount];
    }

    public string getInfo() {
        return "inputCount: " + inputNodes.Length + " outputCount: " + outputNodes.Length;
    }
};

// using System;
// class HelloWorld {
//   static void Main() {
//     Console.WriteLine("Hello World!");
//   }
// }