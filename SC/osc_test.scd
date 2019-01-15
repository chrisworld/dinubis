
(
~soundSourcesGroup = Group.new;
~mobileSoundSourcesArray = [0, 0, 0, 0, 0, 0, 0];
~allowPlayerCollision = true;
~playerCoords = [0, 0, 0, 0];
)


(
~osc = {|msg, time, replyAddr, recvPort|
	if(msg[0] == '/location'){
		~playerCoords = [msg[1], msg[2], msg[3], msg[4]];
		~soundSourcesGroup.set(\playerX, msg[1], \playerY, msg[2], \playerZ, msg[3], \orientation, msg[4]);
	};




	// Bewegliche Objekte
	if(msg[0] == '/soundObjectSustained'){
		var index = msg[4];
		"sustained object created".postln;
		~mobileSoundSourcesArray[index] =
		Synth.new(\sound1a, [\buffer, b[\dinubis][0]], ~soundSourcesGroup);
	};


};
)









//////////////////////////////////////////

(
if(~oscReceiverActive.isNil == true){
	thisProcess.addOSCRecvFunc(~osc);
	~oscReceiverActive = 1;
	"".postln;
	"Osc started".postln;
};
)





