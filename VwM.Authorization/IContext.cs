using System;
using System.Collections.Generic;
using System.Text;

namespace VwM.Authorization
{
    public interface IContext
    {
        User GetUser(string login, string password);
    }
}
