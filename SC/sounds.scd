
(	//	Buffer-Dictionary
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
)




~file = "/Users/Juergen/Documents/KUG-Computermusik/Master/Computermusik und Multimedia/Projekt/Soundfiles/Bounces/Sackerl.wav"
~file = Platform.resourceDir +/+ "sounds/a11wlk01.wav";

~buffer = Buffer.read(s, ~file);



(
SynthDef.new(\sound1a, {
	arg buffer = ~buffer;
	var in, sig, pos = 0;
	var freq, hasFreq;
	var rate = \rate.kr(1);
	var lag;

	in = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 1);
	# freq, hasFreq = Pitch.kr(in);

	lag = LFDNoise3.kr(100).exprange(1, 300);
	lag.poll;
	freq = freq.lag(lag) * 0.0003;
	// freq.poll;

	sig = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * freq, loop: 1);

	Out.ar(0, Pan2.ar(sig, pos));
}).add;
)


~synth1a = Synth.new(\sound1a, [\buffer, b[\dinubis][0]]);
~synth1a.set(\rate, 0.5);
~synth1a.set(\buffer, b[\lachen][0]);
~synth1a.set(\buffer, ~buffer);
~synth1a.free;



(
SynthDef.new(\sound1b, {
	arg buffer = ~buffer;
	var in, sig, pos = 0;
	var amp;
	var rate = \rate.kr(1);
	var lag;

	in = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 1);
	amp = Amplitude.ar(in);

	lag = LFDNoise3.kr(100).exprange(1, 300);
	amp = amp.lag(lag) * 2;
	amp.poll;

	sig = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * amp, loop: 1);

	Out.ar(0, Pan2.ar(sig, pos));
}).add;
)


~synth1b = Synth.new(\sound1b, [\buffer, b[\dinubis][0]]);
~synth1b.set(\rate, 0.5);
~synth1b.set(\buffer, b[\lachen][0]);
~synth1b.set(\buffer, ~buffer);
~synth1b.free;





(
SynthDef.new(\sound2, {
	arg buffer = ~buffer, lag = 10;
	var in, sig, pos = 0;
	var freq, hasFreq, numharm;
	var rate = \rate.kr(1);

	in = PlayBuf.ar(1, buffer, BufRateScale.kr(buffer) * rate, loop: 1);
	# freq, hasFreq = Pitch.kr(in);
	numharm = Amplitude.ar(in);
	numharm = (numharm * 100).lag(lag).round;

	freq = freq.lag(lag);

	numharm.poll;

	sig = Blip.ar(freq, numharm);

	Out.ar(0, Pan2.ar(sig, pos));
}).add;
)

~synth2 = Synth.new(\sound2, [\buffer, b[\dinubis][0]]);
~synth2.set(\rate, 0.5);
~synth2.set(\buffer, b[\lachen][0]);
~synth2.set(\buffer, b[\uuu][1]);
~synth2.set(\buffer, ~buffer);
~synth2.free;







