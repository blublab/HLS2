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

public class KundenrechnungenActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_kundenrechnungen);
                
		@SuppressWarnings("unchecked")
		List<Kundenrechnung> list = (List<Kundenrechnung>)
				getIntent().getExtras().getSerializable("Rechnungen");        
        ListViewAdapter lvAdapter = new ListViewAdapter(list);
        ListView lv = (ListView)findViewById(R.id.listView1);
        lv.setAdapter(lvAdapter);   			
    }
    
    public class ListViewAdapter extends BaseAdapter {
    	private List<Kundenrechnung> rechnungen;
    	
    	public ListViewAdapter(List<Kundenrechnung> rechnungen) {
    		this.rechnungen = rechnungen;
    	}
    	
    	@Override
    	public int getCount() {
    		return rechnungen.size();
    	}

    	@Override
    	public Kundenrechnung getItem(int arg0) {
    		return rechnungen.get(arg0);
    	}

    	@Override
    	public long getItemId(int arg0) {
    		return arg0;
    	}

    	@Override
    	public View getView(int arg0, View arg1, ViewGroup arg2) {
			if(arg1==null)
			{
				LayoutInflater inflater = (LayoutInflater) KundenrechnungenActivity.this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
				arg1 = inflater.inflate(R.layout.listitem_kundenrechnungen, arg2,false);
			}    	
			TextView tv1 = (TextView)arg1.findViewById(R.id.textView1);
			TextView tv2 = (TextView)arg1.findViewById(R.id.textView2);
			
			Kundenrechnung kr = rechnungen.get(arg0);
			
			tv1.setText("[" + kr.rnNr + "] " + kr.betrag + " € (Bezahlt: " + (kr.bezahlt ? "Ja" : "Nein") + ")");
			tv2.setText("Kunde: " + kr.kunde);
			
			return arg1;
    	}
    }    
}
