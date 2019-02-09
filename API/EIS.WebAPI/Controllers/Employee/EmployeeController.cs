using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Filters;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        [Route("GetLocations/{loc}")]
        [HttpGet]
        public IActionResult GetEmployeeLocationWise(int loc)
        {
            if (loc == 0)
            {
                IQueryable<Person> employees = _repository.Employee.FindAllByCondition(e => e.Role.Name != "Admin");
                return Ok(employees);
            }
            else
            {
                IQueryable<Person> employees = _repository.Employee.FindAllByCondition(e => e.LocationId == loc && e.Role.Name != "Admin");
                return Ok(employees);
            }
            
        }
        [HttpGet]
        public IActionResult GetAllEmployee()
        {
            IQueryable<Person> employees = _repository.Employee.FindAllWithNoTracking().Where(x => x.TenantId == TenantId && x.IsActive == true && x.Role.Name!="Admin");
            return Ok(employees);
        }
        [HttpGet("{EmployeeCode}")]
        public IActionResult GetById([FromRoute]string EmployeeCode)
        {
            Person employee = _repository.Employee.FindByCondition(e => e.EmployeeCode == EmployeeCode);
            employee.User = _repository.Users.FindByCondition(u => u.PersonId == employee.Id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(employee);
            }
        }
        [Route("Person")]
        [HttpGet("{EmployeeCode}")]
        public IActionResult GetPerson([FromRoute]string EmployeeCode)
        {
            Person employee = _repository.Employee.FindByConditionWithNoTracking(e => e.EmployeeCode == EmployeeCode);
            employee.User = _repository.Users.FindByConditionWithNoTracking(u => u.PersonId == employee.Id);
            
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(employee);
            }
        }
        [Route("Profile/{EmployeeCode}")]
        [HttpGet]
        public IActionResult GetProfile([FromRoute]string EmployeeCode)
        {
            Person employee = _repository.Employee.GetProfile(EmployeeCode);
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
            int n = _repository.Employee.GenerateNewIdCardNo(TenantId);
            return n;
        }

        [HttpPost("{id}")]
        public IActionResult Create([FromRoute]int id, [FromBody]Person person)
        {
            if (id == 0)
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
                    "Your information have been successfully registered with employee system. : \n" +
                    "Your Code Number: " + person.EmployeeCode + "\n" +
                    "User Name: " + person.EmailAddress + "\n" +
                    "Password: " + pw + "\n" +
                    "Click here http://aclpune.com/ems to login";
                new EmailManager(_configuration).SendEmail(subject, body, To, null);
                return Ok();
            }
            else 
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _repository.Employee.UpdateAndSave(person);
                string To = person.EmailAddress;
                string subject = "Employee Registration";
                string body = "Dear " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                    "Your Information has been successfully updated with employee system. : \n" +
                    "User Name: " + person.EmailAddress + "\n";
                new EmailManager(_configuration).SendEmail(subject, body, To, null);
                return Ok(person);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateData([FromRoute]int id, [FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.UpdateAndSave(person);
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Dear " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                "Your Information has been successfully updated with employee system. : \n" +
                "User Name: " + person.EmailAddress + "\n";
            new EmailManager(_configuration).SendEmail(subject, body, To,null);
            return Ok(person);
        }
        [Route("PersonDelete/{id}")]
        [HttpPost]
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
            Role d = _repository.Employee.GetDesignationById(did);
            return Ok(d.Name);
        }
        
        [Route("AddDesignation/{id}")]
        [HttpPost]
        public IActionResult CreateDesignation([FromRoute]int id,[FromBody]Role designation)
        {
            if (id == 0)
            {
                bool DesignationExists = _repository.Employee.DesignationExists(designation.Name, TenantId);
                if (DesignationExists)
                {
                    return BadRequest("Designation already Exists");
                }
                designation.TenantId = TenantId;
                _repository.Employee.AddDesignationAndSave(designation);
                return CreatedAtAction("GetDesignationById", new { did = designation.Id }, designation);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _repository.Employee.UpdateDesignationAndSave(designation);
                return NoContent();
            }
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
            IQueryable<Person> list = null;
            if (sortGrid.LocationId == 0)
            {
                list = _repository.Employee.FindAllByCondition(x => x.TenantId == TenantId && x.IsActive == sortGrid.IsActive).Include(x=>x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin");
            }
            else
            {
                list = _repository.Employee.FindAllByCondition(x => x.TenantId == TenantId && x.IsActive == sortGrid.IsActive && x.LocationId == sortGrid.LocationId).Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin");
            }
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

        [Route("ActivatePerson/{EmployeeCode}")]
        [HttpGet]
        public IActionResult ActivatePerson([FromRoute]string EmployeeCode)
        {
            Person person=_repository.Employee.ActivatePerson(EmployeeCode);
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Hello " + GetTitle(person.Gender) + " " + person.FirstName + " " + person.LastName + "\n" +
                "Your Information has been successfully activated with employee system. : \n" +
                "User Name: " + person.EmailAddress + "\n"+
                "Click here http://aclpune.com/ems to login";
            new EmailManager(_configuration).SendEmail(subject, body, To,null);
            return Ok();
        }

    }
}