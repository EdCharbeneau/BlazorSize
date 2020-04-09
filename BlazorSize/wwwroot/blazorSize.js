window.blazorSize = (function () {

    let dotnet = {};
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
        log = resizeOptions.enableLogging ? console.log : () => { };
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
        },


    };
}
)();

window.blazorSizeMedia = (function () {
    let mqls = [];
    let dotnet = {};
    let log = () => { };
    let blazorSizeMediaOptions = {
        enableLogging: true
    };

    function callbackReference(args) {
        dotnet.invokeMethodAsync("RaiseOnMediaQueryListChanged", { media: args.media, matches: args.matches });
    };

    function configure(options) {
        blazorSizeMediaOptions = { ...blazorSizeMediaOptions, ...options };
        log = blazorSizeMediaOptions.enableLogging ? console.log : () => { };
    }

    return {
        init: function (dotnetReference, options) {
            configure(options);
            dotnet = dotnetReference;
        },
        addMediaQueryListener: function (mediaQuery) {
            log(`[BlazorSize] Reporting resize events for ${mediaQuery}`);
            let newMql = window.matchMedia(mediaQuery);
            if (!mqls.some(m => m.media === newMql.media)) 
            {
                newMql.addListener(callbackReference);
                mqls.push(window.matchMedia(mediaQuery));
            }
            return { media: newMql.media, matches: newMql.matches };
        },
        cancelListener: function () {
            for (var i = 0; i < mqls.length; i++) {
                mqls[i].removeListener(callbackReference)
            }
        }
    };
}
)();