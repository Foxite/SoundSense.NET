## SoundSense.NET
Because the original soundsense didn't work properly on my linux box, and neither did soundsense-rs.

## Design goals
- Full compatibility with existing SoundSense soundpacks
- Full cross-platform support, targeting at least Windows, Mac, and Linux, with minimal effort needed to port to future platforms

## Structure
This currently consists of a library project that contains soundpack loading and platform-independent logic, and a console app that ties it all together and uses ffplay to play audio.

In the future, a third project will be added that uses some pure .NET gui toolkit to make a desktop app.

## Acknowledgements
Thanks to zwei for his work on the original soundsense.
