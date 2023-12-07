using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Threading;
using System.Management.Automation.Runspaces;
using System.Security;

namespace Ref
{

    class Thread_pool
    {
       static powershell_help ph =new powershell_help();
       static response_handler re_hand;
       static private Queue<IRequest> Thread_queue = new Queue<IRequest>();

       public void process_object(Object o)
       {
           IRequest obj = (IRequest)o;
           int reqid=obj.GetReqid();
        
           switch(reqid)
           {
               case 1:
                   {
                       Authentication ob = (Authentication)obj;
                       ob=ph.hyperv_authenticate(ob);
                       IRequest x = (IRequest)ob;
                       re_hand = new response_handler(x);
                   }
                   break;
               case 2:
                   {
                       
                   }
                   break;
               case 3:
                   {
                       Discover ob = (Discover)obj;
                       ob = ph.DiscoverVM(ob);
                       IRequest x = (IRequest)ob;
                       re_hand = new response_handler(x);
                   }
                   break;
               case 4:
                   {
                       offload_req ob = (offload_req)obj;
                       ob = ph.proc_offload(ob);
                       IRequest x = (IRequest)ob;
                       re_hand = new response_handler(x);

                   }
                   break;
               case 5:
                   {
                       Install_req ob = (Install_req)obj;
                       ob = ph.proc_install(ob);
                       IRequest x = (IRequest)ob;
                       re_hand = new response_handler(x);
                   }
                   break;
           }
       }
        public void add_threadque(IRequest obj)
        {
            Thread_queue.Enqueue(obj);
            ThreadProcManager();
            
        }
        public void addtask_to_thread(IRequest obj)
        {
             ThreadPool.QueueUserWorkItem(process_object,obj);
                
        }
        public void initial_task_add( Authentication obj)
        {
           // ThreadPool.SetMaxThreads(50, 50);
           // ThreadPool.SetMinThreads(10, 10);

            process_object( obj);
        }

        public void ThreadProcManager()
        {
                       
                lock (Thread_queue)
                {
                    if (Thread_queue.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(process_object, Thread_queue.Dequeue());
                    }
                }
         }
       
    }
}
