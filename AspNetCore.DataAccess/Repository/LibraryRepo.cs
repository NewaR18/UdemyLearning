using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using System.Linq.Expressions;

namespace AspNetCore.Repository
{
    public class LibraryRepo : Repo<Library> , ILibraryRepo
    {
        private readonly AppDbContext _context;
        public LibraryRepo(AppDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(Library lib)
        {
            _context.Library.Update(lib);
        }
    }
}
