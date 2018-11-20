○using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EIS.WebAPI.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public EmployeeController(IRepositoryWrapper repository)
        {
            _repository= repository  ;
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
        public IActionResult Create([FromBody]Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.Create(person);
            _repository.Employee.Save();
            return CreatedAtAction("GetAllEmployee", new { id = person.Id }, person);
        }

        [HttpPut("{id}")]
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
    }
}