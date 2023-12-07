using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit;
using System.Runtime.CompilerServices;
using System.ComponentModel;



namespace Ref
{
    /// <summary>
    /// Interaction logic for offload.xaml
    /// </summary>
    /// 

    public partial class offload : UserControl
    {
        public static List<string> vml = new List<string>() ;
        List<String> file_list = new List<String>();
        public List<CheckVM> temp1 = new List<CheckVM>();

        public void set_filelist_off(List<string> x)
        {
            
            if (file_list.SequenceEqual(x)) { }
            else
            {
                file_list = x;
                Dispatcher.Invoke(() =>
                {
                    listview1.ItemsSource = file_list;
                    if (listview1.Items.Count == 0)
                    {
                        add_btn.IsEnabled = false;
                    }
                    else
                    {
                        add_btn.IsEnabled = true;
                    }
                });

                if (listbox2.Items.Count > 0)
                {
                    System.Windows.MessageBox.Show("file list is changed...try again");
                }
                
                Dispatcher.Invoke(() =>
                {
                    listbox2.Items.Clear();
                    listbox3.Items.Clear();
                });
            }
        }


        public void set_vmlist_off(List<string> x)
        {
            if(vml.SequenceEqual(x)){}
            else{
            vml = x;
            if (listbox2.Items.Count > 0)
            {
                System.Windows.MessageBox.Show("Virtual machine list is changed...try again");
            }

                Dispatcher.Invoke(() =>
                {
                    listbox2.Items.Clear();
                    listbox3.Items.Clear();
                       if(vml.Count==0)
                       {
                           add_btn.IsEnabled = false;
                           listbox2.IsEnabled = false;
                           listbox3.IsEnabled = false;
                           btn_confirm.IsEnabled = false;
                       }
                       else{
                           add_btn.IsEnabled = true;
                           listbox2.IsEnabled = true;
                           listbox3.IsEnabled = true;
                           btn_confirm.IsEnabled = true;
                       }
                });
           
            }
        }
        
        public offload(int p)
        {
            InitializeComponent();
            General.IISfile();
            file_list = General.GetFileList();
            

            listview1.ItemsSource=file_list;
            if(listview1.Items.Count==0)
            {
                add_btn.IsEnabled = false;
            }
            else
            {
                add_btn.IsEnabled = true;
            }

            listview1.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            remove_btn.IsEnabled = false;
            if (p == 1)
            {
                Discover d = new Discover();
                d.send(d);
            }
            vml = General.GetVMList();

            if (vml.Count == 0)
            {
                add_btn.IsEnabled = false;
                listbox2.IsEnabled = false;
                listbox3.IsEnabled = false;
                btn_confirm.IsEnabled = false;
            }
            else
            {
                add_btn.IsEnabled = true;
                listbox2.IsEnabled = true;
                listbox3.IsEnabled = true;
                btn_confirm.IsEnabled = true;
            }
            
            
            ICollectionView view = CollectionViewSource.GetDefaultView(file_list);
            Ref.ListView_Search.TextSearchFilter(view, file_search);



        }
        public class Submit
        {
            public string vname { get; set; }
            public string fname { get; set; }
            public string action { get; set; }
            
            public Submit(string a,string b,string c)
            {
                this.vname=a;
                this.fname=b;
                this.action=c;
            }
        }

        public class User
        {
            public string filename { get; set; }
            public List<CheckVM> temp = new List<CheckVM>();
            public string Filename
            {
                get { return filename; }
                set { this.filename = value; }
            }

            public User(string fname)
            {
                filename = fname;
                for (int i = 0; i < vml.Count; i++)
                {
                    temp.Add(new CheckVM(vml[i]));
                }
            }
        }

        public class CheckVM : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string vmname;
            public string VmName
            {
                get
                {
                    return this.vmname;
                }

                set
                {
                    if (value != this.vmname)
                    {
                        this.vmname = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public bool ischecked0;

            public bool IsCheck
            {
                get
                {
                    return this.ischecked0;
                }

                set
                {
                    if (value != this.ischecked0)
                    {
                        this.ischecked0 = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public bool ischecked1;

            public string Offload;
            public string Install;

            public bool IsCheck1
            {
                get
                {
                    return this.ischecked1;
                }

                set
                {
                    if (value != this.ischecked1)
                    {
                        this.ischecked1 = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            public CheckVM()
            {
                this.ischecked0 = false;
                this.ischecked1 = false;
            }
            public CheckVM(string name)
            {
                this.vmname = name;
                this.ischecked0 = false;
                this.ischecked1 = false;
            }
            public CheckVM(string name, bool val0, bool val1)
            {
                this.vmname = name;
                this.ischecked0 = val0;
                this.ischecked1 = val1;


            }

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }
        
        public void show1(User u)
        {

            listbox3.Items.Clear();
            foreach (CheckVM v in u.temp)
            {
                listbox3.Items.Add(v);

            }
            listbox3.Items.SortDescriptions.Add(new SortDescription("vmname", ListSortDirection.Ascending));

        }


        private void plus_popup(object sender, SelectionChangedEventArgs args)
        {
            if (listbox2.Items.Count >= 1)
            {
                User x = ((sender as ListBox).SelectedItem as User);
                show1(x);

                if (listbox3.Visibility.Equals(Visibility.Hidden))
                {
                    listbox3.Visibility = Visibility.Visible;
                }
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            foreach (string item in listview1.SelectedItems)
            {
                User u = new User(item);
                listbox2.Items.Add(u);
            }

            listview1.SelectedItems.Clear();
            foreach (User item2 in listbox2.Items)
            {
                file_list.Remove(item2.filename);
            }


            if (listbox2.Items.Count > 0)
                remove_btn.IsEnabled = true;

            listbox2.Items.SortDescriptions.Add(new SortDescription("filename", ListSortDirection.Ascending));
            listview1.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            User u = (User)listbox2.SelectedItem;
            if (listbox2.Items.Count == 1)
            {

                file_list.Add(u.filename.ToString());
                listbox2.Items.Remove(u);
                listbox3.Visibility = Visibility.Hidden;

                list3.Visibility = Visibility.Hidden;
                remove_btn.IsEnabled = false;

            }
            else
            {
                if (listbox2.SelectedIndex == 0)
                {
                    listbox2.SelectedItem = listbox2.Items[1];
                    listbox2.Focus();

                }
                else
                {
                    listbox2.SelectedItem = listbox2.Items[0];
                    listbox2.Focus();

                }
                file_list.Add(u.filename.ToString());
                listbox2.Items.Remove(u);

                listbox2.Items.SortDescriptions.Add(new SortDescription("filename", ListSortDirection.Ascending));
                listview1.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }

        }

        public static List<Submit> mlist;
        public static List<string>[] off;
        public static List<string>[] install;
        public static Submit _submit;
        public void req_separator()
        {
            mlist = new List<Submit>();
            off = new List<string>[vml.Count];
            install = new List<string>[vml.Count];
            for (int i = 0; i < vml.Count; i++)
            {
                off[i] = new List<string>();
                install[i]=new List<string>();
            }
            List<User> user = new List<User>();
            
            foreach (User x in listbox2.Items)
            {
                user.Add(x);
            }

            for (int i = 0; i < user.Count; i++)
            {
                for (int j = 0; j < vml.Count; j++)
                {
                    if (user[i].temp[j].IsCheck)
                    {
                        off[j].Add(user[i].filename);
                    }
                    if(user[i].temp[j].IsCheck1)
                    {
                        if ((user[i].filename.Contains(".exe")) || (user[i].filename.Contains(".msu")) || (user[i].filename.Contains(".bat")) || (user[i].filename.Contains(".bin")) || (user[i].filename.Contains(".cmd")) || (user[i].filename.Contains(".com")) || (user[i].filename.Contains(".cpl")) || (user[i].filename.Contains(".gadget")) || (user[i].filename.Contains(".inf")) || (user[i].filename.Contains(".ins")) || (user[i].filename.Contains(".inx")) || (user[i].filename.Contains(".isu")) || (user[i].filename.Contains(".job")) || (user[i].filename.Contains(".jse")) || (user[i].filename.Contains(".msc")) || (user[i].filename.Contains(".msi")) || (user[i].filename.Contains(".msp")) || (user[i].filename.Contains(".mst")) || (user[i].filename.Contains(".paf")) || (user[i].filename.Contains(".pif")) || (user[i].filename.Contains(".wsf")) || (user[i].filename.Contains(".ws")) || (user[i].filename.Contains(".vb")) || (user[i].filename.Contains(".vbe")) || (user[i].filename.Contains(".vbs")) || (user[i].filename.Contains(".vbscript")) || (user[i].filename.Contains(".sct")) || (user[i].filename.Contains(".shs")) || (user[i].filename.Contains(".ps1")) || (user[i].filename.Contains(".reg")) || (user[i].filename.Contains(".rgs")) || (user[i].filename.Contains(".u3p")))
                        {
                            install[j].Add(user[i].filename);
                        }
                    }
                }
            }

            for (int i = 0; i < vml.Count; i++)
            {
              for (int j = 0; j < off[i].Count; j++)
               {
                   _submit = new Submit(vml[i], off[i][j], "Offload");
                   mlist.Add(_submit);
               }
               for(int j=0;j<install[i].Count;j++)
               {
                   _submit = new Submit(vml[i], install[i][j], "Execute"); 
                   mlist.Add(_submit);
               }
            }
            datagrid1.ItemsSource = mlist;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            if (listbox2.Items.Count > 0)
            {
                btn_confirm.IsEnabled = false;
                req_separator();
                this.IsHitTestVisible = false;     
                popup1.IsOpen = true;
            }           
        }

        public void init_offload()
        {
            listbox3.Items.Clear();
            listbox3.Visibility = Visibility.Hidden;
            list3.Visibility = Visibility.Hidden;
            remove_btn.IsEnabled = false;
           foreach (var x in listbox2.Items)
           {
               User u = (User)x;
               file_list.Add(u.filename.ToString());       
           }
           listbox2.Items.Clear();
           listview1.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        public void req_sender()
        {
            int cnt = vml.Count;
            cnt--;
            for (int i = 0; i < vml.Count; i++)
            {
                if (off[i].Count > 0)
                {
                    offload_req o = new offload_req(General._vminfo[i].vmhostname, off[i]);
                    o.send(o);
                }
                if (install[cnt - i].Count > 0)
                {
                    Install_req o1=new Install_req(General._vminfo[cnt-i].vmhostname,install[cnt-i]);
                    o1.send(o1);
                }
            } 
        }

        private void popup_submit_Click(object sender, RoutedEventArgs e)
        {
            req_sender();
            init_offload();
            btn_confirm.IsEnabled = true;
            this.IsHitTestVisible = true;
            popup1.IsOpen = false;             
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
           
            popup1.IsOpen = false;
            btn_confirm.IsEnabled = true;
        }

        private void popup_cancel_Click(object sender, RoutedEventArgs e)
        {
            popup1.IsOpen = false;
            btn_confirm.IsEnabled = true;
            this.IsHitTestVisible = true; 
        }

        private static T GetVisualAncestor<T>(DependencyObject o) where T : DependencyObject
        {
            do
            {
                o = VisualTreeHelper.GetParent(o);
            } while (o != null && !typeof(T).IsAssignableFrom(o.GetType()));

            return (T)o;
        }
        private void ins_chek_select(object sender, RoutedEventArgs e)
        {
            ListBoxItem listViewItem = GetVisualAncestor<ListBoxItem>((DependencyObject)sender);

            listbox3.SelectedValue = listbox3.ItemContainerGenerator.ItemFromContainer(listViewItem);
            User u = (User)listbox2.SelectedItem;
            int i = (int)listbox3.SelectedIndex;
            if (u.temp[i].ischecked1 == true)
            {
                if (u.temp[i].ischecked0 == true)
                {
                    u.temp[i].ischecked0 = false;
                }
            }
            show1(u);
        }


        private void off_chek_select(object sender, RoutedEventArgs e)
        {
            ListBoxItem listViewItem = GetVisualAncestor<ListBoxItem>((DependencyObject)sender);

            listbox3.SelectedValue = listbox3.ItemContainerGenerator.ItemFromContainer(listViewItem);

            User u = (User)listbox2.SelectedItem;

            int i = (int)listbox3.SelectedIndex;
            if (u.temp[i].ischecked0 == true)
            {
                if (u.temp[i].ischecked1 == true)
                {
                    u.temp[i].ischecked1 = false;
                }
            }
            show1(u);
        }

    }
    
}
