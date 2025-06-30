namespace EventOn.API.Models.Enums;

public enum EventInteractionType
{
    LikeEvent, // low
    LikeComment,
    PostComment,
    PostImage, // medium
    PostVideo,
    AddToFavourites,
    BuyTicket // big
}