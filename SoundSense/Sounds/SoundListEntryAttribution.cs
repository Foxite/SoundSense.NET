using System.Xml.Serialization;

namespace SoundSense;

public record SoundListEntryAttribution(
	[XmlAttribute] string Url,
	[XmlAttribute] string License,
	[XmlAttribute] string Author
);
