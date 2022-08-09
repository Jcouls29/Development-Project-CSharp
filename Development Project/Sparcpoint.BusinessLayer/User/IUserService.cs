using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.BusinessLayer.User
{
    public interface IUserService
    {
        string Login(string userName, string password);
    }
}
