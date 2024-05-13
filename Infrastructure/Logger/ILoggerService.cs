namespace CommunicationsApp.Infrastructure.Logger;

// fix logout
// clean up those older than 1 month
// test logout, signup, login w/w/o exeternal
// add logger
// add multimedia
// add del channel (make other member leader randomly)
// add / rem members

public interface ILoggerService
{
    // log locally and db?

    Task LogErrorAsync(Exception error);
    Task LogErrorAsync(string message);
}
