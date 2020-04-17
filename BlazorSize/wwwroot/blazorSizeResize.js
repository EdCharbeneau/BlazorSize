class ResizeOptions {
    constructor() {
        this.reportRate = 300;
        this.enableLogging = false;
        this.suppressInitEvent = false;
    }
}
export class ResizeListener {
    constructor() {
        this.logger = (message) => { };
        this.options = new ResizeOptions();
        this.throttleResizeHandlerId = -1;
        this.throttleResizeHandler = () => {
            clearTimeout(this.throttleResizeHandlerId);
            this.throttleResizeHandlerId = setTimeout(this.resizeHandler, this.options.reportRate);
        };
        this.resizeHandler = () => {
            this.dotnet.invokeMethodAsync('RaiseOnResized', {
                height: window.innerHeight,
                width: window.innerWidth
            });
            this.logger("[BlazorSize] RaiseOnResized invoked");
        };
    }
    listenForResize(dotnetRef, options) {
        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : (message) => { };
        this.logger(`[BlazorSize] Reporting resize events at rate of: ${this.options.reportRate}ms`);
        window.addEventListener("resize", this.throttleResizeHandler);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
    }
    cancelListener() {
        window.removeEventListener("resize", this.throttleResizeHandler);
    }
    matchMedia(query) {
        let m = window.matchMedia(query).matches;
        this.logger(`[BlazorSize] matchMedia "${query}": ${m}`);
        return m;
    }
    getBrowserWindowSize() {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }
}
