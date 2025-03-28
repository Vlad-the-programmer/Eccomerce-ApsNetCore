namespace EcommerceRestApi.Helpers.Data.ResponseModels
{
    public class ResponseModel
    {
        public string Message {  get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();

    }
}
