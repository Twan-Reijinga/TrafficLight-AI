import gym
from deepQNetwork import Agent
from utils import plot_learning_curve
import numpy as np

if __name__ == '__main__':
    env = gym.make('LunarLander-v2')
    agent = Agent(gamma=0.99, epsilon=1.0, batch_size=64, n_actions=4,
                  eps_end=0.01, input_dims=[8], lr=0.0005)
    scores, eps_history = [], []
    n_games = 500
    
    for i in range(n_games):
        score = 0
        done = False
        state, _ = env.reset()
        while not done:
            action = agent.choose_action(state)
            next_state, reward, done, _, info = env.step(action)
            score += reward
            agent.store_transition(state, action, reward, next_state, done)
            agent.learn()
            state = next_state
        
        scores.append(score)
        eps_history.append(agent.epsilon)
        
        avg_score = np.mean(scores[-100:])
        
        print('episode ', i, 'score %.2f' % score,
              'avg score %.2f' % avg_score,
              'epsilon %.2f' % agent.epsilon)
    
    x = [i + 1 for i in range(n_games)]
    filename = 'LunarLander-v2_eps_history.png'
    plot_learning_curve(x, scores, eps_history, filename)
    