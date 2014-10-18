# SRC_tool for Nexus S (S-LCD model)

Requirement sw and hw installation:
```bash
1) VC# Ultimate 2012
2) Power meter (option: we use monsoon)
3) Nexus S (S-LCD version)
4) R software
```
## Sample and parse subsystem workload statistics

### 1. Sample process

```bash
1.1 Install sample.o to "/data/local/tmp/" and then create a folder "stat" within "/data/local/tmp/" in Nexus S.
1.2 Run sample.o, e.g., ./data/local/tmp/sample x y. 
    x = index of save file, e.g., if x=1, then the save file, i.e., sample1.txt, will be saved in /stat/ directory. 
    y = Number of sample, e.g, 100.
```

### 2. Parsing process

After finishing sample subsystem workload statistics, the parsing step (i.e., to parse samplex.txt to raw_data_x.txt) is processed as follows:

```bash
2.1 Start parseApp (C#)
2.2 Fill in ADB path (where is your adb program), Root path (where you want to save), and Sample file index which is match to the sampleX.txt.
2.3 Press Parse button
2.4 A file "raw_data_x.txt" is created at the Root path. Later, you can integrate this data to your power trace data.
```

### 3. For analyze asynchronous power consumption behavior

```bash
3.1 Download async_analysis.r
3.2 Specify 3 paths to (1) raw_data_x.txt (line 15) (2) trainModify.txt (line 177) (3) asyncTable.txt (line 182)
3.3 Run async_analysis.r to analyze file (1) and then generate file (2) and (3).
```
