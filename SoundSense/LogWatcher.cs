using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SoundSense; 

public class LogWatcher : IDisposable {
	private readonly CancellationTokenSource m_Cts;
	private readonly AutoResetEvent m_WaitHandle;
	private readonly FileSystemWatcher m_FilesystemWatcher;

	public event Action<string>? Line;

	public LogWatcher(string path) {
		m_Cts = new CancellationTokenSource();
		m_WaitHandle = new AutoResetEvent(false);
		m_FilesystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(path));
		m_FilesystemWatcher.Filter = Path.GetFileName(path);
		m_FilesystemWatcher.Changed += (s,e) => m_WaitHandle.Set();

		Task.Run(() => {
			var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			fs.Seek(0, SeekOrigin.End);
			using (var sr = new StreamReader(fs)) {
				while (true) {
					string? line = sr.ReadLine();
					if (line != null) {
						Line?.Invoke(line);
					} else {
						m_WaitHandle.WaitOne(TimeSpan.FromMilliseconds(100));
					}

					if (m_Cts.IsCancellationRequested) {
						break;
					}
				}
			}

			m_WaitHandle.Dispose();
		});
		
		m_FilesystemWatcher.EnableRaisingEvents = true;
	}

	public void Dispose() {
		m_Cts.Cancel();
		m_Cts.Dispose();
		m_WaitHandle.Dispose();
		m_FilesystemWatcher.Dispose();
	}
}
