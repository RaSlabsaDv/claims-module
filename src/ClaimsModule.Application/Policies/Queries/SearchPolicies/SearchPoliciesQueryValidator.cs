using FluentValidation;

namespace ClaimsModule.Application.Policies.Queries.SearchPolicies;

public sealed class SearchPoliciesQueryValidator : AbstractValidator<SearchPoliciesQuery>
{
    public SearchPoliciesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("Search term must be at least 2 characters.");

        RuleFor(x => x.MaxResults)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("MaxResults must be between 1 and 50.");
    }
}