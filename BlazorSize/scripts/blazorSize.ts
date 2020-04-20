import { BlazorSizeMedia } from './blazorSizeMedia.js';
import { ResizeListener } from './blazorSizeResize.js';
(<any>window).blazorSizeMedia = new BlazorSizeMedia();
(<any>window).blazorSize = new ResizeListener();