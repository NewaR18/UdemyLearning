using AspNetCore.Models.ViewModel;
using AspNetCore.Models;
using AspNetCore.Utilities.Security;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using AspNetCore.Utilities.ApiGateway;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AspNetCore.Utilities.Enumerators;

namespace AspNetCore.Utilities.Payments
{
	public class EsewaPayments
	{
		private readonly IUnitOfWork _repo;
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;
		public EsewaPayments(IUnitOfWork repo, IConfiguration configuration, IHttpClientFactory httpClientFactory)
		{
			_repo = repo;
			_configuration = configuration;
			_httpClientFactory = httpClientFactory;
		}
		public async Task<string> MakePayment(ShoppingCartViewModel shoppingCartViewModel)
		{
			string TotalAmount = shoppingCartViewModel.OrderHeader.OrderTotal.ToString("0.00");
			var successURL = APIGateway.EsewaSuccess;
			var failureURL = APIGateway.EsewaFailure;
			string ProductCode = _configuration["Esewa:ProductCode"]!;
			string EsewaAPIKey = _configuration["Esewa:APIKey"]!;
			string EsewaUrl = _configuration["Esewa:Url"]!;
			var UniqueTransactionId = Guid.NewGuid().ToString() + "_" + shoppingCartViewModel.OrderHeader.Id.ToString();
			var message = "total_amount=" + TotalAmount + ",transaction_uuid=" + UniqueTransactionId + ",product_code=" + ProductCode;
			PaymentGatewayParam paymentGatewayParam = new PaymentGatewayParam()
			{
				Path = EsewaUrl,
				Params = new PaymentGatewayParamInner()
				{
					amount = TotalAmount,
					tax_amount = "0",
					total_amount = TotalAmount,
					transaction_uuid = UniqueTransactionId,
					product_code = ProductCode,
					product_service_charge = "0",
					product_delivery_charge = "0",
					success_url = successURL,
					failure_url = failureURL,
					signed_field_names = "total_amount,transaction_uuid,product_code",
					signature = SHAConfiguration.ComputeHMACSHA256(message, EsewaAPIKey)
				}
			};
			EsewaPayment esewaPayment = new EsewaPayment()
			{
				UserId = shoppingCartViewModel.OrderHeader.ApplicationUserId,
				TransactionId = UniqueTransactionId,
				Status = "Initiated",
				TotalAmount = Convert.ToDecimal(TotalAmount),
				ProductCode = ProductCode,
				ProductId = 19,
				Count = 0,
			};
			_repo.EsewaPaymentRepo.Add(esewaPayment);
			_repo.Save();
			string absoluteURI = await SubmitForm(paymentGatewayParam);
			return absoluteURI;
		}

		public async Task<string> SubmitForm(PaymentGatewayParam paymentGatewayParam)
		{
			var formValues = new Dictionary<string, string>();

			//To Add Form Values to Dictionary
			var paramInner = paymentGatewayParam.Params;
			var properties = typeof(PaymentGatewayParamInner).GetProperties();
			foreach (var property in properties)
			{
				var propertyName = property.Name;
				var propertyValue = property.GetValue(paramInner)?.ToString();

				if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
				{
					formValues.Add(propertyName, propertyValue);
				}
			}
			var formContent = new FormUrlEncodedContent(formValues);
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				var targetUrl = paymentGatewayParam.Path;
				var response = await httpClient.PostAsync(targetUrl, formContent);
				if (response.IsSuccessStatusCode)
				{
					if (response.RequestMessage.RequestUri != null)
					{
						return response.RequestMessage.RequestUri.AbsoluteUri;
					}
				}
				return "";
			}
		}
		public int OnSuccess(string data)
		{
			string decrypedResponse = SHAConfiguration.DecodeHMACSHA256(data);
			EsewaSuccessResponse esewaSuccessReponse = JsonConvert.DeserializeObject<EsewaSuccessResponse>(decrypedResponse)!;
			EsewaPayment esewaPayment = _repo.EsewaPaymentRepo.GetFirstOrDefault(x => x.TransactionId == esewaSuccessReponse.transaction_uuid);
			esewaPayment.TransactionCode = esewaSuccessReponse.transaction_code;
			esewaPayment.Status = esewaSuccessReponse.status;
			_repo.Save();
			int OrderId = Convert.ToInt32(esewaSuccessReponse.transaction_uuid.Split('_').Last());
			var order = _repo.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == OrderId);
			_repo.OrderHeaderRepo.UpdateStatus(OrderId, nameof(OrderEnum.Approved), nameof(PaymentEnum.Approved));
			_repo.OrderHeaderRepo.UpdateStripeData(OrderId, "Esewa", esewaSuccessReponse.transaction_code);
			_repo.Save();
			return OrderId;
		}
		public int OnFailure(string data)
		{
			//Logic
			return 0;
		}
        public void StartRefundProcess(OrderHeaderPayViewModel orderHeaderPayViewModel)
        {
            //Refund Logic not available for Esewa and Khalti, Gotta provide refund manually
            PaymentRefund paymentRefund = new PaymentRefund()
            {
                Id = 0,
                PaymentOptions = nameof(PaymentMethodEnum.Esewa),
                Currency = "npr",
                CreatedDate = DateTime.Now,
                Reason = "requested_by_customer",
                Status = "notRefunded",
                PaymentIntentId = orderHeaderPayViewModel.OrderHeader.PaymentIntentId,
                StripeId = "",
                BalanceTransactionId = "",
                ChargeId = "",
                RequestId = ""
            };
            _repo.PaymentRefundRepo.Add(paymentRefund);
            _repo.Save();
            _repo.OrderHeaderRepo.UpdateStatus(orderHeaderPayViewModel.OrderHeader.Id, nameof(OrderEnum.Cancelled), nameof(PaymentEnum.Refunded));
            _repo.Save();
        }
    }
}
