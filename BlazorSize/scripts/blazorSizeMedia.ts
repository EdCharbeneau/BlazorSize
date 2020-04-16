class MediaQueryListItem {
    constructor(id: number | string) {
        this.id = id;
        this.dotnetCallback = (args) => { };
        this.mediaQueries = new Map<string, Subscribers>();
    }
    public id: number | string;
    public dotnetCallback: (ev: MediaQueryListEvent) => void;
    public mediaQueries: Map<string, Subscribers>;
}

interface Subscribers {
    query: MediaQueryList,
    dotnetIds: (number | string)[]
}

interface MediaQueryArgs {
    media: string;
    matches: boolean;
}

export class BlazorSizeMedia {
    mediaQueryLists: MediaQueryListItem[] = new Array<MediaQueryListItem>();

    //private toMediaQueryArgs = (mql: MediaQueryArgs) => ({ media: mql.media, matches: mql.matches });
    private getMediaQueryListById = (id: number | string) => {
        let mediaQueryList = this.mediaQueryLists.find(mql => mql.id === id)
        if (mediaQueryList === undefined) {
            throw new Error("dotnet reference was not found in the collection of media query lists");
        }
        return mediaQueryList;
    };

    addMediaQueryToList(dotnetMql: any, dotnet: any, mediaQuery: string) {
        let mq = window.matchMedia(mediaQuery);
        let mediaQueryList = this.getMediaQueryListById(dotnetMql._id);
        console.log(`[BlazorSize] MediaQuery Read - media: ${mq.media} matches: ${mq.matches}`);

        if (!mediaQueryList.mediaQueries.has(mediaQuery)) {
            mq.addListener(mediaQueryList.dotnetCallback);
            mediaQueryList.mediaQueries.set(mq.media, { query: mq, dotnetIds: [dotnet._id] });
        }
        else {
            mediaQueryList.mediaQueries.get(mediaQuery)?.dotnetIds.push(dotnet._id);
        }
        return { matches: mq.matches, media: mq.media } as MediaQueryArgs;
    }

    callbackReference(dotnet: any) {
        return (ev: MediaQueryListEvent) => {
            console.log(`[BlazorSize] MediaQuery Changed - media: ${ev.media} matches: ${ev.matches}`);
            dotnet.invokeMethodAsync("MediaQueryChanged", { matches: ev.matches, media: ev.media } as MediaQueryArgs);
        }
    }

    addMediaQueryList(dotnet: any) {
        let list = new MediaQueryListItem(dotnet._id);
        list.dotnetCallback = this.callbackReference(dotnet);
        this.mediaQueryLists.push(list);
    }

    removeMediaQuery(dotnetMql: any, dotnet: any) {
        // Get the media query from the list
        let list = this.getMediaQueryListById(dotnetMql._id);
        let queries = list.mediaQueries;
        let keys = Array.from(queries.keys());
        keys.forEach(key => { 
            let subs = queries.get(key);
            if (subs !== undefined) {
                // Remove children
                subs.dotnetIds = subs.dotnetIds.filter(id => id !== dotnet._id);
                if (subs.dotnetIds.length === 0) {
                    // cancel callbacks
                    subs.query.removeListener(list.dotnetCallback);
                    // remove empty
                    queries.delete(key);
                }
            }
        })
    }

    removeMediaQueryList(dotnetMql: any) {
        // Get the media query from the list
        let list = this.getMediaQueryListById(dotnetMql._id);
        let keys = Array.from(list.mediaQueries.keys());
        // Remove all event handlers
        keys.forEach(k => {
            list.mediaQueries.get(k)?.query.removeListener(list.dotnetCallback);
        });
        // Remove the item from the list
        this.mediaQueryLists = this.mediaQueryLists.filter(f => f.id !== dotnetMql._id);
    }
}