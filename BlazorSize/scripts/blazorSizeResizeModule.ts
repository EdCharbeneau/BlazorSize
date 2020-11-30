import { ResizeListener, ResizeOptions } from './blazorSizeResize.js';

let instance = new ResizeListener();

export function listenForResize(dotnetRef: any, options: ResizeOptions) {
    instance.listenForResize(dotnetRef, options);
}

export function cancelListener() {
    instance.cancelListener();
}

export function matchMedia(query: string) {
    return instance.matchMedia(query);
}

export function getBrowserWindowSize() {
    return instance.getBrowserWindowSize();
}