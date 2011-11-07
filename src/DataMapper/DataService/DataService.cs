using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModel;

namespace DataService
{
    public class DataService
    {
        void dummy()
        {
            BlogMapper m = new BlogMapper();
            Blog b = new Blog();
            m.Insert(b);
            b = m.Get(1);
        }
    }
}
