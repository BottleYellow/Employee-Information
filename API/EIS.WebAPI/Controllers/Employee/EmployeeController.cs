﻿using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Generic;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Filters;
using EIS.WebAPI.RedisCache;
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
    public class EmployeeController : Controller
    {
        RedisAgent Cache = new RedisAgent();
        int TenantId = 0;
        public readonly IRepositoryWrapper _repository;
        public readonly IConfiguration _configuration;
        public EmployeeController(IRepositoryWrapper repository, IConfiguration configuration)
        {
            TenantId = Convert.ToInt32(Cache.GetStringValue("TenantId"));
            _repository = repository  ;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllEmployee()
        {        
            var employees = _repository.Employee.FindAll().Where(x=>x.TenantId==TenantId);
            return Ok(employees);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
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
            person.IdCard = _repository.Employee.GenerateNewIdCardNo(TenantId).ToString();
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
                PersonId=person.Id
            };
            string pw = u.Password;
            _repository.Users.CreateUserAndSave(u);
            string To = person.EmailAddress;
            string subject = "Employee Registration";
            string body = "Hello "+GetTitle(person.Gender)+" "+person.FirstName + " " + person.LastName + "\n" +
                "Your have been successfully registered with employee system. : \n" +
                "Id Card Number: " + person.IdCard + "\n" +
                "User Name: " + person.EmailAddress + "\n" +
                "Password: " + pw;
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
            return Ok(person);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            Person person = _repository.Employee.FindByCondition(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            person.IsActive = false;
            _repository.Employee.UpdateAndSave(person);
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
        [Route("Data/{search?}")]
        [AllowAnonymous]
        public IActionResult GetData([FromBody]SortGrid sortGrid, [FromRoute]string search = null)
        {
            ArrayList employeeslist;
            if (search == null)
            {
                employeeslist = _repository.Employee.GetDataByGridCondition(null, sortGrid,x=>x.TenantId==TenantId);
            }
            else
            {
                employeeslist = _repository.Employee.GetDataByGridCondition(x => x.IdCard == search || x.FirstName.Contains(search) || x.MiddleName.Contains(search) || x.LastName.Contains(search) || x.PanCard == search || x.AadharCard == search || x.MobileNumber == search, sortGrid, x => x.TenantId == TenantId);
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
    }
}