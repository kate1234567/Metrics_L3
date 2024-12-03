using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IO.Swagger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using IO.Swagger.Attributes;

namespace MovieApp.Controllers
{
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private List<Movie> Movies { get; set; } = new List<Movie>();

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
            _logger.LogInformation("MovieController инициализирован.");
        }

        /// <summary>
        /// Добавление нового фильма
        /// </summary>
        [HttpPost]
        [Route("movie")]
        [ValidateModelState]
        [SwaggerOperation("MoviePost")]
        [SwaggerResponse(200, "Фильм успешно добавлен", typeof(Movie))]
        public virtual IActionResult MoviePost([FromBody] Movie body)
        {
            Movies = Movies.Append(body).ToList();
            _logger.LogInformation($"Фильм '{body.Title}' добавлен с ID {body.Id}.");
            return Ok(body);
        }

        /// <summary>
        /// Обновление информации о фильме
        /// </summary>
        [HttpPut]
        [Route("movie")]
        [ValidateModelState]
        [SwaggerOperation("MoviePut")]
        [SwaggerResponse(200, "Фильм успешно обновлен", typeof(Movie))]
        [SwaggerResponse(404, "Фильм не найден")]
        public virtual IActionResult MoviePut([FromBody] Movie body)
        {
            var movie = Movies.FirstOrDefault(x => x.Id == body.Id);
            if (movie == null)
            {
                _logger.LogWarning($"Фильм с ID {body.Id} не найден для обновления.");
                return NotFound();
            }

            movie.Title = body.Title;
            movie.Director = body.Director;
            movie.ReleaseDate = body.ReleaseDate;
            movie.Genre = body.Genre;
            _logger.LogInformation($"Информация о фильме '{body.Title}' обновлена.");
            return Ok(movie);
        }

        /// <summary>
        /// Удаление фильма по ID
        /// </summary>
        [HttpDelete]
        [Route("movie/{movieId}")]
        [ValidateModelState]
        [SwaggerOperation("MovieDelete")]
        [SwaggerResponse(200, "Фильм удален")]
        [SwaggerResponse(404, "Фильм не найден")]
        public virtual IActionResult MovieDelete([FromRoute][Required] int movieId)
        {
            var movie = Movies.FirstOrDefault(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogWarning($"Фильм с ID {movieId} не найден для удаления.");
                return NotFound();
            }

            Movies = Movies.Where(x => x.Id != movieId).ToList();
            _logger.LogInformation($"Фильм с ID {movieId} удален.");
            return Ok();
        }

        /// <summary>
        /// Получение информации о фильме по ID
        /// </summary>
        [HttpGet]
        [Route("movie/{movieId}")]
        [ValidateModelState]
        [SwaggerOperation("MovieGet")]
        [SwaggerResponse(200, "Фильм найден", typeof(Movie))]
        [SwaggerResponse(404, "Фильм не найден")]
        public virtual IActionResult MovieGet([FromRoute][Required] int movieId)
        {
            var movie = Movies.FirstOrDefault(x => x.Id == movieId);
            if (movie == null)
            {
                _logger.LogWarning($"Фильм с ID {movieId} не найден.");
                return NotFound();
            }

            _logger.LogInformation($"Информация о фильме с ID {movieId} получена.");
            return Ok(movie);
        }

        /// <summary>
        /// Получение списка всех фильмов
        /// </summary>
        [HttpGet]
        [Route("movies")]
        [ValidateModelState]
        [SwaggerOperation("MoviesGet")]
        [SwaggerResponse(200, "Список фильмов", typeof(List<Movie>))]
        public virtual IActionResult MoviesGet()
        {
            _logger.LogInformation("Получен список всех фильмов.");
            return Ok(Movies);
        }
    }
}
