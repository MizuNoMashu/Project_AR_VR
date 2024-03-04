from flask import Flask, jsonify, request, current_app
from flask_restful import Resource, Api
from flask_socketio import SocketIO
import json


'''
For everything to work, these versions are needed:

Flask-SocketIO==4.3.1
python-engineio==3.13.2
python-socketio==4.6.0
Flask==2.0.3
Werkzeug==2.0.3


To run the server from a terminal use:
FLASK_APP=server.py flask run --host=0.0.0.0
'''


app = Flask(__name__)
app.lastValue = "prova"

api = Api(app)
app.config['SECRET_KEY'] = 'secret!'
socketio = SocketIO(app)


if __name__ == '__main__':
    socketio.run(app)


class myHUB(Resource):

    def get(self):
        return ""

    def post(self):
        r = request.args['string']
        return str(r)

    @socketio.on('value')
    def change_value(json):
        current_app.lastValue = json.get("value")

    @app.route("/data", methods=['GET'])
    def get_data():
        return current_app.lastValue


api.add_resource(myHUB, '/')



