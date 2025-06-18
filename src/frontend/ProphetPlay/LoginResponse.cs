using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{

    /// <summary>
    /// Stellt die Antwort nach dem Login dar.
    /// </summary>
    public class LoginResponse
    {
        public string benutzername { get; set; }

        /// <summary>
        /// Die Rolle des Benutzers admin und user
        /// </summary>
        public string rolle { get; set; }

    }
}