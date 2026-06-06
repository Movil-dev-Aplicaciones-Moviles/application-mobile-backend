using BackendAwSmartstay.API.Accommodations.Domain.Model.Entities;
using BackendAwSmartstay.API.Accommodations.Domain.Model.Queries;
using BackendAwSmartstay.API.Accommodations.Domain.Repositories;
using BackendAwSmartstay.API.Accommodations.Domain.Services;

namespace BackendAwSmartstay.API.Accommodations.Application.Internal.QueryServices;

public class RoomTypeQueryService(IRoomTypeRepository roomTypeRepository)
    : IRoomTypeQueryService
{
    public async Task<RoomType?> Handle(GetRoomTypeByIdQuery query)
    {
        return await roomTypeRepository.FindByIdAsync(query.RoomTypeId);
    }

    public async Task<IEnumerable<RoomType>> Handle(GetAllRoomTypesQuery query)
    {
        return await roomTypeRepository.ListAsync();
    }
}

