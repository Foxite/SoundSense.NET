using System.Collections.Generic;
using System.Xml.Serialization;

namespace SoundSense;

public record SoundsFile(
	[XmlElement] IReadOnlyList<SoundList> SoundLists,
	[XmlAttribute] string DefaultAnsiPattern = ""
);