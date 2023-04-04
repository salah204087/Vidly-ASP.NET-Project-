﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vidly.Dtos;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    public class NewRentalsController : ApiController
    {
        private ApplicationDbContext _context;
        public NewRentalsController()
        {
            _context =new ApplicationDbContext();
        }
        [HttpPost]
        public IHttpActionResult CreateNewRentals(NewRentalDto newRental)
        {
            if(newRental.MovieIds.Count==0)
                return BadRequest("No Movies Ids have been given.");
            var customer = _context.Customers.SingleOrDefault(m => m.Id==newRental.CustomerId);
            if (customer == null)
                return BadRequest("CustomerId is not valid.");

            var movies = _context.Movies.Where(m => newRental.MovieIds.Contains(m.Id)).ToList();
            if (movies.Count != newRental.MovieIds.Count)
                return BadRequest("One or more MoviesIds are invalid.");

            foreach(var movie in movies)
            {
                if (movie.NumberAvailabe == 0)
                    return BadRequest("Movie is not Avilable");
                movie.NumberAvailabe--;
                var rental=new Rental
                {
                    Customer=customer,
                    Movie=movie,
                    DateRented=DateTime.Now,
                    
                };
                _context.Rentals.Add(rental);
            }
            _context.SaveChanges();

            return Ok();
        }
    }
}
