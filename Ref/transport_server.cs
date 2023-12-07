using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    class transport_server
    {
        IRequest obj;
        static request_handler rh; 
        public transport_server(IRequest o)
        {
            obj = o;
        }
       
        public void send_to_UI()//not used
        {

        }
        
        public void send_to_requesthandler()
        {
           rh = new request_handler(obj);
        }
    }
}
