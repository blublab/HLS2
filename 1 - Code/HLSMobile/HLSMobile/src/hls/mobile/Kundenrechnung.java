package hls.mobile;

import java.io.Serializable;

public class Kundenrechnung implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1254138332940375519L;
	public final int rnNr;
	public final double betrag;
	public final boolean bezahlt;
	public final String kunde;
	
	public Kundenrechnung(int rnNr, double betrag, boolean bezahlt, String kunde) {
		this.rnNr = rnNr;
		this.betrag = betrag;
		this.bezahlt = bezahlt;
		this.kunde = kunde;
	}
}
