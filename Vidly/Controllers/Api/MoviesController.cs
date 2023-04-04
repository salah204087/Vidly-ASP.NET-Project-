using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vidly.Dtos;
using Vidly.Models;
using System.Data.Entity;

namespace Vidly.Controllers.Api
{
    public class MoviesController : ApiController
    {
        private ApplicationDbContext _context;
        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }
        public IEnumerable<MovieDto> GetMovies(string query=null)
        {
            var moviesquery = _context.Movies
                 .Include(m => m.Genre)
                 .Where(m => m.NumberAvailabe > 0);
            if(!string.IsNullOrWhiteSpace(query))
                moviesquery=moviesquery.Where(m=>m.Name.Contains(query));
            return moviesquery.ToList().Select(Mapper.Map<Movie,MovieDto>);
        }
        public IHttpActionResult GetMovie(int id)
        {
            var movie = _context.Movies.SingleOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound();
            return Ok(Mapper.Map<Movie, MovieDto>(movie));
        }
        [HttpPost]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult CreateMovie(MovieDto movieDto)
        {
            if(!ModelState.IsValid)
                return BadRequest();
            var movie = Mapper.Map<MovieDto, Movie>(movieDto);
            _context.Movies.Add(movie);
            _context.SaveChanges();
            movieDto.Id=movie.Id;
            return Created(new Uri(Request.RequestUri+"/"+movie.Id),movieDto);
        }
        [HttpPut]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult UpdateMovie(int id,MovieDto movieDto) 
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var movieInDb= _context.Movies.SingleOrDefault(m=>m.Id == id);
            if (movieInDb == null)
                return NotFound();
            Mapper.Map(movieDto, movieInDb);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        [Authorize(Roles = RoleName.CanManageMoviesAndCustomers)]
        public IHttpActionResult DeleteMovie(int id)
        { 
            var movieInDb=_context.Movies.SingleOrDefault(m=>m.Id == id);
            if (movieInDb == null)
                return NotFound();
            _context.Movies.Remove(movieInDb);
            _context.SaveChanges();
            return Ok();
        }
    }
}
