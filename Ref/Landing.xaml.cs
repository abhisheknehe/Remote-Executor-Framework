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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ref
{
    /// <summary>
    /// Interaction logic for Landing.xaml
    /// </summary>
    public partial class Landing : UserControl
    {
        public Landing()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Landing_loaded);
        }


        public void Landing_loaded(object sender, RoutedEventArgs e)
        {
            hname.Text = "HyperV Name: " + General.Get_MachineName();
            hinfo.Text ="Machine OS: "+ General.Get_machineos();
            hver.Text = "OS Version: "+ General.Get_machinever();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }


    }
}
