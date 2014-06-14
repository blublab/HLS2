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
	public final String abholzeitStart;
	public final String abholzeitEnde;
	public final String gueltigBis;
	public final Auftraggeber auftraggeber;
	
	public Sendungsanfrage(int saNr, String start, String ziel, String status, String abholzeitStart,
			String abholzeitEnde, String gueltigBis, Auftraggeber auftraggeber) {
		this.saNr = saNr;
		this.start = start;
		this.ziel = ziel;
		this.status = status;
		this.abholzeitStart = abholzeitStart;
		this.abholzeitEnde = abholzeitEnde;
		this.gueltigBis = gueltigBis;
		this.auftraggeber = auftraggeber;
	}
}
