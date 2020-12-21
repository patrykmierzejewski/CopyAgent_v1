using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyAgent_v1
{
    class License
    {
    }

    public static class InstanceMutex
    {
        static System.Threading.Mutex mutex;

        public static bool IsActiveInstance()
        {
            bool activeInstance;

            string mutexName = "Local\\" + System.AppDomain.CurrentDomain.FriendlyName + "_" + System.Windows.Forms.SystemInformation.UserName;
            mutex = new System.Threading.Mutex(false, mutexName, out activeInstance);

            return !activeInstance;
        }
    }
}
