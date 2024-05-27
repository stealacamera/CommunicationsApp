using CommunicationsApp.Application.Common;
using CommunicationsApp.Domain.Abstractions;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.IsUserMemberOfChannel;

internal sealed class IsUserMemberOfChannelQueryHandler : BaseCommandHandler, IRequestHandler<IsUserMemberOfChannelQuery, bool>
{
    public IsUserMemberOfChannelQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<bool> Handle(IsUserMemberOfChannelQuery request, CancellationToken cancellationToken)
    {
        IsUserMemberOfChannelQueryValidator requestValidator = new();
        var validationResult = await requestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return false;

        var membership = await _workUnit.ChannelMembersRepository
                                        .GetByIdsAsync(request.UserId, request.ChannelId, cancellationToken);

        return membership != null;
    }
}
