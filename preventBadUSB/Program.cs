using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;


namespace preventBadUSB
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("HID atack preventer activat");
            detector dtc = new detector();
            while (true) ;
        }

    }
}
