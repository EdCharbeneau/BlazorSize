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

var mediaQueryLists = [];
window.blazorSizeMedia = (function () {

    //let mediaQueryLists = [];
    //let example = [{
    //    mediaQueryListId: 1,
    //    dotnetCallback: () => { },
    //    mediaQueries: [
    //        {
    //            query: { media, matches},
    //            children: []
    //        }
    //    ]
    //}]

    const toMediaQueryArgs = (args) => ({ media: args.media, matches: args.matches });

    function callbackReference(dotnet) {
        return function (args) {
            console.log(`[BlazorSize] MediaQuery Changed - media: ${args.media} matches: ${args.matches}`);
            dotnet.invokeMethodAsync("MediaQueryChanged", toMediaQueryArgs(args));
        }
    };

    function addMediaQueryList(dotnet) {
        let callback = callbackReference(dotnet);
        let mql = {
            mediaQueryListId: dotnet._id,
            dotnetCallback: callback,
            mediaQueries: []
        }
        mediaQueryLists.push(mql);
    }

    function addMediaQueryToList(list, dotnet, mediaQuery) {
        let mq = matchMedia(mediaQuery);
        let mediaQueryList = mediaQueryLists.find(mql => mql.mediaQueryListId === list._id);
        let hasMediaQuery = mediaQueryList.mediaQueries.some(q => q.mediaQuery.media === mq.media);
        if (!hasMediaQuery) {
            mq.addListener(mediaQueryList.dotnetCallback);
            let children = [dotnet._id];
            mediaQueryList.mediaQueries.push({ mediaQuery: mq, children: children });
        } else {
            mediaQueryList.mediaQueries.find(m => m.mediaQuery.media === mq.media).children.push(dotnet._id);
        }
        return toMediaQueryArgs(mq);
    }

    function removeAllGlobals(dotnet) {
        // Get the media query from the list
        let list = mediaQueryLists.find(f => f.mediaQueryListId === dotnet._id);
        // Remove all event handlers
        list.mediaQueries.forEach(q => q.mediaQuery.removeListener(list.dotnetCallback));
        // Remove the item from the list
        mediaQueryLists = mediaQueryLists.filter(f => f.mediaQueryListId !== dotnet._id);
        console.log(mediaQueryLists);
    }

    function removeMediaQuery(dotnetParent, dotnet) {
        // Get the media query from the list
        let item = mediaQueryLists.find(f => f.mediaQueryListId === dotnetParent._id);
        let queriesByOwner = item.mediaQueries.filter(q => q.children.some(i => dotnet._id));
        // Remove children
        queriesByOwner.forEach(q => q.children = q.children.filter(f => f !== dotnet._id))
        // cancel callbacks
        queriesByOwner.filter(i => i.children.length === 0).forEach(i => i.mediaQuery.removeListener(item.dotnetCallback));
        // remove empty media queries
        item.mediaQueries = item.mediaQueries.filter(i => i.children.length !== 0);
    }

    return {
        addMediaQuery: function (list, dotnet, mediaQuery) {
            return addMediaQueryToList(list, dotnet, mediaQuery);
        },
        removeMediaQuery: function (list, dotnet) {
            removeMediaQuery(list, dotnet);
        },
        addMediaQueryList: function (dotnet) {
            addMediaQueryList(dotnet);
        },
        removeMediaQueryList: function (dotnetParent, dotnet) {
            removeAllGlobals(dotnetParent, dotnet);
        }
    };
}
)();