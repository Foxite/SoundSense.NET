using System.Collections.Generic;
using System.Xml.Serialization;

namespace SoundSense;

public record SoundListEntry(
	[XmlAttribute] string FileName,
	[XmlElement] IReadOnlyList<SoundListEntryAttribution> Attribution,
	[XmlAttribute] bool RandomBalance = false, // TODO implement
	[XmlAttribute] int Weight = 25 // TODO what should the default value be?
                                   // TODO implement
);
