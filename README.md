# BerlinVRMilgram

Unity (C#) scripts for a VR experiment (the "Emiel experiment"). This repository contains the scripts only, not the Unity project, scenes or assets.

## What is in `Scripts/`

| File | Purpose |
|---|---|
| `HMDRecord.cs` | Records the head-mounted display pose every frame (time, position x/y/z, rotation x/y/z) as semicolon-separated rows. |
| `LogManager.cs` | Singleton that writes timestamped log entries to a text file in `Application.persistentDataPath`. The file name contains the date, time and a participant number (currently a placeholder). |
| `charControl.cs` | Controls a virtual character's gaze with the Animator IK system: smoothly looks at a target position and smoothly turns back. |
| `PlayAudioCue.cs` | Plays one of three audio cues. |
| `PlayAudioFeedback.cs` | Plays a "correct" or "wrong" feedback sound. |
| `UIStartDialog.cs` | Start/cancel dialog. Shows the instructions text loaded from `Resources/Instructions` and loads the experiment scene. |
| `Core/` | `ExperimentManager.cs` and `MainController.cs`. |
| `VR/` | Gaze raycasting and interactive items (`VREyeRaycaster.cs`, `VRInteractiveItem*.cs`). |

## Using the scripts

Copy the `Scripts` folder into the `Assets` folder of a Unity project and attach the components to your scene objects. Scene setup, audio clips and the `Resources/Instructions` text file are not part of this repository.

## Author

Asieh Daneshi, PhD - [LinkedIn](https://www.linkedin.com/in/asieh-daneshi)
