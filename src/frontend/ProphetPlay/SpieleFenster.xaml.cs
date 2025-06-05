using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProphetPlay
{
    /// <summary>
    /// Interaction logic for SpieleFenster.xaml
    /// </summary>
    public partial class SpieleFenster : Window
    {
        private LeaguesArticle _league;
        public SpieleFenster(LeaguesArticle league)
        {
            InitializeComponent();
            _league = league;
            // Nutze _league.LeagueName etc.
        }

    }
}
