using System.Collections.Generic;

namespace BPT_Service.Common.Dtos
{
    public class PagedResult<T> : Dtos.PagedResultBase where T:class
    {
        public PagedResult()
        {
            Results = new List<T>();
        }
        public IList<T> Results { get; set; }
    }
}