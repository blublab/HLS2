package hls.mobile;

import java.util.ArrayList;
import android.support.v4.app.Fragment;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.StrictMode;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.EditText;

public class MainActivity extends Activity {
	final Context context = this;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
		// Enable network access from the main thread.
		StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder()
				.permitAll().build();
		StrictMode.setThreadPolicy(policy);
        
        setContentView(R.layout.activity_main);
        
        findViewById(R.id.button1).setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View arg0) {
        		EditText mEdit = (EditText)findViewById(R.id.editText1);				
        		try {
        			ArrayList<Sendungsanfrage> list = Util.getSendungsanfragen(mEdit.getText().toString());
					Intent simple = new Intent(MainActivity.this,SendungsanfragenActivity.class);
					simple.putExtra("Anfragen", list);
					startActivity(simple);					
				} catch (Exception e) {
					alertDialog("Fehler", "Fehler beim Abrufen");
				}        		
			}
		});        
        
        findViewById(R.id.button2).setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View arg0) {
        		EditText mEdit = (EditText)findViewById(R.id.editText1);
        		try {
        			ArrayList<Kundenrechnung> list = Util.getKundenrechnungen(mEdit.getText().toString());
    				Intent simple = new Intent(MainActivity.this,KundenrechnungenActivity.class);
					simple.putExtra("Rechnungen", list);
					startActivity(simple);					
				} catch (Exception e) {
					alertDialog("Fehler", "Fehler beim Abrufen");
				}         		
			}
		});           
    }
    
    private void alertDialog(String title, String text) {
		AlertDialog dlg = new AlertDialog.Builder(context)
		.setTitle(title)
		.setMessage(text)
		.create();
		
		dlg.setButton(DialogInterface.BUTTON_NEUTRAL, "OK", new DialogInterface.OnClickListener() {
			@Override
			public void onClick(DialogInterface dialog, int which) {
				// TODO Auto-generated method stub
				
			}});
		dlg.show();    	
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
        switch(id) {
        	case R.id.action_beenden:
        		android.os.Process.killProcess(android.os.Process.myPid());
        		break;
        }
        return super.onOptionsItemSelected(item);
    }

    /**
     * A placeholder fragment containing a simple view.
     */
    public static class PlaceholderFragment extends Fragment {

        public PlaceholderFragment() {
        }

        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_main, container, false);
            return rootView;
        }
    }
    
}