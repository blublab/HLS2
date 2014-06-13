package hls.mobile;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.util.Log;

public class Util {
	/**
	 * Fordert beim angegebenen HLS Webservice eine Liste der Sendungsanfragen an.
	 * 
	 * @param url
	 *  Der Host des Webservices für Sendungsanfragen.
	 * @return
	 *  Eine Liste der erhaltenen Sendungsanfragen.
	 * @throws IllegalArgumentException
	 *  Der 'host' Parameter ist null.
	 * @throws JSONException
	 *  Die vom Webservice zurückgelieferte Antwort konnte nicht verarbeitet werden.
	 * @throws IOException
	 *  Es ist ein Fehler während der Anfrage aufgetreten.
	 */
	public static ArrayList<Sendungsanfrage> getSendungsanfragen(String host) throws JSONException, IOException {
		if(host == null) {
			throw new IllegalArgumentException("host");
		}
		String json = httpRequest("http://" + host + "/Sendungsanfragen");
		return parseSendungsanfragenJSON(json);
	}

	/**
	 * Fordert beim angegebenen HLS Webservice eine Liste der Kundenrechnungen an.
	 * 
	 * @param url
	 *  Der Host des Webservices für Kundenrechnungen.
	 * @return
	 *  Eine Liste der erhaltenen Kundenrechnungen.
	 * @throws IllegalArgumentException
	 *  Der 'host' Parameter ist null.
	 * @throws JSONException
	 *  Die vom Webservice zurückgelieferte Antwort konnte nicht verarbeitet werden.
	 * @throws IOException
	 *  Es ist ein Fehler während der Anfrage aufgetreten.
	 */
	
	public static ArrayList<Kundenrechnung> getKundenrechnungen(String host) throws JSONException, IOException {
		if(host == null) {
			throw new IllegalArgumentException("host");
		}
		String json = httpRequest("http://" + host + "/Kundenrechnungen");
		return parseKundenrechnungenJSON(json);
	}

	/**
	 * Führt eine HTTP-GET Anfrage durch und liefert das Ergebnis (HTTP BODY)
	 * als String zurück.
	 * 
	 * @param url
	 *  Die URL der Anfrage.
	 * @return
	 *  Die angeforderte URL.
	 * @throws IllegalArgumentException
	 *  Der 'url' Parameter ist null.
	 * @throws IOException
	 *  Es ist ein Fehler während der Anfrage aufgetreten.
	 */
	private static String httpRequest(String url) throws IOException {
		if(url == null) {
			throw new IllegalArgumentException("url");
		}
		StringBuilder builder = new StringBuilder();
		HttpParams httpParams = new BasicHttpParams();
		// Timeout von 5 Sekunden für Verbindungsaufbau setzen.
		HttpConnectionParams.setConnectionTimeout(httpParams, 5000);
		HttpClient client = new DefaultHttpClient(httpParams);
		HttpGet httpGet = new HttpGet(url);
		HttpResponse response = client.execute(httpGet);
		StatusLine statusLine = response.getStatusLine();
		int statusCode = statusLine.getStatusCode();
		if (statusCode == 200) {
			HttpEntity entity = response.getEntity();
			InputStream content = entity.getContent();
			BufferedReader reader = new BufferedReader(new InputStreamReader(
					content));
			String line;
			while ((line = reader.readLine()) != null) {
				builder.append(line);
			}
		} else {
			Log.e("httpRequest", "Failed to download file (HTTP status "
					+ statusCode + ").");
		}
		return builder.toString();
	}

	/**
	 * Parsed die JSON-kodierte Antwort des HLS Webservices.
	 * 
	 * @param json
	 *            Der vom Webservice erhaltene (JSON) string.
	 * @return Eine Liste der Sendungsanfragen.
	 * @throws IllegalArgumentException
	 *             Der 'json' Parameter ist null.
	 * @throws JSONException
	 *             Der 'json' Parameter enthält kein gültiges JSON oder das
	 *             Format ist ungültig.
	 */
	private static ArrayList<Sendungsanfrage> parseSendungsanfragenJSON(String json) throws JSONException {
		if (json == null)
			throw new IllegalArgumentException("json");
		JSONArray jsonArray = new JSONArray(json);
		ArrayList<Sendungsanfrage> list = new ArrayList<Sendungsanfrage>();
		for (int i = 0; i < jsonArray.length(); i++) {
			JSONObject o = jsonArray.getJSONObject(i);
			// Auftraggeber Informationen parsen.
			Auftraggeber auftraggeber = parseAuftraggeber(o
					.getJSONObject("Auftrageber"));
			Sendungsanfrage sa = new Sendungsanfrage(o.getInt("SaNr"), o
					.getJSONObject("Start").getString("Name"), o.getJSONObject(
					"Ziel").getString("Name"), o.getString("Status"),
					auftraggeber);
			list.add(sa);
		}
		return list;
	}
	
	/**
	 * Parsed die JSON-kodierte Antwort des HLS Webservices.
	 * 
	 * @param json
	 *            Der vom Webservice erhaltene (JSON) string.
	 * @return Eine Liste der Kundenrechnungen.
	 * @throws IllegalArgumentException
	 *             Der 'json' Parameter ist null.
	 * @throws JSONException
	 *             Der 'json' Parameter enthält kein gültiges JSON oder das
	 *             Format ist ungültig.
	 */
	private static ArrayList<Kundenrechnung> parseKundenrechnungenJSON(String json) throws JSONException {
		if (json == null)
			throw new IllegalArgumentException("json");
		JSONArray jsonArray = new JSONArray(json);
		ArrayList<Kundenrechnung> list = new ArrayList<Kundenrechnung>();
		for (int i = 0; i < jsonArray.length(); i++) {
			JSONObject o = jsonArray.getJSONObject(i);
			Kundenrechnung kr = new Kundenrechnung(o.getInt("RnNr"),
					o.getJSONObject("Betrag").getDouble("Wert"), o.getBoolean("Bezahlt"),
					o.getString("Kunde"));
			list.add(kr);
		}
		return list;
	}	

	/**
	 * Parsed die Auftraggeberinformationen aus dem angegebenen JSON-Objekt.
	 * 
	 * @param json
	 *            Ein JSON-Objekt mit Auftraggeberinformationen.
	 * @return Eine initialisierte Instanz der Auftraggeber Klasse.
	 * @throws IllegalArgumentException
	 *             Der 'o' Parameter ist null.
	 * @throws JSONException
	 *             Der 'o' Parameter enthält kein gültiges JSON oder das Format
	 *             ist ungültig.
	 */
	private static Auftraggeber parseAuftraggeber(JSONObject o) throws JSONException {
		if (o == null) {
			throw new IllegalArgumentException("o");
		}
		JSONArray adressen = o.getJSONArray("Adressen");
		if (adressen.length() < 1)
			throw new JSONException("Keine Adresse");
		JSONObject addr = adressen.getJSONObject(0);
		Auftraggeber auftraggeber = new Auftraggeber(o.getInt("GpNr"),
				o.getString("Vorname"), o.getString("Nachname"), o
						.getJSONObject("Email").getString("EMail"),
				addr.getString("Strasse"), addr.getString("Hausnummer"),
				addr.getString("PLZ"), addr.getString("Wohnort"),
				addr.getString("Land"));
		return auftraggeber;
	}
}
