using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using IO.Swagger.Models;
using IO.Swagger.Services;
using IO.Swagger.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System;

namespace IO.Swagger.Controllers
{
    [ApiController]
    public class MovieControllerApiController : ControllerBase
    {
        private readonly MetricsService _metricsService;

        public MovieControllerApiController()
        {
            _metricsService = new MetricsService();
        }

        /// <summary>
        /// Получение списка всех фильмов в библиотеке
        /// </summary>
        /// <response code="200">Список фильмов</response>
        [HttpGet]
        [Route("/allMovies")]
        [ValidateModelState]
        [SwaggerOperation("AllMoviesGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Movie>), description: "Список фильмов")]
        public virtual IActionResult AllMoviesGet()
        {
            string exampleJson = "[{\n  \"duration\": 6,\n  \"releaseDate\": \"2000-01-23\",\n  \"director\": \"director\",\n  \"genre\": \"genre\",\n  \"id\": 0,\n  \"title\": \"title\"\n}]";

            var example = exampleJson != null
                ? JsonConvert.DeserializeObject<List<Movie>>(exampleJson)
                : default(List<Movie>);

            return new ObjectResult(example);
        }

        /// <summary>
        /// Удаление фильма по идентификатору
        /// </summary>
        /// <param name="movieId"></param>
        /// <response code="200">Фильм успешно удалён</response>
        /// <response code="500">Ошибка при удалении фильма</response>
        [HttpDelete]
        [Route("/movie/{movieId}")]
        [ValidateModelState]
        [SwaggerOperation("MovieMovieIdDelete")]
        public virtual IActionResult MovieMovieIdDelete([FromRoute][Required] int? movieId)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            // Логика удаления фильма...

            throw new NotImplementedException();
        }

        /// <summary>
        /// Получение фильма по идентификатору
        /// </summary>
        /// <param name="movieId"></param>
        /// <response code="200">Фильм с указанным идентификатором</response>
        [HttpGet]
        [Route("/movie/{movieId}")]
        [ValidateModelState]
        [SwaggerOperation("MovieMovieIdGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(Movie), description: "Фильм с указанным идентификатором")]
        public virtual IActionResult MovieMovieIdGet([FromRoute][Required] int? movieId)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            string exampleJson = "{\n  \"duration\": 6,\n  \"releaseDate\": \"2000-01-23\",\n  \"director\": \"director\",\n  \"genre\": \"genre\",\n  \"id\": 0,\n  \"title\": \"title\"\n}";

            var example = exampleJson != null
                ? JsonConvert.DeserializeObject<Movie>(exampleJson)
                : default(Movie);

            return new ObjectResult(example);
        }

        /// <summary>
        /// Частичное обновление данных фильма по идентификатору
        /// </summary>
        /// <param name="body"></param>
        /// <param name="movieId"></param>
        /// <response code="200">Данные фильма успешно обновлены</response>
        /// <response code="400">Некорректные данные для обновления</response>
        /// <response code="404">Фильм не найден</response>
        /// <response code="500">Ошибка при обновлении данных фильма</response>
        [HttpPatch]
        [Route("/movie/{movieId}")]
        [ValidateModelState]
        [SwaggerOperation("MovieMovieIdPatch")]
        public virtual IActionResult MovieMovieIdPatch([FromBody] MoviePartialUpdate body, [FromRoute][Required] int? movieId)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Получение списка фильмов с пагинацией
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <response code="200">Список фильмов</response>
        [HttpGet]
        [Route("/movies")]
        [ValidateModelState]
        [SwaggerOperation("MoviesGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Movie>), description: "Список фильмов")]
        public virtual IActionResult MoviesGet([FromQuery] int? page, [FromQuery] int? limit)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            string exampleJson = "[{\n  \"duration\": 6,\n  \"releaseDate\": \"2000-01-23\",\n  \"director\": \"director\",\n  \"genre\": \"genre\",\n  \"id\": 0,\n  \"title\": \"title\"\n}]";

            var example = exampleJson != null
                ? JsonConvert.DeserializeObject<List<Movie>>(exampleJson)
                : default(List<Movie>);

            return new ObjectResult(example);
        }

        /// <summary>
        /// Добавление нового фильма в библиотеку
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Фильм успешно добавлен</response>
        /// <response code="500">Ошибка при добавлении фильма</response>
        [HttpPost]
        [Route("/movies")]
        [ValidateModelState]
        [SwaggerOperation("MoviesPost")]
        public virtual IActionResult MoviesPost([FromBody] Movie body)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Обновление данных фильма
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">Фильм успешно обновлён</response>
        /// <response code="500">Ошибка при обновлении фильма</response>
        [HttpPut]
        [Route("/movies")]
        [ValidateModelState]
        [SwaggerOperation("MoviesPut")]
        public virtual IActionResult MoviesPut([FromBody] Movie body)
        {
            // Отслеживаем запрос
            _metricsService.TrackRequest(HttpContext.Request.Path, HttpContext.Request.Method);

            throw new NotImplementedException();
        }
    }
}
