using Leo.Project.Portfolio.Api.DbSet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Leo.Project.Portfolio.Api.Model;
using System.Linq.Expressions;
using LinqKit;
using NETCore.MailKit.Core;

namespace Leo.Project.Portfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestEmailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        public RequestEmailController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [HttpPost("GetRequestEmails")]
        public async Task<ActionResult<DataSourceResponse>> GetRequestEmails([FromBody] DataSourceRequest request)
        {
            IQueryable<RequestEmail> query = _context.RequestEmails;

            // Apply filtering if the filter is provided
            if (request.Filter != null && request.Filter.Filters.Any())
            {
                ApplyFilter(ref query, request.Filter);
            }

            // Apply sorting if provided
            if (request.Sort != null && request.Sort.Any())
            {
                ApplySorting(ref query, request.Sort);
            }

            // Pagination: Apply skip and take
            var totalCount = await query.CountAsync();
            var result = await query.Skip(request.Skip).Take(request.Take).ToListAsync();

            // Return the result in the expected format
            var response = new DataSourceResponse
            {
                Total = totalCount,
                Data = result
            };

            return Ok(response);
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromQuery] string ClientEmail, [FromQuery] string Subject, [FromQuery] string Body)
        {
            // Validation to ensure the message is not empty
            if (string.IsNullOrWhiteSpace(Body))
                return BadRequest(new { message = "Message body cannot be empty." });

            try
            {
                // Call the EmailService to send the email with the subject and message
                await _emailService.SendAsync(ClientEmail, Subject, Body);
                return Ok(new { message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                // Log the error and return a generic error message
                return StatusCode(500, new { message = "An error occurred while sending the email.", details = ex.Message });
            }
        }

        private void ApplyFilter(ref IQueryable<RequestEmail> query, Filter filter)
        {
            if (filter.Logic == "or")
            {
                // If the logic is 'or', apply an 'OR' condition
                var predicate = PredicateBuilder.New<RequestEmail>(x => false); // Start with 'false' as the base predicate

                foreach (var filterItem in filter.Filters)
                {
                    if (!string.IsNullOrEmpty(filterItem.Field) && !string.IsNullOrEmpty(filterItem.Operator) && !string.IsNullOrEmpty(filterItem.Value))
                    {
                        predicate = predicate.Or(BuildPredicate(filterItem));
                    }
                }

                query = query.Where(predicate);
            }
            else
            {
                // Default behavior (AND logic)
                foreach (var filterItem in filter.Filters)
                {
                    if (!string.IsNullOrEmpty(filterItem.Field) && !string.IsNullOrEmpty(filterItem.Operator) && !string.IsNullOrEmpty(filterItem.Value))
                    {
                        query = query.Where(BuildPredicate(filterItem));
                    }
                }
            }
        }

        private Expression<Func<RequestEmail, bool>> BuildPredicate(FilterItem filterItem)
        {
            // Dynamically build the predicate based on the operator
            switch (filterItem.Operator.ToLower())
            {
                case "contains":
                    return x => EF.Functions.Like(EF.Property<string>(x, filterItem.Field), $"%{filterItem.Value}%");
                case "equals":
                    return x => EF.Property<string>(x, filterItem.Field) == filterItem.Value;
                case "startswith":
                    return x => EF.Functions.Like(EF.Property<string>(x, filterItem.Field), $"{filterItem.Value}%");
                case "endswith":
                    return x => EF.Functions.Like(EF.Property<string>(x, filterItem.Field), $"%{filterItem.Value}");
                default:
                    throw new InvalidOperationException($"Operator '{filterItem.Operator}' is not supported.");
            }
        }
        private void ApplySorting(ref IQueryable<RequestEmail> query, List<Sort> sort)
        {
            foreach (var sorting in sort)
            {
                if (sorting.Dir == "asc")
                {
                    query = query.OrderBy(x => EF.Property<object>(x, sorting.Field));
                }
                else if (sorting.Dir == "desc")
                {
                    query = query.OrderByDescending(x => EF.Property<object>(x, sorting.Field));
                }
            }
        }

    }

}
