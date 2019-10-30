window.blazorSize = (function () {

    let dotnet = {};

    let throttleResizeHandlerId = -1;

    function throttleResizeHandler() {
        clearTimeout(throttleResizeHandlerId);
        throttleResizeHandlerId = setTimeout(resizeHandler, 300);
    }

    function resizeHandler()  {
        dotnet.invokeMethodAsync(
            'RaiseOnResized', {
            height: window.innerHeight,
            width: window.innerWidth
        });
        console.log("resized");
    }

    return {

        listenForResize: function (dotnetHelper) {
            console.log("Watch started");
            dotnet = dotnetHelper;

            window.addEventListener("resize", throttleResizeHandler);

            // report inital value
            resizeHandler();

            return true;
        },
        cancelListener: function () {
            window.removeEventListener("resize", throttleResizeHandler);
        },
        matchMedia: function (query) {
            var m = window.matchMedia(query).matches;
            console.log(m);
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
