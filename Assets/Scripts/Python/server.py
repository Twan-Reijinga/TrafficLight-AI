import http.server
import socketserver
import json

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
