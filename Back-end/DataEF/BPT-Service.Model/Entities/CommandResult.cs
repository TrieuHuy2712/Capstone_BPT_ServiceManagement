namespace BPT_Service.Model.Entities
{
    public class CommandResult<T> where T : class
    {
        public bool isValid { get; set; }
        public T myModel { get; set; }
        public string errorMessage { get; set; }
    }
}