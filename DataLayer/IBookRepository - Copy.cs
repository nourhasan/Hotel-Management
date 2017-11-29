using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IBookRepository
    {
        List<Book> GetAll();
        Book Get(int id);
        int Insert(Book book);
        int Update(Book book);
        int Delete(int id);
    }
}
