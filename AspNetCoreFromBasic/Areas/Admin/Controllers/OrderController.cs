using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _repo;
        public OrderController(IUnitOfWork repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        //In Default, It has local pagination. And For filter and pagination, it does not hit backend.
        //But Using processing: true,serverSide: true   -- It hits backend on every pagination and filter.
        #region API
        [HttpPost]
        public JsonResult GetAll([FromBody] DataTableAjaxModel model)
        {
            var TotalRecord = _repo.OrderHeaderRepo.GetCount();
            PaginatedOrderHeader entities = _repo.OrderHeaderRepo.GetPaginatedRows(model.Start, model.Length,model.Columns);
            var responseData = new
            {
                draw = model.Draw,
                recordsTotal = TotalRecord, 
                recordsFiltered = entities.TotalCount, 
                data = entities.Data
            };
            return Json(responseData);
        }
        #endregion
    }
}
