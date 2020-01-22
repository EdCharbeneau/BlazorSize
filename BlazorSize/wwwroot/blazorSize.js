window.blazorSize = (function () {

    let dotnet = { };
    let throttleResizeHandlerId = -1;
    let log = () => { };
    let resizeOptions = {
        reportRate: 300,
        enableLogging: false,
        suppressInitEvent: false
    };

    function throttleResizeHandler() {
        clearTimeout(throttleResizeHandlerId);
        throttleResizeHandlerId = setTimeout(resizeHandler, resizeOptions.reportRate);
    }

    function resizeHandler() {
        dotnet.invokeMethodAsync(
            'RaiseOnResized', {
            height: window.innerHeight,
            width: window.innerWidth
        });
        log("[BlazorSize] RaiseOnResized invoked");
    }

    function configure(options) {
        resizeOptions = { ...resizeOptions, ...options };
        log = options.enableLogging ? console.log : () => { };
    }

    return {

        listenForResize: function (dotnetHelper, options) {
            configure(options);

            log(`[BlazorSize] Reporting resize events at rate of: ${resizeOptions.reportRate}ms`);
            dotnet = dotnetHelper;
            window.addEventListener("resize", throttleResizeHandler);

            if (resizeOptions.suppressInitEvent) return true;

            resizeHandler();
            return true;
        },
        cancelListener: function () {
            window.removeEventListener("resize", throttleResizeHandler);
        },
        matchMedia: function (query) {
            let m = window.matchMedia(query).matches;
            log(`[BlazorSize] matchMedia "${query}": ${m}`);
            return m;
        },
        getBrowserWindowSize: function () {
            return {
                height: window.innerHeight,
                width: window.innerWidth
            };
        }

    };
}
)();
