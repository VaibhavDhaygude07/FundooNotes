namespace FundooNotes.API.Controllers
{
    internal class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}