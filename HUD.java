package com.example.tranparentapp;

import android.app.Service;
import android.content.Intent;
import android.graphics.PixelFormat;
import android.os.AsyncTask;
import android.os.IBinder;
import android.view.Gravity;
import android.view.MotionEvent;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;

public class HUD extends Service
{
    Button mButton;
    TextView tv;
    
    @Override
    public IBinder
    onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        //mView = new HUDView(this);
        mButton = new Button(this);
        mButton.setText("My Overlay Button");
        mButton.setClickable(true);
        mButton.setOnTouchListener(new View.OnTouchListener() {
          @Override
          public boolean onTouch(View arg0, MotionEvent arg1) {
            mButton.setText("CLICKED!!!");
            return true;
          }
        });
        
        tv = new TextView(this);
        tv.setText("Voltage = 4.2");

        WindowManager.LayoutParams params = new WindowManager.LayoutParams(
                WindowManager.LayoutParams.WRAP_CONTENT,
                WindowManager.LayoutParams.WRAP_CONTENT,
                WindowManager.LayoutParams.TYPE_SYSTEM_OVERLAY,
                //WindowManager.LayoutParams.TYPE_SYSTEM_ALERT,
                WindowManager.LayoutParams.FLAG_WATCH_OUTSIDE_TOUCH,
                PixelFormat.TRANSLUCENT);
        params.gravity = Gravity.TOP | Gravity.CENTER;
        params.setTitle("Load Average");
        WindowManager wm = (WindowManager) getSystemService(WINDOW_SERVICE);
        wm.addView(tv, params);
        
        new UpdateTask(this).execute();
        
    }
    
    private class UpdateTask extends AsyncTask<Void, String, String> {
    	 
        //private static final String DEBUG_TAG = "TutListDownloaderService$DownloaderTask";
    	Service ser =  null;
    	int updateTime = 10;
    	
    	public UpdateTask(Service s){
    		
    		ser = s;
    	}
    	
        @Override
        protected String doInBackground(Void... params) {
        	
        	String[] res = new String[2];
        	for(int i=0; i<=this.updateTime; i++){
        		
        		String voltData = FileMgr.readVolt();
        		res[0] = voltData;
        		res[1] = ""+i;
        		this.publishProgress(res);
        		
        		try {
					Thread.sleep(1000);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
        	}
        	
			return null;
            // TBD
        }
        
        
    	protected void onProgressUpdate(String... arg1)
    	{
    		String result1 = arg1[0];
			double ret = Double.parseDouble(result1)/1000000;
    		tv.setText(arg1[1]+","+ret);
    		
    		if(arg1[1].equals(this.updateTime+"")){
    			
    			//HUD.stopstopSelf();
    			this.cancel(true);
    			ser.stopSelf();
    		}
    	}
 
       
    }
    

    @Override
    public void onDestroy() {
        super.onDestroy();
        if(mButton != null)
        {
            ((WindowManager) getSystemService(WINDOW_SERVICE)).removeView(mButton);
            mButton = null;
        }
        
        if(tv != null)
        {
            ((WindowManager) getSystemService(WINDOW_SERVICE)).removeView(tv);
            tv = null;
        }
    }
}
