using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Runtime.InteropServices;

namespace preventBadUSB
{
    class detector
    {
        private HashSet<string> dispositius;
        private Stack<string> dispositiuNou;

        public detector()
        {
            dispositius = GetUSBDevices2();
            dispositiuNou = new Stack<string>();
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Query = query;
            watcher.Start();
            watcher.WaitForNextEvent();
            
        }
        
        private void watcher_EventArrived(Object sender, EventArrivedEventArgs e)
        {
            string resp = "";
            if (ExtreuDispositiusNous())
            {
                LockWorkStation();
                Console.WriteLine("Vols afegir el nou dispositiu a la llista? (Si = S, No = N)");
                resp = Console.ReadLine();
                if (resp == "S")
                {
                    while (dispositiuNou.Count > 0)
                    {
                        dispositius.Add(dispositiuNou.Pop());
                    }
                }
            }  
        }

        static HashSet<string> GetUSBDevices2()
        {
            HashSet<string> devices = new HashSet<string>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity"))
                collection = searcher.Get();
            
            foreach (var device in collection)
            {
                string[] dispNou = (string [] )device.GetPropertyValue("HardwareID");
                if (dispNou != null)
                {
                    string deviceID = dispNou[0];
                    devices.Add(deviceID);
                }
                
            }
            return devices;
        }

        private bool ExtreuDispositiusNous()
        {
            HashSet<string> dispositiusNous = GetUSBDevices2();
            dispositiuNou.Clear();

            foreach (string disp in dispositiusNous)
            {
                if (!dispositius.Contains(disp))
                {
                    dispositiuNou.Push(disp);
                }
            }
            return dispositiuNou.Count > 0;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();
    }
}
