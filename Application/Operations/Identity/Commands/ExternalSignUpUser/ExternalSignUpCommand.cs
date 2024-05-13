using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ExternalSignUpUser;

public record ExternalSignUpCommand() : IRequest<Result>;