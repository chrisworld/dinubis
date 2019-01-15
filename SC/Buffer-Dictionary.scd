

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




// Test

k = b[\dinubis][3].play;
k = b[\hey][0].play;
k = b[\lachen][1].play;
k = b[\uuu][2].play;

k.stop;


