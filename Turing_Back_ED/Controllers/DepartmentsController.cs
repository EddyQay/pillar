using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentStore departments;
        readonly ILogger<DepartmentsController> logger;

        public DepartmentsController(DepartmentStore _departments, ILogger<DepartmentsController> _logger)
        {
            departments = _departments;
            logger = _logger;
        }
        

        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindDepartment(int id)
        {
            var department = await departments.FindByIdAsync(id);

            if (department != null)
            {
                return new OkObjectResult(department);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.DEP_02.ToString("g"),
                Message = Constants.ErrorMessages.DEP_02,
                Status = StatusCodes.Status400BadRequest
            });
        }

        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel model)
        {
            if (model == null)
                model = new GeneralQueryModel();

            var query = await departments.GetAllAsync(model);

            if (query != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = query.Count(),
                    Rows = query
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = query
                });
        }
    }
}