using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
   public class transport_UI
    {
       IRequest obj;
       static transport_server ts;
       public transport_UI(IRequest o)
       {
           obj = o;
       }
       public transport_UI()
       { }
       
        public void send_to_server()
        {
            if ((obj.GetReqid() == (int)Req_type.offload) || (obj.GetReqid() == (int)Req_type.install))
            {
                Ref.requst_wait.request_queue.Add(obj);
            }
            ts = new transport_server(obj);
            ts.send_to_requesthandler();
        }
        public void received_from_server(IRequest obj)

        {     
              if(obj.GetReqid()==1)
                    {
                        Authentication o = (Authentication)obj;
                        General.set_AUobj(o);
                    }
              else{
                  if(obj.GetReqid()==3)
                  {
                      Discover o = (Discover)obj;
                      General.set_discoverobj(o);
                  }
              else{
                    Ref.requst_wait.remove_from_request_que(obj);
                  }
              }              
        }               
    }
}
