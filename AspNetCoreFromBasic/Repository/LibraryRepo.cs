using AspNetCore.DataAccess;
using AspNetCore.Models;

namespace AspNetCoreFromBasic.Repository
{
    public class LibraryRepo : ILibraryRepo
    {
        private readonly AppDbContext _context;
        public LibraryRepo(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Library> Index() {
            IEnumerable<Library> libraries = _context.Library;
            return libraries;
        }
        public void Create(Library lib)
        {
            _context.Library.Add(lib);
            _context.SaveChanges();
        }
        public Library GetById(int id)
        {
            Library lib = _context.Library.Find(id);
            return lib;
        }
        public void Update(Library lib)
        {
            _context.Library.Update(lib);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            Library lib=_context.Library.Find(id);
            _context.Library.Remove(lib);
            _context.SaveChanges();
        }
    }
}
