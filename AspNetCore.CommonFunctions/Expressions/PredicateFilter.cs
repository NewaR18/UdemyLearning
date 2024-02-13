using AspNetCore.CommonFunctions.PredicateBuilderThis;
using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.CommonFunctions.Expressions
{
    public class PredicateFilter
    {
        public Expression<Func<OrderHeader, bool>> predicate(List<ColumnFilterModel> Columns)
        {
            var searchCondition = PredicateBuilder.True<OrderHeader>();
            foreach (ColumnFilterModel filter in Columns)
            {
                if (!string.IsNullOrEmpty(filter.Search.Value))
                {
                    string searchValue = filter.Search.Value.Replace("(","").Replace(")", "").ToLower();
                    switch (filter.Data)
                    {
                        case ("name"):
                            searchCondition = searchCondition.And(p => p.Name.ToLower().Contains(searchValue));
                            break;
                        case ("phoneNumber"):
                            searchCondition = searchCondition.And(p => p.PhoneNumber.ToLower().Contains(searchValue));
                            break;
                        case ("orderTotal"):
                            searchCondition = searchCondition.And(p => p.OrderTotal.ToString().ToLower().Contains(searchValue));
                            break;
                        case ("orderDate"):
                            searchCondition = searchCondition.And(p => p.OrderDate.ToString().ToLower().Contains(searchValue));
                            break;
                        case ("paymentDate"):
                            searchCondition = searchCondition.And(p => p.PaymentDate.ToString().ToLower().Contains(searchValue));
                            break;
                        case ("shippingDate"):
                            searchCondition = searchCondition.And(p => p.ShippingDate.ToString().ToLower().Contains(searchValue));
                            break;
                        case ("orderStatus"):
                            searchCondition = searchCondition.And(p => p.OrderStatus.ToLower().Contains(searchValue));
                            break;
                        case ("paymentStatus"):
                            searchCondition = searchCondition.And(p => p.PaymentStatus.ToLower().Contains(searchValue));
                            break;
                        default:
                            searchCondition = searchCondition.And(p => 1 == 1);
                            break;
                    }
                }
                int iValue;
                
            }
            return searchCondition;
        }
    }
}
