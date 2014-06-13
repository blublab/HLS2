package hls.mobile;

import java.io.Serializable;

public class Auftraggeber implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = -2443154709434675837L;
	public final int gpNr;
	public final String vorname;
	public final String nachname;
	public final String email;
	public final String strasse;
	public final String hausnummer;
	public final String plz;
	public final String stadt;
	public final String land;
	
	public Auftraggeber(int gpNr, String vorname, String nachname, String email, String strasse,
			String hausnummer, String plz, String stadt, String land) {
		this.gpNr = gpNr;
		this.vorname = vorname;
		this.nachname = nachname;
		this.email = email;
		this.strasse = strasse;
		this.hausnummer = hausnummer;
		this.plz = plz;
		this.stadt = stadt;
		this.land = land;
	}
}
