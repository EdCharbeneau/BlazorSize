"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class MediaQueryListItem {
    constructor(id) {
        this.id = id;
        this.dotnetCallback = (args) => { };
        this.mediaQueries = [];
    }
}
class BlazorSizeMedia {
    constructor() {
        this.mediaQueryLists = new Array();
        //private toMediaQueryArgs = (mql: MediaQueryArgs) => ({ media: mql.media, matches: mql.matches });
        this.getMediaQueryListById = (id) => {
            let mediaQueryList = this.mediaQueryLists.find(mql => mql.id === id);
            if (mediaQueryList === undefined) {
                throw new Error("dotnet reference was not found in the collection of media query lists");
            }
            return mediaQueryList;
        };
    }
    addMediaQueryToList(dotnetMql, mediaQuery) {
        let mq = window.matchMedia(mediaQuery);
        let mediaQueryList = this.getMediaQueryListById(dotnetMql._id);
        console.log(`[BlazorSize] MediaQuery Read - media: ${mq.media} matches: ${mq.matches}`);
        mq.addListener(mediaQueryList.dotnetCallback);
        mediaQueryList.mediaQueries.push(mq);
        return { matches: mq.matches, media: mq.media };
    }
    callbackReference(dotnet) {
        return (ev) => {
            console.log(`[BlazorSize] MediaQuery Changed - media: ${ev.media} matches: ${ev.matches}`);
            dotnet.invokeMethodAsync("MediaQueryChanged", { matches: ev.matches, media: ev.media });
        };
    }
    addMediaQueryList(dotnet) {
        let list = new MediaQueryListItem(dotnet._id);
        list.dotnetCallback = this.callbackReference(dotnet);
        this.mediaQueryLists.push(list);
    }
    removeMediaQuery(dotnetMql, mediaQuery) {
        let list = this.getMediaQueryListById(dotnetMql._id);
        let queries = list.mediaQueries;
        let toRemove = queries.find(q => q.media == mediaQuery);
        toRemove === null || toRemove === void 0 ? void 0 : toRemove.removeListener(list.dotnetCallback);
        list.mediaQueries = queries.filter(q => q.media !== (toRemove === null || toRemove === void 0 ? void 0 : toRemove.media));
    }
    removeMediaQueryList(dotnetMql) {
        // Get the media query from the list
        let list = this.getMediaQueryListById(dotnetMql._id);
        // Remove all event handlers
        list.mediaQueries.forEach(q => q.removeListener(list.dotnetCallback));
        // Remove the item from the list
        this.mediaQueryLists = this.mediaQueryLists.filter(f => f.id !== dotnetMql._id);
    }
}
exports.BlazorSizeMedia = BlazorSizeMedia;
