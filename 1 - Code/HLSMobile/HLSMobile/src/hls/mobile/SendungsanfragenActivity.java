package hls.mobile;

import java.util.List;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ListView;
import android.widget.TextView;

public class SendungsanfragenActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sendungsanfragen);
                
		@SuppressWarnings("unchecked")
		List<Sendungsanfrage> list = (List<Sendungsanfrage>)
				getIntent().getExtras().getSerializable("Anfragen");
		final ListViewAdapter lvAdapter = new ListViewAdapter(list);
	    ListView lv = (ListView)findViewById(R.id.listView1);
	    lv.setAdapter(lvAdapter);
    }
    
    public class ListViewAdapter extends BaseAdapter {
    	private final List<Sendungsanfrage> anfragen;
    	
    	public ListViewAdapter(List<Sendungsanfrage> anfragen) {
    		this.anfragen = anfragen;
    	}
    	
    	@Override
    	public int getCount() {
    		return anfragen.size();
    	}

    	@Override
    	public Sendungsanfrage getItem(int arg0) {
    		return anfragen.get(arg0);
    	}

    	@Override
    	public long getItemId(int arg0) {
    		return arg0;
    	}

    	@Override
    	public View getView(int arg0, View arg1, ViewGroup arg2) {
			if(arg1==null)
			{
				LayoutInflater inflater = (LayoutInflater) SendungsanfragenActivity.this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
				arg1 = inflater.inflate(R.layout.listitem_sendungsanfragen, arg2,false);
			}    		
			TextView tv1 = (TextView)arg1.findViewById(R.id.textView1);
			TextView tv2 = (TextView)arg1.findViewById(R.id.textView2);
			
			Sendungsanfrage sa = anfragen.get(arg0);
			
			tv1.setText("[" + sa.saNr + "] " + sa.start + " --> " + sa.ziel + " (" + sa.status + ")");
			tv2.setText(sa.auftraggeber.vorname + " " + sa.auftraggeber.nachname + ", " + sa.auftraggeber.land);
			
			return arg1;
    	}
    }    
    
}
