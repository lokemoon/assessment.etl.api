namespace assessment.web.api.models
{
    public class Paginable<T>
    {
        public IList<T> Items { get; set; }
        public int Count { get; set; }
    }
}