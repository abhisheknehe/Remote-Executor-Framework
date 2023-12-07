using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Remoting;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Ref
{
   class powershell_help
    {
        public  Runspace create_Runspace(ref int result,string remotecomputer,string username,SecureString password )
       //value of result 1= successful; -1= username pass do not match ;-2 = destination unreachable 
       // ;-3 = runspace already open ; -4 = Error occured while creating runspace
        {
            string ShellUri = @"http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            PSCredential cred = new PSCredential(username, password);
            String connectTo = String.Format("http://{0}:5985/wsman", remotecomputer);
            WSManConnectionInfo connection = new WSManConnectionInfo(new Uri(connectTo), ShellUri, cred);
            Runspace runspace=null;   
            try
                {
                    runspace = RunspaceFactory.CreateRunspace(connection);
                    runspace.Open();
                    result = 1;
                }
                catch (System.Management.Automation.Remoting.PSRemotingTransportException Exception1)
                {
                    ErrorRecord ErRecord = Exception1.ErrorRecord;
                    string fa = ErRecord.CategoryInfo.Category.ToString();
                    if (Exception1.ErrorCode == 5)
                    {
                        //    Console.WriteLine("Access denied,Username or password do not match");
                        result = -1;
                    }
                    else
                    {
                        if (Exception1.ErrorCode == -2144108103)
                        {
                            //Console.WriteLine("Destination Unreachable ");
                            result= -2;
                        }
                        result= -4;
                    }
                }
                catch (System.Management.Automation.Runspaces.InvalidRunspaceStateException Remex)
                {
                    //Console.WriteLine("Current Runspace State-" + Remex.CurrentState);
                    result= -3;
                }

             

               return runspace;
        }
        public Collection<PSObject> RunScript_onhyperv(String Command,Runspace rs)
        {
            Collection<PSObject> results = null;
            
                using (var powershell = PowerShell.Create())
                {
                    powershell.Runspace = rs;
                    powershell.AddScript(Command);
                    results = powershell.Invoke();
                }
            
            return results;
        }
        public Collection<PSObject> RunCommand_onhyperv(String Command,Runspace rs)
        {
            Collection<PSObject> results = null;
            using (var powershell = PowerShell.Create())
            {
                powershell.Runspace = rs;
                powershell.AddCommand(Command);
                results = powershell.Invoke();
                return results;
            }
        }

       public Authentication hyperv_authenticate(Authentication obj) //value of result 0 = successful;-12=HyperV not present;-14 =error
                          //value of result -1= username pass do not match ;-2 = destination unreachable 
                          // ;-3 = runspace already open ; -4 = Error occured while creating runspace

        {
            int k=-99;
            Runspace rs = create_Runspace( ref k,obj.GetIp(),obj.GetUname(),obj.GetPass());
           
           if (k == 1)
            {
         
                try
                {
                    Collection<PSObject> results = RunCommand_onhyperv("Get-WindowsFeature",rs);
                    int flg = 1;
                    foreach (var o in results)
                    {
                        if (o.Members["DisplayName"].Value.ToString() == "Hyper-V" && o.Members["InstallState"].Value.ToString() == "Installed")
                        {
                            // Console.WriteLine("Authentication successful---HyperV present"); 
                            flg = 0;
         
                            obj.set_runspace(rs);
                            Collection<PSObject> res = RunScript_onhyperv("Get-WmiObject Win32_OperatingSystem",rs);
                            foreach (var x in res)
                            {
                                obj.set_mos(x.Members["caption"].Value.ToString());
                                obj.set_mver(x.Members["version"].Value.ToString());
         
                                break;
                            }
                            Collection<PSObject> res1 = RunScript_onhyperv("hostname", rs);
                            obj.set_ipname(res1[0].ToString());
                            obj.result= 0;

                        }
                    }
                    if (flg == 1)
                    {

         //               Console.WriteLine("Authentication successful---HyperV not present");
                        obj.result= -12;
                    }
                  }
                catch (Exception e)
                {
           //           Console.WriteLine("Run command1 failed" + e);
                    obj.result= -14;
                }
            }
           else
           {
              obj.result=k;
           }
            return obj;
        }

       public Discover DiscoverVM(Discover obj)
       {
              string cmd = @"function GetValue{
                              param(
                                 $xml,
                                 $valueName
                                 )
                               $value = $null
                               $GuestExchangeItemXml = $xml.SelectSingleNode(""/INSTANCE/PROPERTY[@NAME='Name']/VALUE[child::text()='$valueName']"") 
                               if($GuestExchangeItemXml -ne $null) 
                                { 
                                 $value = $GuestExchangeItemXml.SelectSingleNode(""/INSTANCE/PROPERTY[@NAME='Data']/VALUE/child::text()"").Value 
                                } 
                               return $value
                              }
                              $vmList = Get-VM | ?{$_.State -eq 'Running'}
                              foreach ($vm in $vmList)
                               {
                                $rightVm = $false
                                 $vmOsVersion = ""
                                 $vmFQDN = ""
                                 $vmObj = Get-WmiObject -Namespace root\virtualization\v2 -Class Msvm_ComputerSystem -Filter ""ElementName='$($vm.Name)'""
                                 $vmObj.GetRelated(""Msvm_KvpExchangeComponent"").GuestIntrinsicExchangeItems | % { 
                                 $itemXml = ([XML]$_)
                                  if ($itemXml -ne $null)
                                   {
                                    $rightVm = $true
                                    $val = GetValue $itemXml ""FullyQualifiedDomainName""
                                     if ($val -ne $null)
                                      {
                                       $vmFQDN = $val
                                      }
                                    $val = GetValue $itemXml ""OSVersion""
                                   if ($val -ne $null)
                                     {
                                      $vmOsVersion = $val
                                     }
                                    }
                                 }
                                if ($rightVm)
                                  {
                                   if($vmOsVersion -ne """"){
                                                $VMO = New-Object -TypeName PSObject
                                                $VMO | Add-Member -MemberType NoteProperty -Name vname -Value $($vm.Name)
                                                $VMO | Add-Member -MemberType NoteProperty -Name vhost -Value $vmFQDN
                                                echo $VMO                                               
                                             }
                                  }
                            }";
  
           var vms = RunScript_onhyperv(cmd,obj._runspace);
           
           foreach (var v in vms)
           {
               VM o = new VM(v.Members["vname"].Value.ToString(), v.Members["vhost"].Value.ToString()); 
                obj.result.Add(o);
           }
             return obj;
       }

       public offload_req proc_offload(offload_req obj)
       {
           int k = -99;
           Runspace rs = create_Runspace(ref k,obj._ip,obj._uname,obj._pass);
           if(k==1)
           {
               
               string dest = "C:\\Users\\Public\\Exec\\";
                   for(int i=0;i<obj._filelist.Count;i++)
                   {
                       status s = obj._filelist[i];
                       string iis = obj._iis_url;
                       int l=iis.Length;
                       l--;
                        if(iis[l]=='/'){iis=iis+s.fname;}else{ iis=iis+'/'+s.fname;}
                           
                           string cmd=@" $url='"+ iis + "'\n";
                           cmd=cmd+"$TARGETDIR = '"+ dest +"'\n";
                           cmd=cmd+"$dest='"+dest+s.fname+"'\n";
                           cmd=cmd + @"Try
                                       {
                                         if(!(Test-Path -Path $TARGETDIR )){
                                           $p= New-Item -Force -ItemType directory -Path $TARGETDIR
                                            }
                                           (New-Object System.Net.WebClient).DownloadFile($url,$dest)
                                         echo '1'
                                        }
                                     Catch
                                     {
                                     echo '0' 
                                     }";

                       try
                       {
                           var ps = PowerShell.Create();
                           ps.Runspace = rs;
                           ps.AddScript(cmd);
                           Collection<PSObject> results = ps.Invoke();
                           if(results[0].ToString()=="1")
                               {
                                   obj._filelist[i].set_status(1);
                               }
                           else
                           {
                               obj._filelist[i].set_status(0);
                           }
                           
                           ps.Dispose();
                           ps = null;
                       }
                       catch(Exception e)
                       {
                           obj._filelist[i].set_status(0);
                       }
                   }
           }
           else
           {
               for(int i=0;i<obj._filelist.Count;i++)
               {
                   obj._filelist[i]._status = k;
               }
           }
           rs.Close();
           rs.Dispose();
           return obj;
       }

       public Install_req proc_install(Install_req obj)
       {
           int k = -99;
           Runspace rs = create_Runspace(ref k, obj._ip, obj._uname, obj._pass);
           if (k == 1)
           {

               string dest = "C:\\Users\\Public\\Exec\\";
               string ps_script = "ps_script/run_exe.ps1";
               string dest_script = dest + "run_exe.ps1";
               string iis1 = obj._iis_url;
               int l1 = iis1.Length;
               l1--;
               if (iis1[l1] == '/') { iis1 = iis1 + ps_script; } else { iis1 = iis1 + '/' + ps_script;}
               string cmd = @" $url='" + iis1 + "'\n";
               cmd = cmd + "$TARGETDIR = '" + dest + "'\n";
               cmd = cmd + "$dest='" + dest_script + "'\n";
               cmd = cmd + @"Try
                                       {
                                         if(!(Test-Path -Path $TARGETDIR )){
                                           $p= New-Item -Force -ItemType directory -Path $TARGETDIR
                                            }
                                           (New-Object System.Net.WebClient).DownloadFile($url,$dest)
                                         echo '1'
                                        }
                                     Catch
                                     {
                                     echo '0' 
                                     }";
                          try
                           {
                              var ps = PowerShell.Create();
                              ps.Runspace = rs;
                              ps.AddScript(cmd);
                              Collection<PSObject> results = ps.Invoke();
                              if (results[0].ToString() == "1")
                              {
                                  for (int i = 0; i < obj._filelist.Count; i++)
                                  {
                                      status s = obj._filelist[i];
                                      string iis = obj._iis_url;
                                      int l = iis.Length;
                                      l--;
                                      if (iis[l] == '/') { iis = iis + s.fname; } else { iis = iis + '/' + s.fname; }
                                      string tname=DateTime.Now.ToString("h:mm:ss");
                                      tname = Regex.Replace(tname, @":", string.Empty);
                                      string cmd1= @"$TaskName = '"+ tname  +"'\n"; 
                                      cmd1=cmd1+"$iis='"+ iis +"'\n";
                                      cmd1=cmd1+"$Tg = '"+ dest +"'\n";
                                      cmd1=cmd1+"$fname='"+s.fname +"'\n";
                                      cmd1 = cmd1 + @"$script= $Tg + 'run_exe.ps1'
                                                  $TaskRun = ""$PSHome\powershell.exe $script -url $iis -target $Tg -fname $fname""
                                                  $start = (Get-Date).AddMinutes(1).ToString(""HH:mm"")
                                                  Try{ 
                                                  [string]$Result = schtasks /create /ru ""BUILTIN\Users"" /rp * /tn $Taskname /tr $TaskRun /sc once /st $start /f
                                                  $Result += schtasks /run /tn $TaskName 
                                                  echo '1'  }
                                                   Catch { echo '2'}";
                                      
                                      try
                                      {
                                          var ps1 = PowerShell.Create();
                                          ps1.Runspace = rs;
                                          ps1.AddScript(cmd1);
                                          Collection<PSObject> results1 = ps1.Invoke();
                                          if (results1[0].ToString() == "1")
                                          {
                                              obj._filelist[i].set_status(1);
                                          }
                                          else
                                          {
                                              obj._filelist[i].set_status(2);
                                          }

                                          ps1.Dispose();
                                          ps1 = null;
                                      }
                                      catch (Exception e)
                                      {
                                          obj._filelist[i].set_status(0);
                                      }
                                   } 
                              }
                              else
                              {
                                  for (int i = 0; i < obj._filelist.Count; i++)
                                  {
                                      obj._filelist[i]._status = 0;
                                  }
                              }
                           ps.Dispose();
                           ps = null;
                           }
               catch (Exception e)
               {
                   for (int i = 0; i < obj._filelist.Count; i++)
                   {
                       obj._filelist[i]._status = 0;
                   }
               }
           }
           else
           {
               for (int i = 0; i < obj._filelist.Count; i++)
               {
                   obj._filelist[i]._status = k;
               }
           }
           rs.Close();
           rs.Dispose();
           return obj;
       }
   }
}
