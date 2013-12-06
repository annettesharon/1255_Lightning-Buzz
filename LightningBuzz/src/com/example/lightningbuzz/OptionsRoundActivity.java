package com.example.lightningbuzz;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;

public class OptionsRoundActivity extends Activity {
	String PKey ="";
	String ServerIp="10.201.4.1";
	String PortAddr="5000";
	String UserNm="";
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_options_round);
		
		Bundle extras = getIntent().getExtras(); 
		if(extras !=null) {
		    ServerIp = extras.getString("sip");
		    PortAddr = extras.getString("pa");
		    UserNm = extras.getString("un");
		    PKey = extras.getString("pk");
		    
		    if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
		    {
		    Toast toast = Toast.makeText(OptionsRoundActivity.this,"Connection Details Configured. Buzzer Ready!", Toast.LENGTH_LONG);
			toast.show();
		    }
		    else{
			Toast toast = Toast.makeText(OptionsRoundActivity.this,"Invalid Connection Details!", Toast.LENGTH_LONG);
			toast.show();
		    }
		}
		
		Button Opt1 = (Button) findViewById (R.id.Button1);
		Button Opt2 = (Button) findViewById (R.id.Button2);
		Button Opt3 = (Button) findViewById (R.id.Button3);
		Button Opt4 = (Button) findViewById (R.id.Button4);
		
		Opt1.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
				SendOptionUDP("1");
			}
		});
		
		Opt2.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
				SendOptionUDP("2");
			}
		});
		
		Opt3.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
				SendOptionUDP("3");
			}
		});
		
		Opt4.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
				SendOptionUDP("4");
			}
		});
	}
	
	public void SendOptionUDP(String Opt) {
		 if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
		    {
		    	SendUDP obj=new SendUDP();
		    	obj.ServerIp=ServerIp;
		    	obj.PortAddr=PortAddr;
		    	obj.Command="2";
		    	obj.UserNm=UserNm;
		    	obj.PKey=PKey;
		    	obj.Ans=Opt;
		    	Thread t = new Thread(obj);
		        t.start();
		        
		    Toast toast = Toast.makeText(OptionsRoundActivity.this,"Option " + Opt + " Pressed!!!", Toast.LENGTH_LONG);
			toast.show();
		    }
		    else{
			Toast toast = Toast.makeText(OptionsRoundActivity.this,"Invalid Connection Details!", Toast.LENGTH_LONG);
			toast.show();
		    }
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.options_round, menu);
		return true;
	}
	
	@Override
    public boolean onOptionsItemSelected(MenuItem item) {
			switch (item.getItemId()) {
		    case R.id.action_settings:
		    	Intent intent = new Intent(this, MainActivity.class);
		    	intent.putExtra("sip", ServerIp);
				intent.putExtra("pa", PortAddr);
				intent.putExtra("un", UserNm);
				intent.putExtra("pk", PKey);
		    	startActivity(intent);
		        return true;
		     default:
		        return super.onOptionsItemSelected(item);
		    }
}

}


