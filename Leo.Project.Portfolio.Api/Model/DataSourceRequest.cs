using Leo.Project.Portfolio.Api.DbSet;

namespace Leo.Project.Portfolio.Api.Model
{
    public class DataSourceRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<Sort> Sort { get; set; } = new List<Sort>();  // Default empty list
        public Filter Filter { get; set; } = new Filter(); // Default empty filter
        public List<Group> Group { get; set; } = new List<Group>(); // Default empty list
        public List<Aggregate> Aggregate { get; set; } = new List<Aggregate>(); // Default empty list
    }

    public class Sort
    {
        public string Field { get; set; }
        public string Dir { get; set; } // "asc" or "desc"
    }

    public class Filter
    {
        public string Logic { get; set; }
        public List<FilterItem> Filters { get; set; } = new List<FilterItem>(); // Default empty list
    }

    public class FilterItem
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class Group
    {
        public string Field { get; set; }
        public string Dir { get; set; }
        public List<Aggregate> Aggregates { get; set; }
    }

    public class Aggregate
    {
        public string Field { get; set; }
        public string AggregateType { get; set; } // e.g., "sum", "count", etc.
    }
    public class DataSourceResponse
    {
        public int Total { get; set; } // Total number of records
        public IEnumerable<RequestEmail> Data { get; set; } // Paginated data
    }
}
