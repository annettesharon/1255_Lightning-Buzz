package com.example.lightningbuzz;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
//import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class MainActivity extends Activity {
	String ServerIp="";
	String PortAddr="";
	String UserNm="";
	String PKey="";
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		
		final EditText txtServer = (EditText) findViewById(R.id.editTextServer);
		final EditText txtPort = (EditText) findViewById(R.id.EditTextPort);
		final EditText txtUser = (EditText) findViewById(R.id.EditTextUser);
		final EditText txtKey = (EditText) findViewById(R.id.EditTextMsg);
		
		Button RegBtn = (Button) findViewById (R.id.ButtonStop);
		Button Round1Btn = (Button) findViewById (R.id.ButtonBack1);
		Button Round2Btn = (Button) findViewById (R.id.ButtonBack2);
		
		Bundle extras = getIntent().getExtras(); 
		if(extras !=null) {
		    ServerIp = extras.getString("sip");
		    PortAddr = extras.getString("pa");
		    UserNm = extras.getString("un");
		    PKey = extras.getString("pk");
		    txtServer.setText(ServerIp);
		    txtPort.setText(PortAddr);
		    txtUser.setText(UserNm);
		    txtKey.setText(PKey);
		}
		
			RegBtn.setOnClickListener(new OnClickListener(){
			public void onClick(View v)
			{
			ServerIp=txtServer.getText().toString();
			PortAddr=txtPort.getText().toString();
			UserNm=txtUser.getText().toString();
			PKey=txtKey.getText().toString();
			
			if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
		    {
			SendUDP obj=new SendUDP();
        	obj.ServerIp=ServerIp;
        	obj.PortAddr=PortAddr;
        	obj.Command="1";
        	obj.UserNm=UserNm;
        	obj.PKey=PKey;
        	obj.Ans="0";
        	Thread t = new Thread(obj);
            t.start();
            
            Toast toast = Toast.makeText(MainActivity.this,"Register clicked", Toast.LENGTH_LONG);
			toast.show();
			}
			else
			{
				  Toast toast = Toast.makeText(MainActivity.this,"Insufficient Configuration Details!", Toast.LENGTH_LONG);
					toast.show();
			}}
        });
			
			Round1Btn.setOnClickListener(new OnClickListener(){
				public void onClick(View v)
				{
					ServerIp=txtServer.getText().toString();
					PortAddr=txtPort.getText().toString();
					UserNm=txtUser.getText().toString();
					PKey=txtKey.getText().toString();
					
					if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
				    {
			Intent intent = new Intent(MainActivity.this, OptionsRoundActivity.class);
			intent.putExtra("sip", ServerIp);
			intent.putExtra("pa", PortAddr);
			intent.putExtra("un", UserNm);
			intent.putExtra("pk", PKey);
			startActivity(intent);
				}
				else
				{
					  Toast toast = Toast.makeText(MainActivity.this,"Insufficient Configuration Details!", Toast.LENGTH_LONG);
						toast.show();
				}
				}
	        });
			
			
			Round2Btn.setOnClickListener(new OnClickListener(){
				public void onClick(View v)
				{
					ServerIp=txtServer.getText().toString();
					PortAddr=txtPort.getText().toString();
					UserNm=txtUser.getText().toString();
					PKey=txtKey.getText().toString();
					
					if((ServerIp.equals("")==false) && (PortAddr.equals("")==false) && (UserNm.equals("")==false) && (PKey.equals("")==false))
				    {
			Intent intent = new Intent(MainActivity.this, BuzzerRoundActivity.class);
			intent.putExtra("sip", ServerIp);
			intent.putExtra("pa", PortAddr);
			intent.putExtra("un", UserNm);
			intent.putExtra("pk", PKey);
			startActivity(intent);
				    }
					else
					{
						  Toast toast = Toast.makeText(MainActivity.this,"Insufficient Configuration Details!", Toast.LENGTH_LONG);
							toast.show();
					}
				}
	        });
	}

	/*@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
*/
}
