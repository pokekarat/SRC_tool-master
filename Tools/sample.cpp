/******************************************************************************

 @File         OGLES2PVRScopeExample.cpp

 @Title        Iridescence

 @Version      

 @Copyright    Copyright (c) Imagination Technologies Limited.

 @Platform     Independent

 @Description  Shows how to use our example PVRScope graph code.

******************************************************************************/
#include <GLES2/gl2.h>
#include <pthread.h>
#include "PVRScopeGraph.h"
/*********************************************************************************************
*
* This example outputs the values of the hardware counters found in Group 0
* to Android Logcat once a second for 60 seconds, and consists of five steps:
*
* 1. Define a function to initialise PVRScopeStats
* 4. Read and output the counter information for group 0 to Logcat
* 2. Initialise PVRScopeStats
* 3. Set the active group to 0
* 5. Shutdown PVRScopeStats
*
*********************************************************************************************/

//#define NUMBER_OF_LOOPS 140

#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <jni.h>
//
#include <android/log.h>
#include <android/bitmap.h>
#define LOGI(...) __android_log_print(ANDROID_LOG_INFO , "PVRScope", __VA_ARGS__)
#include "PVRScopeStats.h"

// Step 1. Define a function to initialise PVRScopeStats
bool PSInit(SPVRScopeImplData **ppsPVRScopeData, SPVRScopeCounterDef **ppsCounters, SPVRScopeCounterReading* const psReading, unsigned int* const pnCount)
{
	//Initialise PVRScope
	const EPVRScopeInitCode eInitCode = PVRScopeInitialise(ppsPVRScopeData);
	if(ePVRScopeInitCodeOk == eInitCode)
	{
		printf("Initialised services connection.\n");
	}
	else
	{
		printf("Error: failed to initialise services connection.\n");
		*ppsPVRScopeData = NULL;
		return false;
	}
	
	//Initialse the counter data structures.
	if (PVRScopeGetCounters(*ppsPVRScopeData, pnCount, ppsCounters, psReading))
	{
		printf("Total counters enabled: %d.", *pnCount);
	}
	return true;
}

double prev_total[4] = {0,0,0,0};
double prev_idle[4] = {0,0,0,0};

double csit[4][3] = {{0,0,0},{0,0,0},{0,0,0}, {0,0,0}};
double csiu[4][3] = {{0,0,0},{0,0,0},{0,0,0}, {0,0,0}};

double prev_tx = 0;
double prev_rx = 0;

double parseMem(char totalBuf[], char freeBuf[], char bufferBuf[])
{
	
	double ret = 0;
	double total = 0;
	double free = 0;
	double buffer = 0;
	
	char *tok = NULL;	
	tok = strtok(totalBuf, " ");
	tok = strtok(NULL, " ");
	total = atof(tok);
	//printf("%s\n", tok);
	
	tok = strtok(freeBuf, " ");
	tok = strtok(NULL, " ");
	free = atof(tok);
	//printf("%f\n", free);
	
	tok = strtok(bufferBuf, " ");
	tok = strtok(NULL, " ");
	buffer = atof(tok);
	//printf("%f\n", buffer);
	
	ret = (total - (free+buffer))/total;
	ret = ret * 100;
	
	return ret;

}

double parseCPU(char cpuLine[],int core)
{
	//printf("prev total = %f\n",prev_total);
	
	double total = 0;
	double idle = 0;
	char *tok = NULL;	
	tok = strtok(cpuLine, " ");
	
	int count = 0;

	while(tok)
	{
		//printf("Token: %s\n", tok);
		if(count > 0){
			total += atof(tok);
		}
		
		if(count == 4){
			idle = atof(tok);
		}
		
		tok = strtok(NULL, " ");
		++count;
	}

	double diff_idle = idle - prev_idle[core];
	double diff_total = total - prev_total[core];
	double diff_util = (1000 * (diff_total - diff_idle) / diff_total) / 10;

	prev_total[core] = total;
	prev_idle[core] = idle;
	free(tok);
	//printf("parse CPU util = %f\n",diff_util);
	
	return diff_util;
}

char buffer[4096];
char header[9999];
char saveFile[2048];
char **samples;

FILE *fp_cpu;
FILE *fp_cpu_chk;
FILE *fp;
FILE *fp_save;
FILE *fp_mem;

int numCol = 9999;
int fileIndex = 0; //  = atoi(argv[1]);    
int nRows = 0; //atoi(argv[2]);
int delay = 0;
int start_strc = 0;
int stop_strc = 0;
int start_load = 50000;
int stop_load = 45000;
int cur_cap = 100; // 100%
int high_cap = 100;
int low_cap = 0;
int size = 0;
int mode = 0;
int network = 1;
int freq = 0;
int bright = 0;
int end_time = 0;

int getCap()
{
	int capacity = -1;
	if((fp = fopen("/sys/class/power_supply/battery/capacity","r")) != NULL) 
	{
		fgets(buffer,sizeof buffer, fp);
		capacity = atoi(buffer);			
		memset(buffer, 0, sizeof(buffer));
		fclose(fp);
	}
	return capacity;
}

int numState = 1;
int setState(int status){

	printf("Status = %d\n",status);
	//set to a state status
	char command[1024];
	
	if(status == 0) // kill strc and whitescreen app
	{
		sprintf(command,"su -c /data/local/tmp/busybox killall strc com.example.whitescreen");
	}
	else if(status == 1) // set frequency and brightness
	{
		sprintf(command,"su -c sh /data/local/tmp/set_state.sh %d %d", freq, bright);
		printf("%s\n",command);
	}
	else if(status == 2) //set lowest brightness
	{
		sprintf(command,"su -c \"echo 0 > /sys/class/backlight/panel/brightness &\"");
	}
	else if(status == 3) //call strc
	{
		sprintf(command,"su -c /data/local/tmp/strc %d %d &",start_load, stop_load);
	}
	else if(status == 4)
	{
		sprintf(command,"am start -a android.intent.action.VIEW -c android.intent.category.DEFAULT -e foo bar -n com.example.whitescreen/com.example.whitescreen.MainActivity");
	}
	else if(status == 5) //set max brightness
	{
		sprintf(command,"su -c \"echo 255 > /sys/class/backlight/panel/brightness &\"");
		//sprintf(command,"echo 255 > /sys/class/backlight/panel/brightness");
	}
	
	system(command);
		
	return 1;
}

int forceBreak = 0;
int isRunOnce = 1;
char path_rx[1024];
char path_tx[1024];
char path_oper[1024];

void method()
{		
		//wifi
		if(network==1)
		{
			sprintf(path_tx,"/sys/class/net/wlan0/statistics/tx_packets");
			sprintf(path_rx,"/sys/class/net/wlan0/statistics/rx_packets");
			sprintf(path_oper,"/sys/class/net/wlan0/operstate");
		}
		//3G
		else if(network==0)
		{
			sprintf(path_tx,"/sys/class/net/rmnet0/statistics/tx_packets");
			sprintf(path_rx,"/sys/class/net/rmnet0/statistics/rx_packets");
			sprintf(path_oper,"/sys/class/net/rmnet0/operstate");
		}
		
		isRunOnce = 1;
		
		size = nRows;
		
		samples = (char **)malloc(nRows * sizeof(char *));
		
		int c2=0;
		for(c2=0; c2<nRows; c2++)
			samples[c2] = (char *) malloc(numCol * sizeof(char));
			
		
		printf("test passed2\n");
		
		SPVRScopeImplData *psData;
		
		//Counter information (set at init time)
		SPVRScopeCounterDef    *psCounters;
		unsigned int      uCounterNum;
		
		//Counter reading data
		unsigned int      uActiveGroup;
		unsigned int      uActiveGroupSelect;
		bool        bActiveGroupChanged;
		SPVRScopeCounterReading  sReading;
		
		// Step 2. Initialise PVRScopeStat
		if (PSInit(&psData, &psCounters, &sReading, &uCounterNum)){
			//LOGI("PVRScope up and running.");
			printf("PVRScore up and running...\n");
			
			/*sprintf(aut,"%s\n","su");
			system(aut);*/
			
		}else{
			//LOGE("Error initializing PVRScope.");
			printf("Error initializing PVRScope...\n");
		}
		
		bActiveGroupChanged = true;
		uActiveGroupSelect = 0;
		unsigned int sampleRate = delay;
		unsigned int index = 0;
		unsigned int j = 0;
		unsigned int k = 0;
				
			
		struct timeval tv_start;
		gettimeofday(&tv_start, NULL);
		unsigned long time_in_sec_start = tv_start.tv_sec;
						
		printf("Begin working...\n");
		
		/*
		strcat(header,"Start_time(second)=");
		char startTime[1024];
		snprintf(startTime,1024,"%lu\n",time_in_sec_start);
		strcat(header,startTime);
				
		strcat(header,"delay=");
		char delayData[50];
		snprintf(delayData,50,"%d\n",delay);
		strcat(header,delayData);
		*/
		
		int startIndex = 0;
						
		while (startIndex < nRows)
		{
				// Ask for the active group 0 only on the first run. Then set it to 0xffffffff
				if(bActiveGroupChanged)
				{
						uActiveGroup = uActiveGroupSelect;
				}
				else 
				{
						uActiveGroup = 0xffffffff;
				}
				
				++index;
				
				if (index < 100) 
				{
					// Sample the counters every 10ms. Don't read or output it.
					PVRScopeReadCountersThenSetGroup(psData, NULL, 0, uActiveGroup);
				} 
				else 
				{				
					if(k < delay)
					{
						++k;
						index = 0;
						//continue;
					}
					else
					{
						
						k = 0;
						index = 0;
						printf("sample %d\n", startIndex);

						struct timeval tv;
						gettimeofday(&tv, NULL);
						unsigned long time_in_mill = (tv.tv_sec) * 1000 + (tv.tv_usec) / 1000;
						
						// Step 4. Read and output the counter information for group 0 to Logcat
						if(PVRScopeReadCountersThenSetGroup(psData, &sReading, time_in_mill * 1000, uActiveGroup))
						{
																		
							printf("Start sample\n");		
						
							strcat(header,"\n_LOOP_");
							char loop[50];
							snprintf(loop, 50, "%d\n", startIndex);
							strcat(header,loop);
							
							/*if(startIndex == 10){
								setState(2);
							}
							else if(startIndex == 15){
								setState(5);
							}*/
							
							//Read bigLittle_status
							if((fp_cpu = fopen("su -c /dev/bL_status","r")) != NULL) 
							{	
								printf("bl\n");
								strcat(header,"_BL_\n");
								fgets(buffer,sizeof buffer, fp_cpu);
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,buffer);
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,buffer);
								memset(buffer, 0, sizeof(buffer));
								fclose(fp_cpu);
							}
												
							strcat(header,"_CPU_\n");
							if((fp_cpu = fopen("/proc/stat","r")) != NULL) 
							{		
								
								fgets(buffer,sizeof buffer, fp_cpu);
								
								//CPU0
								fgets(buffer,sizeof buffer, fp_cpu);
								double cpu_util = parseCPU(buffer,0);
								char output[50];
								snprintf(output,50,"util=%.2f",cpu_util);
								strcat(header,output);
								printf("cpu0 util = %s \n",header);
								memset(buffer, 0, sizeof(buffer));
								memset(output, 0, sizeof(output));
								
								if((fp_cpu_chk = fopen("/sys/devices/system/cpu/cpu1/online","r")) != NULL)
								{ 
									char test[50];
									fgets(test, sizeof test, fp_cpu_chk);
									
									if(atoi(test) == 1)
									{
										fgets(buffer,sizeof buffer, fp_cpu);
										cpu_util = parseCPU(buffer,1);
										snprintf(output,50," %.2f",cpu_util);
										strcat(header,output);
										memset(buffer, 0, sizeof(buffer));
										memset(output, 0, sizeof(output));
										//fclose(fp_cpu);
									}
									else
									{							
										strcat(header," x");
									}
									fclose(fp_cpu_chk);
								}
								
								if((fp_cpu_chk = fopen("/sys/devices/system/cpu/cpu2/online","r")) != NULL)
								{ 
									char test[50];
									fgets(test, sizeof test, fp_cpu_chk);
									if(atoi(test) == 1)
									{
										fgets(buffer,sizeof buffer, fp_cpu);
										cpu_util = parseCPU(buffer,2);
										snprintf(output,50," %.2f",cpu_util);
										strcat(header,output);
										memset(buffer, 0, sizeof(buffer));
										memset(output, 0, sizeof(output));
										//fclose(fp_cpu);
									}
									else
									{
										strcat(header," x");
										//printf(" x");
									}
									fclose(fp_cpu_chk);
								}
								
								if((fp_cpu_chk = fopen("/sys/devices/system/cpu/cpu3/online","r")) != NULL)
								{ 
									char test[50];
									fgets(test, sizeof test, fp_cpu_chk);
									if(atoi(test) == 1)
									{
										fgets(buffer,sizeof buffer, fp_cpu);
										cpu_util = parseCPU(buffer,3);
										snprintf(output,50," %.2f",cpu_util);
										strcat(header,output);
										memset(buffer, 0, sizeof(buffer));
										memset(output, 0, sizeof(output));
										//fclose(fp_cpu);
									}
									else
									{
										strcat(header," x");
										//printf("last x\n");
									}
									
									fclose(fp_cpu_chk);
									
								}
								
								fclose(fp_cpu);		
								
							}
							
							strcat(header,"\n_FREQ_");
							
							//Read freq0
							if((fp_cpu = fopen("/sys/devices/system/cpu/cpu0/cpufreq/scaling_cur_freq","r")) != NULL)
							{	
								
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,"\nfreq0=");
								strcat(header,buffer);
								//printf("%s\n",header);
								memset(buffer, 0, sizeof(buffer));
								fclose(fp_cpu);
							}
							
							//Read freq1
							if((fp_cpu = fopen("/sys/devices/system/cpu/cpu1/cpufreq/scaling_cur_freq","r")) != NULL)
							{	
								//printf("freq1\n");
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,"freq1=");
								strcat(header,buffer);
								memset(buffer, 0, sizeof(buffer));
								fclose(fp_cpu);
							}
							else
							{
								strcat(header,"freq1=x");
								//printf("freq1=x");
							}					

							//Read freq2
							if((fp_cpu = fopen("/sys/devices/system/cpu/cpu2/cpufreq/scaling_cur_freq","r")) != NULL) {	
								
								//printf("freq2\n");
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,"freq2=");
								strcat(header,buffer);
								memset(buffer, 0, sizeof(buffer));
								fclose(fp_cpu);	
							}
							else
							{
								strcat(header,"\nfreq2=x");
								//printf("\nfreq2=x");
							}
						
							//Read freq3
							if((fp_cpu = fopen("/sys/devices/system/cpu/cpu3/cpufreq/scaling_cur_freq","r")) != NULL) {	
								//printf("freq3\n");
								fgets(buffer,sizeof buffer, fp_cpu);
								strcat(header,"freq3=");
								strcat(header,buffer);
								memset(buffer, 0, sizeof(buffer));
								fclose(fp_cpu);
															
							}
							else
							{
								strcat(header,"\nfreq3=x");
								//printf("\nfreq3=x");
							}
							
							//printf("%s\n",header);
							
							//Read CPU idle time C0S0IT //////////////////////////////////////////////////////////////////////////
							strcat(header,"\n_IDLE_TIME_\n");
							//printf("idle time\n");
							for(int core=0; core<4; core++)
							{
								char title[100];
								snprintf(title,100,"idle_time_%d=",core);
								strcat(header,title);
							
								for(int state=0; state<3; state++)
								{
								
									char proc[100];
									sprintf(proc,"/sys/devices/system/cpu/cpu%d/cpuidle/state%d/time",core,state);
									
									if((fp_cpu = fopen(proc,"r")) != NULL) 
									{	
									
										fgets(buffer,sizeof buffer, fp_cpu);
										strcat(header," ");
										double cur = atof(buffer);
										double diff_time = cur - csit[core][state];
										char output_idle[50];
										snprintf(output_idle,50,"%.2f",diff_time/1000);
										strcat(header,output_idle);				
										memset(buffer, 0, sizeof(buffer));
										csit[core][state] = cur;
										fclose(fp_cpu);
									}
									else{
										strcat(header," x");
									}
								}
								
								strcat(header,"\n");
									
							}
							
							strcat(header,"_IDLE_USAGE_\n");
							for(int core=0; core<4; core++)
							{
								char title[100];
								snprintf(title,100,"idle_usage_%d=",core);
								strcat(header,title);
							
								for(int state=0; state<3; state++)
								{
								
									char proc[100];
									sprintf(proc,"/sys/devices/system/cpu/cpu%d/cpuidle/state%d/usage",core,state);
									
									if((fp_cpu = fopen(proc,"r")) != NULL) 
									{	
									
										fgets(buffer,sizeof buffer, fp_cpu);
										strcat(header," ");
										int cur = atoi(buffer);
										int diff_entry = cur - csiu[core][state];
										char output_entry[50];
										snprintf(output_entry,50,"%d",diff_entry);
										strcat(header,output_entry);				
										memset(buffer, 0, sizeof(buffer));
										csiu[core][state] = cur;
										fclose(fp_cpu);
									}
									else{
										strcat(header," x");
									}
								}
								
								strcat(header,"\n");
									
							}
							
							///////////////////////// Read memory usage /////////////////////////////////////////
							strcat(header,"_MEM_\n");
							//if((fp = fopen("/data/local/tmp/busybox free -m", "r")) != NULL)
							if((fp_mem = fopen("/proc/meminfo","r")) != NULL)
							{
								//printf("mem\n");
								fgets(buffer, sizeof buffer, fp_mem);
								//printf("buffer = %s\n",buffer);
								strcat(header,"mem=");
								char buffer2[1024];
								char buffer3[1024];
								fgets(buffer2, sizeof buffer, fp_mem);
								fgets(buffer3, sizeof buffer, fp_mem);
								double mem_util = parseMem(buffer, buffer2, buffer3);
								char output[50];
								snprintf(output,50,"%.2f",mem_util);		
								strcat(header,output);
															
								memset(buffer, 0, sizeof(buffer));
								memset(buffer2, 0, sizeof(buffer2));
								memset(buffer3, 0, sizeof(buffer3));		
								fclose(fp_mem);
								
							}
						
							strcat(header,"\n_DISPLAY_");
							//Read bright
							//Nexus ("/sys/class/backlight/s5p_bl/brightness")
							if((fp = fopen("/sys/class/backlight/panel/brightness","r")) != NULL) {	
							
								//printf("brightness\n");
								fgets(buffer,sizeof buffer, fp);
								//sprintf(header,"\n_DISPLAY_\n%s","bright=");
								strcat(header,"\nbright=");
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
								
							}
							
							if((fp = fopen("/sys/class/backlight/s5p_bl/brightness","r")) != NULL) {	
							
								//printf("brightness\n");
								fgets(buffer,sizeof buffer, fp);
								//sprintf(header,"\n_DISPLAY_\n%s","bright=");
								strcat(header,"\nbright=");
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
								
							}
						
							strcat(header,"_WIFI_\n");
							//wifi				
							if((fp = fopen(path_tx,"r")) != NULL) {
								
								//printf("WiFi\n");
								fgets(buffer,sizeof buffer, fp);
								double cur = atof(buffer);
								double diff_tx = cur - prev_tx;
								char output_tx[50];
								snprintf(output_tx,50,"%.2f",diff_tx);
								strcat(header,"tx=");
								strcat(header,output_tx);
								memset(buffer, 0, sizeof(buffer));
								
								prev_tx = cur;
								fclose(fp);
							}
							else{
								strcat(header,"tx=0");
							}
							
							if((fp = fopen(path_rx,"r")) != NULL) {
							
								fgets(buffer,sizeof buffer, fp);
								double cur = atof(buffer);
								double diff_rx = cur - prev_rx;
								char output_rx[50];
								snprintf(output_rx,50,"%.2f",diff_rx);
								strcat(header,"\nrx=");
								strcat(header,output_rx);
								memset(buffer, 0, sizeof(buffer));
								
								prev_rx = cur;
								fclose(fp);
								
							}
							else{
								strcat(header,"\nrx=0");
							}
							
							if((fp = fopen(path_oper,"r")) != NULL) {
							
								fgets(buffer,sizeof buffer, fp);
								strcat(header,"\noperstate=");				
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
							}
							else{
								strcat(header,"\noperstate=0");
							}
							
							
							//Battery						
							strcat(header,"\n_BATTERY_\n");
							if((fp = fopen("/sys/class/power_supply/battery/voltage_now","r")) != NULL) {
							
								fgets(buffer,sizeof buffer, fp);
								strcat(header,"batt_volt=");				
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
							}
							
							if((fp = fopen("/sys/class/power_supply/battery/current_now","r")) != NULL) {
								fgets(buffer,sizeof buffer, fp);
								strcat(header,"batt_current=");				
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
							}
							
							if((fp = fopen("/sys/class/power_supply/battery/capacity","r")) != NULL) {
								fgets(buffer,sizeof buffer, fp);
								strcat(header,"batt_capacity=");				
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
							}
							
							if((fp = fopen("/sys/class/power_supply/battery/temp","r")) != NULL) {
								fgets(buffer,sizeof buffer, fp);
								strcat(header,"batt_temperature=");				
								strcat(header,buffer);				
								memset(buffer, 0, sizeof(buffer));
								fclose(fp);
							}
							
							// Check for all the counters in the system if the counter has a value on the given active group and ouptut it.
							
							strcat(header,"_GPU_\n");
							
							for(int p = 0; p < uCounterNum; ++p)
							{					
								
								if(p < sReading.nValueCnt)
								{										
									
									strcat(header,psCounters[p].pszName);
									strcat(header,"=");								
									char params[50];
									snprintf(params,50,"%.2f\n",sReading.pfValueBuf[p]);
									strcat(header, params);				
									//memset(buffer, 0, sizeof(buffer));		
								}
							}			
						}
						
								strcpy(samples[startIndex],header);
								printf("%s\n",samples[startIndex]);
								memset(header, 0, sizeof(header));
								
								
								//realloc
								/*if(j == nRows-1)
								{	
									samples = (char **)realloc(samples, (nRows + size) * sizeof(char *));
									
									int s=0;
									nRows = nRows + size;
									for(s=j+1; s<nRows; s++)
										samples[s] = (char *) malloc(numCol * sizeof(char));
								}
								
								++j;
								
								cur_cap = getCap();
								*/
								
								++startIndex;
								//printf("Current start index = %d num of rows = %d \n", startIndex, nRows);
								printf("End_loop %d\n", startIndex);
						}		
				}
				
				//Poll for the counters once a second
				usleep(10000);
				//usleep(1000000);	
		}
					
		// Step 5. Shutdown PVRScopeStats
		PVRScopeDeInitialise(&psData, &psCounters, &sReading);
		
		struct timeval tv_stop;
		gettimeofday(&tv_stop, NULL);
		unsigned long time_in_sec_stop = tv_stop.tv_sec;
		char endTime[1024];
		snprintf(endTime,1024,"%lu\n",time_in_sec_stop);
		
		char elapseTime[1024];
		snprintf(elapseTime,1024,"%lu\n",(time_in_sec_stop - time_in_sec_start)/60);
		
		printf("Save file\n");
		sprintf(saveFile,"/data/local/tmp/stat/sample%d.txt", fileIndex);
		fp_save = fopen(saveFile,"w+");
		
		for(int i = 0; i < nRows; i++)
	    {
			//printf("%s",samples[i]);
			fprintf(fp_save, "%s", samples[i]);
		}
		
		fprintf(fp_save, "%s\n",endTime);
		//fprintf(fp_save, "Use time(min)=%s",elapseTime);
		
		printf("close all file\n");
		fclose(fp_save);
		
		printf("end file\n");
		
		//Clear array memory
		//sample is a heap data structure
		free(samples);
		
		memset(saveFile, 0, sizeof(saveFile));
		memset(prev_total, 0, sizeof(prev_total));
		memset(prev_idle, 0, sizeof(prev_idle));
		memset(csit, 0, sizeof(csit));
		memset(csiu, 0, sizeof(csiu));
		prev_tx = 0;
		prev_rx = 0;
		
		printf("clear file\n");
		
		exit(0);
}

int main(int argc, char **argv)
{
		printf("Start training...\n");
		
		if(argc < 4) 
		{
			printf("Please put ./sample fileIndex num_sample delay(second) network(1=wifi,0=3g) \n");
		}
		else 
		{
			fileIndex = atoi(argv[1]);    
			nRows = atoi(argv[2]);
			delay = atoi(argv[3]); //1 is too delay (~ 2 seconds) and 0.8 is closed to 1 sec.
			network = atoi(argv[4]);
			method();
		}
		return 0;
}

