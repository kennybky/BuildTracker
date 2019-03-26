using BuildTrackerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BuildTrackerApi.Middleware
{
    public class RestExceptionHander
    {
        private readonly RequestDelegate next;

        public const int SqlServerViolationOfUniqueIndex = 2601;
        public const int SqlServerViolationOfUniqueConstraint = 2627;

        public const int SqlForeignKeyContraint = 547;

        public RestExceptionHander(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await HandleDatabaseExceptionAsync(context, ex);
            }
            catch (DbUpdateException dbUpdateEx)
            {
                var sqlEx = dbUpdateEx?.InnerException as SqlException;
                if (sqlEx != null)
                {
                    await HandleSqlExceptionAsync(context, sqlEx);
                }
                else
                {
                    await HandleDatabaseExceptionAsync(context, dbUpdateEx);
                }
            } catch(SqlException ex)
            {
                await HandleSqlExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleDatabaseExceptionAsync(HttpContext context, DbUpdateException ex)
        {
            var code = HttpStatusCode.BadRequest;
            var message = "The database encountered an unexpected error. Please try again or  contact the Administrator if it persists";

            var result = JsonConvert.SerializeObject(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            var code = HttpStatusCode.InternalServerError; 

            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }


        private static Task HandleAppExceptionAsync(HttpContext context, AppException appException)
        {

            var code = appException.StatusCode;

            var result = JsonConvert.SerializeObject(new { error = appException.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        private static Task HandleSqlExceptionAsync(HttpContext context, SqlException sqlEx)
        {
            var code = HttpStatusCode.BadRequest;
            var message = "The Request could not be carried out due to an error in the database or connection";

            if (sqlEx.Number == SqlServerViolationOfUniqueIndex ||
          sqlEx.Number == SqlServerViolationOfUniqueConstraint)
            {
                code = HttpStatusCode.Conflict;
                message = "This resource already exists in the database.";
            }
            else if (sqlEx.Number == SqlForeignKeyContraint)
            {
                code = HttpStatusCode.Forbidden;
                message = "There was a foreign key conflict.";
            }

            var result = JsonConvert.SerializeObject(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
