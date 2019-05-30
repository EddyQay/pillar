using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Turing_Back_ED.DAL;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.DomainModels
{
    public class PaymentsStore
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public PaymentsStore(DatabaseContext context, TokenManager _tokenManager,
            IOptions<TokenSection> _tokenSection)
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        /// <summary>
        /// Uses Stripe's sdk to charge an account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Charge> ChargeWithStripe(PaymentsInputModel model)
        {
            //configure stripe with your api key
            StripeConfiguration.SetApiKey("sk_test_cawk9z0V2PrGauWlCLFtXrEt00RYAXHGkO");//turing's API key

            //add charge options/ parameters to send in request
            var options = new ChargeCreateOptions
            {
                Amount = model.Amount,
                Currency = model.Currency,
                Description = model.Description,
                SourceId = model.StripeToken, // obtained with CardsUI,
                Metadata = new Dictionary<string, string>
                {
                    { "OrderId", model.OrderId.ToString() }
                }
            };

            //initialize a stripe charge service
            var service = new ChargeService();

            //asynchonously charge the account with set options
            Charge charge = await service.CreateAsync(options);
            charge.OrderId = model.OrderId.ToString();

            //return the response of the trnsaction
            return charge;

        }
    }
}
