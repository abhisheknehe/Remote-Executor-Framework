using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Ref
{
    public class IRequest
    {
       private System.Guid JobId;
       private int ReqId;
       public static transport_UI tu;
        public Guid GetGuid()
        {
            return JobId;
        }
        public void SetGuid()
        {
            this.JobId = Guid.NewGuid();
        }
        
        public int GetReqid()
        {
            return ReqId;
        }

        public void SetReqid(int x)
        {
            this.ReqId = x;
        }

    }

   public class Authentication :IRequest
      {
        string _uname,_ip,m_ver,m_os;
        Runspace m_runspace=null;
        SecureString _pass = new SecureString();
        public int result = -99;//initially -99 i.e null
        public Authentication(string ip,string uname,string pass,int type)
            {
                this._ip = ip;
                this._uname = uname;
                foreach (char c in pass)
                {
                    this._pass.AppendChar(c);
                }
                SetGuid();
                if (type == 1) // hyperV authentication also store runspace
                  {            
                    SetReqid((int)Req_type.hyperv_auth);
                  }
                else  //general VM authentication 
                {
                     SetReqid((int)Req_type.auth); 
                }
                
            }
        public string GetUname()
        {
            return _uname;
        }
        public string machine_os()
        {
            return m_os;
        }
        public string machine_version()
        {
            return m_ver;
        }
        
        public string GetIp()
        {
            return _ip;
        }
        public SecureString GetPass()
        {
            return _pass;
        }
        public Runspace GetRunspace()
        {
            return m_runspace;
        }
        public void set_runspace(Runspace r)
        {
            this.m_runspace = r;
        }
        public void set_mos(string r)
        {
            this.m_os = r;
        }
        public void set_ipname(string r)
        {
            this._ip = r;
        }
        public void set_mver(string r)
        {
            this.m_ver = r;
        }
        public void send(Authentication a)
        {
            tu = new transport_UI(a);
            tu.send_to_server();
        }
    }
   public class VM
   {
       public string vmname;
       public string vmhostname;
       public VM(string vname,string vhost){
           this.vmname = vname;
           this.vmhostname = vhost;
        }
   }  
    public class Discover :IRequest
    {
        public Runspace _runspace;
        public List<VM> result = new List<VM>();
        public Discover()
        {
           SetGuid();
           SetReqid((int)Req_type.discover);
           _runspace = General.get_runspace();
        }
        public Runspace get_runspace() 
        {
            return _runspace; 
        }
        public void send(Discover a)
        {
            tu = new transport_UI(a);
            tu.send_to_server();
        }

    }
    public class status
    {
       public string fname;
       public int _status = 0;
        public status(string x)
        {
            this.fname = x;
            this._status = 0;
        }
        public void set_status(int x)
        {
           _status=x;
        }
    }
    public class offload_req :IRequest
    {
        public string _uname, _ip,_iis_url;
        public  SecureString _pass = new SecureString();
        public List<status> _filelist=new List<status>();
        public string start_time;    
          public offload_req(string ip,List<string> flist)
            {
                this._ip = ip;
                if(ip=="windows7-PC")
                {
                    this._uname = "administrator7";
                }
                else
                {
                    if (ip == "win8") { this._uname = "administrator8"; }
                    else
                    {
                        this._uname="administrator";
                    }

                }
                this._uname = ip +"\\" +this._uname;
                string pass="passw0rd@12";
                this.start_time = DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt");  
                foreach (char c in pass)
                {
                    this._pass.AppendChar(c);
                }
                _iis_url = Ref.General.Get_IISuri();
                SetGuid();
                SetReqid((int)Req_type.offload); 
                
                foreach(string v in flist)
                {
                    status x = new status(v);
                    _filelist.Add(x);
                }
            }
        
        public string GetUname()
        {
            return _uname;
        }
        public string Getiis()
        {
            return _iis_url;
        }
        public string GetIp()
        {
            return _ip;
        }
        public SecureString GetPass()
        {
            return _pass;
        }
        public List<status> Get_filelist()
        {
            return _filelist;
        }
        public void send(offload_req a)
        {
            tu = new transport_UI(a);
            tu.send_to_server();
        }
    }

    public class Install_req : IRequest
    {
        public string _uname, _ip, _iis_url;
        public SecureString _pass = new SecureString();
        public List<status> _filelist = new List<status>();
        public string start_time;    
          public Install_req(string ip,List<string> flist)
            {
                this._ip = ip;
                if (ip == "windows7-PC")
                {
                    this._uname = "administrator7";
                }
                else
                {
                    if (ip == "win8") { this._uname = "administrator8"; }
                    else
                    {
                        this._uname = "administrator";
                    }

                }
                this._uname = ip + "\\" + this._uname;
                string pass = "passw0rd@12";
                this.start_time = DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt"); 
                foreach (char c in pass)
                {
                    this._pass.AppendChar(c);
                }
                _iis_url = Ref.General.Get_IISuri();
                SetGuid();
                SetReqid((int)Req_type.install);

                foreach (string v in flist)
                {
                    status x = new status(v);
                    _filelist.Add(x);
                }
            }
        
        public string GetUname()
        {
            return _uname;
        }
        public string GetIp()
        {
            return _ip;
        }
        public SecureString GetPass()
        {
            return _pass;
        }

        public string Getiis()
        {
            return _iis_url;
        }

        public List<status> Get_filelist()
        {
            return _filelist;
        }
        public void send(Install_req a)
        {
            tu = new transport_UI(a);
            tu.send_to_server();
        }
      
    }   
}
