const channel = new BroadcastChannel('overlay-control');
const state = structuredClone(defaultOverlayState);

function sendUpdate(partial) {
    Object.assign(state, partial);
    channel.postMessage({ type: 'overlay-state', payload: state });
    renderPreview();
}

function onToggleTower(ev) {
    sendUpdate({ showTower: ev.target.checked });
}

function onToggleDriver(ev) {
    sendUpdate({ showDriverCard: ev.target.checked });
}

function onPushStandings() {
    const raw = document.querySelector('#standings-json').value;
    try {
        const parsed = JSON.parse(raw);
        sendUpdate({ standings: parsed });
    } catch (err) {
        alert('Invalid JSON for standings');
    }
}

function onPushDriver() {
    const raw = document.querySelector('#driver-json').value;
    try {
        const parsed = JSON.parse(raw);
        sendUpdate({ focusDriver: parsed });
    } catch (err) {
        alert('Invalid JSON for driver');
    }
}

function renderPreview() {
    const tower = document.querySelector('#preview-standings');
    tower.innerHTML = '';
    state.standings.slice(0, 10).forEach((entry) => {
        const row = document.createElement('div');
        row.className = 'tower-row';
        row.innerHTML = `
            <div class="pos">${entry.position}</div>
            <div class="name">#${entry.carNumber} ${entry.driverName}</div>
            <div class="gap">${entry.interval ?? '-'}</div>
            <div class="gap">${entry.gap ?? '-'}</div>
        `;
        tower.appendChild(row);
    });

    const driver = document.querySelector('#preview-driver');
    driver.innerHTML = `
        <div class="driver-card">
            <div class="number">#${state.focusDriver.carNumber}</div>
            <div>
                <div class="name">${state.focusDriver.driverName}</div>
                <div class="meta">${state.focusDriver.team}</div>
                <div class="meta">P${state.focusDriver.position} · Lap ${state.focusDriver.lap}</div>
            </div>
        </div>
    `;
}

function initForms() {
    document.querySelector('#toggle-tower').checked = state.showTower;
    document.querySelector('#toggle-driver').checked = state.showDriverCard;
    document.querySelector('#standings-json').value = JSON.stringify(state.standings, null, 2);
    document.querySelector('#driver-json').value = JSON.stringify(state.focusDriver, null, 2);

    document.querySelector('#toggle-tower').addEventListener('change', onToggleTower);
    document.querySelector('#toggle-driver').addEventListener('change', onToggleDriver);
    document.querySelector('#push-standings').addEventListener('click', onPushStandings);
    document.querySelector('#push-driver').addEventListener('click', onPushDriver);
}

window.addEventListener('DOMContentLoaded', () => {
    initForms();
    sendUpdate(state);
});
