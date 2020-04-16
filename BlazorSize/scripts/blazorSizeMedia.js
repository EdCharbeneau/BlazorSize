"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class MediaQueryListItem {
    constructor(id) {
        this.id = id;
        this.dotnetCallback = (args) => { };
        this.mediaQueries = new Map();
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
    addMediaQueryToList(dotnetMql, dotnet, mediaQuery) {
        var _a;
        let mq = window.matchMedia(mediaQuery);
        let mediaQueryList = this.getMediaQueryListById(dotnetMql._id);
        console.log(`[BlazorSize] MediaQuery Read - media: ${mq.media} matches: ${mq.matches}`);
        if (!mediaQueryList.mediaQueries.has(mediaQuery)) {
            mq.addListener(mediaQueryList.dotnetCallback);
            mediaQueryList.mediaQueries.set(mq.media, { query: mq, dotnetIds: [dotnet._id] });
        }
        else {
            (_a = mediaQueryList.mediaQueries.get(mediaQuery)) === null || _a === void 0 ? void 0 : _a.dotnetIds.push(dotnet._id);
        }
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
    removeMediaQuery(dotnetMql, dotnet) {
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
        });
    }
    removeMediaQueryList(dotnetMql) {
        // Get the media query from the list
        let list = this.getMediaQueryListById(dotnetMql._id);
        let keys = Array.from(list.mediaQueries.keys());
        // Remove all event handlers
        keys.forEach(k => {
            var _a;
            (_a = list.mediaQueries.get(k)) === null || _a === void 0 ? void 0 : _a.query.removeListener(list.dotnetCallback);
        });
        // Remove the item from the list
        this.mediaQueryLists = this.mediaQueryLists.filter(f => f.id !== dotnetMql._id);
    }
}
exports.BlazorSizeMedia = BlazorSizeMedia;
