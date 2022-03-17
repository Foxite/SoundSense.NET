using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SoundSense;

public record SoundList(
	[XmlAttribute] Regex LogPattern,
	[XmlAttribute] string? Channel,
	[XmlAttribute("concurency")] int? Concurrency, // TODO implement
	[XmlElement] IReadOnlyList<SoundListEntry> Entries,
	[XmlAttribute] Loop Loop = Loop.Stop,
	[XmlAttribute] int Timeout = 0, // TODO implement
	[XmlAttribute] string AnsiFormat = "", // TODO implement
	[XmlAttribute] bool HaltOnMatch = true, // TODO what does this do
	[XmlAttribute("propability")] int Probability = 100, // TODO implement
	[XmlAttribute("playbackThreshhold")] PlaybackThreshold PlaybackThreshold = PlaybackThreshold.Everything
) {
	public string BasePath { get; set; }
}
