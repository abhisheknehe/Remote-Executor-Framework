using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Net;
using System.Text.RegularExpressions;

using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Remoting;
using FirebirdSql.Data.FirebirdClient;

namespace Ref
{
    public enum Req_type
    {
        hyperv_auth = 1, auth = 2, discover = 3, offload = 4, install = 5
    };

    static class requst_wait
    {
        public static List<IRequest> request_queue = new List<IRequest>();

       public static int search_ireq(IRequest o)
        {
            for(int i=0;i<request_queue.Count;i++)
            {
                if(o.GetGuid().Equals(request_queue[i].GetGuid()))
                {
                    return i;
                }
            }
            return -1;
        }
      public static void add_to_database(string hypname,string vmname,string fname,string action,string time,string status)
       {           
           try
           {
               string ConnectionString = "user=SYSDBA;password=masterkey;" +
               "Database=Server12:C:\\Users\\Public\\DB\\REFDB.FDB; " +
               "DataSource=Server12;Charset=NONE;";

               FbConnection addDetailsConnection = new FbConnection(ConnectionString);
               addDetailsConnection.Open();

               FbTransaction addDetailsTransaction = addDetailsConnection.BeginTransaction();
               string SQLCommandText = "INSERT into History Values('"+hypname+"','"+vmname+"','"+fname+"','"+action+"','"+time+"','"+status+"');";

               FbCommand addDetailsCommand = new FbCommand(SQLCommandText,addDetailsConnection, addDetailsTransaction);
               addDetailsCommand.ExecuteNonQuery();
               addDetailsTransaction.Commit();
           }
           catch (Exception )
           {
 
           }
       }
      public static void remove_from_request_que(IRequest o)
        {
            int k = search_ireq(o);
            string hname = General.Get_MachineName();
             if(k!=-1)
             {
                 request_queue.RemoveAt(k);
                 if(o.GetReqid()==5) 
                 {
                     Install_req obj = (Install_req)o;
                     string vmname = obj.GetIp();
                     string status = "No Status";
                     foreach (var fi in obj._filelist)
                     {
                         if (fi._status == -1) { status = "wrong VM credentials"; }
                         if (fi._status == -2) { status = "VM unreachable"; }
                         if (fi._status == -3) { status = "VM-error while openinng runspace"; }
                         if (fi._status == -4) { status = "VM-error Runspace not opened"; }
                         if (fi._status == 1) { status = "Successfull"; }
                         if (fi._status == 2) { status = "Failed"; }
                         if (fi._status == 0) { status = "error scheduling job"; }
                         add_to_database(hname, vmname, fi.fname, "Install", obj.start_time, status);
                     }
                     General.updateUI();                     
                 }
                 if(o.GetReqid()==4)
                 {
                     offload_req obj = (offload_req)o;
                     string vmname = obj.GetIp();
                     string status="No Status";
                     foreach (var fi in obj._filelist)
                     {
                         if (fi._status == -1) { status = "wrong VM credentials"; }
                         if (fi._status == -2) { status = "VM unreachable"; }
                         if (fi._status == -3) { status = "VM-error while openinng runspace";}
                         if (fi._status == -4) { status = "VM-error Runspace not opened"; }
                         if (fi._status == 1) { status = "Successfull";}
                         if (fi._status == 0) { status = "Failed"; }
                         add_to_database(hname, vmname, fi.fname, "offload", obj.start_time, status);
                     }
                     General.updateUI();
                 }
             }
        }
    }

    static class General
    {
        
        private static Runspace _hyperv_runspace;
        private static string _iisurl;
        private static string _username;
        private static string _machinename, _machine_os, _machine_version;
        private static SecureString _password = new SecureString();
        private static List<string> _filelist = new List<string>();
        public static List<string> _vmlist = new List<string>();
        public static List<VM> _vminfo = new List<VM>();
        private static int _filecount, _vmcount;

        public static int Get_fcnt() { return _filecount; }
        public static int Get_vmcnt() { return _vmcount; }
        public static string Get_machineos() { return _machine_os; }
        public static string Get_machinever() { return _machine_version; }
        public static string Get_MachineName() { return _machinename; }
        public static List<string> GetFileList() { return _filelist; }
        public static List<string> GetVMList() { return _vmlist; }
        public static SecureString GetPassword() { return _password; }
        public static string Get_IISuri() { return _iisurl; }

        public static Runspace get_runspace() { return _hyperv_runspace; }

        public static void set_runspace(Runspace x) { _hyperv_runspace = x; }
        public static Authentication au_obj;
        public static Landing ld;
        public static Inventory inv=null;
        public static offload off=null;
        public static Inprogress in_pro;
        public static History history;

        public static void close_runspace()
        {
            try
            {
                _hyperv_runspace.Close();
                _hyperv_runspace.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("error closing hyperV runspace" + e.ToString());
            }
        }
        public static void set_AUobj(Authentication x)
        {
            au_obj = x;
        }
        public static void set_discoverobj(Discover x)
        {
            set_vminfolist(x.result);
            set_Mcnt(x.result.Count);
            List<string> temp = new List<string>();
             foreach(VM v in x.result)
             {
                 temp.Add(v.vmhostname);
             }
             set_vmlist(temp);         
            if (General.inv != null)
            {                           
                inv.set_vmlist_inventory(temp);             
            }
            if (General.off != null)
            {
                off.set_vmlist_off(temp);
            }
                     
        }
        public static void updateUI()
        {
            if (General.in_pro != null)
            {
                General.in_pro.Inprogress_update();
            }
            if(General.history !=null)
            {
                General.history.update(); 
            }          
        }
        
        public static Authentication get_AUobj()
        {
            return au_obj;
        }
        
        public static void SetIISUri()
        {
            _iisurl = Properties.Settings.Default.iisuri;
        }

        public static void set_machineos(string s) { _machine_os = s; }
        public static void set_machinever(string s) { _machine_version = s; }
        public static void set_pass(SecureString s)
        {
            _password = s;
        }
        public static void set_username(string s)
        {
            _username = s;
        }
        public static void set_machinename(string s)
        {
            _machinename = s;
        }
        public static void set_fcnt(int s)
        {
            _filecount = s;
        }
        public static void set_Mcnt(int s)
        {
            _vmcount = s;
        }
        public static void set_vminfolist(List<VM> s)
        {
            _vminfo = s;
        }
        public static void set_vmlist(List<string>s)
        {
            _vmlist = s;
        }
        public static void set_filelist(List<string> s)
        {
            _filelist = s;
        }

        public static void IISfile() // for getting list of files from iis server
        {
            List<String> filelist = new List<String>();
            try
            {
                WebClient client = new WebClient();
                string html = client.DownloadString(Get_IISuri());

                int cnt = -1;
                string[] split1 = new string[] { "\r\n" };

                html = html.Remove(html.IndexOf("<head>"), html.IndexOf("</head>") + 1);
                html = html.Remove(html.IndexOf("<H1>"), html.IndexOf("</H1>") + 1);
                html = html.Replace("<br>", "\r\n ");
                html = Regex.Replace(html, "<style>(.|\n)*?</style>", string.Empty);
                html = Regex.Replace(html, @"<xml>(.|\n)*?</xml>", string.Empty);
                html = Regex.Replace(html, @"<(.|\n)*?>", string.Empty);

                string[] lines = html.Split(split1, StringSplitOptions.None);

                foreach (string s in lines)
                {
                    cnt++;
                    if ((cnt == 0) || (cnt == 1) || (s.CompareTo(String.Empty) == 0) || (s.CompareTo(" ") == 0)||(!(s.Contains("."))))
                    {
                        continue;
                    }
                    string l = s.Trim();
                    filelist.Add(l);                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error connecting IIS LINK" + e.ToString());
            }
            finally
            {
                set_fcnt(filelist.Count);
                set_filelist(filelist);
            }
        }
        
        public static int check_update(string ab)
        {//return 1= sucsess 0: empty file server -1: error occured
            List<String> filelist = new List<String>();
            try
            {
                WebClient client = new WebClient();
                string html = client.DownloadString(ab);

                int cnt = -1;
                string[] split1 = new string[] { "\r\n" };

                html = html.Remove(html.IndexOf("<head>"), html.IndexOf("</head>") + 1);
                html = html.Remove(html.IndexOf("<H1>"), html.IndexOf("</H1>") + 1);
                html = html.Replace("<br>", "\r\n ");
                html = Regex.Replace(html, "<style>(.|\n)*?</style>", string.Empty);
                html = Regex.Replace(html, @"<xml>(.|\n)*?</xml>", string.Empty);
                html = Regex.Replace(html, @"<(.|\n)*?>", string.Empty);

                string[] lines = html.Split(split1, StringSplitOptions.None);

                foreach (string s in lines)
                {
                    cnt++;
                    if ((cnt == 0) || (cnt == 1) || (s.CompareTo(String.Empty) == 0) || (s.CompareTo(" ") == 0) || (!(s.Contains("."))))
                    {
                        continue;
                    }
                    string l = s.Trim();
                    filelist.Add(l);
                    break;
                }
                if (filelist.Count > 0)
                {
                    return 1;
                }
                else
                {
                    if(filelist.Count==0)
                      return 0;
                    return -2;
                }
            }
            catch (Exception e)
            {
                return -1;
            }

        }

    }
}



      

   
