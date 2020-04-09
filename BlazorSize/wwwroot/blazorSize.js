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
        dotnet.invokeMethodAsync("HandleMediaQueryListEvent", { media: args.media, matches: args.matches });
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
            newMql.addListener(callbackReference);
            mqls.push(window.matchMedia(mediaQuery));
        },
        removeMediaQueryListener: function (mediaQuery) {
            log(`[BlazorSize] Removing resize events for ${mediaQuery}`);
            mqls[mediaQuery].removeEventListener(callbackReference);
        },
        getState() {
            return mqls.map(m => ({ media: m.media, matches: m.matches }));
        }

    };
}
)();

//var mqls = [ // list of window.matchMedia() queries
//    window.matchMedia("(max-width: 860px)"),
//    window.matchMedia("(max-width: 600px)"),
//    window.matchMedia("(max-height: 500px)")
//]

//function mediaqueryresponse(mql) {
//    document.getElementById("match1").innerHTML = mqls[0].matches // width: 860px media match?
//    document.getElementById("match2").innerHTML = mqls[1].matches // width: 600px media match?
//    document.getElementById("match3").innerHTML = mqls[2].matches // height: 500px media match?
//}

//for (var i = 0; i < mqls.length; i++) { // loop through queries
//    mediaqueryresponse(mqls[i]) // call handler function explicitly at run time
//    mqls[i].addListener(mediaqueryresponse) // call handler function whenever the media query is triggered
//}