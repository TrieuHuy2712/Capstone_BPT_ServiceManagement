namespace BPT_Service.Model.Interfaces
{
    public class IHasOwner<T>
    {
        T OwnerId { set; get; }
    }
}