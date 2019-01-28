
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

	// Player_move
	if(msg[0] == '/player_move') {
		"player_move".postln;
		//Synth.new(\hey, [\buffer, b[\movement][0]]);
		Pbind(
	\instrument, \player_move,
	\fc, rrand(4000, 12000),
	\fm, rrand(100, 200),
	\idx, 10,
	\amp, 0.2,
	\atk, 0.01,
	\rls, 0.001,
	\dur, Pwhite(0.1, 0.05, 10)
).play;
	};


	// Player_run
	if(msg[0] == '/player_run') {
		"player_run".postln;
		Synth.new(\run, [\freq, ((rrand(10, 40))-12).midicps, \mod, rrand(2, 6), \time, rrand(1.5, 2)]);
	};

	// Resource_dig
	if(msg[0] == '/resource_dig') {
		"resource_dig".postln;
		Synth.new(\dig, [\buffer, b[\hihat][rrand(0,21)], \rate, rrand(0.1, 0.4)]);
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



SynthDef.new(\dig, {| out = 0, buffer = 0, rate |

    Out.ar(out,
        PlayBuf.ar(2, buffer, rate,1, 0, 0, doneAction: 2) * 0.5
    )
}).add;



SynthDef(\player_move, {
	|
	amp=0.2,
	fc=1000,
	fm = 100,
	idx = 100,
	atk=0.01,
	rls=0.1
	|
	var env = EnvGen.ar(Env.perc(atk, rls), doneAction:2);
	var modulator = SinOsc.ar(fm);
	var carrier = SinOsc.ar(fc + (idx*modulator) );
	var sig = carrier *  env * amp;
	sig = HPF.ar(sig, 1000);
	Out.ar(0, sig!2);
}).add;


SynthDef(\run, {
	|
	time=2,
	freq=100,
	mod =2
	|
var saw, tone;
	saw = VarSaw.ar(freq/2 * mod, width: XLine.ar(0.5,2,time, doneAction: 2)).range(0,XLine.ar(1,1/1000,time, doneAction: 2));
	tone =  SinOsc.ar(freq).fold(-1*saw,saw);
	Out.ar(0, tone.dup);
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








