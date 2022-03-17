using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SoundSense;

public class Soundpack {
	private readonly Dictionary<string, SoundsFile> m_SoundsFiles = new();

	public static Soundpack CreateFromFolder(string path) {
		var ret = new Soundpack();
		foreach (string directory in Directory.GetDirectories(path)) {
			foreach (string xmlFile in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories)) {
				// System.Xml doesn't support 1.1, and for some reason, *some* files in the soundpack are v1.1.
				string text = File.ReadAllText(xmlFile).Replace("<?xml version=\"1.1\"", "<?xml version=\"1.0\"");
				var xml = new XmlDocument();
				xml.LoadXml(text);
				var soundsFile = XmlUtil.Deserialize<SoundsFile>(xml.DocumentElement);

				foreach (SoundList soundList in soundsFile.SoundLists) {
					soundList.BasePath = Path.GetDirectoryName(xmlFile)!;
				}
				
				ret.m_SoundsFiles[Path.GetRelativePath(path, xmlFile)] = soundsFile;
			}
		}

		return ret;
	}

	public IReadOnlyDictionary<string, SoundsFile> GetFiles() => m_SoundsFiles;

	public SoundList? MatchLine(string line) {
		foreach ((string path, SoundsFile soundsFile) in m_SoundsFiles) {
			foreach (SoundList soundList in soundsFile.SoundLists) {
				if (soundList.Entries.Count > 0 && soundList.LogPattern.IsMatch(line)) {
					return soundList;
				}
			}
		}

		return null;
	}
}
