using BPT_Service.Model.Enums;

namespace BPT_Service.Model.Interfaces
{
    public interface ISwitchable
    {
         Status Status { set; get; }
    }
}