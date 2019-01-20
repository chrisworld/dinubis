
//	DINUBIS


(
~soundSourcesGroup = Group.new;



//	Buffer-Dictionary
~path = PathName.new(thisProcess.nowExecutingPath).parentPath ++ "buffers/";
~makeBuffers = {
	b = Dictionary.new;
	PathName.new(~path).entries.do{
		arg subfolder;
		b.add(
			subfolder.folderName.asSymbol ->
			Array.fill(
				subfolder.entries.size, {
					arg i;
					Buffer.read(s, subfolder.entries[i].fullPath);
				}
			)
		)
	}
};
~makeBuffers.();



// OSC-Messages (Triggering)
~osc = { arg msg, time, replyAddr, recvPort;

	// Number of Resi
	if(msg[0] == '/num_resi') {
		"num_resi".postln;
		~numOfResi = msg[1];
		~resiSynth = Array.fill(~numOfResi, {arg i; i});
		~ampScale = 1 / ~numOfResi;
	};

	// Spawning Resi
	if(msg[0] == '/spawn_resi') {
		"spawn_resi".postln;
		~resiSynth[msg[1]] = Synth.new(\sineOfResi, [\freq, msg[2], \amp, msg[3] * ~ampScale]);
	};

	// Distanz-Update-Resi
	if(msg[0] == '/update_resi') {
		"update_resi".postln;
		~resiSynth[msg[1]].set(\freq, msg[2], \amp, msg[3] * ~ampScale);
	};

	// Collide-Resi
	if(msg[0] == '/collide_resi') {
		"collide_resi".postln;
		~resiSynth[msg[1]].set(\freq, msg[2]);
	};

	// Contact - Hey
	if(msg[0] == '/hey') {
		"contact_hey".postln;
		Synth.new(\hey, [\buffer, b[\hey][0]]);
	};
};



//	SynthDefs
SynthDef.new(\sineOfResi, {
	arg freq, amp, bus;
	var sig;

	sig = SinOsc.ar(freq, 0, amp);

	Out.ar(bus, sig);
}).add;



SynthDef.new(\hey, {
	arg buffer = ~buffer;
	var in, sig, pos = 0;
	var freq, hasFreq;
	var rate = \rate.kr(1);
	var lag;

	in = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 0);
	# freq, hasFreq = Pitch.kr(in);

	lag = LFDNoise3.kr(100).exprange(1, 300);
	freq = freq.lag(lag) * 0.0003;

	sig = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * freq, loop: 0, doneAction: 2);

	Out.ar(0, Pan2.ar(sig, pos));
}).add;



/////////////////////////////////////////////
//	OSC Start
if(~oscReceiverActive.isNil == true){
	thisProcess.addOSCRecvFunc(~osc);
	~oscReceiverActive = 1;
	"".postln;
	"OSC started".postln;
};


//	OSC Stop
CmdPeriod.doOnce{
	if(~oscReceiverActive == 1){
		thisProcess.removeOSCRecvFunc(~osc);
		~oscReceiverActive = nil;
		"".postln;
		"OSC stopped".postln;
	};
};


)







