using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalLoginUser;

public record ExternalLoginCommand() : IRequest<Result>;