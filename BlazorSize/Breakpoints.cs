namespace BlazorSize
{
    public static class Breakpoints
    {
        ///@media(min-width: 576px) { ... }
        /// Medium devices (tablets, 768px and up)
        public const string SmallUp = "(min-width: 576px)";

        /// @media(min-width: 768px) { ... }
        /// Large devices (desktops, 992px and up)
        public const string MediumUp = "(min-width: 768px)";

        /// @media(min-width: 992px) { ... }
        public const string LargeUp = "(min-width: 992px)";

        /// Extra large devices (large desktops, 1200px and up)
        /// @media(min-width: 1200px) { ... }
        public const string XLargeUp = "(min-width: 1200px)";

    }
}
