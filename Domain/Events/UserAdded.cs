using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Events;

public record UserAdded(
    int Id, 
    string Email, 
    string EmailConfirmationBaseUrl, 
    DateTime OccurredAt) 
    : BaseEvent(OccurredAt);