using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Filters;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EIS.WebAPI.Controllers
{
    [TypeFilter(typeof(Authorization))]
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeController : Controller
    {

        public readonly IDistributedCache distributedCache;
        public readonly IRepositoryWrapper _repository;
        public readonly IConfiguration configuration;
        public EmployeeController(IRepositoryWrapper repository,IDistributedCache distributedCache, IConfiguration configuration)
        {
            _repository= repository  ;
            this.distributedCache = distributedCache;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllEmployee()
        {
            var employees = _repository.Employee.FindAll();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employee = _repository.Employee.FindByCondition(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(employee);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create([FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.Create(person);
            _repository.Employee.Save();
            Users u = new Users
            {
                UserName = person.EmailAddress,
                Password = person.FirstName + person.DateOfBirth.Day,
                PersonId=person.Id
            };
            _repository.Users.CreateUser(u);
            _repository.Users.Save();
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            //var password = ;
            string body = "Hello! Mr."+person.FirstName+" "+person.LastName+"\n" +
                "Your have been successfully registered with employee system. : \n" +
                "User Name: "+person.EmailAddress+"\n"+
                "Password: " + person.FirstName + person.DateOfBirth.Day;
            new EmailManager(configuration).SendEmail(subject, body, To);
            return Ok();
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public IActionResult UpdateData([FromRoute]int id, [FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != person.Id)
            {
                return BadRequest();
            }
            _repository.Employee.Update(person);
            try
            {
                _repository.Employee.Save();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person person = _repository.Employee.FindByCondition(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            _repository.Employee.Delete(person);
            _repository.Employee.Save();
            return Ok(person);
        }
        [Route("Designations")]
        [HttpGet]
        public IEnumerable<Role> GetDesignations()
        {
            return _repository.Employee.GetDesignations();
        }
        [Route("Designations/{did}")]
        [HttpGet]
        public Role GetDesignationById([FromRoute]int did)
        {
            return _repository.Employee.GetDesignationById(did);
        }
        [Route("DesignationName/{did}")]
        [HttpGet]
        public IActionResult GetDesignationNameById([FromRoute]int did)
        {
            var d = _repository.Employee.GetDesignationById(did);
            return Ok(d.Name);
        }
        [Route("AddDesignation")]
        [HttpPost]
        public IActionResult CreateDesignation([FromBody]Role designation)
        {
            var DesignationExists=_repository.Employee.DesignationExists(designation.Name);
            if (DesignationExists)
            {
                return BadRequest("Designation already Exists");
            }
            _repository.Employee.AddDesignation(designation);
            _repository.Employee.Save();
            return CreatedAtAction("GetDesignationById", new { did = designation.Id }, designation);
        }
        [Route("UpdateDesignation")]
        public IActionResult UpdateDesignationData([FromBody]Role designation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.UpdateDesignation(designation);
            try
            {
                _repository.Employee.Save();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

            return NoContent();
        }
    }
}