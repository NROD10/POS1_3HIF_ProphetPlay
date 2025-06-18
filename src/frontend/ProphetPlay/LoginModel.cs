using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    /// <summary>
    /// Stellt die Login-Daten eines Benutzers dar und wird verwendet um sich anzumelden
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Der Benutzername mit dem sich der Benutzer anmeldet
        /// </summary>
        public string benutzername { get; set; }

        /// <summary>
        /// Das zugehörige Passwort des Benutzers
        /// </summary>
        public string passwort { get; set; }
    }

}