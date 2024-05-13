using CommunicationsApp.Domain.Common;
using MediatR;

namespace CommunicationsApp.Application.Operations.Identity.Commands.ExternalLoginUser;

public record ExternalLoginCommand() : IRequest<Result>;