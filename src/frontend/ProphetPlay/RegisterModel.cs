using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    /// <summary>
    /// Modell zur Registrierung eines neuen Benutzers
    /// Wird z.B. im API-Request beim Erstellen eines Accounts verwendet
    /// </summary>
    public class RegisterModel
    {
        public string benutzername { get; set; }
        public string passwort { get; set; }

        /// <summary>
        /// Die RollenID des Benutzers 1 = normaler Benutzer und 2 = Admin
        /// </summary>
        public int role_id { get; set; }
    }
}
