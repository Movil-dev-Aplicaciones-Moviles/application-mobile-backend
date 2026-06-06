namespace BackendAwSmartstay.API.Accommodations.Domain.Model.Commands;

public record CreateRoomCommand(
    int HotelId,  
    int RoomTypeId, 
    decimal Price, 
    string Description, 
    List<string> Amenities
);