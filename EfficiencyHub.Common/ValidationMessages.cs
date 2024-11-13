namespace EfficiencyHub.Common
{
    public static class ValidationMessages
    {
        public const string NameLength = "The name must be between 5 and 50 characters.";
        public const string DescriptionLength = "The description must be between 10 and 500 characters.";
        public const string EndDateAfterStartDate = "End date must be after the start date.";
    }
}
