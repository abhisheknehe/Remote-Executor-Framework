using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
using FirebirdSql.Data.FirebirdClient;

namespace Ref
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl
    {
        private FbConnection conn = null;
        public static FbDataAdapter da = null;
        public static DataSet ds = new DataSet();
        public static List<History_Display> his = new List<History_Display>();
       
        string ConnectionString = "user=SYSDBA;password=masterkey;" +
              "Database=Server12:C:\\Users\\Public\\DB\\REFDB.FDB; " +
              "DataSource=Server12;Charset=NONE;";
        public void update()
        {
            his = new List<History_Display>();
            conn = new FbConnection(ConnectionString);
            conn.Open();
            string stm = "select * from history order by start_time desc";
            da = new FbDataAdapter(stm, conn);
            da.Fill(ds, "history");
            for (int i = 0; i < ds.Tables[0].Rows.Count - 1; i++)
            {
                History_Display hd = new History_Display { hname = ds.Tables[0].Rows[i].ItemArray[0].ToString(), vname = ds.Tables[0].Rows[i].ItemArray[1].ToString(), fname = ds.Tables[0].Rows[i].ItemArray[2].ToString(), action = ds.Tables[0].Rows[i].ItemArray[3].ToString(), time = ds.Tables[0].Rows[i].ItemArray[4].ToString(), status = ds.Tables[0].Rows[i].ItemArray[5].ToString()};
                his.Add(hd);
            }

            Dispatcher.Invoke(() =>
            {
                datagrid1.ItemsSource = his;
            });  

            if (conn != null)
            {
                conn.Close();
            }
        }

        public History()
        {
            InitializeComponent();
            his = new List<History_Display>();
            conn = new FbConnection(ConnectionString);
            conn.Open();
            string stm = "select * from history order by start_time desc";
            da = new FbDataAdapter(stm, conn);
            da.Fill(ds,"history");
            for (int i = 0; i < ds.Tables[0].Rows.Count-1;i++)
            {
                History_Display hd = new History_Display { hname = ds.Tables[0].Rows[i].ItemArray[0].ToString(), vname = ds.Tables[0].Rows[i].ItemArray[1].ToString(), fname = ds.Tables[0].Rows[i].ItemArray[2].ToString(), action = ds.Tables[0].Rows[i].ItemArray[3].ToString(), time = ds.Tables[0].Rows[i].ItemArray[4].ToString(), status = ds.Tables[0].Rows[i].ItemArray[5].ToString() };
                his.Add(hd);
            }

            datagrid1.ItemsSource=his;

                if (conn != null)
                {
                    conn.Close();
                }
          
        }

        public class History_Display
        {
            public string hname { get; set; }
            public string vname { get; set; }
            public string fname { get; set; }
            public string action { get; set; }
            public string time { get; set; }
            public string status { get; set; }
        }

        

    }
}
