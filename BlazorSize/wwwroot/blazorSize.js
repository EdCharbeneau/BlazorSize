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

window.blazorSizeMedia = (function () {
    let mqls = [];
    let mediaQueryLists = [];
    //let example = [{
    //    mediaQueryListId: 1,
    //    dotnetCallback: () => { },
    //    mediaQueries: [
    //        {
    //            query: {},
    //            children: []
    //        }
    //    ]
    //}]

    function callbackReference(dotnet) {
        return function (args) {
            console.log(`[BlazorSize] MediaQuery Changed - media: ${args.media} matches: ${args.matches}`);
            dotnet.invokeMethodAsync("MediaQueryChanged", { media: args.media, matches: args.matches });
        }
    };

    function addMediaQueryWithCallback(mediaQuery, dotnet) {
        let newMql = window.matchMedia(mediaQuery);
        let callback = callbackReference(dotnet);
        newMql.addListener(callback);
        mqls.push(
            {
                ref: dotnet,
                query: newMql,
                callbackRef: callback
            }
        );
        console.log(`[BlazorSize] Listening for MediaQuery: ${newMql.media}`);
        return { media: newMql.media, matches: newMql.matches };
    };

    function addMediaQueryList(dotnet) {
        let callback = callbackReference(dotnet);
        let mql = {
            mediaQueryListId: dotnet._id,
            dotnetCallback: callback,
            queries: []
        }
        mediaQueryLists.push(mql);
    }

    function addMediaQueryToList(list, dotnet, mediaQuery) {
        let mq = matchMedia(mediaQuery);
        let mediaQueryList = mediaQueryLists.find(mql => mql.mediaQueryListId === list._id);
        let hasMediaQuery = mediaQueryList.queries.some(q => q.media === mq.media);
        if (!hasMediaQuery) {
            mq.addListener(mediaQueryList.dotnetCallback);
            let children = [dotnet._id];
            mediaQueryList.queries.push({ mediaQuery: mq, children: children });
        } else {
            mediaQueryList.queries.find(m => m.media === mq.media).children.push(dotnet._id);
        }
        console.log(mediaQueryList.queries);
        return { media: mq.media, matches: mq.matches };
    }

    function removeAll(dotnet) {
        mqls.filter(f => f.ref._id === dotnet._id)
            .forEach(o => o.query.removeListener(o.callbackRef));
    };

    function removeAllGlobals(dotnet) {
        let item = globalMqls.filter(f => f.ref._id === dotnet._id)[0];
        item.queries.forEach(q => q.removeListener(item.callback));
    }

    return {
        addMediaQueryListener: function (mediaQuery, dotnet) {
            return addMediaQueryWithCallback(mediaQuery, dotnet);
        },
        addMediaQuery: function (list, dotnet, mediaQuery) {
            return addMediaQueryToList(list, dotnet, mediaQuery);
        },
        addMediaQueryList: function (dotnet) {
            addMediaQueryList(dotnet);
        },
        removeMediaQueryListeners: function (dotnet) {
            removeAll(dotnet);
        },
        removeMediaQueryList: function (dotnet) {
            removeAllGlobals(dotnet);
        }
    };
}
)();