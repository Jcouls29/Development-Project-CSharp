// EVAL: Tests for GlobalExceptionMiddleware verifying correct HTTP status codes
// and error response format for different exception types.

using Interview.Web.Middleware;
using Interview.Web.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Tests
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly Mock<IHostEnvironment> _mockEnv;

        public GlobalExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _mockEnv = new Mock<IHostEnvironment>();
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
        }

        private GlobalExceptionMiddleware CreateMiddleware(RequestDelegate next)
        {
            return new GlobalExceptionMiddleware(next, _mockLogger.Object, _mockEnv.Object);
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<ErrorResponse?> ReadErrorResponse(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<ErrorResponse>(json);
        }

        [Fact]
        public async Task InvokeAsync_NoException_PassesThrough()
        {
            var middleware = CreateMiddleware(_ => Task.CompletedTask);
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(200, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ArgumentException_Returns400()
        {
            var middleware = CreateMiddleware(_ => throw new ArgumentException("Bad param"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(400, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("VALIDATION_ERROR", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_SqlException547_Returns400WithReferenceNotFound()
        {
            // Create a SqlException with error number 547 (FK violation)
            var sqlEx = CreateSqlException(547);
            var middleware = CreateMiddleware(_ => throw sqlEx);
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(400, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("REFERENCE_NOT_FOUND", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_SqlExceptionTimeout_Returns503()
        {
            var sqlEx = CreateSqlException(-2);
            var middleware = CreateMiddleware(_ => throw sqlEx);
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(503, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("DATABASE_TIMEOUT", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_SqlExceptionConnectionFailure_Returns503()
        {
            var sqlEx = CreateSqlException(53);
            var middleware = CreateMiddleware(_ => throw sqlEx);
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(503, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("DATABASE_UNAVAILABLE", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_TimeoutException_Returns503()
        {
            var middleware = CreateMiddleware(_ => throw new TimeoutException("Timed out"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(503, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("REQUEST_TIMEOUT", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_GenericException_Returns500()
        {
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Something broke"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal(500, context.Response.StatusCode);
            var error = await ReadErrorResponse(context);
            Assert.Equal("INTERNAL_ERROR", error!.Code);
        }

        [Fact]
        public async Task InvokeAsync_GenericException_Development_IncludesMessage()
        {
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Detailed error info"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            var error = await ReadErrorResponse(context);
            Assert.Contains("Detailed error info", error!.Message);
        }

        [Fact]
        public async Task InvokeAsync_GenericException_Production_HidesMessage()
        {
            _mockEnv.Setup(e => e.EnvironmentName).Returns("Production");
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Secret details"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            var error = await ReadErrorResponse(context);
            Assert.DoesNotContain("Secret details", error!.Message);
            Assert.Contains("unexpected error", error.Message);
        }

        [Fact]
        public async Task InvokeAsync_AllResponses_HaveJsonContentType()
        {
            var middleware = CreateMiddleware(_ => throw new Exception("test"));
            var context = CreateHttpContext();

            await middleware.InvokeAsync(context);

            Assert.Equal("application/json", context.Response.ContentType);
        }

        /// <summary>
        /// Creates a SqlException with a specific error number using reflection.
        /// SqlException has no public constructor, so we must use reflection.
        /// </summary>
        private static SqlException CreateSqlException(int errorNumber)
        {
            // SqlException cannot be instantiated directly -- use reflection
            var errorCollection = (SqlErrorCollection)typeof(SqlErrorCollection)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!
                .Invoke(null);

            var sqlError = (SqlError)typeof(SqlError)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int), typeof(uint), typeof(Exception) }, null)!
                .Invoke(new object?[] { errorNumber, (byte)0, (byte)0, "server", "errMsg", "procedure", 0, (uint)0, null });

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(errorCollection, new object[] { sqlError });

            var sqlException = (SqlException)typeof(SqlException)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) }, null)!
                .Invoke(new object?[] { "Test SQL Exception", errorCollection, null, Guid.Empty });

            return sqlException;
        }
    }
}
