using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Personne
    {
        private int idPersonne;
        private string nomPersonne;
        private string prenomPersonne;
        private DateTime dateNaissancePersonne;
        private string numeroPersonne;
        private string ruePersonne;
        private string cpPersonne;
        private string villePersonne;
        private string telephonePersonne;
        private string mailPersonne;
        private DateTime dateCreationPersonne;

        public Personne()
        {
        }

        public Personne(int idPersonne, string nomPersonne, string prenomPersonne,
                        string telephonePersonne, string mailPersonne)
        {
            this.idPersonne = idPersonne;
            this.nomPersonne = nomPersonne;
            this.prenomPersonne = prenomPersonne;
            this.telephonePersonne = telephonePersonne;
            this.mailPersonne = mailPersonne;
        }

        public int IdPersonne { get => idPersonne; set => idPersonne = value; }
        public string NomPersonne { get => nomPersonne; set => nomPersonne = value; }
        public string PrenomPersonne { get => prenomPersonne; set => prenomPersonne = value; }
        public DateTime DateNaissancePersonne { get => dateNaissancePersonne; set => dateNaissancePersonne = value; }
        public string NumeroPersonne { get => numeroPersonne; set => numeroPersonne = value; }
        public string RuePersonne { get => ruePersonne; set => ruePersonne = value; }
        public string CpPersonne { get => cpPersonne; set => cpPersonne = value; }
        public string VillePersonne { get => villePersonne; set => villePersonne = value; }
        public string TelephonePersonne { get => telephonePersonne; set => telephonePersonne = value; }
        public string MailPersonne { get => mailPersonne; set => mailPersonne = value; }
        public DateTime DateCreationPersonne { get => dateCreationPersonne; set => dateCreationPersonne = value; }

        public string NomComplet => $"{prenomPersonne} {nomPersonne}";
    }
}
