using System.Linq;
using NUnit.Framework;

namespace SoundSense.Tests;

public class Tests {
	private Soundpack m_Soundpack;
	
	[SetUp]
	public void Setup() {
		// TODO download soundpack from https://github.com/jecowa/soundsensepack
		// For now, assume it's in here:
		string path = "/opt/soundsense/packs";
		m_Soundpack = Soundpack.CreateFromFolder(path);
	}

	[Test]
	public void LoadingTest() {
		Assert.AreEqual(68, m_Soundpack.GetFiles().Count);
		Assert.AreEqual(7, m_Soundpack.GetFiles()["mandates/mandates.xml"].SoundLists.Count);
		Assert.AreEqual(2, m_Soundpack.GetFiles()["mandates/mandates.xml"].SoundLists[1].Entries.Count);
		Assert.AreEqual("Clearing Throat Male-SoundBible.com-37691700.mp3", m_Soundpack.GetFiles()["mandates/mandates.xml"].SoundLists[1].Entries[0].FileName);
		Assert.AreEqual("music", m_Soundpack.GetFiles()["system/system.xml"].SoundLists[0].Channel);
		Assert.AreEqual(Loop.Stop, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[0].Loop);
		Assert.AreEqual(Loop.Start, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[3].Loop);
		Assert.AreEqual(false, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[2].HaltOnMatch);
		Assert.AreEqual(true, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[3].HaltOnMatch);
		Assert.AreEqual(PlaybackThreshold.Environmental, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[2].PlaybackThreshold);
		Assert.AreEqual(false, m_Soundpack.GetFiles()["system/system.xml"].SoundLists[0].Entries[0].RandomBalance);
		Assert.AreEqual(true, m_Soundpack.GetFiles()["social/visitors.xml"].SoundLists[0].Entries[0].RandomBalance);
		Assert.AreEqual("CC BY 3.0", m_Soundpack.GetFiles()["social/visitors.xml"].SoundLists[0].Entries[0].Attribution[0].License);
		Assert.AreEqual(10, m_Soundpack.GetFiles()["production/production.xml"].SoundLists[0].Concurrency);
		Assert.AreEqual(500, m_Soundpack.GetFiles()["production/production.xml"].SoundLists[0].Timeout);
		Assert.AreEqual(100, m_Soundpack.GetFiles()["production/production.xml"].SoundLists[0].Probability);
		Assert.AreEqual(25, m_Soundpack.GetFiles()["production/production.xml"].SoundLists[1].Probability);
		Assert.AreEqual("[34m", m_Soundpack.GetFiles()["production/production.xml"].SoundLists[1].AnsiFormat);
	}

	[Test]
	public void MatchTest() {
		Assert.AreEqual("NewMandate/NewMandate1.mp3", m_Soundpack.MatchLine("Urist McMayor has mandated the construction of certain goods.")!.Entries[2].FileName);
	}
}
