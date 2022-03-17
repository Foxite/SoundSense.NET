using System.Diagnostics;
using CliWrap;

namespace SoundSense.Console;

public class FfplayAudioChannel : AudioChannel {
	private readonly Random m_Random = new();
	private CancellationTokenSource m_Cts = new();
	
	public override void Start() {
		Stop();

		m_Cts = new CancellationTokenSource();
		CancellationToken token = m_Cts.Token;
		
		Task.Run(async () => {
			do {
				SoundListEntry entry = SoundList.Entries[m_Random.Next(0, SoundList.Entries.Count)];

				Command command = Cli.Wrap("ffplay")
					.WithArguments($"-i \"{Path.Combine(SoundList.BasePath, entry.FileName)}\" -nodisp -autoexit");

				CommandTask<CommandResult> task = command.ExecuteAsync(token);
				try {
					await task;
				} catch (TaskCanceledException) {
					Process.GetProcessById(task.ProcessId).Kill();
				}
			} while (Loop && !token.IsCancellationRequested);
		}, token);
	}

	public override void Stop() {
		m_Cts.Cancel();
	}
}
