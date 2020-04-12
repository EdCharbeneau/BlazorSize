namespace BlazorPro.BlazorSize
{
    public static class Breakpoints
    {
        /// @media(min-width: 576px) { ... }
        /// Small devices (landscape phones, 576px and up)
        public const string SmallUp = "(min-width: 576px)";

        /// @media(min-width: 768px) { ... }
        /// Medium devices (tablets, 768px and up)
        public const string MediumUp = "(min-width: 768px)";

        // Large devices (desktops, 992px and up)
        /// @media(min-width: 992px) { ... }
        public const string LargeUp = "(min-width: 992px)";

        /// Extra large devices (large desktops, 1200px and up)
        /// @media(min-width: 1200px) { ... }
        public const string XLargeUp = "(min-width: 1200px)";

        /// Extra small devices (portrait phones, less than 576px)
        /// @media(max-width: 575.98px) { ... }
        public const string XSmallDown = "(max-width: 575.98px)";

        /// Small devices (landscape phones, less than 768px)
        /// @media(max-width: 767.98px) { ... }
        public const string SmallDown = "(max-width: 767.98px)";

        /// Medium devices (tablets, less than 992px)
        /// @media(max-width: 991.98px) { ... }
        public const string MediumDown = "(max-width: 991.98px)";

        /// Large devices (desktops, less than 1200px)
        /// @media(max-width: 1199.98px) { ... }
        public const string LargeDown = "(max-width: 1199.98px)";


        /// Small devices (landscape phones, 576px and up)
        /// @media(min-width: 576px) and(max-width: 767.98px) { ... }
        public static string OnlySmall => Between(SmallUp, SmallDown);

        /// Medium devices (tablets, 768px and up)
        /// @media(min-width: 768px) and(max-width: 991.98px) { ... }
        public static string OnlyMedium => Between(MediumUp, MediumDown);

        /// Large devices (desktops, 992px and up)
        /// @media(min-width: 992px) and(max-width: 1199.98px) { ... }
        public static string OnlyLarge => Between(LargeUp, LargeDown);

        /// <summary>
        /// Combines two media queries with the `and` keyword.
        /// Values must include parenthesis.
        /// Ex: (min-width: 992px) and (max-width: 1199.98px)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static string Between(string min, string max) => $"{min} and {max}";
    }
}
