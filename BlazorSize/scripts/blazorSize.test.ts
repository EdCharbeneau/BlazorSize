import { BlazorSizeMedia } from './blazorSizeMedia';
import MatchMediaMock from 'jest-matchmedia-mock';
import MatchMedia from 'jest-matchmedia-mock';

let matchMedia: MatchMedia;

describe('Your testing module', () => {
    beforeAll(() => {
        matchMedia = new MatchMediaMock();
        const mediaQuery = '(min-width: 240px) and (max-width: 767px)';
        matchMedia.useMediaQuery(mediaQuery);
    });

    afterEach(() => {
        matchMedia.clear();
    });

    let fakeDotNetList = {
        _id: 1,
        invokeMethodAsync(method: string, callback: any) { }
    }

    let fakeDotNetQuery = {
        _id: 9,
    }


    test('Can crate', () => {
        let x = new BlazorSizeMedia();
        expect(x.mediaQueryLists).toHaveLength(0);
    })

    test('Can add MediaQueryList', () => {
        let x = new BlazorSizeMedia();
        x.addMediaQueryList(fakeDotNetList);
        expect(x.mediaQueryLists).toHaveLength(1);
    })

    test('Can add MediaQuery', () => {
        let x = new BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        expect(x.mediaQueryLists[0].mediaQueries.get(query)?.dotnetIds[0]).toBe(9);
        expect(result.media).toBe(query);
        expect(result.matches).toBe(true);
    })

    test('Can remove MediaQuery', () => {
        let x = new BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        x.removeMediaQuery(fakeDotNetList, fakeDotNetQuery);
        expect(matchMedia.getListeners(query).length).toBe(0);
    })

    test('Can remove MediaQueryList', () => {
        let x = new BlazorSizeMedia();
        let query = '(min-width: 240px) and (max-width: 767px)';
        x.addMediaQueryList(fakeDotNetList);
        let result = x.addMediaQueryToList(fakeDotNetList, fakeDotNetQuery, query);
        x.removeMediaQueryList(fakeDotNetList);
        expect(matchMedia.getListeners(query).length).toBe(0);
        expect(x.mediaQueryLists.length).toBe(0);
    })

})