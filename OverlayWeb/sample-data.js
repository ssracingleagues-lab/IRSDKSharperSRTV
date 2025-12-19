const sampleStandings = Array.from({ length: 20 }).map((_, i) => ({
    position: i + 1,
    carNumber: String(3 + i),
    driverName: `Driver ${i + 1}`,
    interval: i === 0 ? 'Leader' : `+${(i * 0.6).toFixed(1)}s`,
    gap: i === 0 ? '-' : `+${(i * 1.2).toFixed(1)}s`
}));

const sampleDriver = {
    carNumber: '42',
    driverName: 'Focus Driver',
    team: 'iRacing Broadcast Demo',
    irating: 3200,
    license: 'A 3.75',
    lap: 24,
    position: 7,
    lastLap: '1:24.362',
    bestLap: '1:23.917',
    delta: '+0.214'
};

const defaultOverlayState = {
    showTower: true,
    showDriverCard: true,
    standings: sampleStandings,
    focusDriver: sampleDriver
};
