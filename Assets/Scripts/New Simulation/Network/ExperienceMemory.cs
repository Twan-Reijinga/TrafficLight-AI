using System;
using System.Collections.Generic;
using System.Linq;

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

    public (List<float>, int, float, List<float>) GetSample(int sampleSize) {
        // TODO: give multiple samples //
        Random rand = new Random();
        int randomIndex = rand.Next();

        List<float> state = states[randomIndex].ToList();
        int action = actions[randomIndex];
        float reward = rewards[randomIndex];
        List<float> nextState = nextStates[randomIndex].ToList();


        return (state, action, reward, nextState);        
    }
}