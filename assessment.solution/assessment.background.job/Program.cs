// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;

if (Directory.Exists(args[0]))
{
  var incomingDirectory = Path.Combine(args[0], "incoming");
  var inprogressDirectory = Path.Combine(args[0], "inprogress");
  var resultDirectory = Path.Combine(args[0], "result");
  var archivedDirectory = Path.Combine(args[0], "archived");
  foreach (var incomingFilePath in Directory.GetFiles(incomingDirectory))
  {
    var incomingFileInfo = new FileInfo(incomingFilePath);
    //var inprogressFileInfo = new FileInfo(incomingFilePath);
    Console.WriteLine("Processing incoming file: {0}", incomingFileInfo.Name);
    if (incomingFileInfo.Exists)
    {
      var inprogressFileInfo = new FileInfo(Path.Combine(inprogressDirectory, incomingFileInfo.Name));
      incomingFileInfo.MoveTo(inprogressFileInfo.FullName);
      Console.WriteLine("Move to in progress: {0}", incomingFileInfo.Name);
      using (var fileStreamRead = File.OpenRead(inprogressFileInfo.FullName))
      using (var streamReader = new StreamReader(fileStreamRead, Encoding.UTF8, true))
      using (var fileStreamWrite = File.OpenWrite(Path.Combine(resultDirectory, 
        $"{inprogressFileInfo.Name.Remove(inprogressFileInfo.Name.Length - inprogressFileInfo.Extension.Length)}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff")}{inprogressFileInfo.Extension}")))
      using (var streamWriter = new StreamWriter(fileStreamWrite, Encoding.UTF8))
      using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
      using (var sqlCmd = sqlConnection.CreateCommand())
      {

        var firstLine = streamReader.ReadLine();
        if (firstLine == null)
        {
          return;
        }
        streamWriter.WriteLine($"{firstLine}|status|remarks");
        var headerNames = firstLine.Split("|");
        string line;
        sqlCmd.Connection = sqlConnection;
        sqlCmd.CommandText = "dbo.insertTransaction";
        sqlCmd.CommandType = CommandType.StoredProcedure;

        var accountNumberParam = new SqlParameter("@AccountNum", SqlDbType.VarChar);
        var amountParam = new SqlParameter("@Amount", SqlDbType.Decimal);
        var descriptionParam = new SqlParameter("@Description", SqlDbType.NVarChar);
        var transactionDateParam = new SqlParameter("@TransactionDate", SqlDbType.Date);
        var transactionTimeParam = new SqlParameter("@TransactionTime", SqlDbType.Time);
        var customerIdParam = new SqlParameter("@CustomerId", SqlDbType.VarChar);

        sqlCmd.Parameters.Add(accountNumberParam);
        sqlCmd.Parameters.Add(amountParam);
        sqlCmd.Parameters.Add(descriptionParam);
        sqlCmd.Parameters.Add(customerIdParam);
        sqlCmd.Parameters.Add(transactionDateParam);
        sqlCmd.Parameters.Add(transactionTimeParam);
        sqlCmd.Parameters.AddWithValue("@CreatedBy", "assessment.job");
        sqlConnection.Open();
        while ((line = streamReader.ReadLine()) != null)
        {
          var status = "";
          try
          {
            var lineValues = line.Split("|");

            var accountNumber = lineValues[0];
            var transactionAmount = decimal.Parse(lineValues[1]);
            var description = lineValues[2];
            var transactionDate = DateTime.ParseExact(lineValues[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var transactionTime = TimeSpan.ParseExact(lineValues[4], "hh\\:mm\\:ss", CultureInfo.InvariantCulture);

            var customerId = lineValues[5];
            accountNumberParam.Value = accountNumber;
            amountParam.Value = transactionAmount;
            descriptionParam.Value = description;
            transactionDateParam.Value = transactionDate;
            transactionTimeParam.Value = transactionTime;
            customerIdParam.Value = customerId;

            var scalar = sqlCmd.ExecuteScalar();

            Console.WriteLine("Account Num: {0}", accountNumber);
            Console.WriteLine("Customer Id: {0}", customerId);
            Console.WriteLine("Description: {0}", description);
            Console.WriteLine("Date/Time: {0:yyyy-MM-dd}-{1:hh\\:mm\\:ss}", transactionDate, transactionTime);
            Console.WriteLine("Amount: {0:0.00}", transactionAmount);
            status = "success|OK";

          }
          catch (SqlException ex)
          {
            status = $"error|{ex.Message}";
            Console.WriteLine(ex.Message);
          }
          catch (Exception ex)
          {
            status = $"error|{ex.Message}";
            Console.WriteLine(ex.Message);
          }
          finally
          {
            streamWriter.WriteLine($"{line}|{status}");
          }
        }
        sqlConnection.Close();
      }
      inprogressFileInfo.MoveTo(Path.Combine(archivedDirectory, inprogressFileInfo.Name));
    }
  }

}
else
{
  throw new ArgumentException("Invalid batch job directory");
}