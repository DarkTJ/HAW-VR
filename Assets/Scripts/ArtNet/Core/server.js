var net = require('net');

// Configuration parameters
var HOST = '173.212.192.241';
var PORT = 8886;

//Set of all connected sockets
const connectedSockets = new Set();

connectedSockets.broadcast = function(data, except) {
    for (let sock of this) {
        if (sock !== except) {
            sock.write(data);
        }
    }
}

var message = "";


// Create Server instance 
var server = net.createServer(onClientConnected);

server.listen(PORT, HOST, function() {
    console.log('server listening on %j', server.address());
});

function onClientConnected(sock) {
    var remoteAddress = sock.remoteAddress + ':' + sock.remotePort;
    console.log('new client connected: %s', remoteAddress);
    connectedSockets.add(sock);

    sock.on('data', function(data) {
        console.log('%s Says: %s', remoteAddress, data);

        connectedSockets.broadcast(data, sock);

    });

    sock.on('close', function() {
        console.log('connection from %s closed', remoteAddress);
        connectedSockets.delete(sock);
    });

    sock.on('error', function(err) {
        console.log('Connection %s error: %s', remoteAddress, err.message);
    });

    sock.on('end', function() {
        connectedSockets.delete(sock);
    });

    sock.on('destroy', function() {
        connectedSockets.delete(sock);
    });
};