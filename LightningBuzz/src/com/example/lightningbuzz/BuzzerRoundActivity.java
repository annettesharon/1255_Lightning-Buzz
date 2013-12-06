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

public class BuzzerRoundActivity extends Activity {
	String PKey ="";
	String ServerIp="10.201.4.1";
	String PortAddr="5000";
	String UserNm="";
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_buzzer_round);
		
		
		Bundle extras = getIntent().getExtras(); 
		if(extras !=null) {
		    ServerIp = extras.getString("sip");
		    PortAddr = extras.getString("pa");
		    UserNm = extras.getString("un");
		    PKey = extras.getString("pk");
		    
		    if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
		    {
		    Toast toast = Toast.makeText(BuzzerRoundActivity.this,"Connection Details Configured. Buzzer Ready!", Toast.LENGTH_LONG);
			toast.show();
		    }
		    else{
			Toast toast = Toast.makeText(BuzzerRoundActivity.this,"Invalid Connection Details!", Toast.LENGTH_LONG);
			toast.show();
		    }
		}
		
		Button btnBuzz = (Button) findViewById (R.id.buttonBuzz);
		
		btnBuzz.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
				if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
			    {
			    	SendUDP obj=new SendUDP();
			    	obj.ServerIp=ServerIp;
			    	obj.PortAddr=PortAddr;
			    	obj.Command="3";
			    	obj.UserNm=UserNm;
			    	obj.PKey=PKey;
			    	obj.Ans="0";
			    	Thread t = new Thread(obj);
			        t.start();
			        
			    Toast toast = Toast.makeText(BuzzerRoundActivity.this,"Buzzer Pressed!!!", Toast.LENGTH_LONG);
				toast.show();
			    }
			    else{
				Toast toast = Toast.makeText(BuzzerRoundActivity.this,"Invalid Connection Details!", Toast.LENGTH_LONG);
				toast.show();
			    }
			}
		});
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.buzzer_round, menu);
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
