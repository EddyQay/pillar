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
    /// <summary>
    /// Controls all department information
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentStore departments;

        public DepartmentsController(DepartmentStore _departments)
        {
            departments = _departments;
        }
        
        /// <summary>
        /// Finds a particular department
        /// </summary>
        /// <param name="Id">Department Id</param>
        /// <returns>A Department object</returns>
        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindDepartment(int Id)
        {
            var department = await departments.FindByIdAsync(Id);

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

        /// <summary>
        /// Gets all departments there is
        /// </summary>
        /// <param name="filter">an object of filtering options</param>
        /// <returns></returns>
        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel filter)
        {
            if (filter == null)
                filter = new GeneralQueryModel();

            var query = await departments.GetAllAsync(filter);

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