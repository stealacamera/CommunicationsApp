using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Operations.Multimedia.Queries.GetAllForMessage;

internal class GetAllMediaForMessageQueryHandler : BaseCommandHandler, IRequestHandler<GetAllMediaForMessageQuery, Result<IList<Media>>>
{
    public GetAllMediaForMessageQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<Media>>> Handle(GetAllMediaForMessageQuery request, CancellationToken cancellationToken)
    {
        if (!await _workUnit.MessagesRepository.DoesInstanceExistAsync(request.MessageId, cancellationToken))
            return MessageErrors.NotFound;

        var media = await _workUnit.MediaRepository
                                   .GetAllForMessage(request.MessageId, cancellationToken);

        return media.Select(e => new Media(
                        e.Filename, 
                        MediaType.FromValue(e.MediaTypeId)))
                    .ToList();
    }
}
