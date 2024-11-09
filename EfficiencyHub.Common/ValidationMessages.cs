namespace EfficiencyHub.Common
{
    public static class ValidationMessages
    {
        public const string NameLength = "The name must be between 3 and 100 characters.";
        public const string DescriptionLength = "The description must be between 10 and 500 characters.";
        public const string EndDateAfterStartDate = "End date must be after the start date.";
    }
}
