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
using Xceed.Wpf.Toolkit;

using System.Windows.Forms;

using System.ComponentModel;
namespace Ref

{
   public partial class MainWindow : Window
    {
       


        public MainWindow()
        {
            InitializeComponent();

            txt_iis.Text = Properties.Settings.Default.iisuri;
            General.SetIISUri();
            hs_panel.IsEnabled = false;
        }

        
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            if (frame.Visibility == Visibility.Hidden)
            {

                txt_iis.Visibility = Visibility.Visible;
                lbl_iis.Visibility = Visibility.Visible;
                btn_iis.Visibility = Visibility.Visible;
                frame.Visibility = Visibility.Visible;

            }

            else {

                hideiis();
            }


        }

        public void hideiis(){
            txt_iis.Visibility = Visibility.Hidden;
            lbl_iis.Visibility = Visibility.Hidden;
            btn_iis.Visibility = Visibility.Hidden;
            frame.Visibility = Visibility.Hidden;
       }
        private void outeriis_Click(object sender, RoutedEventArgs e)
        {
            hideiis();
        }
        
        
        private void iis_Click(object sender, RoutedEventArgs e)
        {

            txt_iis.Visibility = Visibility.Hidden;
            lbl_iis.Visibility = Visibility.Hidden;
            btn_iis.Visibility = Visibility.Hidden;
            frame.Visibility = Visibility.Hidden;
            int r = General.check_update(txt_iis.Text);
            if(r==1)
            {
              Properties.Settings.Default.iisuri =txt_iis.Text;
              Properties.Settings.Default.Save();
              General.SetIISUri();
              General.IISfile();
              List<string> x = new List<string>();
              x = General.GetFileList();
              if (General.inv != null)
              {
                  General.inv.set_filelist_inventory(x);
              }
              if (General.off != null)
              {
                  General.off.set_filelist_off(x);
              }
              System.Windows.MessageBox.Show("IIS LINK UPDATED successfully..!!");  
              txt_iis.Text=Properties.Settings.Default.iisuri;
            }
            else
            {
                if(r==0)
                {
                     System.Windows.MessageBox.Show("EMPTY FILE SERVER..!!");  
                }
                else
                {
                     System.Windows.MessageBox.Show("Invalid file server link..!!");  
                }
                txt_iis.Text=Properties.Settings.Default.iisuri;
 
            }      
        }

        public async void  btn_connect_Click(object sender, RoutedEventArgs e)
        {
            string ipaddress = txt_ipaddress.Text;
            string username  =ipaddress +"\\" + txt_username.Text;
            string password = txt_password.Password;

        

            if (txt_ipaddress.Text.Equals(""))
            {
                
                lbl_error.Visibility = Visibility.Visible;
                lbl_error.Content = "Enter IPaddress";
                                
            }
            else if (txt_username.Text.Equals(""))
            {
                lbl_error.Visibility = Visibility.Visible;
                lbl_error.Content = "Enter Username";
            }
            else if (txt_password.Password.Equals(""))
            {
                lbl_error.Visibility = Visibility.Visible;
                lbl_error.Content = "Enter Password";
            }

            else
            {
                lbl_error.Visibility = Visibility.Hidden;

                txt_ipaddress.IsEnabled = false;
                txt_password.IsEnabled = false;
                txt_username.IsEnabled = false;
                btn_connect.IsEnabled = false;

                General.au_obj = new Ref.Authentication(ipaddress, username, password,1);
                                 
                int x=0;
                while (x < 150)
                {
                    load_image.Visibility = Visibility.Visible;
                   if(x==0)
                    {
                        General.au_obj.send(General.au_obj);
                    }
                    await Task.Delay(2000);
                    if (General.au_obj.result != -99)
                    {
                        break;
                    }                   
                    x++;
                }
                
                load_image.Visibility = Visibility.Hidden;

                if (General.au_obj.result == 0)
                 {
                     General.set_machinename(General.au_obj.GetIp());
                     General.set_machineos(General.au_obj.machine_os());
                     General.set_machinever(General.au_obj.machine_version());
                     General.set_runspace(General.au_obj.GetRunspace());
                     General.set_username(username);
                     General.set_pass(General.au_obj.GetPass());
                    
                     if(General.au_obj.GetRunspace()==null)
                     {
                         Console.WriteLine("null runspace");
                     }
                     inventory.IsEnabled = true;
                     jobs.IsEnabled = true;
                     history.IsEnabled = true;
                     offload.IsEnabled = true;
                     hs_panel.IsEnabled = true;
                     txt_ipaddress.Visibility = Visibility.Hidden;
                     txt_username.Visibility = Visibility.Hidden;
                     txt_password.Visibility = Visibility.Hidden;
                     btn_connect.Visibility = Visibility.Hidden;
                     lbl_error.Visibility = Visibility.Hidden;

                     General.ld = new Landing();
                     ParentPanel.Children.Add(General.ld);
                     hs_panel.Visibility = Visibility.Visible;
                     btn_setting.Visibility = Visibility.Visible;
                     btn_home.Visibility = Visibility.Visible;

                 }
                 else
                 {
                     if (General.au_obj.result == -99)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "Request TimeOut";
                     }

                     if (General.au_obj.result == -1)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "invalid Username Password  ";
                     }
                     if (General.au_obj.result == -2)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "Destination Unreachable";

                     }
                     if (General.au_obj.result == -12)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "It is not HyperV server";
                     }
                     if (General.au_obj.result == -3)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "Runspace alredy opened";

                     }
                     if (General.au_obj.result == -4)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "Error while opening runspace";

                     }
                     if (General.au_obj.result == -14)
                     {
                         lbl_error.Visibility = Visibility.Visible;
                         lbl_error.Content = "Error occured";
                     }
                  
                     txt_ipaddress.IsEnabled = true;
                     txt_password.IsEnabled = true;
                     txt_username.IsEnabled = true;
                     btn_connect.IsEnabled = true;
                   
                 }

            }
        }

        private void mywindow_Close(Object e, CancelEventArgs t)
        {
            Properties.Settings.Default.iisuri = txt_iis.Text;
            Properties.Settings.Default.Save();
        } 


        private void Inv_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            General.inv = new Inventory();

            ParentPanel.Children.Clear();
            ParentPanel.Children.Add(General.inv);

            vd_panel.Visibility = Visibility.Visible;


            //Discover dsc = new Discover(txt_ipaddress.Text,txt_password.ToString(),txt_username.Text);

        }

        private void VInv_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

           General.inv = new Inventory();

            ParentPanel.Children.Clear();
            ParentPanel.Children.Add(General.inv);

            vd_panel.Visibility = Visibility.Visible;


        }


        private void Voff_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            ParentPanel.Children.Clear();
            if (General.off == null)
            {
                General.off = new offload(1);
            }
            else
            {
                General.off = new offload(2);
            }

            ParentPanel.Children.Add(General.off);
            
            vd_panel.Visibility = Visibility.Visible;


        }


        private void VInprogress_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            ParentPanel.Children.Clear();
            General.in_pro = new Inprogress ();

            ParentPanel.Children.Add(General.in_pro);

            vd_panel.Visibility = Visibility.Visible;


        }



        private void Inprogress_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            ParentPanel.Children.Clear();
            General.in_pro = new Inprogress ();

            ParentPanel.Children.Add(General.in_pro);

            vd_panel.Visibility = Visibility.Visible;


        }




        private void off_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;
            ParentPanel.Children.Clear();
            
            if (General.off == null)
            {
                General.off = new offload(1);
            }
            else
            {
                General.off = new offload(2);
            }
                                    
            ParentPanel.Children.Add(General.off);
            
            vd_panel.Visibility = Visibility.Visible;

            


        }
        private void VHistory_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            ParentPanel.Children.Clear();
            General.history = new History();

            ParentPanel.Children.Add(General.history);

            vd_panel.Visibility = Visibility.Visible;


        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            hs_panel.Visibility = Visibility.Hidden;

            ParentPanel.Children.Clear();
            General.history = new History ();

            ParentPanel.Children.Add(General.history);
            vd_panel.Visibility = Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            
        }

        

        private void Hide_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            popup1.IsOpen = true;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = true;
            popup1.IsOpen = false;

        }

        private void btn_home_Click(object sender, RoutedEventArgs e)
        {
            
            vd_panel.Visibility = Visibility.Hidden;
            ParentPanel.Children.Clear();
            General.ld = new Landing();
            ParentPanel.Children.Add(General.ld);
            hs_panel.Visibility = Visibility.Visible;
        }

       
        


      
           
}


}


