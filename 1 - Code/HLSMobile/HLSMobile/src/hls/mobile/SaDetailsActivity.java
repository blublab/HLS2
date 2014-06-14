package hls.mobile;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

public class SaDetailsActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sa_details);        
		Sendungsanfrage sa = (Sendungsanfrage)
				getIntent().getExtras().getSerializable("Sendungsanfrage");
		tv(R.id.tvSendungsanfrage).setText("Sendungsanfrage #" + sa.saNr);
		tv(R.id.tvStartZiel).setText(sa.start + " - " + sa.ziel);
		tv(R.id.tvStatus).setText(sa.status);
		tv(R.id.tvGueltigBis).setText(sa.gueltigBis);
		tv(R.id.tvAbholzeitraum).setText(sa.abholzeitStart + " - " + sa.abholzeitEnde);
		StringBuilder b = new StringBuilder()
			.append("Partner-Nr. #" + sa.auftraggeber.gpNr + "\n\n")
			.append(sa.auftraggeber.vorname + " " + sa.auftraggeber.nachname + "\n")
			.append(sa.auftraggeber.strasse + " " + sa.auftraggeber.hausnummer + "\n")
			.append(sa.auftraggeber.plz + " " + sa.auftraggeber.stadt +
					", " + sa.auftraggeber.land + "\n\n")
			.append("Kontakt: " + sa.auftraggeber.email);
		tv(R.id.tvGeschaeftspartner).setText(b);
    }
    
    private TextView tv(int id) {
    	return ((TextView)findViewById(id));
    }
}
