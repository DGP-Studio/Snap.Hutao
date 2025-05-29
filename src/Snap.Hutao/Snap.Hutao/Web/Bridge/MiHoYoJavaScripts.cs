// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge;

internal static class MiHoYoJavaScripts
{
    /* lang=javascript */
    public const string InitializeJsInterfaceScript = """
        window.MiHoYoJSInterface = {
            postMessage: function(arg) { window.chrome.webview.postMessage(arg) },
            closePage: function() { this.postMessage('{"method":"closePage"}') },
        };
        """;

    /* lang=javascript */
    public const string HideScrollBarScript = """
        let hideStyle = document.createElement('style');
        hideStyle.innerHTML = '::-webkit-scrollbar{ display:none }';
        document.querySelector('body').appendChild(hideStyle);
        """;

    /* lang=javascript */
    public const string ConvertMouseToTouchScript = """
        function mouseListener (e, event) {
            let touch = new Touch({
                identifier: Date.now(),
                target: e.target,
                clientX: e.clientX,
                clientY: e.clientY,
                screenX: e.screenX,
                screenY: e.screenY,
                pageX: e.pageX,
                pageY: e.pageY,
            });
            let touchEvent = new TouchEvent(event, {
                cancelable: true,
                bubbles: true,
                touches: [touch],
                targetTouches: [touch],
                changedTouches: [touch],
            });
            e.target.dispatchEvent(touchEvent);
        }

        let mouseMoveListener = (e) => {
            mouseListener(e, 'touchmove'); 
        };

        let mouseUpListener = (e) => {
            mouseListener(e, 'touchend'); 
            document.removeEventListener('mousemove', mouseMoveListener);
            document.removeEventListener('mouseup', mouseUpListener);
        };

        let mouseDownListener = (e) => {
            mouseListener(e, 'touchstart'); 
            document.addEventListener('mousemove', mouseMoveListener);
            document.addEventListener('mouseup', mouseUpListener);
        };
        document.addEventListener('mousedown', mouseDownListener);
        """;
}