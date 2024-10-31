using System;
using System.Diagnostics;
using Prometheus;

namespace IO.Swagger.Services;

/// <summary>
/// Сервис для отслеживания метрик запросов, состояний сервера и времени выполнения.
/// </summary>
public class MetricsService
{
    // Счетчик запросов
    private readonly Counter _apiRequestCounter = Prometheus.Metrics.CreateCounter(
        "api_request_total", "Общее количество API-запросов, поступивших на сервер.");

    // Счетчики по категориям HTTP-ответов
    private readonly Counter _response2xxCounter = Prometheus.Metrics.CreateCounter(
        "http_2xx_responses", "Количество успешных ответов (2xx).");
    private readonly Counter _response4xxCounter = Prometheus.Metrics.CreateCounter(
        "http_4xx_responses", "Количество клиентских ошибок (4xx).");
    private readonly Counter _response5xxCounter = Prometheus.Metrics.CreateCounter(
        "http_5xx_responses", "Количество ошибок сервера (5xx).");

    // Активные соединения
    private readonly Gauge _activeConnectionsGauge = Prometheus.Metrics.CreateGauge(
        "active_connections", "Количество активных соединений на сервере.");

    // Время ответа сервера в секундах 
    private readonly Summary _responseTimeSummary = Prometheus.Metrics.CreateSummary(
        "server_response_time_seconds", "Время ответа сервера на запрос.");

    // Счетчик количества подключений
    private readonly Counter _totalConnectionsCounter = Prometheus.Metrics.CreateCounter(
        "total_connections", "Общее количество подключений к серверу.");

    // Счетчик количества отключений
    private readonly Counter _totalDisconnectionsCounter = Prometheus.Metrics.CreateCounter(
        "total_disconnections", "Общее количество отключений от сервера.");

    // Счетчик для отслеживания популярных запросов
    private readonly Counter _popularRequestsCounter = Prometheus.Metrics.CreateCounter(
        "popular_requests_total", "Общее количество запросов по конечной точке и методу.",
        new CounterConfiguration
        {
            LabelNames = new[] { "endpoint", "method" }
        });

    private readonly Gauge _cpuUsageGauge = Prometheus.Metrics.CreateGauge(
        "cpu_usage_percentage", "Процент использования CPU.");

    // Общее количество фильмов в базе данных
    private readonly Gauge _movieCountGauge = Prometheus.Metrics.CreateGauge(
        "movie_count", "Количество фильмов в базе данных.");

    // Общее количество жанров фильмов в базе данных
    private readonly Gauge _genreCountGauge = Prometheus.Metrics.CreateGauge(
        "genre_count", "Количество уникальных жанров в базе данных.");

    // Количество фильмов по жанрам
    private readonly Gauge _moviesPerGenreGauge = Prometheus.Metrics.CreateGauge(
        "movies_per_genre", "Количество фильмов в каждом жанре.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "genre" }
        });

    // Количество фильмов по годам выпуска
    private readonly Gauge _moviesPerYearGauge = Prometheus.Metrics.CreateGauge(
        "movies_per_year", "Количество фильмов по годам выпуска.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "release_year" }
        });

    /// <summary>
    /// Регистрация нового запроса.
    /// </summary>
    /// <param name="endpoint">Имя конечной точки запроса.</param>
    /// <param name="method">Метод HTTP (GET, POST и т.д.).</param>
    public void TrackRequest(string endpoint, string method)
    {
        _apiRequestCounter.Inc();
        _popularRequestsCounter.WithLabels(endpoint, method).Inc();
    }

    /// <summary>
    /// Регистрация HTTP-ответа с определенным статусом.
    /// </summary>
    /// <param name="statusCode">Код статуса HTTP-ответа.</param>
    public void TrackResponse(int statusCode)
    {
        var counter = statusCode switch
        {
            >= 200 and < 300 => _response2xxCounter,
            >= 400 and < 500 => _response4xxCounter,
            >= 500 and < 600 => _response5xxCounter,
            _ => null
        };

        counter?.Inc();
    }

    /// <summary>
    /// Начало отслеживания активного соединения.
    /// </summary>
    public void BeginConnection()
    {
        _activeConnectionsGauge.Inc();
        _totalConnectionsCounter.Inc();
    }

    /// <summary>
    /// Завершение активного соединения.
    /// </summary>
    public void EndConnection()
    {
        _activeConnectionsGauge.Dec();
        _totalDisconnectionsCounter.Inc();
    }

    /// <summary>
    /// Регистрация времени выполнения запроса.
    /// </summary>
    /// <param name="elapsed">Время выполнения запроса.</param>
    public void TrackResponseTime(TimeSpan elapsed)
    {
        _responseTimeSummary.Observe(elapsed.TotalSeconds);
    }

    /// <summary>
    /// Обновление показателя использования CPU.
    /// </summary>
    public void TrackCpuUsage()
    {
        var cpuUsage = GetCpuUsage();
        _cpuUsageGauge.Set(cpuUsage);
    }

    /// <summary>
    /// Получение текущего использования CPU.
    /// </summary>
    private double GetCpuUsage()
    {
        using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        cpuCounter.NextValue();
        System.Threading.Thread.Sleep(100);
        return cpuCounter.NextValue();
    }
}
