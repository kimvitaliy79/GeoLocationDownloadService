using DBApplication.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser;
using UpdateUtility.Services;

namespace UpdateUtility
{
    class Program
    {
       
        static void Main(string[] args)
        {
            UpdateManager manager = new UpdateManager();
            manager.Start();
        }    
    }
}
