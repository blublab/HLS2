using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace ApplicationCore.TransportnetzKomponente.AccessLayer
{
    public interface ITransportnetzServices
    {
        /// <summary>
        /// Fügt dem Repository eine neue Lokation hinzu.
        /// </summary>
        /// <throws>ArgumentException, falls lokationDTO == null.</throws>
        /// <throws>ArgumentException, falls lokationDTO.LokNr != 0.</throws>
        void CreateLokation(ref LokationDTO lokationDTO);

        /// <summary>
        /// Fügt dem Repository eine neue Transportbeziehung hinzu.
        /// </summary>
        /// <throws>ArgumentException, falls tbDTO == null.</throws>
        /// <throws>ArgumentException, falls tbDTO.TbNr != -1.</throws>
        void CreateTransportbeziehung(ref TransportbeziehungDTO tbDTO);

        /// <summary>
        /// Sucht eine Lokation nach Lokationsnummer.
        /// </summary>
        /// <throws>ArgumentException, lokNr < 0.</throws>
        /// <returns>Lokation zur Lokationsnummer; null, falls keine solche Lokation gefunden.</returns>
        LokationDTO FindLokation(long lokNr);

        /// <summary>
        /// Sucht eine Lokation nach Lokationsnamen.
        /// </summary>
        /// <throws>ArgumntException, falls lokName == null.</throws>
        /// <returns>Lokation zum Lokationsnamen; null, falls keine solche Lokation gefunden.</returns>
        LokationDTO FindLokation(string lokName);

        /// <summary>
        /// Sucht eine Transportbeziehung zwischen den angebenen Lokationen.
        /// </summary>
        /// <throws>ArgumentException, start == null oder ziel == null.</throws>
        /// <returns>Transportbeziehung zur Transportbeziehungsnummer; null, falls keine solche Transportbeziehung gefunden.</returns>
        TransportbeziehungDTO FindTransportbeziehung(string start, string ziel);

        /// <summary>
        /// Sucht alle Transportbeziehungen für die Lokation mit der Lokationsnummer.
        /// </summary>
        /// <throws>ArgumentException, lokNr < 0.</throws>
        /// <returns>Einen Iterator zum Durchlaufen aller gefundenen Transportbeziehungen.</returns>
        IEnumerable<TransportbeziehungDTO> FindTransportbeziehungen(long lokNr);

        /// <summary>
        /// Sucht alle Transportbeziehungen für die Lokation mit dem angegebenen Namen.
        /// </summary>
        /// <throws>ArgumentException, lokName == null.</throws>
        /// <returns>Einen Iterator zum Durchlaufen aller gefundenen Transportbeziehungen.</returns>
        IEnumerable<TransportbeziehungDTO> FindTransportbeziehungen(string lokName);

        /// <summary>
        /// Sucht eine Transportbeziehung nach Transportbeziehungsnummer.
        /// </summary>
        /// <throws>ArgumentException, tbNr < 0.</throws>
        /// <returns>Transportbeziehung zur Transportbeziehungsnummer; null, falls keine solche Transportbeziehung gefunden.</returns>
        TransportbeziehungDTO FindTransportbeziehung(long tbNr);

        /// <summary>
        /// Löscht alle Knoten und Beziehungen aus dem Repository.
        /// </summary>
        void DeleteTransportnetz();

        /// <summary>
        /// Löscht alle Knoten und Beziehungen aus dem Repository, die die angegebene regular expression matchen.
        /// </summary>
        void DeleteTransportnetz(string regExp);
    }
}
