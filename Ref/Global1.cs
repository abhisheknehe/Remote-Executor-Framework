using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Ref
{
    class Global1
    {
        
      public  List<Object> Record = new List<Object>();

            

        private static string uname;

        public string GetUname()
        {
            return uname;
        }

        public void SetUname(string value)
        {
            uname = value;
        }

        private static string ipaddress;

        public string GetIp()
        {
            return ipaddress;
        }

        public void SetIp(string value)
        {
            ipaddress = value;
        }

        private static SecureString password;

        
        public SecureString GetPass()
        {
            return password;
        }

        public void SetPass(string value)
        {
            int i;
            for (i = 0; i != value.Length; i++)
                password.AppendChar(value[i]);
        }


        private static string IisUrl;

        public string GetIisUrl()
        {
            return IisUrl;
        }

        public void SetIisUrl(string value)
        {
            IisUrl = value;
        }


        public static List<string> vmlist;

        public void setVmList(List<string> mylist)
        {
            foreach (string s in mylist)
            {
                vmlist.Add(s);
            }
        }


        public List<string> getVmList()
        {
            return vmlist;

        }


        public static List<string> filelist;

        public void setFileList(List<string> mylist)
        {
            foreach (string s in mylist)
            {
                filelist.Add(s);
            }
        }


        public List<string> getFileList()
        {
            return filelist;

        }
    }

    public static class TempDetails
    {


        private static string username;
        private static string MachineName;
        private static SecureString password = new SecureString();
        private static List<string> filelist = new List<string>();
        private static List<string> vmlist = new List<string>();
        private static int filecount,vmcount;

        public static SecureString GetPassword { get { return password; } }
        public static string GetUsername { get { return username; } }
        public static string GetMachineName { get { return MachineName; } }
        public static List<string> GetFileList { get { return filelist; } }
        public static List<string> GetVMList { get { return vmlist; } }
    }
}


    