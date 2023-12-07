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
    /// Interaction logic for Inprogress.xaml
    /// </summary>
    public partial class Inprogress : UserControl
    {
        public static List<Progress> temp1 = new List<Progress>();
        public void prog(IRequest obj)
        {
            string hypname = General.Get_MachineName();
            if(obj.GetReqid()==4)
            {
                offload_req o = (offload_req)obj;
                foreach (var fi in o._filelist)
                {
                    Progress p = new Progress { hname = hypname, vname = o._ip, fname = fi.fname, action = "Offloading", time =o.start_time };
                    temp1.Add(p);
                }
            }
            if(obj.GetReqid()==5)
            {
                Install_req o = (Install_req)obj;
                foreach (var fi in o._filelist)
                {
                    Progress p = new Progress { hname = hypname, vname = o._ip, fname = fi.fname, action = "Installing", time = o.start_time };
                    temp1.Add(p);
                }
            }            
        }
   
        public Inprogress()
        {
            InitializeComponent();
            temp1.Clear();
            for (int i = 0; i < Ref.requst_wait.request_queue.Count; i++)
            {
                prog(Ref.requst_wait.request_queue[i]);
            }
            datagrid1.ItemsSource = temp1;
        }

        public void Inprogress_update()
        {
            temp1 = new List<Progress>();
                  
            for (int i = 0; i < Ref.requst_wait.request_queue.Count; i++)
            {
                prog(Ref.requst_wait.request_queue[i]);
            }
            Dispatcher.Invoke(() =>
            {
                datagrid1.ItemsSource = temp1;
            });          
        }
    }

    public class Progress
    {
        public string hname { get; set; }
        public string vname { get; set; }
        public string fname { get; set; }
        public string action { get; set; }
        public string time { get; set; }
    }
}
