using CliWrap.EventStream;

namespace SoundSense.Console;

public abstract class AudioChannel {
	public SoundList SoundList { get; set; }
	public bool Loop { get; set; } = false;
	
	public abstract void Start();
	public abstract void Stop();
	//public abstract void Mute(bool mute);
}