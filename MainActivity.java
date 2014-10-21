package com.example.tranparentapp;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

public class MainActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
	
		super.onCreate(savedInstanceState);
		//setContentView(R.layout.activity_main);
		
		Intent i = new Intent(MainActivity.this, HUD.class);
		i.putExtra("name", "SurvivingwithAndroid");
		MainActivity.this.startService(i);
		
		this.finish();
		
	}
}

