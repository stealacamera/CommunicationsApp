using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Events;

public record UserVerificationResubmitted(
    string Email, 
    string EmailVerificationBaseUrl, 
    DateTime OccurredAt) 
    : BaseEvent(OccurredAt);
