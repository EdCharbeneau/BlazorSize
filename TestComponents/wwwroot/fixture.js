var myWindow;

function openWin(page, w, h) {
    myWindow = window.open(`/${page}`, "", `width=${w}, height=${h}`);
}

function resizeWin(w, h) {
    myWindow.resizeTo(w, h);
}