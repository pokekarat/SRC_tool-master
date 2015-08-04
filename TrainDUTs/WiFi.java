package com.example.trainapp;

import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.RandomAccessFile;
import java.net.Socket;
import java.net.UnknownHostException;

import android.content.Context;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;

public class WiFi {

	public WiFi(){}
	
	public static WifiInfo wifiInfo;
	public static WifiManager wifiMgr;
	static String[] outMsg2 = new String[15];
	static String outMsg = "";
	
	public static void createTestFile(){
		
		for(int i=1; i<= 20000; i++){
			outMsg += "A";
		}
	}
	
	public static void createTestFile2(){
		String data = "";
    	int[] size = {1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 }; //, 20000, 30000, 40000, 50000, 100000, 200000, 300000, 400000, 500000};
    	
    	for(int i=0; i<size.length; i++)
    	{
    		int dataLen = data.length();
    		int len = size[i] - dataLen;
    		for(int j=0; j<len; j++)
    		{
    			data += "a";
    		}
    		outMsg2[i] = data;
    	}
    	
    	int chk;
		for(int k=0; k<outMsg2.length; k++){
    		chk =	outMsg2[k].length();
		}
	}
	
	public static void init(MainActivity act)
	{
		wifiMgr = (WifiManager)act.getSystemService(Context.WIFI_SERVICE);
		wifiInfo = wifiMgr.getConnectionInfo();
	}
	
	static BufferedWriter out;
	static Socket s;
	
	public static void initToServer(){
		
		try {
			
			s = new Socket("192.168.1.10",5555);
				
			out = new BufferedWriter(new OutputStreamWriter(s.getOutputStream()));
				
		} catch (UnknownHostException e) {
			
			e.printStackTrace();
			
		} catch (IOException e) {
				
			e.printStackTrace();
		}	
	}
	
	public static void closeToServer(){
		
		try {
			
			out.write("x\n");
			
			out.flush();
	    	
	    	out.close();
	    	
	    	s.close();
	    	
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} 
	}
	
	public static void sendFileToServer()
	{
		try {
				for(int i=1; i<=20; i++)
				{
					for(int j=0; j<i; j++)
			    	{
			    		
						out.write(outMsg+"\n");
						out.flush();	
					
			    		int delayTime = 1000/i;
			    		Util.delay(delayTime); //100 milli seconds
			    	}
				
					Util.delay(10000); // 10 seconds
				}
				
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
    	/*
    	Old
    	for(int j=0; j<=2000; j+=100)
    	{
    		out.write(outMsg+"\n");
    		out.flush();	
    	
    		try 
    		{
    			Thread.sleep(j);
    		} 
    		catch (InterruptedException e) 
    		{
    			e.printStackTrace();
    		}
    	}*/		
			
	}
	
	private static int prevTx = -1;
	private static int prevRx = -1;
	
	public static int TxPacket(String path)
	{	
		if(prevTx == -1)
		{
			prevTx = Integer.parseInt(FileMgr.readOneLine(path));
			return 0;	
			
		}else{
			int currTx = Integer.parseInt(FileMgr.readOneLine(path));
			int numTx = currTx - prevTx;
			prevTx = currTx;
			return numTx;
		}
	}
	
	public static int RxPacket(String path)
	{	
		if(prevRx == -1)
		{
			prevRx = Integer.parseInt(FileMgr.readOneLine(path));
			return 0;	
			
		}else{
			int currRx = Integer.parseInt(FileMgr.readOneLine(path));
			int numRx = currRx - prevRx;
			prevRx = currRx;
			return numRx;
		}
	}
}
