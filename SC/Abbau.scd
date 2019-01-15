
(
SynthDef.new(\abbau, {
	arg freq = 220;
	var numharm, sig;
	var update;

	update = SinOsc.kr(1).range(0.5, 10);

	numharm = LFDNoise3.kr(update).exprange(1, 30);

	sig = Blip.ar(freq, numharm);
	Out.ar(0, Pan2.ar(sig));
}).add;
)

x = Synth.new(\abbau);