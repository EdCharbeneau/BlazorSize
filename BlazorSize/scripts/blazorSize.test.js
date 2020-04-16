"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const blazorSizeMedia_1 = require("./blazorSizeMedia");
const jest_matchmedia_mock_1 = __importDefault(require("jest-matchmedia-mock"));
let matchMedia;
describe('Your testing module', () => {
    beforeAll(() => {
        matchMedia = new jest_matchmedia_mock_1.default();
        const mediaQuery = '(min-width: 240px) and (max-width: 767px)';
        matchMedia.useMediaQuery(mediaQuery);
    });
    afterEach(() => {
        matchMedia.clear();
    });
    let fakeDotNetList = {
        _id: 1,
        invokeMethodAsync(method, callback) { }
    };
    let fakeDotNetQuery = {
        _id: 9,
    };
    test('Can crate', () => {
        let x = new blazorSizeMedia_1.BlazorSizeMedia();
        expect(x.mediaQueryLists).toHaveLength(0);
    });
    test('Can add MediaQueryList', () => {
        let x = new blazorSizeMedia_1.BlazorSizeMedia();
        x.addMediaQueryList(fakeDotNetList);
        expect(x.mediaQueryLists).toHaveLength(1);
    });
    test('Can add MediaQuery', () => {
        var _a;
        let x = new blazorSizeMedia_1.BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        expect((_a = x.mediaQueryLists[0].mediaQueries.get(query)) === null || _a === void 0 ? void 0 : _a.dotnetIds[0]).toBe(9);
        expect(result.media).toBe(query);
        expect(result.matches).toBe(true);
    });
    test('Can remove MediaQuery', () => {
        let x = new blazorSizeMedia_1.BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        x.removeMediaQuery(fakeDotNetList, fakeDotNetQuery);
        expect(matchMedia.getListeners(query).length).toBe(0);
    });
    test('Can remove MediaQueryList', () => {
        let x = new blazorSizeMedia_1.BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        x.removeMediaQueryList(fakeDotNetList);
        expect(matchMedia.getListeners(query).length).toBe(0);
        expect(x.mediaQueryLists.length).toBe(0);
    });
});
