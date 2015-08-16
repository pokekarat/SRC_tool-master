package com.example.tranparentapp;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.RandomAccessFile;

public class FileMgr {

	static String vPath = "/sys/class/power_supply/battery/voltage_now";
	//static String cPath = "/sys/class/power_supply/battery/current_now";
	
	public static String readVolt(){
		
		return FileMgr.readOneLine(vPath);
	}
	
	public static String readOneLine(String path){

		String result = "";
		
		try 
		{
			RandomAccessFile file = new RandomAccessFile(path, "r");		
			result = file.readLine();
			file.close();
			
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		
		return result;
	}
	
}
