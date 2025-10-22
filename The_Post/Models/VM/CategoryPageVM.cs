using Azure.Data.Tables;

namespace The_Post.Models.VM
{
    public class CategoryPageVM
    {
        public List<Article> Articles { get; set; } = new List<Article>();
        public string Category { get; set; }

        public List<TableEntity> HistoricalElectricityPrices { get; set; }

        public bool NoHistoricalData { get; set; }

        public string SelectedDate { get; set; }
    }
}
