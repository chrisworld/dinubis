
(
~soundSourcesGroup = Group.new;
~resiSynth = Array.fill(10, {arg i; i});



~osc = {|msg, time, replyAddr, recvPort|
	// Spawning Resi
	if(msg[0] == '/spawn_resi'){
		~resiSynth[msg[1]] = Synth.new(\sineOfResi, [\freq, msg[2], \amp, msg[3]]);
	};

	// Distanz-Update-Resi
	if((msg[0] == '/update_resi') {
		~resiSynth[msg[1]].set(\freq, msg[2], \amp, msg[3]);
	};

	// Collide-Resi
	if((msg[0] == '/collide_resi') {
		~resiSynth[msg[1]].set(\freq, msg[2]);
	};
};





SynthDef.new(\sineOfResi, {
	arg freq, amp, bus;
	var sig;

	sig = SinOsc.ar(freq, 0, amp);

	Out.ar(bus, sig);
}).add;




//////////////////////////

if(~oscReceiverActive.isNil == true){
	thisProcess.addOSCRecvFunc(~osc);
	~oscReceiverActive = 1;
	"".postln;
	"Osc started".postln;
};




CmdPeriod.doOnce{
	if(~oscReceiverActive == 1){
		thisProcess.removeOSCRecvFunc(~osc);
		~oscReceiverActive = nil;
		"".postln;
		"Osc stopped".postln;
	};
};


)

