class ResizeOptions {
    reportRate: number = 300;
    enableLogging: boolean = false;
    suppressInitEvent: boolean = false;
}

export class ResizeListener {

    private logger: (message: any) => void = (message: any) => { };
    private options: ResizeOptions = new ResizeOptions();
    private throttleResizeHandlerId: number = -1;
    private dotnet: any;

    public listenForResize(dotnetRef: any, options: ResizeOptions) {
        this.options = options;
        this.dotnet = dotnetRef;
        this.logger = options.enableLogging ? console.log : (message: any) => { };
        this.logger(`[BlazorSize] Reporting resize events at rate of: ${this.options.reportRate}ms`);
        window.addEventListener("resize", this.throttleResizeHandler);
        if (!this.options.suppressInitEvent) {
            this.resizeHandler();
        }
    }

    public throttleResizeHandler = () => {
        clearTimeout(this.throttleResizeHandlerId);
        this.throttleResizeHandlerId = window.setTimeout(this.resizeHandler, this.options.reportRate);
    }

    public resizeHandler = () => {
        this.dotnet.invokeMethodAsync(
            'RaiseOnResized', {
            height: window.innerHeight,
            width: window.innerWidth
        });
        this.logger("[BlazorSize] RaiseOnResized invoked");
    }

    public cancelListener() {
        window.removeEventListener("resize", this.throttleResizeHandler);
    }

    public matchMedia(query: string) {
        let m = window.matchMedia(query).matches;
        this.logger(`[BlazorSize] matchMedia "${query}": ${m}`);
        return m;
    }

    public getBrowserWindowSize() {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        };
    }
}