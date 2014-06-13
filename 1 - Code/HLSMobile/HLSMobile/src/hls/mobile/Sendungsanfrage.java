package hls.mobile;

import java.io.Serializable;

public class Sendungsanfrage implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 8401630230262681132L;
	public final int saNr;
	public final String start;
	public final String ziel;
	public final String status;
	public final Auftraggeber auftraggeber;
	
	public Sendungsanfrage(int saNr, String start, String ziel, String status, Auftraggeber auftraggeber) {
		this.saNr = saNr;
		this.start = start;
		this.ziel = ziel;
		this.status = status;
		this.auftraggeber = auftraggeber;
	}
}
