package com.example.whitescreen;

import android.support.v7.app.ActionBarActivity;
import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.media.MediaPlayer;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.widget.Toast;


public class MainActivity extends ActionBarActivity {

	//Button mButton;
	
    @Override
    protected void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        MyView mv = new MyView(this);
        setContentView(mv);
       
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
       /* mButton = (Button)findViewById(R.id.button1);
        
        
        mButton.setOnClickListener(new View.OnClickListener() {
			
			@Override
			public void onClick(View v) {
				// TODO Auto-generated method stub
				 getWindow().getDecorView().setBackgroundColor(Color.parseColor("0xCC0000"));
			}
		}); */
    }
    
    public class MyView extends View 
    {
        public MyView(Context context) 
        {
             super(context);
             // TODO Auto-generated constructor stub
        }

        public int color = Color.parseColor("#660000");
        Paint paint = new Paint();
        
        @Override
        protected void onDraw(Canvas canvas) 
        {
           // TODO Auto-generated method stub
           super.onDraw(canvas);
           paint.setStyle(Paint.Style.FILL);
           paint.setColor(color);
           canvas.drawPaint(paint);
       }
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }
    
    @Override
    public void onStart(){
        super.onStart();
        
        Bundle extras = this.getIntent ( ).getExtras ( );

        if ( extras != null ) {
          if ( extras.containsKey ( "foo" ) ) {
        	
        	  try{
                  
         		 MediaPlayer mplayer = MediaPlayer.create(this, R.raw.kalimba);
                 mplayer.start();
                  
              }catch(Exception e){
                  
             	 Log.d("<your TAG here>" , "error: " + e);
              
              }
        	  
        	  finish();
          }
        }
    }
    
    @Override
    protected void onDestroy()
    {
        super.onDestroy();
        Toast.makeText(getApplicationContext(),"16. onDestroy()", Toast.LENGTH_SHORT).show();
        try{
            
   		 	MediaPlayer mplayer = MediaPlayer.create(this, R.raw.kalimba);
            mplayer.start();
            
        }catch(Exception e){
            
       	 Log.d("<your TAG here>" , "error: " + e);
        
        }
    }
	
}
