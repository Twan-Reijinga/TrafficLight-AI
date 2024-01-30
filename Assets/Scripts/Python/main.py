import gym
from deepQNetwork import Agent
from utils import plot_learning_curve
import numpy as np

# from server import RequestHandler
import http.server
import socketserver
import json

DQN_runner = None

class DQN_Runner():
    def __init__(self, number_of_inputs, PORTstr):
        self.port = PORTstr
        self.agent = Agent(gamma=0.99, epsilon=1.0, batch_size=128, n_actions=4,
                    eps_end=0.05, input_dims=[number_of_inputs], lr=0.001, states_back = 1)
        self.scores, self.eps_history = [], []
        self.score = 0
        self.current_episode = 0
        self.current_episode_batch = 0
        self.max_episodes = 100
        
    def getAction(self):
        return self.agent.choose_action()
    
    def store_transition(self, state, action, reward, done):
        self.agent.store_transition(state, action, reward, done)
        self.score += reward
        print("score", self.score, "reward", reward)
        
        self.agent.learn()
        # self.next_state = next_state
        # self.state = self.next_state
        if(done):
            self.current_episode += 1
            self.scores.append(self.score)
            self.eps_history.append(self.agent.epsilon)
            self.avg_score = np.mean(self.scores[-100:])
            print('episode ', self.current_episode, 'score %.2f' % self.score,
                'avg score %.2f' % self.avg_score,
                'epsilon %.2f' % self.agent.epsilon)
            self.score = 0
            if((self.current_episode % self.max_episodes) == 0):
                self.graph_learning_curve()
                
    
    def graph_learning_curve(self):
        self.current_episode_batch += 1
        
        end = (self.current_episode_batch) * self.max_episodes
        filename = self.port + 'traficLearningCurve1-' + str(end) +  '.png'

        x = [i + 1 for i in range(end)]
        plot_learning_curve(x, self.scores, self.eps_history, filename)
    


class RequestHandler(http.server.SimpleHTTPRequestHandler):
    def log_message(self, format, *args):
        pass

    def do_GET(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()
        action = DQN_runner.getAction()
        # print(action)
        response_message = {'action': int(action)}
        self.wfile.write(json.dumps(response_message).encode('utf-8'))
    
    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length).decode('utf-8')
        data = json.loads(post_data)

        try:
            if self.path == '/save':
                name = data['name']
                DQN_runner.agent.save(name)
                response_message = {'message': 'Saved succesfully.'}
            elif self.path == '/load':
                name = data['name']
                DQN_runner.agent.load(name)
                response_message = {'message': 'Loaded succesfully.'}
            else:
                state = data['state']
                action = data['action']
                reward = data['reward']
                # next_state = data['nextState']
            
                done = data['done'] == 1 
                # print("test: ", state, action, reward, done)
                
                DQN_runner.store_transition(state, action, reward, done)
                response_message = {'message': 'Replay memory received and updated.'}

                # replay_memory.push(state, action, reward, next_state)

                # Perform a Q-learning update
                # q_learning_update()

            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            self.wfile.write(json.dumps(response_message).encode('utf-8'))
        except json.JSONDecodeError as e:
            self.send_response(400)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            response_message = {'message': 'Error decoding JSON data', 'error': str(e)}
            self.wfile.write(json.dumps(response_message).encode('utf-8'))

if __name__ == '__main__':
    # global DQN_runner
    PORT = int(input("Enter port: "))
    DQN_runner = DQN_Runner(24, str(PORT))
    httpd = socketserver.TCPServer(("", PORT), RequestHandler)
    print(f"Serving on port {PORT}")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        httpd.shutdown()
        print("\nKeyboardInterrupt received. Shutting down gracefully.")
    
    # env = gym.make('LunarLander-v2', render_mode="human")
    # agent = Agent(gamma=0.99, epsilon=1.0, batch_size=128, n_actions=4,
    #               eps_end=0.05, input_dims=[8], lr=0.001)
    # scores, eps_history = [], []
    # n_games = 800
    
    # for i in range(n_games):
    #     score = 0
    #     done = False
    #     state, _ = env.reset()
    #     while not done:
    #         action = agent.choose_action(state)
    #         next_state, reward, done, _, info = env.step(action)
    #         score += reward
    #         agent.store_transition(state, action, reward, next_state, done)
    #         agent.learn()
    #         state = next_state
        
    #     scores.append(score)
    #     eps_history.append(agent.epsilon)
        
    #     avg_score = np.mean(scores[-100:])
        
    #     print('episode ', i, 'score %.2f' % score,
    #           'avg score %.2f' % avg_score,
    #           'epsilon %.2f' % agent.epsilon)
    
    # x = [i + 1 for i in range(n_games)]
    # filename = 'LunarLander-v2_eps_history.png'
    # plot_learning_curve(x, scores, eps_history, filename)
    