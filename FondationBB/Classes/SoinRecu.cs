using System;

namespace FondationBB
{
    // Classe d'association Animal × Soin × Date (relation ternaire du MCD).
    // Porte la date du soin et une éventuelle date de rappel (vaccin, etc.).
    public class SoinRecu
    {
        public Soin? Soin { get; set; }
        public DateTime DateSoin { get; set; }
        public DateTime? DateRappel { get; set; }

        public SoinRecu() { }

        public SoinRecu(Soin soin, DateTime dateSoin, DateTime? dateRappel = null)
        {
            Soin = soin;
            DateSoin = dateSoin;
            DateRappel = dateRappel;
        }
    }
}
