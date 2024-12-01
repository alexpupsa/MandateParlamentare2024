﻿using MandateParlamentare2024.Models;

namespace MandateParlamentare2024.Services
{
    public class DataMapper
    {
        public static VoturiJudet MapJsonDataToVoturiJudet(Root root, TipVot tipVot)
        {
            var votKey = tipVot == TipVot.Senat ? "S" : "CD";
            var items = root.Scopes["PRCNCT"].Categories[votKey].Table.Values.ToList();
            var countyVotes = new VoturiJudet
            {
                Voturi = new List<VoturiCandidat>()
            };
            foreach (var item in items)
            {
                countyVotes.Judet = item.CountyName;
                foreach (var candidate in item.Candidates)
                {
                    var votes = int.Parse(candidate.Votes);

                    var foundCandidate = countyVotes.Voturi.FirstOrDefault(x => x.Candidat == candidate.CandidateName);
                    if (foundCandidate == null)
                    {
                        countyVotes.Voturi.Add(new VoturiCandidat
                        {
                            Candidat = candidate.Party,
                            Voturi = votes
                        });
                    }
                    else
                    {
                        foundCandidate.Voturi += votes;
                    }
                }
            }

            return countyVotes;
        }
    }
}
