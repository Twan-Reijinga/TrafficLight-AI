import torch as T
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np
import itertools

class DeepQNetwork(nn.Module):
    def __init__(self, lr, input_dims, fc1_dims, fc2_dims, n_actions):
        super(DeepQNetwork, self).__init__()
        self.input_dims = input_dims
        self.fc1_dims = fc1_dims
        self.fc2_dims = fc2_dims
        self.n_actions = n_actions
        
        self.fc1 = nn.Linear(*self.input_dims, self.fc1_dims)
        self.fc2 = nn.Linear(self.fc1_dims, self.fc2_dims)
        self.fc3 = nn.Linear(self.fc2_dims, self.n_actions)
        
        self.optimizer = optim.Adam(self.parameters(), lr=lr)
        self.loss = nn.MSELoss()
        self.device = T.device('cuda:0' if T.cuda.is_available() else 'cpu')

        self.to(self.device)

    def forward(self, state):
        x = F.relu(self.fc1(state))
        x = F.relu(self.fc2(x))
        actions = self.fc3(x)
        return actions
    
class Agent():
    def __init__(self, gamma, epsilon, lr, input_dims, batch_size, n_actions, 
                 max_mem_size=1000000, eps_end=0.05, eps_decay=5e-8):
        self.gamma = gamma
        self.epsilon = epsilon
        self.lr = lr
        self.input_dims = input_dims
        self.batch_size = batch_size
        self.mem_size = max_mem_size
        self.eps_end = eps_end
        self.eps_decay = eps_decay
        
        self.mem_counter = 0
        self.action_space = [i for i in range(n_actions)]

        self.Q_eval = DeepQNetwork(lr, input_dims=input_dims, fc1_dims=256, 
                                   fc2_dims=256, n_actions=n_actions)

        self.state_memory = np.zeros((self.mem_size, *input_dims), dtype=np.float32)
        self.next_state_memory = np.zeros((self.mem_size, *input_dims), dtype=np.float32)
        
        self.action_memory = np.zeros(self.mem_size, dtype=np.int32)
        self.reward_memory = np.zeros(self.mem_size, dtype=np.float32)
        self.terminal_memory = np.zeros(self.mem_size, dtype=np.bool)

    def store_transition(self, state, action, reward, next_state, done):
        index = self.mem_counter % self.mem_size
        
        
        self.state_memory[index] = state 
        self.next_state_memory[index] = next_state
        self.action_memory[index] = action
        self.reward_memory[index] = reward
        self.terminal_memory[index] = done
        
        self.mem_counter += 1
    
    def choose_action(self, state):
        if np.random.random() > self.epsilon:
            state = T.tensor([np.array(state)]).to(self.Q_eval.device)
            actions = self.Q_eval.forward(state)
            action = T.argmax(actions).item()
        else:
            action = np.random.choice(self.action_space)
        
        return action
    
    def learn(self):
        if self.mem_counter < self.batch_size:
            return
        
        self.Q_eval.optimizer.zero_grad()
        
        max_mem_size = min(self.mem_counter, self.mem_size)
        batch = np.random.choice(max_mem_size, self.batch_size, replace=False)
        batch_index = np.arange(self.batch_size, dtype=np.int32)
        
        state_batch = T.tensor(self.state_memory[batch]).to(self.Q_eval.device)   # ? [batch_index]
        reward_batch = T.tensor(self.reward_memory[batch]).to(self.Q_eval.device)
        next_state_batch = T.tensor(self.next_state_memory[batch]).to(self.Q_eval.device)
        terminal_batch = T.tensor(self.terminal_memory[batch]).to(self.Q_eval.device)
        
        action_batch = self.action_memory[batch]
        
        Q_eval = self.Q_eval.forward(state_batch)[batch_index, action_batch]
        Q_next = self.Q_eval.forward(next_state_batch)
        Q_next[terminal_batch] = 0.0
        
        Q_target = reward_batch + self.gamma * T.max(Q_next, dim=1)[0]
        
        # backprogagation
        loss = self.Q_eval.loss(Q_target, Q_eval).to(self.Q_eval.device)
        loss.backward()
        self.Q_eval.optimizer.step()
        
        self.epsilon = self.epsilon - self.eps_decay if self.epsilon > self.eps_end else self.eps_end