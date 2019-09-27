window.resizeListener = {
    watchOnResize: function (dotnetHelper, mediaQuery) {

        console.log("Watch started");
        const handleResized = function () {
            dotnetHelper.invokeMethodAsync(
                'RaiseOnResized', {
                height: window.innerHeight,
                width: window.innerWidth,
                mediaQueryMatched: window.matchMedia(mediaQuery).matches
            });
            console.log("resized");
        };

        // set onresize handler
        window.onresize = handleResized;

        // report browser's initial values
        handleResized();

        return true;
    }
};