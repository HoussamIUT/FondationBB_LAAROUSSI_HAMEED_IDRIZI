using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FondationBB
{
    class Session
    {
        public static Employe? EmployeConnecte { get; set; }
        public static bool EstResponsable =>
            EmployeConnecte != null &&
            EmployeConnecte.RoleEmploye.Trim().Equals("Responsable", System.StringComparison.OrdinalIgnoreCase);
    }
}
