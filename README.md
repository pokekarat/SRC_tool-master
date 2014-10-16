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

Sample,

1) Install sample.o to "/data/local/tmp/" and then create a folder "stat" within "/data/local/tmp/" in Nexus S.

2) Run sample.o, e.g., ./data/local/tmp/sample x y.

x = index of save file, e.g., if x=1, then the save file, i.e., sample1.txt, will be saved in /stat/ directory.

y = Number of sample, e.g, 100.

Parse,

