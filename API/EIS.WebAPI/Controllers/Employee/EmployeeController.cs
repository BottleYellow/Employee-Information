using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Filters;
using EIS.WebAPI.Services;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EIS.WebAPI.Controllers
{
    [TypeFilter(typeof(Authorization))]
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        public readonly IConfiguration _configuration;
        public EmployeeController(IRepositoryWrapper repository, IConfiguration configuration) : base(repository)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllEmployee()
        {
            var employees = _repository.Employee.FindAll().Where(x => x.TenantId == TenantId && x.IsActive == true);
            return Ok(employees);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var employee = _repository.Employee.FindByCondition(e => e.Id == id);
            employee.User = _repository.Users.FindByCondition(u => u.PersonId == id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(employee);
            }
        }
        [Route("Profile/{id}")]
        [HttpGet]
        public IActionResult GetProfile([FromRoute]int id)
        {
            var employee = _repository.Employee.GetProfile(id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(employee);
            }
        }
        [Route("GenNewIdCardNo")]
        [HttpGet]
        public int CreateNewIdCardNo()
        {
            var n = _repository.Employee.GenerateNewIdCardNo(TenantId);
            return n;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create([FromBody]Person person)
        {
            //person.EmployeeCode = _repository.Employee.GenerateNewIdCardNo(TenantId).ToString();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            person.TenantId = TenantId;
            _repository.Employee.CreateAndSave(person);
            Users u = new Users
            {
                TenantId = TenantId,
                UserName = person.EmailAddress,
                Password = EmailManager.CreateRandomPassword(8),
                PersonId = person.Id
            };
            string pw = u.Password;
            _repository.Users.CreateUserAndSave(u);
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Hello " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                "Your have been successfully registered with employee system. : \n" +
                "Your Code Number: " + person.EmployeeCode + "\n" +
                "User Name: " + person.EmailAddress + "\n" +
                "Password: " + pw;
            new EmailManager(_configuration).SendEmail(subject, body, To);
            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            Person person = _repository.Employee.FindByCondition(x => x.Id == id);
            Users users = _repository.Users.FindByCondition(x => x.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }
            person.IsActive = false;
            users.IsActive = false;
            _repository.Employee.UpdateAndSave(person);
            _repository.Users.UpdateAndSave(users);
            return Ok(person);
        }
        [Route("Designations")]
        [HttpGet]
        public IEnumerable<Role> GetDesignations()
        {
            return _repository.Employee.GetDesignations(TenantId);
        }

        [DisplayName("Manage Roles")]
        [Route("Designations/{did}")]
        [HttpGet]
        public Role GetDesignationById([FromRoute]int did)
        {
            return _repository.Employee.GetDesignationById(did);
        }

        [DisplayName("Manage Roles")]
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
            var DesignationExists=_repository.Employee.DesignationExists(designation.Name,TenantId);
            if (DesignationExists)
            {
                return BadRequest("Designation already Exists");
            }
            designation.TenantId = TenantId;
            _repository.Employee.AddDesignationAndSave(designation);
            return CreatedAtAction("GetDesignationById", new { did = designation.Id }, designation);
        }
        [Route("UpdateDesignation")]
        public IActionResult UpdateDesignationData([FromBody]Role designation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.UpdateDesignationAndSave(designation);            
            return NoContent();
        }

        [HttpPost]
        [Route("Data")]
        [AllowAnonymous]
        public IActionResult GetData([FromBody]SortGrid sortGrid)
        {
            ArrayList employeeslist;
            var list = _repository.Employee.FindAllByCondition(x => x.TenantId == TenantId);
            if (sortGrid.Search == null)
            {
                employeeslist = _repository.Employee.GetDataByGridCondition(null, sortGrid,list);
            }
            else
            {
                employeeslist = _repository.Employee.GetDataByGridCondition(x => x.EmployeeCode == sortGrid.Search || x.FirstName.ToLower().Contains(sortGrid.Search.ToLower()) || x.MiddleName.ToLower().Contains(sortGrid.Search.ToLower()) || x.LastName.ToLower().Contains(sortGrid.Search.ToLower())||x.EmailAddress.ToLower().Contains(sortGrid.Search.ToLower()) || x.PanCard.Contains(sortGrid.Search) || x.AadharCard.Contains(sortGrid.Search) || x.MobileNumber.Contains(sortGrid.Search), sortGrid, list);
            }
            return Ok(employeeslist);
        }
        public string GetTitle(Gender gender)
        {
            string Title="";
            if (gender == Gender.Male) Title = "Mr.";
            else if (gender == Gender.Female) Title = "Ms.";
            return Title;
        }

        [HttpGet]
        [Route("ActivatePerson/{id}")]
        public IActionResult ActivatePerson([FromRoute]int id)
        {
            Person person=_repository.Employee.ActivatePerson(id);
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Hello " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                "Your Information have been successfully activated with employee system. : \n" +
                "User Name: " + person.EmailAddress + "\n";
            new EmailManager(_configuration).SendEmail(subject, body, To);
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
            _repository.Employee.UpdateAndSave(person); 
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Hello " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                "Your Information have been successfully activated with employee system. : \n" +
                "User Name: " + person.EmailAddress + "\n";
            new EmailManager(_configuration).SendEmail(subject, body, To);
            return Ok(person);
        }
    }
}