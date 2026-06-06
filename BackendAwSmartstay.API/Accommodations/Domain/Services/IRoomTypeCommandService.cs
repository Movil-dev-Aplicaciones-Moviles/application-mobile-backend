using BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Entities;

namespace BackendAwSmartstay.API.Accommodations.Domain.Services;


public interface IRoomTypeCommandService
{

    public Task<RoomType?> Handle(CreateRoomTypeCommand command);
}

