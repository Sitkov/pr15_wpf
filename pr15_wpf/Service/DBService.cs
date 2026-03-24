using pr15_wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr15_wpf.Service
{
    internal class DBService
    {
        private ElectroStoreContext context;

        public ElectroStoreContext Context => context;
        private static DBService? instence;
        public static DBService Instance
        {
            get
            {
                if (instence == null)
                {
                    instence = new DBService();
                }
                return instence;
            }
        }
        private DBService ()
        {
            context = new ElectroStoreContext();
        }
    }
}
