import {
    pageCache,
    imageCache,
    staticResourceCache,
    googleFontsCache,
    offlineFallback,
} from 'workbox-recipes'
import { precacheAndRoute } from './node_modules/workbox-precaching/index';
import { NetworkOnly } from './node_modules/workbox-strategies/index';

precacheAndRoute(self.__WB_MANIFEST);

googleFontsCache();

staticResourceCache();

imageCache();

offlineFallback();

//trang feedback network only
self.addEventListener('fetch', (event) => {
    const { request } = event;
    const url = new URL(request.url);

    if (url.origin === location.origin && url.pathname === '/feedback') {
        event.respondWith(new NetworkOnly().handle({ event, request }).catch(() => caches.match('/offline/feedback/index.html')));
    }
});

precacheAndRoute([{ url: '/offline/feedback/index.html', revision: '1' }]);



