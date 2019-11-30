namespace BPT_Service.Model.Interfaces
{
    public interface IHasSoftDelete
    {
         bool IsDeleted { set; get; }
    }
}