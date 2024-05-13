using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;

namespace CommunicationsApp.Infrastructure.Logger;

internal class LoggerService : BaseRepository<ErrorLog>, ILoggerService
{
    private readonly AppDbContext _dbContext;
    private static string _logsFolderPath;

    public LoggerService(IHostEnvironment hostEnv, AppDbContext dbContext) : base(dbContext)
    {
        _logsFolderPath = hostEnv.ContentRootPath + "_Logs";
        _dbContext = dbContext;
        Directory.CreateDirectory(_logsFolderPath);
    }

    public async Task LogErrorAsync(Exception error)
    {
        await WriteErrorAsync(_logsFolderPath, error.ToString());

        await AddAsync(new ErrorLog
        {
            Description = error.Message,
            Source = error.Source,
            OccurredAt = DateTime.Now
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task LogErrorAsync(string message)
    {
        await WriteErrorAsync(_logsFolderPath, message);

        try
        {
            await AddAsync(new ErrorLog
            {
                Description = message,
                OccurredAt = DateTime.Now
            });

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            
        }
    }

    private async Task WriteErrorAsync(string filename, string message)
    {
        filename = $"{filename}_{DateTime.Now:dd-MM-yyyy}";
        message = $"{Environment.NewLine}" +
                  $"{DateTime.Now:F}" +
                  $"{Environment.NewLine}" +
                  $"\t{message}" +
                  $"{Environment.NewLine}{Environment.NewLine}";

        await File.AppendAllTextAsync(Path.Combine(_logsFolderPath, filename), message);

    }
}
