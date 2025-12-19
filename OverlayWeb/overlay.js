const channel = new BroadcastChannel('overlay-control');
const overlayState = structuredClone(defaultOverlayState);

channel.addEventListener('message', (event) => {
    const { type, payload } = event.data;
    if (type === 'overlay-state') {
        Object.assign(overlayState, payload);
        render();
    }
});

function renderStandings(list) {
    const container = document.querySelector('#standings');
    container.innerHTML = '';
    list.slice(0, 20).forEach((entry) => {
        const row = document.createElement('div');
        row.className = 'tower-row';
        row.innerHTML = `
            <div class="pos">${entry.position}</div>
            <div class="name">#${entry.carNumber} ${entry.driverName}</div>
            <div class="gap">${entry.interval ?? '-'}</div>
            <div class="gap">${entry.gap ?? '-'}</div>
        `;
        container.appendChild(row);
    });
}

function renderDriverCard(driver) {
    const card = document.querySelector('#driver-card');
    card.innerHTML = `
        <div class="number">#${driver.carNumber ?? '--'}</div>
        <div>
            <div class="name">${driver.driverName ?? 'No driver selected'}</div>
            <div class="meta">${driver.team ?? 'Team TBD'}</div>
            <div class="meta">P${driver.position ?? '-'} · Lap ${driver.lap ?? '-'} · Δ ${driver.delta ?? '-'}</div>
            <div class="meta">Last ${driver.lastLap ?? '-'} · Best ${driver.bestLap ?? '-'}</div>
            <div class="meta">iRating ${driver.irating ?? '-'} · Lic ${driver.license ?? '-'}</div>
        </div>
    `;
}

function render() {
    document.querySelector('#tower-panel').style.display = overlayState.showTower ? 'block' : 'none';
    document.querySelector('#driver-panel').style.display = overlayState.showDriverCard ? 'block' : 'none';

    renderStandings(overlayState.standings || []);
    renderDriverCard(overlayState.focusDriver || {});
}

window.addEventListener('DOMContentLoaded', render);
