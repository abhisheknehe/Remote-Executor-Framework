using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ref
{
    class request_handler
    {
        IRequest obj;
        static Thread_pool th=new Thread_pool();
        public request_handler(IRequest o)
        {       
            obj = o;
            process_req();
        }
      
      
      
        public void process_req()
        {
            int type=obj.GetReqid();
            if(type==1||type==2||type==3)
            {
              if(type==1)
              {
                   Authentication o = (Authentication)obj;
                  th.initial_task_add( o);
              }
              else
              {
                  if(type==2)
                  {
         
                  }
                  else
                  {
                      Discover o = (Discover)obj;
                      th.addtask_to_thread(o);
                  }

              }

            }
            else
            {
                th.add_threadque(obj);
            }
        }

    }
}
