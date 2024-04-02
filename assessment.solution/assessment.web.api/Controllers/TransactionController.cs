using assessment.web.api.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using static assessment.web.api.Controllers.TransactionController;

namespace assessment.web.api.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public partial class TransactionController : ControllerBase
  {
    private IConfiguration _config;
    public TransactionController(IConfiguration config)
    {
      _config = config;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return Ok();
    }
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetTransactionById(int id)
    {
      using (var sqlConn = new SqlConnection(_config["ConnectionStrings:default"]))
      using (var sqlCmd = sqlConn.CreateCommand())
      {
        sqlCmd.CommandText = "[dbo].[getTransactionById]";
        sqlCmd.CommandType = CommandType.StoredProcedure;
        sqlCmd.Connection = sqlConn;

        sqlCmd.Parameters.AddWithValue("@Id", id);
        
        try
        {
          sqlConn.Open();
          using (var reader = await sqlCmd.ExecuteReaderAsync())
          {
            if (!await reader.ReadAsync())
            {
              return NotFound();
            }
            var transactionDate = reader.GetDateTime("transactionDate");
            var transactionTime = reader.GetTimeSpan(reader.GetOrdinal("transactionTime"));
            return Ok(new TransactionModel
            {
              Id = reader.GetInt32("id"),
              AccountNum = reader.GetString("accountnum"),
              CustomerId = reader.GetString("CustomerId"),
              Amount = reader.GetDecimal("amount"),
              Description = reader.GetString("description"),
              TransactionOn = transactionDate.Add(transactionTime),
              LastModifyTime = reader.GetDateTime("LastModifyTime")
            });
          }
        }
        catch (SqlException ex)
        {
          throw ex;
        }
        catch (Exception ex)
        {
          throw ex;
        }
      }
    }
    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> PatchAsync(int id, [FromBody] UpdatePayload updatePayload)
    {
      using (var sqlConn = new SqlConnection(_config["ConnectionStrings:default"]))      
      using (var sqlCmd = sqlConn.CreateCommand())
      {
        sqlCmd.CommandText = "[dbo].[updateTransaction]";
        sqlCmd.CommandType = CommandType.StoredProcedure;
        sqlCmd.Connection = sqlConn;
        
        sqlCmd.Parameters.AddWithValue("@Id", id);
        sqlCmd.Parameters.AddWithValue("@LastModifyBy", HttpContext.User.Claims.First().Value);
        //sqlCmd.Parameters.AddWithValue("@PreviousLastModifyTime", updatePayload.previousLastModifyTime);
        sqlCmd.Parameters.AddWithValue("@LastModifyTime", DateTime.Now);
        sqlCmd.Parameters.AddWithValue("@Description", updatePayload.description);
        sqlConn.Open();
        using (var sqlTransaction = sqlConn.BeginTransaction())
        {
          sqlCmd.Transaction = sqlTransaction;
          try
          {
            var result= await EvaluateTransactionUpdate(sqlCmd);
            await sqlTransaction.CommitAsync();
            return result;
          }
          catch (SqlException ex)
          {
            sqlTransaction.Rollback();
            throw ex;
          }
          catch (Exception ex)
          {
            sqlTransaction.Rollback();
            throw ex;
          }
        }
        
      }

      async Task<IActionResult> EvaluateTransactionUpdate(SqlCommand sqlCmd)
      {
        using (var reader = await sqlCmd.ExecuteReaderAsync())
        {
          if (await reader.ReadAsync())
          {
            if (reader.GetInt32(0) == 404)
            {
              return NotFound();
            }
            //if (reader.GetInt32(0) == 409)
            //{
            //  return Conflict();
            //}
            return Accepted();
          }
          return NotFound();
        }
      }
    }
    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> SearchAsync(
      [FromQuery(Name = "de")] string? description,
      [FromQuery(Name = "ci")] string? customerId,
      [FromQuery(Name = "an")] string? accountNum,
      [FromQuery] int page = 1,
      [FromQuery] int limit = 10)
    {
      if (page < 1) { page = 1; }
      if (limit < 1) { limit = 10; }
      if (limit > 1000) { limit = 1000; }
      using (var sqlConn = new SqlConnection(_config["ConnectionStrings:default"]))
      using (var sqlCmd = sqlConn.CreateCommand())
      {
        sqlCmd.CommandText = "[dbo].[searchTransactions]";
        sqlCmd.CommandType = CommandType.StoredProcedure;
        sqlCmd.Connection = sqlConn;
        sqlCmd.Parameters.AddWithValue("@Offset", (page - 1) * limit);
        sqlCmd.Parameters.AddWithValue("@Limit", limit);
        sqlCmd.Parameters.AddWithValue("@CustomerId", customerId);
        sqlCmd.Parameters.AddWithValue("@AccountNum", accountNum);
        sqlCmd.Parameters.AddWithValue("@Description", description);
        sqlConn.Open();

        using (var reader = await sqlCmd.ExecuteReaderAsync())
        {

          var transactionModels = new List<TransactionModel>();
          await reader.ReadAsync();
          var count = reader.GetInt32(0);
          if (count > 0)
          {
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
              var transactionDate = reader.GetDateTime("transactionDate");
              var transactionTime = reader.GetTimeSpan(reader.GetOrdinal("transactionTime"));
              transactionModels.Add(new TransactionModel
              {
                Id = reader.GetInt32("id"),
                AccountNum = reader.GetString("accountnum"),
                CustomerId = reader.GetString("CustomerId"),
                Amount = reader.GetDecimal("amount"),
                Description = reader.GetString("description"),
                TransactionOn = transactionDate.Add(transactionTime),
                LastModifyTime = reader.GetDateTime("LastModifyTime")
              });
            }
          }
          return Ok(new Paginable<TransactionModel>()
          {
            Count = count,
            Items = transactionModels
          });
        }
      };
    }
  }
}
