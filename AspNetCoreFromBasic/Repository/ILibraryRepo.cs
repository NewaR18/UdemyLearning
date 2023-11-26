using AspNetCore.Models;

namespace AspNetCoreFromBasic.Repository
{
    public interface ILibraryRepo
    {
        public IEnumerable<Library> Index();
        public void Create(Library lib);
        public Library GetById(int id);
        public void Update(Library lib);
        public void Delete(int id);
    }
}
