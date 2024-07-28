using AutoMapper;
using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RidesController : ControllerBase
    {

        private readonly CarpoolPlatformDbContext _dbContext;
        private readonly IRideRepository _rideRepository;
        private readonly IMapper _mapper;

        public RidesController(CarpoolPlatformDbContext dbContext, IRideRepository rideRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _rideRepository = rideRepository;
            _mapper = mapper;
        }

/*        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var rides = await _rideRepository.GetAllAsync(includeProperties: "Genres");

            return Ok(_mapper.Map<List<MovieDTO>>(movies));
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMovieById([FromRoute] int id)
        {
            var movie = await _rideRepository.GetAsync(x => x.Id == id, includeProperties: "Genres");

            if (movie == null)
                return NotFound();

            return Ok(_mapper.Map<MovieDTO>(movie));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreateDTO movieCreateDTO)
        {
            var movieDomainModel = _mapper.Map<Movie>(movieCreateDTO);

            movieDomainModel = await _rideRepository.CreateAsync(movieDomainModel);

            var movieDTO = _mapper.Map<MovieDTO>(movieDomainModel);

            return CreatedAtAction(nameof(GetMovieById), new { id = movieDomainModel.Id }, movieDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateMovie([FromRoute] int id, [FromBody] MovieUpdateDTO movieUpdateDTO)
        {
            var movieDomainModel = await _rideRepository.GetAsync(x => x.Id == id);

            if (movieDomainModel == null)
                return NotFound();

            movieDomainModel = _mapper.Map<Movie>(movieUpdateDTO);

            movieDomainModel = await _rideRepository.UpdateAsync(movieDomainModel);

            return Ok(_mapper.Map<MovieDTO>(movieDomainModel));
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteMovie([FromRoute] int id)
        {
            var movieDomainModel = await _rideRepository.GetAsync(x => x.Id == id);

            if (movieDomainModel == null)
                return NotFound();

            movieDomainModel = await _rideRepository.RemoveAsync(movieDomainModel);

            return Ok(_mapper.Map<MovieDTO>(movieDomainModel));
        }*/

    }
}
