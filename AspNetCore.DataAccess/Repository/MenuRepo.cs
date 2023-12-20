using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class MenuRepo : Repo<Menu>, IMenuRepo
    {
        private readonly AppDbContext _context;
        public MenuRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Menu menu)
        {
            _context.Menu.Update(menu);
        }
    }
}
