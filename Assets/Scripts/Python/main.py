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
    def __init__(self, number_of_inputs):
        self.agent = Agent(gamma=0.99, epsilon=1.0, batch_size=128, n_actions=4,
                    eps_end=0.05, input_dims=[number_of_inputs], lr=0.001)
        self.scores, self.eps_history = [], []
        self.score = 0
        self.current_game = 0
        self.state = None
        # n_games = 800
        
        # for i in range(n_games):
        #     self.score = 0
        #     self.done = False
        #     # state, _ = env.reset()
        #     # while not self.done:
        #         # action = agent.choose_action(state)
        #         # next_state, reward, done, _, info = env.step(action)
        #         # score += reward
        #         # agent.store_transition(state, action, reward, next_state, done)
        #         # if(self.ready):
        #         #     self.agent.learn()
        #         #     self.state = self.next_state
        #         #     self.ready = False

        #     self.scores.append(self.score)
        #     self.eps_history.append(self.agent.epsilon)
            
        #     self.avg_score = np.mean(self.scores[-100:])
            
        #     print('episode ', i, 'score %.2f' % self.score,
        #         'avg score %.2f' % self.avg_score,
        #         'epsilon %.2f' % self.agent.epsilon)
        
        # x = [i + 1 for i in range(n_games)]
        # filename = 'eps_history.png'
        # plot_learning_curve(x, scores, eps_history, filename)
        
    def getAction(self):
        if not(self.state):
            print('state is empty')
            return -1
        return self.agent.choose_action(self.state)
    
    def store_transition(self, state, action, reward, done):
        self.agent.store_transition(state, action, reward, done)
        self.score += reward
        
        
        self.agent.learn()
        # self.next_state = next_state
        # self.state = self.next_state
        if(done):
            self.current_game += 1
            self.scores.append(self.score)
            self.eps_history.append(self.agent.epsilon)
            self.avg_score = np.mean(self.scores[-100:])
            print('episode ', self.current_game, 'score %.2f' % self.score,
                'avg score %.2f' % self.avg_score,
                'epsilon %.2f' % self.agent.epsilon)
    


class RequestHandler(http.server.SimpleHTTPRequestHandler):
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

        try:
            data = json.loads(post_data)
            print(data) 
            state = data['state']
            action = data['action']
            reward = data['reward']
            # next_state = data['nextState']
            
            done = data['done'] == 1 
            print("values: ", state, action, reward, done)
                
            DQN_runner.store_transition(state, action, reward, done)

            # replay_memory.push(state, action, reward, next_state)

            # Perform a Q-learning update
            # q_learning_update()

            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            response_message = {'message': 'Replay memory received and updated.'}
            self.wfile.write(json.dumps(response_message).encode('utf-8'))
        except json.JSONDecodeError as e:
            self.send_response(400)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            response_message = {'message': 'Error decoding JSON data', 'error': str(e)}
            self.wfile.write(json.dumps(response_message).encode('utf-8'))

if __name__ == '__main__':
    # global DQN_runner
    DQN_runner = DQN_Runner(136)
    PORT = 8006
    httpd = socketserver.TCPServer(("", PORT), RequestHandler)
    print(f"Serving on port {PORT}")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\nKeyboardInterrupt received. Shutting down gracefully.")
        httpd.shutdown()
    
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
    