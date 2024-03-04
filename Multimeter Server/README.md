# Multimeter Server

To show the multimeter data inside the HoloLens application, we send these data to a server, and then in the application we request the data from this server.

To do this, we used two python files:
- server.py			("server")
- bluetooth-connection.py	("client")

The communication between the server and the client is managed using SocketIO, a library that offers a low-latency bidirectional communication.


## server.py

This file uses Flask to create a (local) web server. The server keeps track of the last read value from the multimeter, and it provides it at the route "/data".<br/>
The server allows clients to modify this value through the library Flask-SocketIO: if it receives a socketio event of type "value", it updates the saved value with the new one that is being passed.

The server must be run from terminal using the command:<br/>
<i>FLASK_APP=server.py flask run --host=0.0.0.0</i>


## bluetooth-connection.py

This file manages two operations:
- it connects via Bluetooth to the multimeter using the bluepy library, it reads the multimeter's packets, and parses them in order to obtain the measured values,
- through SocketIO, it connects to the Flask server, and sends every value it reads to the server, by creating an event of type "value".

The Bluetooth connection and the parsing of the data have been done using the code provided <a href="https://github.com/mweimerskirch/ble-multimeter-reader?tab=readme-ov-file">here</a>. 

The client must be run from terminal using the command:<br/>
<i>python3 bluetooth_connection.py \<MAC address of the multimeter\></i>