const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const source = fs.readFileSync(path.join(__dirname, '../EditorsChoicePlugin/Api/client.js'), 'utf8');
const flush = () => new Promise(resolve => setImmediate(resolve));

function harness() {
    const nodes = [];
    const timers = new Map();
    const elements = [];
    let nextTimer = 0;
    let observer;
    const document = {
        readyState: 'loading',
        addEventListener() {},
        documentElement: { classList: { contains: () => true } },
        body: { addEventListener() {} },
        getElementById() { return null; },
        querySelector(selector) {
            return nodes.find(node => selector.startsWith(node.tagName));
        },
        querySelectorAll() { return elements; },
        createElement(tagName) {
            const listeners = new Map();
            return {
                tagName,
                setAttribute() {},
                addEventListener(type, callback) { listeners.set(type, callback); },
                removeEventListener(type) { listeners.delete(type); },
                fire(type) { listeners.get(type)?.(); },
                remove() { const i = nodes.indexOf(this); if (i >= 0) nodes.splice(i, 1); },
            };
        },
        head: { appendChild(node) { nodes.push(node); } },
    };
    class Element {
        constructor() {
            this.slider = null;
            this.isConnected = true;
            const classes = new Set();
            this.classList = {
                add: value => classes.add(value),
                remove: value => classes.delete(value),
                contains: value => classes.has(value),
            };
        }
        querySelector() { return this.slider; }
        matches() { return true; }
    }
    const context = vm.createContext({
        document, Element,
        window: { requestAnimationFrame: callback => callback() },
        console: { log() {}, warn() {} },
        setTimeout(callback, delay) { const id = ++nextTimer; timers.set(id, { callback, delay }); return id; },
        clearTimeout(id) { timers.delete(id); },
        ApiClient: { getUrl: value => value, fetch: () => Promise.resolve({ json: () => ({ hideOnTvLayout: true }) }) },
        MutationObserver: class { constructor(callback) { observer = callback; } observe() {} },
    });
    vm.runInContext(source, context);
    return {
        context, document, elements, nodes, timers, Element,
        run: code => vm.runInContext(code, context),
        notify: mutations => observer(mutations),
        tick() {
            const [id, timer] = timers.entries().next().value;
            timers.delete(id);
            timer.callback();
        },
    };
}

test('failed Splide download is removed; concurrent callers share a fresh successful retry', async () => {
    const h = harness();
    const failed = h.run('ensureSplideLoaded()');
    h.document.querySelector('script').fire('error');
    await assert.rejects(failed, /download failed/);
    assert.equal(h.document.querySelector('script'), undefined);
    const retry = h.run('ensureSplideLoaded()');
    assert.equal(h.run('ensureSplideLoaded()'), retry);
    assert.equal(h.nodes.filter(node => node.tagName === 'script').length, 1);
    h.context.window.Splide = function () {};
    h.document.querySelector('script').fire('load');
    await retry;
    assert.equal(h.timers.size, 0);
});

test('a stalled Splide request times out and permits another download', async () => {
    const h = harness();
    const stalled = h.run('ensureSplideLoaded()');
    h.tick();
    await assert.rejects(stalled, /timed out/);
    const retry = h.run('ensureSplideLoaded()');
    h.context.window.Splide = function () {};
    h.document.querySelector('script').fire('load');
    await retry;
});

test('a load event without a Splide global is rejected and can be retried', async () => {
    const h = harness();
    const loaded = h.run('ensureSplideLoaded()');
    h.document.querySelector('script').fire('load');
    await assert.rejects(loaded, /did not initialize/);
    assert.equal(h.document.querySelector('script'), undefined);
});

test('API client startup is retried automatically without locking the home container', async () => {
    const h = harness();
    const element = new h.Element();
    h.elements.push(element);
    h.context.window.Splide = function () {};
    const api = h.context.ApiClient;
    delete h.context.ApiClient;
    await h.run('setup()');
    assert.equal(element.classList.contains('editorsChoiceLoading'), false);
    assert.equal(h.timers.size, 1);
    let calls = 0;
    h.context.ApiClient = { ...api, fetch() { calls++; return api.fetch(); } };
    h.tick();
    await flush();
    assert.equal(calls, 1);
    assert.equal(element.classList.contains('editorsChoiceLoading'), false);
});

test('synchronous API exceptions release loading state and schedule a retry', async () => {
    const h = harness();
    const element = new h.Element();
    h.elements.push(element);
    h.context.window.Splide = function () {};
    h.context.ApiClient.fetch = () => { throw new Error('API not ready'); };
    await h.run('setup()');
    await flush();
    assert.equal(element.classList.contains('editorsChoiceLoading'), false);
    assert.equal(h.timers.size, 1);
});

test('removing a previously initialized banner permits rendering on the same parent', async () => {
    const h = harness();
    const element = new h.Element();
    element.slider = {};
    h.elements.push(element);
    h.context.window.Splide = function () {};
    await h.run('setup()');
    assert.equal(element.classList.contains('editorsChoiceAdded'), true);
    element.slider = null;
    let calls = 0;
    h.context.ApiClient.fetch = () => { calls++; return new Promise(() => {}); };
    await h.run('setup()');
    await h.run('setup()');
    await flush();
    assert.equal(calls, 1, 'a replacement is requested once, with concurrent setup suppressed');
});

test('DOM removal triggers reconciliation', async () => {
    const h = harness();
    h.run('initializeEditorsChoice()');
    let scheduled = 0;
    h.context.window.requestAnimationFrame = () => scheduled++;
    h.notify([{ type: 'childList', target: new h.Element(), removedNodes: [{}], addedNodes: [] }]);
    assert.equal(scheduled, 1);
});

test('startup retries stop after five attempts', async () => {
    const h = harness();
    delete h.context.ApiClient;
    await h.run('setup()');
    const delays = [];
    while (h.timers.size) {
        delays.push(h.timers.values().next().value.delay);
        h.tick();
    }
    assert.deepEqual(delays, [1000, 2000, 4000, 8000, 16000]);
});
