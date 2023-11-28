using AspNetCore.Models;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface ILibraryRepo:IRepo<Library>
    {
        public void Update(Library entity);
    }
}
