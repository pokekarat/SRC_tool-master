SRC_tool for Nexus S (S-LCD model) (In progress of clean them all)
========

Duty: Training and Monitoring subsystems.

Requirement sw and hw installation:

1) VC# Ultimate 2012

2) Monsoon power meter (option)

3) Nexus S (S-LCD version)

===
Sample and parse subsystem workload statistics
===

1. Sample process,

1.1 Install sample.o to "/data/local/tmp/" and then create a folder "stat" within "/data/local/tmp/" in Nexus S.

1.2 Run sample.o, e.g., ./data/local/tmp/sample x y.

x = index of save file, e.g., if x=1, then the save file, i.e., sample1.txt, will be saved in /stat/ directory.

y = Number of sample, e.g, 100.

2. Parsing process,

After finishing sample subsystem workload statistics, the parsing step (i.e., to parse samplex.txt to raw_data_x.txt) is processed as follows:

2.1 Start parseApp (C#)

2.2 Fill in ADB path (where is your adb program), Root path (where you want to save), and Sample file index which is match to the sampleX.txt.

2.3 Press Parse button

2.4 finish
