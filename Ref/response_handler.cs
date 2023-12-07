using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    class response_handler
    {
          static Object locker;
          static transport_UI tu=new transport_UI();
        public response_handler( IRequest obj)
        {
            locker = new Object();
            lock(locker)
            {
                 tu.received_from_server(obj);
            }
        }
    }
}
