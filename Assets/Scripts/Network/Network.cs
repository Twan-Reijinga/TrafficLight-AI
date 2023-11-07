using System;

class Test{
    static void Main() {
        Console.WriteLine("test");
        int[] neuronCounts = {8, 2, 4};
        Network network = new Network(neuronCounts);
    }
}

class Network {
    int[] layers;
    public Network(int[] neuronCounts) {
        Layer layer = new Layer(neuronCounts[0], neuronCounts[1]);
        Console.WriteLine(layer.getInfo());
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