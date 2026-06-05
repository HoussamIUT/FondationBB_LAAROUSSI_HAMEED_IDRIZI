using System;

namespace FondationBB
{
    public class Espece
    {
        public int IdEspece { get; set; }
        public string LibelleEspece { get; set; } = "";

        public Espece() { }

        public Espece(int idEspece, string libelleEspece)
        {
            IdEspece = idEspece;
            LibelleEspece = libelleEspece;
        }

        // Raccourci utilisé par le barème de frais (chat).
        public bool EstChat => LibelleEspece.Trim().Equals("Chat", StringComparison.OrdinalIgnoreCase);

        public override string ToString() => LibelleEspece;
    }
}
