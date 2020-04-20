class MediaQueryListItem {
    constructor(id: number | string) {
        this.id = id;
        this.dotnetCallback = (args) => { };
        this.mediaQueries = [];
    }
    public id: number | string;
    public dotnetCallback: (ev: MediaQueryListEvent) => void;
    public mediaQueries: MediaQueryList[];
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

    addMediaQueryToList(dotnetMql: any, mediaQuery: string) {
        let mq = window.matchMedia(mediaQuery);
        let mediaQueryList = this.getMediaQueryListById(dotnetMql._id);
        //console.log(`[BlazorSize] MediaQuery Read - media: ${mq.media} matches: ${mq.matches}`);
        mq.addListener(mediaQueryList.dotnetCallback);
        mediaQueryList.mediaQueries.push(mq);
        return { matches: mq.matches, media: mq.media } as MediaQueryArgs;
    }

    callbackReference(dotnet: any) {
        return (ev: MediaQueryListEvent) => {
            //console.log(`[BlazorSize] MediaQuery Changed - media: ${ev.media} matches: ${ev.matches}`);
            dotnet.invokeMethodAsync("MediaQueryChanged", { matches: ev.matches, media: ev.media } as MediaQueryArgs);
        }
    }

    addMediaQueryList(dotnet: any) {
        let list = new MediaQueryListItem(dotnet._id);
        list.dotnetCallback = this.callbackReference(dotnet);
        this.mediaQueryLists.push(list);
    }

    removeMediaQuery(dotnetMql: any, mediaQuery: string) {
        let list = this.getMediaQueryListById(dotnetMql._id);
        let queries = list.mediaQueries;
        let toRemove = queries.find(q => q.media == mediaQuery);
        toRemove?.removeListener(list.dotnetCallback);
        list.mediaQueries = queries.filter(q => q.media !== toRemove?.media);
    }

    removeMediaQueryList(dotnetMql: any) {
        // Get the media query from the list
        let list = this.getMediaQueryListById(dotnetMql._id);
        // Remove all event handlers
        list.mediaQueries.forEach(q => q.removeListener(list.dotnetCallback));
        // Remove the item from the list
        this.mediaQueryLists = this.mediaQueryLists.filter(f => f.id !== dotnetMql._id);
    }
}