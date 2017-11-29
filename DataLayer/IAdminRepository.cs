using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IAdminRepository
    {
        List<Admin> GetAll();
        Admin Get(int id);
        int Insert(Admin booking);
        int Update(Admin booking);
        int Delete(int id);
    }
}
