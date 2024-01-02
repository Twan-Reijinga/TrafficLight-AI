using System;
using System.Collections.Generic;
using System.Linq;
// using QLearnAgent;
// using Network;

namespace NetworkAPI 
{
    public class NetworkInput 
    {
        private QLearnAgent agent;
        public NetworkInput(int[] neuronCounts) 
        {
           agent = new QLearnAgent(neuronCounts, 0.5f);
        }
        
        public void SubmitStates(float[] states) {
            agent.SelectAction(states);
        }
    }
    
}