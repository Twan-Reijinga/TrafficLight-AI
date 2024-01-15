import http.server
import socketserver
import json
import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
from collections import namedtuple, deque
import random

# Define the Q-network
class QNetwork(nn.Module):
    def __init__(self, state_size, action_size):
        super(QNetwork, self).__init__()
        self.fc1 = nn.Linear(state_size, 64)
        self.fc2 = nn.Linear(64, 64)
        self.fc3 = nn.Linear(64, action_size)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = torch.relu(self.fc2(x))
        x = self.fc3(x)
        return x

# Define the Replay Memory
Transition = namedtuple('Transition', ('state', 'action', 'reward', 'next_state'))

class ReplayMemory:
    def __init__(self, capacity):
        self.memory = deque(maxlen=capacity)

    def push(self, *args):
        self.memory.append(Transition(*args))

    def sample(self, batch_size):
        return random.sample(self.memory, batch_size)

# Initialize Q-network and Replay Memory
state_size = 4  # Example: state with 4 features
action_size = 2  # Example: 2 possible actions
learning_rate = 0.001
batch_size = 32
memory_capacity = 10000

q_network = QNetwork(state_size, action_size)
optimizer = optim.Adam(q_network.parameters(), lr=learning_rate)
replay_memory = ReplayMemory(memory_capacity)

# Define the Q-learning update function
def q_learning_update():
    if len(replay_memory.memory) < batch_size:
        return

    transitions = replay_memory.sample(batch_size)
    batch = Transition(*zip(*transitions))

    state_batch = torch.tensor(batch.state, dtype=torch.float32)
    action_batch = torch.tensor(batch.action, dtype=torch.long).view(-1, 1)
    reward_batch = torch.tensor(batch.reward, dtype=torch.float32)
    next_state_batch = torch.tensor(batch.next_state, dtype=torch.float32)

    # Q-values for the current state
    q_values = q_network(state_batch)

    # Q-values for the next state
    next_q_values = q_network(next_state_batch)
    next_max_q_values = next_q_values.max(dim=1)[0].detach()

    # Compute the target Q-values
    target_q_values = reward_batch + 0.99 * next_max_q_values

    # Compute the loss and update the Q-network
    loss = nn.functional.mse_loss(q_values.gather(1, action_batch), target_q_values.view(-1, 1))
    optimizer.zero_grad()
    loss.backward()
    optimizer.step()

# Define a custom request handler
class RequestHandler(http.server.SimpleHTTPRequestHandler):
    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length).decode('utf-8')

        try:
            data = json.loads(post_data)
            print(data)

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

# Run the HTTP server
if __name__ == "__main__":
    PORT = 8002
    with socketserver.TCPServer(("", PORT), RequestHandler) as httpd:
        print(f"Serving on port {PORT}")
        httpd.serve_forever()
