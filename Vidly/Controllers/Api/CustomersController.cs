using AutoMapper;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Vidly.Dtos;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    public class CustomersController : ApiController
    {
        private ApplicationDbContext _context;
        public CustomersController()
        {
            _context=new ApplicationDbContext();
        }
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult GetCustomers(string query=null)
        {
            var customersQuery = _context.Customers
                .Include(m => m.MembershipType);
            if(!string.IsNullOrWhiteSpace(query))
                customersQuery=customersQuery.Where(c=>c.Name.Contains(query));

            var customerDtos=customersQuery
                .ToList()
                .Select(Mapper.Map<Customer, CustomerDto>);

            return Ok(customerDtos);
        }
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult GetCustomer(int id)
        {
            var customer=_context.Customers.SingleOrDefault(x => x.Id == id);
            if (customer == null)
                return NotFound();
            return Ok(Mapper.Map<Customer,CustomerDto>(customer));
        }
        [HttpPost]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult CreateCustomer(CustomerDto customerDto)
        {
            if(!ModelState.IsValid)
                return BadRequest();
            var customer=Mapper.Map<CustomerDto,Customer>(customerDto);
            _context.Customers.Add(customer);
            _context.SaveChanges();
            customerDto.Id= customer.Id;
            return Created(new Uri(Request.RequestUri+"/"+customer.Id),customerDto);
        }
        [HttpPut]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult UpdateCustomer(int id, CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var customerInDb=_context.Customers.SingleOrDefault(m=>m.Id == id);
            if (customerInDb==null)
                return NotFound();
            Mapper.Map(customerDto, customerInDb); 
            _context.SaveChanges();
            return Ok();

        }
        [HttpDelete]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult DeleteCustomer(int id)
        {
            var customerInDb = _context.Customers.SingleOrDefault(m => m.Id == id);
            if (customerInDb == null)
                return NotFound();
            _context.Customers.Remove(customerInDb);
            _context.SaveChanges();
            return Ok();
        }
    }
}
