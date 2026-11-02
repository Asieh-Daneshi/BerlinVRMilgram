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

## Log file format

`LogManager` appends every entry to one text file in `Application.persistentDataPath`, named `yyyyMMdd-HHmmss_<participant>_data.txt`. The participant number is currently the fixed placeholder `000001` in `LogManager.cs`, so set it before running real sessions.

- `WriteTimeStampedEntry(msg)` writes `<Time.realtimeSinceStartup>;<msg>`.
- `WriteEntry(msg)` and `WriteEntry(list)` write the text as it is, one line per string.
- `WriteCSV(header, rows)` writes a comma-separated header line followed by one comma-separated line per row.
- `HMDRecord` buffers one row per rendered frame while recording and writes them in `StopAndWriteData()`. A row is `time;HMD;pos_x;pos_y;pos_z;rot_x;rot_y;rot_z` with the pose rounded to four decimals. `WriteHeader()` writes the matching header line (`time;event;pos_x;...;rot_z`).

Numbers are converted with `ToString`, which uses the current system culture. On a machine set to a locale with a decimal comma (for example German), values are written with a comma, so check the locale of the recording computer before parsing the files.

## Using the scripts

Copy the `Scripts` folder into the `Assets` folder of a Unity project and attach the components to your scene objects. Scene setup, audio clips and the `Resources/Instructions` text file are not part of this repository.

## Author

Asieh Daneshi, PhD - [LinkedIn](https://www.linkedin.com/in/asieh-daneshi)
