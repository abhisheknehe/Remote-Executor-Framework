using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
namespace Ref
{
    
        enum Req_type
        {
            auth = 1, inv_fetch, offload, install
        };
        

        public abstract class IRequest
        {
            //reference object of transport layer...bt for now requesthandler
            
            public System.Guid JobId;

            public Guid GetGuid()
            {
                return JobId;
            }

 
            public int ReqId;

            public int GetReqid()
            {
                return ReqId;
            }

 
        }
    

        public class Discover : IRequest
        {
            public Discover(string ip, string uname, string pass)
            {
            }


        }

        public class Offload :IRequest
        {
            
            SecureString pass=new SecureString();

            public Offload(string vmname,List<string>files)
            {
 
            }            
            


            public void listCreate()
            {

                List <string> l1 = new List<string>();
                l1.Add("");

            }


            public void listcreator()
            {
                List <string> l2=new List<string>();
                l2.Add("asdfsaf");

            }

            

            
    
        }

        public class Install : IRequest
        {


            
        }


        public class Authentication :IRequest
        {

            string _uname,_ip;
            SecureString _pass = new SecureString();
            public Authentication(string ip,string uname,string pass)
            {
                this._ip = ip;
                this._uname = uname;
                foreach (char c in pass)
                {
                    this._pass.AppendChar(c);
                }
                this.JobId = Guid.NewGuid();
                this.ReqId = (int)Req_type.auth;
            }



            
            


        }
    }
