using CommunicationsApp.Application.Common;
using CommunicationsApp.Application.Common.Results.Errors;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Domain.Common.Enums;
using MediatR;

namespace CommunicationsApp.Application.Behaviour.Operations.Multimedia.Queries.GetAllForMessage;

internal class GetAllMediaForMessageQueryHandler : BaseCommandHandler, IRequestHandler<GetAllMediaForMessageQuery, Result<IList<Media>>>
{
    public GetAllMediaForMessageQueryHandler(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Result<IList<Media>>> Handle(GetAllMediaForMessageQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await IsRequestValidAsync(request, cancellationToken);

        if (validationResult.Failed)
            return validationResult.Errors.ToArray();

        var media = await _workUnit.MediaRepository
                                   .GetAllForMessage(request.MessageId, cancellationToken);

        return media.Select(e => new Media(e.Filename, MediaType.FromValue(e.MediaTypeId)))
                    .ToList();
    }

    private async Task<Result> IsRequestValidAsync(GetAllMediaForMessageQuery request, CancellationToken cancellationToken)
    {
        GetAllMediaForMessageQueryValidator validator = new();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult;

        if (!await _workUnit.MessagesRepository
                            .DoesInstanceExistAsync(request.MessageId, cancellationToken))
            return MessageErrors.NotFound(nameof(request.MessageId));

        return Result.Success();
    }
}
