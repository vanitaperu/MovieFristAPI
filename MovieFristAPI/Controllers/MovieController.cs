using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieFristAPI.Data;
using MovieFristAPI.Models;
using MovieFristAPI.Models.DTO;
using Nest;
using System.Collections.Generic;
using System.Linq;

namespace MovieFristAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
       private readonly ApplicationDbContext _db;
        public MovieController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MovieDTO>> GetMovies()
        {
          

            return Ok(_db.Movies.ToList());

        }
        [HttpGet("{id:int}", Name = "GetMovie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type =typeof(MovieDTO))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        public ActionResult<MovieDTO> GetMovie(int id)
        {
            if (id == 0)
            { 
                
                return BadRequest();
            }
            var movie = _db.Movies.FirstOrDefault(u => u.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<MovieDTO> CreateMovie([FromBody] MovieDTO movieDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (_db.Movies.FirstOrDefault(u => u.Name.ToLower() == movieDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Movie already Exist!");
                return BadRequest(ModelState);
            }
            if (movieDTO == null)
            {
                return BadRequest(movieDTO);
            }
            if (movieDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Movie model = new()
            {
                Amenity = movieDTO.Amenity,
                Details = movieDTO.Details,
                Id = movieDTO.Id,
                ImageUrl = movieDTO.ImageUrl,
                Name = movieDTO.Name,
                Occupancy = movieDTO.Occupancy,
                Rate = movieDTO.Rate,
                Sqft = movieDTO.Sqft

            };

            _db.Movies.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetMovie", new { id = movieDTO.Id }, movieDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteMovie")]

        public IActionResult DeleteMovie(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var movie = _db.Movies.FirstOrDefault(u => u.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            _db.Movies.Remove(movie);
            _db.SaveChanges();  
            return NoContent();
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        
        [HttpPut("{id:int}", Name = "UpdateMovie")]
        public IActionResult UpdateMovie(int id, [FromBody] MovieDTO movieDTO)
        {
            if (movieDTO == null || id != movieDTO.Id)
            {
                return BadRequest();
            }
            //var movie = MovieStore.movieList.FirstOrDefault(u => u.Id == id);
            //movie.Name = movieDTO.Name;
            //movie.Sqft = movieDTO.Sqft;
            //movie.Occupancy = movieDTO.Occupancy;

            Movie model = new()
            {
                Amenity = movieDTO.Amenity,
                Details = movieDTO.Details,
                Id = movieDTO.Id,
                ImageUrl = movieDTO.ImageUrl,
                Name = movieDTO.Name,
                Occupancy = movieDTO.Occupancy,
                Rate = movieDTO.Rate,
                Sqft = movieDTO.Sqft

            };
            _db.Movies.Update(model);
            _db.SaveChanges();
            return NoContent();
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}", Name = "UpdatePartialMovie")]
        public IActionResult UpdatePartialMovie(int id, JsonPatchDocument<MovieDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var movie = _db.Movies.AsNoTracking().FirstOrDefault(u => u.Id == id);
            
            movie.Name = "new name";
            _db.SaveChanges();
            
            MovieDTO  movieDTO = new()
            {
                Amenity = movie.Amenity,
                Details = movie.Details,
                Id = movie.Id,
                ImageUrl = movie.ImageUrl,
                Name = movie.Name,
                Occupancy = movie.Occupancy,
                Rate = movie.Rate,
                Sqft = movie.Sqft

            };

            if (movie == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(movieDTO , ModelState);

            Movie model = new Movie()
            {
                Amenity = movieDTO.Amenity,
                Details = movieDTO.Details,
                Id = movieDTO.Id,
                ImageUrl = movieDTO.ImageUrl,
                Name = movieDTO.Name,
                Occupancy = movieDTO.Occupancy,
                Rate = movieDTO.Rate,
                Sqft = movieDTO.Sqft

            };

            _db.Movies.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }

}
