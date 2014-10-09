//For sampling Nexus s
#include "PVRScopeGraph.h"
#include <pthread.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include "PVRScopeStats.h"


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

double prev_total = 0;
double prev_idle = 0;
double prev_idle_time = 0;
double prev_idle_entry = 0;
double prev_tx = 0;
double prev_rx = 0;

double parseCPU(char cpuLine[])
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

	double diff_idle = idle - prev_idle;
	double diff_total = total - prev_total;
	double diff_util = (1000 * (diff_total - diff_idle) / diff_total) / 10;

	prev_total = total;
	prev_idle = idle;
	free(tok);
	
	//printf("parse CPU util = %f\n",diff_util);
	
	
	return diff_util;
}

int nTest = 0; //  = atoi(argv[1]);    
int nRows = 0; //atoi(argv[2]);

char** sample; //[numTime * 25][numCol];

//void *method(void *ptr)
void method()
{
		
		char buffer[2048];
		char header[1024];
		char header2[2048];
		char aut[1024];
		int nCols = 2048;
		
		sample = (char **)malloc(nRows * sizeof(char *));		
		for(int x=0; x < nRows; x++)
		{
			sample[x] = (char *)malloc(nCols * sizeof(char));
		}
		
		
		printf("%d %d\n",nRows,nCols);
		
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
		}else{
			//LOGE("Error initializing PVRScope.");
			printf("Error initializing PVRScope...\n");
		}
		
		// Step 3. Set the active group to 0
		bActiveGroupChanged = true;
		uActiveGroupSelect = 0;
		unsigned int sampleRate = 100;
		unsigned int index = 0;
		unsigned int j = 0;
		
		FILE *fp;
		
		while (j < nRows)
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
				if (index < sampleRate) 
				{
					// Sample the counters every 10ms. Don't read or output it.
					PVRScopeReadCountersThenSetGroup(psData, NULL, 0, uActiveGroup);
				} 
				else 
				{

					index = 0;
					struct timeval tv;
					gettimeofday(&tv, NULL);
					unsigned long time_in_mill = (tv.tv_sec) * 1000 + (tv.tv_usec) / 1000;
					
					// Step 4. Read and output the counter information for group 0 to Logcat
					if(PVRScopeReadCountersThenSetGroup(psData, &sReading, time_in_mill * 1000, uActiveGroup))
					{
											
						//Read CPU util every 1 second
						if((fp = fopen("/proc/stat","r")) != NULL) {	
									
							fgets(buffer,sizeof buffer, fp);
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"\nloop_%d\n_CPU_\n%s",j,"util0=");
							
							//calculate cpu util
							double cpu_util = parseCPU(buffer);
							char output[50];
							snprintf(output,50,"%.2f",cpu_util);
							strcat(header,output);
							strcat(header2,header);
					
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							
							//printf("util %s \n", sample[j]);
							fclose(fp);
						}
						
						
						//Read CPU Freq
						if((fp = fopen("/sys/devices/system/cpu/cpu0/cpufreq/scaling_cur_freq","r")) != NULL) {	
							
							fgets(buffer,sizeof buffer, fp);
							sprintf(header,"\n%s","freq0=");
							strcat(header,buffer);
							strcat(header2,header);
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf(" %s \n",sample[j]);
							
							fclose(fp);
						}
						
						//Read CPU idle time
						if((fp = fopen("/sys/devices/system/cpu/cpu0/cpuidle/state0/time","r")) != NULL) {	
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"%s","it=");
							double cur = atof(buffer);
							double diff_time = cur - prev_idle_time;
							char output_idle[50];
							snprintf(output_idle,50,"%.2f",diff_time/1000);
							strcat(header,output_idle);				
							strcat(header2,header);
							
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							
							prev_idle_time = cur;
							fclose(fp);
						}
						
						//Read CPU idle usage
						if((fp = fopen("/sys/devices/system/cpu/cpu0/cpuidle/state0/usage","r")) != NULL) {	
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"\n%s","ie=");
							double cur = atof(buffer);
							double diff_entry = cur - prev_idle_entry;
							char output_entry[50];
							snprintf(output_entry,50,"%.2f",diff_entry);
							strcat(header,output_entry);				
							strcat(header2,header);
							
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							
							prev_idle_entry = cur;
							fclose(fp);
						}
						
						//Read bright
						if((fp = fopen("/sys/class/backlight/s5p_bl/brightness","r")) != NULL) {	
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"\n_DISPLAY_\n%s","bright=");
							strcat(header,buffer);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							fclose(fp);
						}
						
						//wifi				
						if((fp = fopen("/sys/class/net/wlan0/statistics/tx_packets","r")) != NULL) {
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"_WIFI_\n%s","tx=");
							
							double cur = atof(buffer);
							double diff_tx = cur - prev_tx;
							char output_tx[50];
							snprintf(output_tx,50,"%.2f",diff_tx);
							
							strcat(header,output_tx);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							
							prev_tx = cur;
							fclose(fp);
						}
						
						
						if((fp = fopen("/sys/class/net/wlan0/statistics/rx_packets","r")) != NULL) {
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"\n%s","rx=");
							
							double cur = atof(buffer);
							double diff_rx = cur - prev_rx;
							char output_rx[50];
							snprintf(output_rx,50,"%.2f",diff_rx);
							
							strcat(header,output_rx);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							
							prev_rx = cur;
							fclose(fp);
						}
						
						//Battery capacity
						if((fp = fopen("/sys/class/power_supply/battery/capacity","r")) != NULL) {
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"\n_BATTERY_\n%s","capacity=");
							strcat(header,buffer);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							fclose(fp);
						}
						
						//Battery voltage
						if((fp = fopen("/sys/class/power_supply/battery/voltage_now","r")) != NULL) {
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"%s","voltage=");
							strcat(header,buffer);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							fclose(fp);
						}
						
						//Battery temperature
						if((fp = fopen("/sys/class/power_supply/battery/temp","r")) != NULL) {
						
							fgets(buffer,sizeof buffer, fp);
							
							sprintf(header,"%s","temperature=");
							strcat(header,buffer);				
							strcat(header2,header);
						
							memset(&buffer[0], 0, sizeof(buffer));
							memset(&header[0], 0, sizeof(header));
							//printf("%s \n",sample[j]);
							fclose(fp);
						}
						
						//printf("%s \n",header2);
					
						// Check for all the counters in the system if the counter has a value on the given active group and ouptut it.
						bool isFirst = true;
						for(int i = 0; i < uCounterNum; ++i)
						{					
										
							if(i < sReading.nValueCnt)
							{										
								if ((strcmp(psCounters[i].pszName, "Frame time") == 0) || (strcmp(psCounters[i].pszName, "Frames per second (FPS)") == 0) || (strcmp(psCounters[i].pszName, "GPU task load: 3D core") == 0) || 
								(strcmp(psCounters[i].pszName, "GPU task load: TA core") == 0) || (strcmp(psCounters[i].pszName, "GPU task time: 3D core") == 0) || (strcmp(psCounters[i].pszName, "GPU task time: TA core") == 0) || 
								(strcmp(psCounters[i].pszName, "TA load") == 0) || (strcmp(psCounters[i].pszName, "Texture unit(s) load") == 0) || (strcmp(psCounters[i].pszName, "USSE clock cycles per pixel") == 0) ||
								(strcmp(psCounters[i].pszName, "USSE clock cycles per vertex") == 0) || (strcmp(psCounters[i].pszName, "USSE load: Pixel") == 0) || (strcmp(psCounters[i].pszName, "USSE load: Vertex") == 0) ||
								(strcmp(psCounters[i].pszName, "Vertices per frame") == 0) ||(strcmp(psCounters[i].pszName, "Texture unit(s) load") == 0)||(strcmp(psCounters[i].pszName, "USSE load: Pixel") == 0) ||
								(strcmp(psCounters[i].pszName, "USSE load: Stall") == 0)||(strcmp(psCounters[i].pszName, "Vertices per second: on-screen") == 0))
								{
									
									if(isFirst)
									{
										isFirst = false;
										sprintf(header,"_GPU_\n%s=",psCounters[i].pszName);
									}
									else
										sprintf(header,"%s=",psCounters[i].pszName);
									
									sprintf(buffer, "%f\n",sReading.pfValueBuf[i]);
									strcat(header,buffer);								
									strcat(header2,header);

									memset(&buffer[0], 0, sizeof(buffer));
									memset(&header[0], 0, sizeof(header));
									//strcat(sample[j]," \n");
									//printf("%d : %s : %f\n", j,  psCounters[i].pszName, sReading.pfValueBuf[i]);
									//printf("%s \n",sample[j]);
								}
								
							}
							
						}
						isFirst = true;
						
					}
					
					strcat(sample[j],header2);
					memset(&header2[0], 0, sizeof(header2));
					printf("Data >> %d \n %s",j,sample[j]);
					++j;
					
				}
				
				//Poll for the counters once a second
				usleep(10 * 1000);
				//usleep(1000000);
		}
			
		// Step 5. Shutdown PVRScopeStats
		PVRScopeDeInitialise(&psData, &psCounters, &sReading);
		
		printf("save file\n");
		sprintf(aut,"/data/local/tmp/stat/sample%d.txt",nTest);
		fp = fopen(aut,"w+");
		for(int i = 0; i < nRows; i++)
	    {
			fprintf(fp, "%s", sample[i]);
		}
		fclose(fp);
		
		printf("end file\n");
		//Clear array memory
		memset(&sample[0], 0, sizeof(sample));
		memset(&aut[0], 0, sizeof(aut));
		
		prev_total = 0;
		prev_idle = 0;
		prev_idle_time = 0;
		prev_idle_entry = 0;
		prev_tx = 0;
		prev_rx = 0;

		
		printf("clear file\n");
		exit(0);
}

int main(int argc, char **argv)
{
		printf("Start sampling...\n");
		//pthread_t thread1;
		//const char *message1 = "Thread 1";
		//int iret1;
		nTest  = atoi(argv[1]);    
		nRows = atoi(argv[2]);
		//iret1 = pthread_create(&thread1, NULL, method, (void*) message1);
		method();
		
		return 0;
}
