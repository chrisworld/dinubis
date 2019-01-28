(
//	DINUBIS



~transpose = 1;
~ampScale = 1;


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
	var harmFactor = 10;

	// Number of Resi
	if(msg[0] == '/num_resi') {
		~numOfResi = msg[1];
		~resiSynth = Array.fill(~numOfResi, {arg i; i});
	};

	// Spawning Resi
	if(msg[0] == '/spawn_resi') {
		~resiSynth[msg[1]] = Synth.new(\sineOfResi, [\freq, msg[2], \amp, msg[3] * ~ampScale, \numHarm, msg[3] * harmFactor, \height, 0.0]);
	};

	// Number of active Resis
	if(msg[0] == '/num_active') {
		if(msg[1] != 0) {
			~ampScale = 1 / msg[1] } {
			~ampScale = 1
		}
	};

	// Distanz-Update-Resi
	if(msg[0] == '/update_resi') {
		~resiSynth[msg[1]].set(\freq, msg[2] * ~transpose, \amp, msg[3] * ~ampScale, \numHarm, msg[3] * harmFactor, \height, msg[4]);
	};

	// Collide-Resi
	if(msg[0] == '/collide_resi') {
		~resiSynth[msg[1]].set(\freq, msg[2]);
	};

	// Contact - Hey
	if(msg[0] == '/hey') {
		Synth.new(\hey, [\buffer, b[\hey][0]]);
	};

	// Abbau fertig - Transpose
	if(msg[0] == '/abbau_end') {
		~transpose = ~transpose * msg[1];
	};

	// Player_walk
	if(msg[0] == '/player_walk') {
		"walk ".post; msg[1].postln;
		if (msg[1] == 1) { ~walk = Synth.new(\move, [\buffer, b[\uuu][0], \rate, 2]) }
		{ ~walk.set(\gate, 0) }
	};

	// Player_run
	if(msg[0] == '/player_run') {
		"run ".post; msg[1].postln;

		if (msg[1] == 1) { ~run = Synth.new(\move, [\buffer, b[\uuu][0], \rate, 3]) }
		{ ~run.set(\gate, 0) }
	};

	// Resource_dig
	if(msg[0] == '/resource_dig') {
		Synth.new(\dig, [\buffer, b[\hihat][rrand(0,21)], \rate, rrand(0.1, 0.4)]);
	};

};

/////////////////////////////////////////////////////////////////////////////////////
//	SynthDefs
SynthDef.new(\sineOfResi, {
	arg freq, amp, numHarm, height, bus = 0, pos = 0;
	var sig;

	height = (height * (12000)) + 25;

	sig = Blip.ar(freq, numHarm, amp);
	sig = HPF.ar(sig, height);

	Out.ar(bus, Limiter.ar(Pan2.ar(sig, pos)));
}).add;



SynthDef.new(\hey, {
	arg buffer = ~buffer, bus = 0, pos = 0, rate = 0.5, freqFac = 0.0045;
	var in, sig;
	var freq, hasFreq;
	var lag;

	in = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 0);
	# freq, hasFreq = Pitch.kr(in);

	// lag = LFDNoise3.kr(100).exprange(1, 10);
	freq = freq * freqFac;

	sig = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * freq, loop: 0, doneAction: 2);

	Out.ar(bus, Limiter.ar(Pan2.ar(sig, pos)));
}).add;



SynthDef.new(\move, {
	arg buffer = ~buffer, bus = 0, pos = 0, rate = 0.5, gate = 1;
	var sig, env;

	env = EnvGen.kr(Env.asr(releaseTime: 0.01), gate, doneAction: 2);

	sig = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 1);
	sig = sig * env * 0.1;

	Out.ar(bus, Limiter.ar(Pan2.ar(sig, pos)));
}).add;



SynthDef.new(\dig, {| out = 0, buffer = 0, rate |

	Out.ar(out,
		PlayBuf.ar(2, buffer, rate,1, 0, 0, doneAction: 2) * 0.5
	)
}).add;



/////////////////////////////////////////////////////////////////////////////////////
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






