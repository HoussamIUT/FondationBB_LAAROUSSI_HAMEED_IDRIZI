using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    public class Employe
    {
        private int idEmploye;
        private string nomEmploye;
        private string prenomEmploye;
        private string loginEmploye;
        private string roleEmploye;

        public int IdEmploye { get => idEmploye; set => idEmploye = value; }
        public string NomEmploye { get => nomEmploye; set => nomEmploye = value; }
        public string PrenomEmploye { get => prenomEmploye; set => prenomEmploye = value; }
        public string LoginEmploye { get => loginEmploye; set => loginEmploye = value; }
        public string RoleEmploye { get => roleEmploye; set => roleEmploye = value; }
    }
}
