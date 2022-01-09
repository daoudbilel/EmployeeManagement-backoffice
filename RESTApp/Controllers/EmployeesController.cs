﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApp.Model;
using RESTApp.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            try
            {
                return Ok(await employeeRepository.GetEmployees());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var result = await employeeRepository.GetEmployee(id);
                if (result == null) return NotFound();
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        /*public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                    return BadRequest();
                var createdEmployee = await employeeRepository.AddEmployee(employee);
                return CreatedAtAction(nameof(GetEmployee),
                new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error creating new employee record");
            }
        }*/
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest();
                }
                else
                // Add custom model validation error
                {
                    var emp = await employeeRepository.GetEmployeeByEmail(employee.Email);
                    if (emp != null)
                    {
                        ModelState.AddModelError("email", "Employee email already in use");
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        var createdEmployee = await employeeRepository.AddEmployee(employee);
                        return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId },
                        createdEmployee);
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, Employee employee)
        {
            try
            {
                if (id != employee.EmployeeId)
                    return BadRequest("Employee ID mismatch ");
                var employeeToUpdate = await employeeRepository.GetEmployee(id);

                if (employeeToUpdate == null)
                    return NotFound($"Employee with ID ={id} not found");

                return await employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "ERROR update data");
            }


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            try
            {
                var employeeToDelete = await employeeRepository.GetEmployee(id);

                if (employeeToDelete == null)
                    return NotFound($"Employee with ID ={id} not found");

                return await employeeRepository.DeleteEmployee(id);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  "ERROR delete data");
            }

        }

        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search (string name , Gender gender)
        {
            try
            {
                var result = await employeeRepository.Search(name, gender);
                if(result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }



    }
}
