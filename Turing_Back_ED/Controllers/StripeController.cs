using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Turing_Back_ED.Workers;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    /// <summary>
    /// Handles all payments transactions, using Stripe
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly PaymentsWorker payments;

        public StripeController(PaymentsWorker _payments)
        {
            payments = _payments;
        }
        

        /// <summary>
        /// Charges a customer's payment account
        /// </summary>
        /// <param name="paymentsInput">Object of values representing the payment parameters</param>
        /// <returns>Stripe charge object</returns>
        [HttpPost("Charge")]
        [ModelValidate]
        public async Task<ActionResult> Charge(PaymentsInputModel paymentsInput)
        {
            var chargeResult = await payments.ChargeWithStripe(paymentsInput);

            if (chargeResult != null && chargeResult.Status != "failed")
            {
                return new OkObjectResult(chargeResult);
            }

            return new BadRequestObjectResult(new 
            {
                Code = Constants.ErrorCodes.ERR_02.ToString("g"),
                Message = Constants.ErrorMessages.ERR_02,
                Status = StatusCodes.Status400BadRequest,
                Result = chargeResult
            });
        }


        /// <summary>
        /// Webhooks endpoint for synchronization with stripe,
        /// for the purposes of receiving statuses on
        /// recurrent, deferred or delayed transactions.
        /// This endpoint is used ONLY by the payments gateway
        /// </summary>
        /// <returns>No content</returns>
        [HttpPost("webhooks")]
        [ModelValidate]
        public ActionResult Webhooks()
        {
            //var json = await new StreamReader(Request.Body).ReadToEndAsync();
            //var stripeEvent = EventUtility.ParseEvent(json);
            

            Debug.WriteLine($"One hit => {Request.Host.Value}");

            Trace.WriteLine($"One hit => {Request.Host.Value}");

            return new OkResult();


        }
    }
}