using SoundSense;
using SoundSense.Console;

var channels = new Dictionary<string, AudioChannel>();

string soundpackPath = "/opt/soundsense/packs";
var soundpack = Soundpack.CreateFromFolder(soundpackPath);
using var watcher = new LogWatcher("/home/foxite/.dwarffortress/gamelog.txt");
var random = new Random();
watcher.Line += (line) => {
	SoundList? result = soundpack.MatchLine(line);
	if (result != null) {
		AudioChannel channel;
		if (result.Channel == null) {
			channel = new FfplayAudioChannel();
		} else {
			channel = channels.GetOrAdd(result.Channel, _ => new FfplayAudioChannel());
		}

		channel.Loop = result.Loop == Loop.Start;
		channel.SoundList = result;
		channel.Start();
	}
};

await Task.Delay(-1);
