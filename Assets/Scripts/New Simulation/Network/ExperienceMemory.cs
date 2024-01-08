using System;
using System.Collections.Generic;
using System.Linq;

struct Experience {
    public List<float> state;
    public int action;
    public float reward;
    public List<float> nextState;
}

class ExperienceReplay {
    private int replayIndex;
    private int capacity;
    public List<Experience> experiences = new List<Experience>();

    public ExperienceReplay(int capacity) {
        replayIndex = 0;
        this.capacity = capacity;
        // states = new float[capacity][];
        // actions = new int[capacity];
        // rewards = new float[capacity];
        // nextStates = new float[capacity][];
    }

    // TODO: state and nextState in List<float> form //
    public bool Add(List<float> state, int action, float reward, List<float> nextState) {
        if(replayIndex > capacity) return false;

        Experience experience;
        experience.state = state;
        experience.action = action;
        experience.reward = reward;
        experience.nextState = nextState;

        experiences.Add(experience);
        return true;
    }

    public bool IsEmpty() {
        return replayIndex == 0;
    }

    public List<Experience> GetSample(int sampleSize) {
        // TODO: give multiple samples //
        List<Experience> samples = new List<Experience>();
        Random rand = new Random();
        for(int i = 0; i < sampleSize; i++) {
            int randomIndex = rand.Next();
            samples.Add(experiences[randomIndex]);
        }

        return (state, action, reward, nextState);        
    }
}